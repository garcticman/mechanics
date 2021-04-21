using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MaterialManager : MonoBehaviour
{
    public MeshRenderer MeshRenderer { get; private set; }
    public bool CanDoDamage { get; private set; } = true;

    void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
    }
    public void SetMaterialToFracts(Material material)
    {
        foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material = material;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ColorLine")
        {
            MeshRenderer.material = other.GetComponent<MeshRenderer>().material;

            StartCoroutine(BecomeSafer());
        }
    }

    IEnumerator BecomeSafer()
    {
        CanDoDamage = false;
        yield return new WaitForSeconds(1.5f);
        CanDoDamage = true;
    }
}
