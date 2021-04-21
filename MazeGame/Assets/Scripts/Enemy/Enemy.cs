using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] LevelBuilder levelBuilder;
    [SerializeField] float enemySpeed = 0.2f;

    Pathfinding pathfinding;
    Player player;
    Transform fov;

    Vector2Int targetCell;
    List<Vector2Int> path;
    bool[,] maze;
    bool alarm = false;
    int pathIndex = 0;

    void Start()
    {
        initData();
        initPathfinding();
    }

    void initData()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        fov = transform.GetChild(0);

        if (levelBuilder == null)
        {
            levelBuilder = Camera.main.GetComponent<LevelBuilder>();
        }
        maze = levelBuilder.MazeInfo;
    }

    void initPathfinding()
    {
        targetCell = levelBuilder.FreeCells[Random.Range(0, levelBuilder.FreeCells.Count - 1)];
        pathfinding = new Pathfinding(maze);
        var startPos = new Vector2Int((int)transform.position.x, -(int)transform.position.z);
        path = pathfinding.CalculatePath(startPos, targetCell);
    }

    void Update()
    {
        handleMovement();

        if (!alarm)
        {
            if (pathIndex >= path.Count)
            {
                targetCell = levelBuilder.FreeCells[Random.Range(0, levelBuilder.FreeCells.Count - 1)];
                calculateNewPath(targetCell);
            }
        }
        else
        {
            if (pathIndex >= path.Count || targetCell != MazeGenerator.ConvertToMazeCoord(player.transform.position))
            {
                targetCell = MazeGenerator.ConvertToMazeCoord(player.transform.position);
                calculateNewPath(MazeGenerator.ConvertToMazeCoord(player.transform.position));
            }
        }

        if (player.Noise >= 10)
        {
            levelBuilder.CallAlarm();
        }
    }

    void handleMovement()
    {
        var targetPos = path[pathIndex];
        Vector3 dir = new Vector3(targetPos.x, transform.position.y, -targetPos.y) - transform.position;
        dir.y = 0;
        float speed = Time.deltaTime * enemySpeed;
        transform.Translate(dir.normalized * speed);
        fov.rotation = Quaternion.Lerp(fov.rotation, Quaternion.LookRotation(dir.normalized), Time.deltaTime * 2);
        if (dir.magnitude <= speed)
        {
            pathIndex++;
        }
    }

    void calculateNewPath(Vector2Int target)
    {
        path = pathfinding.CalculatePath(MazeGenerator.ConvertToMazeCoord(transform.position),
                    target);
        pathIndex = 0;
    }

    public void Alarm()
    {
        if (!alarm)
        {
            alarm = true;
            path = pathfinding.CalculatePath(MazeGenerator.ConvertToMazeCoord(transform.position),
                MazeGenerator.ConvertToMazeCoord(player.transform.position));
            pathIndex = 0;
        }
    }
}
