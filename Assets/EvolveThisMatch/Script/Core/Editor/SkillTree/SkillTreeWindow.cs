using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using XNodeEditor.Internal;

namespace EvolveThisMatch.Core
{
    public class SkillTreeWindow : NodeEditorWindow
    {
        private SkillTreeGraph skillTreeGraph;
        private SkillNode selectedSkillNode;
        private Vector2Int prevGridSize = Vector2Int.one;

        private Rect sidePanelRect;
        private float sidePanelWidth = 250f;
        private const float sidePanelMinWidth = 250f;
        private const float sidePanelMaxWidth = 500f;
        private const float handleWidth = 4f;
        private bool isResizing = false;

        public static SkillTreeWindow Open(SkillTreeGraph graph)
        {
            if (!graph) return null;

            SkillTreeWindow w = GetWindow(typeof(SkillTreeWindow), false, "스킬트리", true) as SkillTreeWindow;
            w.wantsMouseMove = true;
            w.graph = graph;
            w.skillTreeGraph = graph;

            return w;
        }

        protected override void OnGUI()
        {
            if (graph == null) return;

            Matrix4x4 oldMatrix = GUI.matrix;

            float graphWidth = position.width - sidePanelWidth - handleWidth;

            // 각 Rect 생성
            Rect handdleRect = new Rect(graphWidth, 0, handleWidth, position.height);
            sidePanelRect = new Rect(graphWidth + handleWidth, 0, sidePanelWidth, position.height);

            // 그래프 영역
            DrawGraph();
            
            // 구분선 영역
            EditorGUIUtility.AddCursorRect(handdleRect, MouseCursor.ResizeHorizontal);
            EditorGUI.DrawRect(handdleRect, new Color(0.3f, 0.3f, 0.3f));
            HandleResizeEvents(handdleRect);

            // 사이드 패널 영역
            GUI.BeginGroup(sidePanelRect);
            {
                DrawSidePanel(new Rect(0, 0, sidePanelWidth, position.height));
            }
            GUI.EndGroup();

            GUI.matrix = oldMatrix;
        }

        #region 그래프
        private void DrawGraph()
        {
            ValidateGraphEditor();
            Controls();

            DrawConnections();
            DrawDraggedConnection();
            DrawNodes();
            graphEditor.OnGUI();
        }

