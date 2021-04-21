using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] LevelBuilder levelBuilder = default;
    [SerializeField] RectTransform noiseImage = default;
    [SerializeField] int playerSpeed = default;

    public float Noise { get; private set; } = 0;

    bool[,] maze;
    bool isMoving = false;
    Vector2Int targetPos = new Vector2Int();

    void Start()
    {
        initData();
    }

    void initData()
    {
        if (levelBuilder == null)
        {
            levelBuilder = Camera.main.GetComponent<LevelBuilder>();
        }
        if (noiseImage == null)
        {
            noiseImage = GameObject.Find("NoiseImage").GetComponent<RectTransform>();
        }
        maze = levelBuilder.MazeInfo;
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
        if (!isMoving)
        {
            decreaseNoise();
        }
        setNoiseImageHeight(Noise * 10f);
    }

    void handleMovement()
    {
        var convertedPlayerPos = MazeGenerator.ConvertToMazeCoord(transform.position);
        if (Input.GetKey(KeyCode.W))
        {
            var newPos = convertedPlayerPos - Vector2Int.up;
            handleDirection(newPos);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            var newPos = convertedPlayerPos - Vector2Int.down;
            handleDirection(newPos);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            var newPos = convertedPlayerPos + Vector2Int.right;
            handleDirection(newPos);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            var newPos = convertedPlayerPos + Vector2Int.left;
            handleDirection(newPos);
        }

        Vector3 dir = new Vector3(targetPos.x, transform.position.y, -targetPos.y) - transform.position;
        dir.y = 0;
        float speed = Time.deltaTime * playerSpeed;
        transform.Translate(dir.normalized * speed);

        if (dir.magnitude <= speed)
        {
            isMoving = false;
            playerSpeed = 0;
        }
    }

    void decreaseNoise()
    {
        Noise = Mathf.Clamp(Noise - Time.deltaTime * 2, 0, 10);
    }

    void setNoiseImageHeight(float height)
    {
        noiseImage.sizeDelta = new Vector2(noiseImage.sizeDelta.x, height);
    }

    void handleDirection(Vector2Int newPos)
    {
        if (!MazeGenerator.IsPointOutsideOfMaze(maze, newPos) && !maze[newPos.y, newPos.x])
        {
            if (!isMoving)
            {
                targetPos = newPos;
                isMoving = true;
                playerSpeed = 1;
                Noise = Mathf.Clamp(Noise + 3, 0, 10);
            }
        }
    }

    public void Death()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Won()
    {
        SceneManager.LoadScene("WinScene");
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Death();
        }
        if (other.tag == "Finish")
        {
            Won();
        }
    }
}
