using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// A* 알고리즘 시스템
    /// </summary>
    public class AStarSystem : MonoBehaviour, IBattleSystem
    {
        public enum NodeTag
        {
            Ground,
            Sky,
            // 필요한 다른 태그를 추가
        }

        [System.Serializable]
        public class Node
        {
            public Vector3 Position;
            public bool Walkable;
            public NodeTag Tag;
            public float Cost;
            public Node Parent;

            public Node(Vector3 position, bool walkable, NodeTag tag)
            {
                Position = position;
                Walkable = walkable;
                Tag = tag;
                Cost = float.MaxValue;
                Parent = null;
            }
        }

        public Transform mapParent;
        private Dictionary<NodeTag, List<Node>> graph;

        public void Initialize()
        {
            InitializeGraph();
        }

        public void Deinitialize()
        {
            graph = null;
        }

        #region 그래프 찾기
        private void InitializeGraph()
        {
            graph = new Dictionary<NodeTag, List<Node>>();
            NodeTag[] tags = (NodeTag[])System.Enum.GetValues(typeof(NodeTag));
            FindNodes(mapParent, tags);
        }

        private void FindNodes(Transform parent, NodeTag[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                if (parent.CompareTag(tags[i].ToString()))
                {
                    Vector3 position = parent.position;
                    Node newNode = new Node(position, true, tags[i]);

                    if (!graph.TryGetValue(tags[i], out var nodeList))
                    {
                        nodeList = new List<Node>();
                        graph[tags[i]] = nodeList;
                    }

                    nodeList.Add(newNode);
                    break;
                }
            }

            foreach (Transform child in parent)
            {
                FindNodes(child, tags);
            }
        }
        #endregion

        public Queue<Vector3> SearchPath(Vector3 start, Vector3 target, NodeTag tag)
        {
            List<Node> openList = new List<Node>();
            HashSet<Node> closedList = new HashSet<Node>();

            Node startNode = GetClosestNode(start, tag);
            Node targetNode = GetClosestNode(target, tag);

            // 시작 또는 목표 노드가 유효하지 않은 경우
            if (startNode == null || targetNode == null)
            {
                return new Queue<Vector3>();
            }

            startNode.Cost = 0;
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = GetLowestCostNode(openList);

                if (currentNode == targetNode)
                {
                    return ReconstructPath(currentNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (Node neighbor in GetNeighbors(currentNode, tag))
                {
                    if (closedList.Contains(neighbor) || !neighbor.Walkable)
                    {
                        continue;
                    }

                    float tentativeCost = currentNode.Cost + (currentNode.Position - neighbor.Position).sqrMagnitude;

                    if (tentativeCost < neighbor.Cost)
                    {
                        neighbor.Cost = tentativeCost;
                        neighbor.Parent = currentNode;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }

            return new Queue<Vector3>();
        }


        public void Move(Transform unitTransform, Queue<Vector3> path, float moveSpeed)
        {
            if (path == null || path.Count == 0)
            {
                return;
            }

            Vector3 currentTarget = path.Peek();
            float distanceToTarget = (unitTransform.position - currentTarget).sqrMagnitude;

            if (distanceToTarget < 0.01f)
            {
                path.Dequeue();
                if (path.Count > 0)
                {
                    currentTarget = path.Peek();
                }
                else
                {
                    return;
                }
            }

            unitTransform.position = Vector3.MoveTowards(unitTransform.position, currentTarget, moveSpeed * Time.deltaTime);
        }


        public void Move(Transform unitTransform, Queue<Vector3> path, float moveSpeed, out float remainingDistance)
        {
            if (path == null || path.Count == 0)
            {
                remainingDistance = 0f;
                return;
            }

            Vector3 currentTarget = path.Peek();
            float distanceToTarget = (unitTransform.position - currentTarget).sqrMagnitude;

            if (distanceToTarget < 0.01f)
            {
                path.Dequeue();
                if (path.Count > 0)
                {
                    currentTarget = path.Peek();
                }
                else
                {
                    remainingDistance = 0f;
                    return;
                }
            }

            unitTransform.position = Vector3.MoveTowards(unitTransform.position, currentTarget, moveSpeed * Time.deltaTime);
            remainingDistance = CalculateRemainingDistance(unitTransform.position, path);
        }

        private float CalculateRemainingDistance(Vector3 currentPosition, Queue<Vector3> path)
        {
            float totalDistance = 0f;
            Vector3 previousPosition = currentPosition;

            foreach (var point in path)
            {
                totalDistance += (previousPosition - point).magnitude;
                previousPosition = point;
            }

            return totalDistance;
        }

        private Node GetClosestNode(Vector3 position, NodeTag tag)
        {
            Node closestNode = null;
            float closestSqrDistance = float.MaxValue; // 제곱거리로 변경

            // 태그에 해당하는 노드들만 검색
            if (graph.TryGetValue(tag, out List<Node> nodes))
            {
                foreach (Node node in nodes)
                {
                    float sqrDistance = (position - node.Position).sqrMagnitude;
                    if (sqrDistance < closestSqrDistance)
                    {
                        closestNode = node;
                        closestSqrDistance = sqrDistance;
                    }
                }
            }

            return closestNode;
        }

        private Node GetLowestCostNode(List<Node> openList)
        {
            Node lowestCostNode = openList[0];

            foreach (Node node in openList)
            {
                if (node.Cost < lowestCostNode.Cost)
                {
                    lowestCostNode = node;
                }
            }

            return lowestCostNode;
        }

        private List<Node> GetNeighbors(Node currentNode, NodeTag tag)
        {
            List<Node> neighbors = new List<Node>();
            int x = (int)currentNode.Position.x;
            int y = (int)currentNode.Position.z; // y축 대신 z축 사용

            // 상하좌우의 이웃 노드를 추가
            if (x - 1 >= 0 && graph.TryGetValue(tag, out List<Node> nodes) && nodes.Contains(graph[tag][x - 1]))
                neighbors.Add(graph[tag][x - 1]);
            if (x + 1 < graph[tag].Count && graph.TryGetValue(tag, out nodes) && nodes.Contains(graph[tag][x + 1]))
                neighbors.Add(graph[tag][x + 1]);
            if (y - 1 >= 0 && graph.TryGetValue(tag, out nodes) && nodes.Contains(graph[tag][y - 1]))
                neighbors.Add(graph[tag][y - 1]);
            if (y + 1 < graph[tag].Count && graph.TryGetValue(tag, out nodes) && nodes.Contains(graph[tag][y + 1]))
                neighbors.Add(graph[tag][y + 1]);

            return neighbors;
        }

        private Queue<Vector3> ReconstructPath(Node currentNode)
        {
            Queue<Vector3> path = new Queue<Vector3>();
            while (currentNode != null)
            {
                path.Enqueue(currentNode.Position);
                currentNode = currentNode.Parent;
            }

            return path;
        }

    }
}