        #region 조작
        private new void Controls()
        {
            wantsMouseMove = true;
            Event e = Event.current;
            switch (e.type)
            {
                #region 드래그­·드롭
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    if (e.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        graphEditor.OnDropObjects(DragAndDrop.objectReferences);
                    }
                    break;
                #endregion
                #region 줌­·아웃
                case EventType.ScrollWheel:
                    float oldZoom = zoom;
                    if (e.delta.y > 0) zoom += 0.1f * zoom;
                    else zoom -= 0.1f * zoom;
                    if (NodeEditorPreferences.GetSettings().zoomToMouse) panOffset += (1 - oldZoom / zoom) * (WindowToGridPosition(e.mousePosition) + panOffset);
                    break;
                #endregion
                #region 그래프 이동, 포트 연결 시도
                case EventType.MouseDrag:
                    #region 포트 연결 시도
                    if (e.button == 0)
                    {
                        if (IsDraggingPort)
                        {
                            if (IsHoveringPort && hoveredPort.IsInput && !draggedOutput.IsConnectedTo(hoveredPort))
                            {
                                draggedOutputTarget = hoveredPort;
                            }
                            else
                            {
                                draggedOutputTarget = null;
                            }
                            Repaint();
                        }
                    }
                    #endregion
                    #region 그래프 이동
                    else if (e.button == 1 || e.button == 2)
                    {
                        if (e.delta.magnitude > dragThreshold)
                        {
                            panOffset += e.delta * zoom;
                            isPanning = true;
                        }
                    }
                    break;
                    #endregion
                #endregion
                #region 포트 연결 시도 & 포트 연결 해제, 노드 선택, 빈 공간 클릭 시 선택해제)
                case EventType.MouseDown:
                    Repaint();
                    if (e.button == 0)
                    {
                        #region 포트 연결 시도 & 포트 연결 해제
                        if (IsHoveringPort)
                        {
                            if (hoveredPort.IsOutput)
                            {
                                draggedOutput = hoveredPort;
                                autoConnectOutput = hoveredPort;
                            }
                            else
                            {
                                hoveredPort.VerifyConnections();
                                autoConnectOutput = null;
                                if (hoveredPort.IsConnected)
                                {
                                    XNode.Node node = hoveredPort.node;
                                    XNode.NodePort output = hoveredPort.Connection;
                                    int outputConnectionIndex = output.GetConnectionIndex(hoveredPort);
                                    hoveredPort.Disconnect(output);
                                    draggedOutput = output;
                                    draggedOutputTarget = hoveredPort;
                                    if (NodeEditor.onUpdateNode != null) NodeEditor.onUpdateNode(node);
                                }
                            }
                        }
                        #endregion
                        #region 노드를 클릭할 경우
                        else if (IsHoveringNode && IsHoveringTitle(hoveredNode))
                        {
                            if (!Selection.Contains(hoveredNode))
                            {
                                EditorGUI.FocusTextInControl(null);
                                selectedSkillNode = hoveredNode as SkillNode;
                                SelectNode(hoveredNode, false);
                            }

                            e.Use();
                            currentActivity = NodeActivity.HoldNode;
                        }
                        #endregion
                        #region 그리드를 클릭할 경우, 선택 해제
                        else if (!IsHoveringNode && !sidePanelRect.Contains(e.mousePosition))
                        {
                            currentActivity = NodeActivity.HoldGrid;
                            
                            EditorGUI.FocusTextInControl(null);
                            selectedSkillNode = null;
                            Selection.activeObject = null;
                        }
                        #endregion
                    }
                    break;
                #endregion
                #region 포트 드래그 후 연결 완료, 노드 선택, 변경된 상태 저장
                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        #region 포트 연결
                        if (IsDraggingPort)
                        {
                            #region 포트 연결이 가능하면
                            if (draggedOutputTarget != null && draggedOutput.CanConnectTo(draggedOutputTarget))
                            {
                                XNode.Node node = draggedOutputTarget.node;

                                if (graph.nodes.Count != 0) draggedOutput.Connect(draggedOutputTarget);

                                int connectionIndex = draggedOutput.GetConnectionIndex(draggedOutputTarget);
                                if (connectionIndex != -1)
                                {
                                    //draggedOutput.GetReroutePoints(connectionIndex).AddRange(draggedOutputReroutes);
                                    if (NodeEditor.onUpdateNode != null) NodeEditor.onUpdateNode(node);
                                    EditorUtility.SetDirty(graph);
                                }
                            }
                            #endregion

                            // 드래그 상태 해제 및 정리
                            draggedOutput = null;
                            draggedOutputTarget = null;

                            EditorUtility.SetDirty(graph);
                            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
                        }
                        #endregion
                        #region 텍스트 입력 중이던 필드에서 포커스를 해제하고 저장
                        else if (currentActivity != NodeActivity.DragNode && !IsHoveringNode)
                        {
                            if (!isPanning && !EditorGUIUtility.editingTextField)
                            {
                                EditorGUI.FocusTextInControl(null);
                            }
                            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
                        }
                        #endregion
                        
                        #region 노드 선택
                        if (currentActivity == NodeActivity.HoldNode)
                        {
                            EditorGUI.FocusTextInControl(null);
                            selectedSkillNode = hoveredNode as SkillNode;
                            SelectNode(hoveredNode, false);
                        }
                        #endregion

                        Repaint();
                        currentActivity = NodeActivity.Idle;
                    }
                    else if (e.button == 1 || e.button == 2)
                    {
                        isPanning = false;
                    }
                    break;
                #endregion
                #region 단축키 설정
                case EventType.KeyDown:
                    // 가운데 화면으로 이동
                    if (e.keyCode == KeyCode.F1) Home();

                    // 이름 재설정
                    if (NodeEditorUtilities.IsMac())
                    {
                        if (e.keyCode == KeyCode.Return) RenameSelectedNode();
                    }
                    else
                    {
                        if (e.keyCode == KeyCode.F2) RenameSelectedNode();
                    }
                    break;
                #endregion
                #region 마우스가 창 밖으로 이동할 경우, 기본 상태로 변경
                case EventType.Ignore:
                    
                    if (e.rawType == EventType.MouseUp && currentActivity == NodeActivity.DragGrid)
                    {
                        Repaint();
                        currentActivity = NodeActivity.Idle;
                    }
                    break;
                #endregion
            }
        }
        #endregion
        #endregion

