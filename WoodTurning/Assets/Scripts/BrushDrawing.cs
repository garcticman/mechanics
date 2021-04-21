using UnityEngine;

public class BrushDrawing : MonoBehaviour
{
    [SerializeField]
    Collider physBrush;
    [SerializeField]
    Collider woodCollider;
    [SerializeField]
    Material woodMaterial;
    [SerializeField]
    GameObject brushMask;
    [SerializeField]
    RenderTexture brush;
    [SerializeField]
    RenderTexture[] displaceTextures;
    [SerializeField]
    Material computingMaterial;

    Renderer brushRenderer;

    // Start is called before the first frame update
    void Start()
    {
        brushRenderer = brushMask.GetComponent<Renderer>();
        brushMask.GetComponent<Renderer>().sharedMaterial.SetFloat("_Distance", 2);

        ClearRenderTexture(brush);
        foreach (var texture in displaceTextures)
        {
            ClearRenderTexture(texture);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            UpdateDistance();
        }

        SetDisplaceTexture();
    }

    void UpdateDistance()
    {
        var brushPos = GetGameplayBrushPosition();

        var bottomOfWood = new Vector3(woodCollider.transform.position.x + woodCollider.bounds.size.x / 2, woodCollider.transform.position.y, woodCollider.transform.position.z);
        var boundPos = new Vector3(physBrush.transform.position.x - physBrush.bounds.size.x / 2, physBrush.transform.position.y, physBrush.transform.position.z);

        physBrush.transform.position = new Vector3(Mathf.Clamp(brushPos.x, woodCollider.transform.position.x - (boundPos.x - physBrush.transform.position.x), 10), brushPos.y, brushPos.z);
        brushMask.transform.position = new Vector3(brushMask.transform.position.x, brushMask.transform.position.y, -brushPos.z);

        var length = woodCollider.transform.position.x - bottomOfWood.x;


        var coef = Mathf.Clamp((-bottomOfWood.x - boundPos.x + 1) / length, 0, 1);
        brushRenderer.sharedMaterial.SetFloat("_Distance", coef);
    }

    void SetDisplaceTexture()
    {
        computingMaterial.SetTexture("_BrushTexture", brush);

        var inputDisplace = displaceTextures[Time.frameCount % 2];
        computingMaterial.SetTexture("_DisplaceTexture", inputDisplace);

        var outputDisplace = displaceTextures[(Time.frameCount + 1) % 2];
        Graphics.Blit(brush, outputDisplace, computingMaterial);
        woodMaterial.SetTexture("_MainTexture", outputDisplace);
    }

    void ClearRenderTexture(RenderTexture renderTexture)
    {
        var renderTarget = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = renderTarget;
    }

    Vector3 GetGameplayBrushPosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast
        (
            ray: ray,
            hitInfo: out var hitInfo,
            maxDistance: 100f,
            layerMask: LayerMask.NameToLayer("Raycast")
        );

        var position = hitInfo.point;
        position.y = Mathf.Min(0f, position.y);
        return position;
    }
}
