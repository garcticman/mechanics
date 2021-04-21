using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] LevelBuilder levelBuilder;

    public float Fov = 60f;
    public float Step = 1f;
    public float Distance = 5f;

    Mesh mesh
    {
        get
        {
            return GetComponent<MeshFilter>().mesh;
        }
        set
        {
            GetComponent<MeshFilter>().mesh = value;
        }
    }

    private void Start()
    {
        if (levelBuilder == null)
        {
            levelBuilder = Camera.main.GetComponent<LevelBuilder>();
        }
    }

    void OnEnable()
    {
        mesh = new Mesh();
    }

    void OnDisable()
    {
        Destroy(mesh);
    }

    void Update()
    {
        var points = createTargetPointsAndHandlePlayerFinding();
        updateMesh(points);
    }

    Vector3[] createTargetPointsAndHandlePlayerFinding()
    {
        var targetPoints = new List<Vector3>();
        targetPoints.Add(Vector3.zero);

        for (var fov = -Fov * 0.5f; fov < Fov * 0.5f; fov += Step)
        {
            var cast = Physics.Raycast
            (
                origin: transform.position,
                direction: Quaternion.Euler(0f, fov, 0f) * transform.forward,
                hitInfo: out var hitInfo,
                maxDistance: Distance,
                layerMask: Physics.DefaultRaycastLayers,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore
            );

            handlePlayerFinding(cast, hitInfo);
            
            var point = Quaternion.Euler(0f, fov, 0f) * Vector3.forward * (cast ? hitInfo.distance : Distance);
            targetPoints.Add(point);
        }
        return targetPoints.ToArray();
    }

    void handlePlayerFinding(bool cast, RaycastHit hitInfo)
    {
        if (cast && hitInfo.collider.tag == "Player")
        {
            levelBuilder.CallAlarm();
        }
    }

    void updateMesh(Vector3[] points)
    {
        // old
        mesh.Clear();

        // vertices
        mesh.SetVertices(points);

        // triangles
        var triangles = new List<int>();
        for (var i = 1; i < points.Length - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }
        mesh.SetTriangles(triangles, 0);

        // normals
        var normals = points.Select(_ => Vector3.up).ToArray();
        mesh.SetNormals(normals);
    }
}