        #region 사이드 패널
        private void DrawSidePanel(Rect panelRect)
        {
            if (skillTreeGraph == null) return;

            EditorGUI.DrawRect(panelRect, new Color(0.22f, 0.22f, 0.22f, 1f));

            GUILayout.BeginArea(panelRect, EditorStyles.helpBox);
            GUILayout.Label("스킬 트리 설정", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("이름", GUILayout.Width(80));
            skillTreeGraph.displayName = GUILayout.TextField(skillTreeGraph.displayName);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("크기", GUILayout.Width(80));
            skillTreeGraph.gridSize = EditorGUILayout.Vector2IntField(GUIContent.none, skillTreeGraph.gridSize, GUILayout.Width(panelRect.width - 92));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("노드 사이 간격", GUILayout.Width(80));
            skillTreeGraph.nodeSpacing = EditorGUILayout.Vector2IntField(GUIContent.none, skillTreeGraph.nodeSpacing, GUILayout.Width(panelRect.width - 92));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            if (GUILayout.Button("크기 적용"))
            {
                UpdateGrid();
            }

            if (selectedSkillNode != null)
            {
                EditorGUI.BeginChangeCheck();

                GUILayout.Space(10);
                GUILayout.Label("선택한 노드 설정", EditorStyles.boldLabel);
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("스킬 템플릿", GUILayout.Width(80));
                selectedSkillNode.skillTemplate = (SkillTemplate)EditorGUILayout.ObjectField(selectedSkillNode.skillTemplate, typeof(SkillTemplate), false);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("시작 레벨", GUILayout.Width(80));
                selectedSkillNode.startLevel = EditorGUILayout.IntField(selectedSkillNode.startLevel);
                selectedSkillNode.startLevel = Mathf.Clamp(selectedSkillNode.startLevel, selectedSkillNode.minLevel, selectedSkillNode.maxLevel);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("최소 레벨", GUILayout.Width(80));
                selectedSkillNode.minLevel = EditorGUILayout.IntField(selectedSkillNode.minLevel);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("최대 레벨", GUILayout.Width(80));
                selectedSkillNode.maxLevel = EditorGUILayout.IntField(selectedSkillNode.maxLevel);
                GUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(selectedSkillNode, "Node Modify");
                    EditorUtility.SetDirty(selectedSkillNode);
                }
            }

            GUILayout.EndArea();
        }

        private void UpdateGrid()
        {
            if (skillTreeGraph.gridSize == prevGridSize) return;
            var gridSize = skillTreeGraph.gridSize;
            var nodeSpacing = skillTreeGraph.nodeSpacing;
            prevGridSize = gridSize;

            int total = gridSize.x * gridSize.y;

            // 범위를 벗어나는 노드는 제거
            while (graph.nodes.Count > total)
            {
                var nodeToRemove = graph.nodes.Last();
                graphEditor.RemoveNode(nodeToRemove);
            }

            // 부족한 노드는 추가
            while (graph.nodes.Count < total)
            {
                graphEditor.CreateNode(typeof(SkillNode), Vector2.zero);
            }

            // 노드의 위치 정해주기 (가로 -> 세로 순서로 인덱스 증가)
            Vector2 totalGridSize = new Vector2((gridSize.x - 1) * nodeSpacing.x + 500, (gridSize.y - 1) * nodeSpacing.y);
            Vector2 offset = -totalGridSize * 0.5f;

            var nodes = graph.nodes.OfType<SkillNode>().ToList();

            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    int index = y * gridSize.x + x; // 가로 우선 인덱스 증가
                    if (index >= nodes.Count) break;

                    var pos = new Vector2(x * nodeSpacing.x, y * nodeSpacing.y) + offset;
                    var node = nodes[index];
                    node.position = pos;
                    //node.name = $"Skill_{x}_{y}";
                    node.index = index;
                }
            }

            AssetDatabase.SaveAssets();
        }
        #endregion

        #region 구분선
        private void HandleResizeEvents(Rect handdleRect)
        {
            Event e = Event.current;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (handdleRect.Contains(e.mousePosition))
                    {
                        isResizing = true;
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (isResizing)
                    {
                        isResizing = false;
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (isResizing)
                    {
                        sidePanelWidth -= e.delta.x;
                        sidePanelWidth = Mathf.Clamp(sidePanelWidth, sidePanelMinWidth, sidePanelMaxWidth);
                        e.Use();
                        Repaint();
                    }
                    break;
            }
        }
        #endregion
    }
}