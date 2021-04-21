using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MazeGenerator
{
    public static bool[,] GenerateMaze(int width, int height, Vector2Int startPoint, Vector2Int endPoint)
    {
        var maze = createMaze(width, height);
        var breakPoints = createBreakPoints(width, height);

        var point = new Vector2Int(1, 1);
        while (breakPoints.Count > 0)
        {
            var direction = createRandomDirection();
            var nextPoint = point + direction;

            if (IsPointOutsideOfMaze(maze, nextPoint))
            {
                continue;
            }

            if (willPointCreateLoop(maze, point, nextPoint))
            {
                continue;
            }

            breakPath(maze, breakPoints, point, nextPoint);
            point = nextPoint;
        }

        addSomeNoise(maze, width, height);

        maze[startPoint.y, startPoint.x] = false;
        maze[endPoint.y, endPoint.x] = false;

        return maze;
    }


    static bool[,] createMaze(int width, int height)
    {
        var maze = new bool[width, height];
        for (var i = 0; i < maze.GetLength(0); i++)
        {
            for (var j = 0; j < maze.GetLength(1); j++)
            {
                maze[i, j] = true;
            }
        }
        return maze;
    }

    static HashSet<Vector2Int> createBreakPoints(int width, int height)
    {
        var breakPoints = new HashSet<Vector2Int>();
        for (var i = 1; i < height; i += 2)
        {
            for (var j = 1; j < width; j += 2)
            {
                breakPoints.Add(new Vector2Int(j, i));
            }
        }
        return breakPoints;
    }

    static Vector2Int createRandomDirection()
    {
        var step = 2;
        var direction = Vector2Int.zero;
        direction.x = Random.Range(-1, 2) * step;
        direction.y = direction.x == 0 ? (Random.value > 0.5f ? -step : step) : 0;
        return direction;
    }

    public static bool IsPointOutsideOfMaze(bool[,] maze, Vector2Int point)
    {
        return point.x < 0 || point.y < 0 || point.x > maze.GetLength(0) - 1 || point.y > maze.GetLength(1) - 1;
    }

    static bool willPointCreateLoop(bool[,] maze, Vector2Int startPoint, Vector2Int endPoint)
    {
        var center = (startPoint + endPoint) / 2;
        return !maze[startPoint.y, startPoint.x] && maze[center.y, center.x] && !maze[endPoint.y, endPoint.x];
    }

    static void breakPath(bool[,] maze, HashSet<Vector2Int> breakPoints, Vector2Int startPoint, Vector2Int endPoint)
    {
        maze[startPoint.y, startPoint.x] = false;
        breakPoints.Remove(startPoint);

        var center = (startPoint + endPoint) / 2;
        maze[center.y, center.x] = false;
        breakPoints.Remove(center);

        maze[endPoint.y, endPoint.x] = false;
        breakPoints.Remove(endPoint);
    }

    static void addSomeNoise(bool[,] maze, int width, int height)
    {
        for (var i = 0; i < height; i += 2)
        {
            for (var j = 0; j < width; j += 2)
            {
                var wallsAround = getWallsAround(maze, new Vector2Int(j, i));
                removeRandomWalls(maze, wallsAround, new Vector2Int(j, i));
            }
        }
    }

    static HashSet<Vector2Int> getWallsAround(bool[,] maze, Vector2Int startPoint)
    {
        Vector2Int[] sides = new Vector2Int[4];
        HashSet<Vector2Int> resultSides = new HashSet<Vector2Int>();

        sides[0] = startPoint + Vector2Int.up;
        sides[1] = startPoint + Vector2Int.down;
        sides[2] = startPoint + Vector2Int.left;
        sides[3] = startPoint + Vector2Int.right;

        foreach (var side in sides)
        {
            if (IsPointOutsideOfMaze(maze, side))
            {
                continue;
            }
            if (maze[side.y, side.x] == true)
            {
                resultSides.Add(side);
            }
        }

        return resultSides;
    }

    static void removeRandomWalls(bool[,] maze, HashSet<Vector2Int> walls, Vector2Int startPoint)
    {
        byte howMuchToRemove = 0;
    
        if (walls.Count < 2)
        {
            return;
        } 
        else if (walls.Count == 2)
        {
            howMuchToRemove = (byte)Random.Range(0, 2);
        }
        else
        {
            howMuchToRemove = (byte)Random.Range(1, 2);
        }

        foreach (var wall in walls)
        {
            if (howMuchToRemove <= 0)
            {
                return;
            }

            maze[wall.y, wall.x] = false;
        }

        maze[startPoint.y, startPoint.x] = false;
    }

    public static Vector2Int ConvertToMazeCoord(Vector3 coord)
    {
        return new Vector2Int(Mathf.RoundToInt(coord.x), -Mathf.RoundToInt(coord.z));
    }
}
