using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] Transform wall = default;
    [SerializeField] GameObject enemy = default;
    [SerializeField] GameObject player = default;
    [SerializeField] GameObject finish = default;

    public bool[,] MazeInfo { get; private set; }
    public List<Vector2Int> FreeCells { get; private set; }

    Vector2Int startPos;
    Vector2Int endPos = Vector2Int.zero;
    HashSet<Enemy> enemies = new HashSet<Enemy>();
    HashSet<MeshRenderer> fovsRenderers = new HashSet<MeshRenderer>();

    void Awake() {
        spawnPlayer();

        MazeInfo = MazeGenerator.GenerateMaze(10, 10, startPos, endPos);
        generateVisual(MazeInfo);

        FreeCells = getFreeCells(MazeInfo); 
        enemies = spawnEnemies();

        foreach (var item in enemies)
        {
            fovsRenderers.Add(item.transform.GetChild(0).GetComponent<MeshRenderer>());
        }

        spawnFinish();
    }

    int convertToOdd(int even)
    {
        if (even % 2 == 0)
            return even - 1;
        return even;
    }

    void generateVisual(bool[,] maze)
    {
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                if (maze[i, j])
                {
                    spawnWall(new Vector2(j, i));
                }    
            }
        }
    }

    void spawnWall(Vector2 position)
    {
        Instantiate(wall, new Vector3(position.x, 0.5f, -position.y), Quaternion.identity);
    }

    HashSet<Enemy> spawnEnemies()
    {
        HashSet<Enemy> enemies = new HashSet<Enemy>();

        var freeCells = getFreeCells(MazeInfo);
        for (int i = 0; i < 2; i++)
        {
            var spawnPos = -Vector2Int.one;
            while (spawnPos == -Vector2Int.one)
            {
                var tmpPos = freeCells[Random.Range(0, freeCells.Count - 1)];
                if (Vector2Int.Distance(startPos, tmpPos) < 6)
                {
                    continue;
                }
                spawnPos = tmpPos;
            }
            enemies.Add(Instantiate(enemy, new Vector3(spawnPos.x, 1f,
                -spawnPos.y), Quaternion.identity).GetComponent<Enemy>());
        }

        return enemies;
    }

    void spawnFinish()
    {
        Instantiate(finish, new Vector3(endPos.x, 0.5f,
            -endPos.y), Quaternion.identity);
    }

    void spawnPlayer()
    {
        startPos = new Vector2Int(convertToOdd(Random.Range(1, 11)), convertToOdd(Random.Range(1, 11)));
        while (endPos == Vector2Int.zero)
        {
            var tmpEndPos = new Vector2Int(convertToOdd(Random.Range(1, 11)), convertToOdd(Random.Range(1, 11)));
            if (Vector2Int.Distance(tmpEndPos, startPos) < 5)
            {
                continue;
            }
            endPos = tmpEndPos;
        }

        Instantiate(player, new Vector3(startPos.x, 0.5f,
            -startPos.y), Quaternion.identity);
    }

    public void CallAlarm()
    {
        foreach (var item in enemies)
        {
            item.Alarm();
        }
        foreach (var item in fovsRenderers)
        {
            item.material.SetColor("_EmissionColor", Color.red);
        }
    }

    public List<Vector2Int> getFreeCells(bool[,] maze)
    {
        List<Vector2Int> freeCells = new List<Vector2Int>();
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                if (!maze[i, j])
                    freeCells.Add(new Vector2Int(j, i));
            }
        }

        return freeCells;
    }
}
