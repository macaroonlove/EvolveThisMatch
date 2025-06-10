using FrameWork.UIBinding;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class SkillTreeCanvas : UIBase
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private GameObject _nodePrefab;
        [SerializeField] private GameObject _pathPrefab;
        [Space]
        [SerializeField] private int _nodeWidthOffset = 300;
        [SerializeField] private int _nodeHeightOffset = 300;

        private PoolSystem _poolSystem;
        private SkillTreeGraph _skillTree;

        private List<UISkillNodeController> _nodes = new List<UISkillNodeController>();
        private List<GameObject> _paths = new List<GameObject>();

        protected override void Initialize()
        {
            if (_poolSystem != null) return;

            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
        }

        public void Show(SkillTreeGraph skillTree)
        {
            base.Show();

            // 새로운 스킬트리가 들어왔다면
            if (_skillTree != skillTree)
            {
                Clear();
            }
            else
            {
                return;
            }

            _skillTree = skillTree;

            // 배경 크기 설정
            InitializeContentHeight(skillTree);

            // 노드 배치
            GenerateNode(skillTree);

            // 선 배치
            GeneratePath(skillTree);
        }

        private void Clear()
        {
            foreach (var node in _nodes)
            {
                _poolSystem.DeSpawn(node.gameObject);
            }
            foreach (var path in _paths)
            {
                _poolSystem.DeSpawn(path);
            }

            _nodes.Clear();
            _paths.Clear();
        }

        #region Generate
        private void InitializeContentHeight(SkillTreeGraph skillTree)
        {
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, _nodeHeightOffset * skillTree.gridSize.y + 100);
        }

        private void GenerateNode(SkillTreeGraph skillTree)
        {
            var nodeControllerMap = new Dictionary<SkillNode, UISkillNodeController>();

            Vector2 position = GetStartPosition(skillTree.gridSize.x);
            float startX = position.x;
            int index = 0;
            // 노드 생성
            foreach (var node in skillTree.nodes)
            {
                if (node is SkillNode skill)
                {
                    if (skill.skillTemplate != null)
                    {
                        Transform trans = _poolSystem.Spawn(_nodePrefab, _content).transform;
                        (trans as RectTransform).anchoredPosition = position;

                        var nodeController = trans.GetComponent<UISkillNodeController>();
                        nodeController.Initialize(skill);

                        nodeControllerMap[skill] = nodeController;
                        _nodes.Add(nodeController);
                    }
                }

                index++;
                position.x += _nodeWidthOffset;
                if (index == skillTree.gridSize.x)
                {
                    index = 0;
                    position.x = startX;
                    position.y -= _nodeHeightOffset;
                }
            }

            // 연결 설정
            foreach (var node in skillTree.nodes)
            {
                if (node is SkillNode from)
                {
                    if (from.skillTemplate == null) continue;

                    var nodeController = nodeControllerMap[from];
                    var connections = from.GetPort("output").GetConnections();

                    foreach (var connection in connections)
                    {
                        if (connection.node is SkillNode to)
                        {
                            if (nodeControllerMap.TryGetValue(to, out var targetController))
                            {
                                nodeController.connections.Add(targetController);
                            }
                        }
                    }

                    var precedingSkills = from.GetPort("input").GetConnections();

                    foreach (var precedingSkill in precedingSkills)
                    {
                        if (precedingSkill.node is SkillNode to)
                        {
                            if (nodeControllerMap.TryGetValue(to, out var targetController))
                            {
                                nodeController.precedingSkills.Add(targetController);
                            }
                        }
                    }
                }
            }

            _nodes[0].UpdateSkillState();
        }

        private void GeneratePath(SkillTreeGraph skillTree)
        {
            foreach (var from in _nodes)
            {
                foreach (var to in from.connections)
                {
                    GameObject path = _poolSystem.Spawn(_pathPrefab, _content);
                    UISkillPathController pathController = path.GetComponent<UISkillPathController>();
                    pathController.Initialize(from.output, to.input, _nodeHeightOffset);
                    from.connectionPaths.Add(pathController);
                    _paths.Add(path);
                }

                from.UpdatePaths();
            }
        }

        private Vector2 GetStartPosition(int xCount)
        {
            float x = 0;

            if (xCount % 2 == 0)
            {
                float halfCount = xCount * 0.5f;
                x = _nodeWidthOffset * halfCount - _nodeWidthOffset * 0.5f;
            }
            else
            {
                float halfCount = (xCount - 1) * 0.5f;
                x = _nodeWidthOffset * halfCount;
            }

            return new Vector2(-x, -100);
        }
        #endregion

        #region Upgrade
        public void UpgradeSkill()
        {

        }
        #endregion
    }
}