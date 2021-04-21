using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    Vector2Int[] pointsAround = { 
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up,
        Vector2Int.down,
    };

    bool[,] map;
    public Pathfinding(bool[,] map)
    {
        this.map = map;
    }

    public List<Vector2Int> CalculatePath(Vector2Int startPos, Vector2Int endPos)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Collection<Node> openNodes = new Collection<Node>();
        Collection<Node> closedNodes = new Collection<Node>();


        Node startNode = new Node(null, startPos);
        startNode.HCost = getHeuristic(startPos, endPos);
        openNodes.Add(startNode);

        Node endNode = new Node(null, endPos);

        while (openNodes.Count > 0)
        {
            var currentNode = openNodes.OrderBy(node => node.FCost).First();
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            if (currentNode == endNode)
            {
                Node current = currentNode;
                while (current != startNode)
                {
                    path.Add(current.Position);
                    current = current.Parent;
                }
                path.Add(startPos);

                path.Reverse();
                return path;
            }

            List<Node> children = new List<Node>();
            foreach (var newPoint in pointsAround)
            {
                var nodePosition = currentNode.Position + newPoint;
                if (MazeGenerator.IsPointOutsideOfMaze(map, nodePosition))
                {
                    continue;
                }
                if (map[nodePosition.y, nodePosition.x])
                {
                    continue;
                }
                var newNode = new Node(currentNode, nodePosition);

                children.Add(newNode);
            }

            foreach (var child in children)
            {
                var modifiableChild = child;
                if (nodeContains(closedNodes, child))
                {
                    continue;
                }
                modifiableChild.GCost = currentNode.GCost + 1;
                if (nodeContainsAndHaveLessG(openNodes, modifiableChild))
                {
                    continue;
                }
                modifiableChild.HCost = getHeuristic(child.Position, endPos);

                openNodes.Add(modifiableChild);
            }
        }
        return default;
    }

    public uint getHeuristic(Vector2Int a, Vector2Int b)
    {
        return (uint)(Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y));
    }

    bool nodeContains(Collection<Node> nodes, Node searchingNode)
    {
        foreach (Node node in nodes)
        {
            if (searchingNode == node)
            {
                return true;
            }
        }
        return false;
    }

    bool nodeContainsAndHaveLessG(Collection<Node> nodes, Node searchingNode)
    {
        foreach (Node node in nodes)
        {
            if (searchingNode == node && searchingNode.GCost > node.GCost)
            {
                return true;
            }
        }
        return false;
    }

    class Node
    {
        public Node Parent;
        public uint GCost, HCost;
        public Vector2Int Position;
        public uint FCost
        {
            get { return GCost + HCost ; } 
        }

        public Node(Node parent, Vector2Int position)
        {
            Parent = parent;
            Position = position;
            HCost = GCost = 0;
        }
        public static bool operator == (Node node1, Node node2)
        {
            return node1.Position == node2.Position;
        }
        public static bool operator !=(Node node1, Node node2)
        {
            return node1.Position != node2.Position;
        }
    }
}
