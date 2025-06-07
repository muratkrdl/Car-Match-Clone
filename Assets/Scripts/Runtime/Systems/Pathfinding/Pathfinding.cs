using System.Collections.Generic;
using Runtime.Enums;
using Runtime.Systems.GridSystem;
using UnityEngine;

namespace Runtime.Systems.Pathfinding
{
    public class Pathfinding
    {
        private static GridSystem<PathNode> _gridSystem;
        private static GridSystem<GridObject> _refGridSystem;
        private static List<PathNode> openList;
        private static List<PathNode> closedList;
        
        public Pathfinding(int width, int height, GridSystem<GridObject> refGridSystem)
        {
            _refGridSystem = refGridSystem;
            _gridSystem = new GridSystem<PathNode>(width, height, Vector2.zero, (GridSystem<PathNode> g, GridPosition gridpos) => new PathNode(g,gridpos));
        }

        public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
        {
            PathNode startNode = _gridSystem.GetGridObject(new GridPosition(startX, startY));
            PathNode endNode = _gridSystem.GetGridObject(new GridPosition(endX, endY));
            
            openList = new List<PathNode> { startNode };
            closedList = new List<PathNode>();

            for (int x = 0; x < _gridSystem.GetWidth(); x++)
            {
                for (int y = 0; y < _gridSystem.GetHeight(); y++)
                {
                    PathNode node = _gridSystem.GetGridObject(new GridPosition(x, y));
                    node.gCost = int.MaxValue;
                    node.CalculateFCost();
                    node.cameFromNode = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode) // Reach Goal
                {
                    return CalculatePath(endNode);
                }
                
                openList.Remove(currentNode);
                closedList.Add(currentNode);
                
                if (!_refGridSystem.GetGridObject(new GridPosition(currentNode.x, currentNode.y)).GetIsInteractable())
                {
                    continue;
                }
                
                foreach (var neighbourNode in GetNeighbourList(currentNode))
                {
                    if (closedList.Contains(neighbourNode) ||
                        !_refGridSystem.GetGridObject(new GridPosition(neighbourNode.x, neighbourNode.y)).GetIsInteractable()) continue;
                    
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }

            return null;
        }

        private List<PathNode> GetNeighbourList(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();
            
            if (currentNode.x -1 >= 0) // Left
            {
                neighbourList.Add(GetNode(currentNode.x -1, currentNode.y));
            }
            if (currentNode.x +1 < _gridSystem.GetWidth()) // Right
            {
                neighbourList.Add(GetNode(currentNode.x +1, currentNode.y));
            }
            if (currentNode.y -1 >= 0) // Down
            {
                neighbourList.Add(GetNode(currentNode.x, currentNode.y -1));
            }
            if (currentNode.y +1 < _gridSystem.GetWidth()) // Up
            {
                neighbourList.Add(GetNode(currentNode.x, currentNode.y +1));
            }

            return neighbourList;
        }

        private PathNode GetNode(int x, int y)
        {
            return _gridSystem.GetGridObject(new GridPosition(x, y));
        }

        private List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);
            PathNode currentNode = endNode;
            while (currentNode.cameFromNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.cameFromNode;
            }
            path.Reverse();
            
            return path;
        }

        private int CalculateDistanceCost(PathNode startNode, PathNode endNode)
        {
            int xDistance = Mathf.Abs(startNode.x - endNode.x);
            int yDistance = Mathf.Abs(startNode.y - endNode.y);
            int remaining = xDistance + yDistance;
            
            return remaining;;
        }

        private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostNode = pathNodeList[0];
            for (int i = 0; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                {
                    lowestFCostNode = pathNodeList[i];
                }
            }
            
            return lowestFCostNode;
        }
    }
}