using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Moving))]
public class Player : MonoBehaviour
{
    [SerializeField] Material[] materials = default;
    [SerializeField] Transform fracturedSphere = default;

    MeshRenderer meshRenderer;
    Moving moving;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        moving = GetComponent<Moving>();

        meshRenderer.material = getRandomMaterial();
    }

    void Update()
    {
        moving.HandleMoving();
    }

    void FixedUpdate()
    {
        moving.UpdatePlayerVelocity();
    }

    Material getRandomMaterial()
    {
        return materials[Random.Range(0, materials.Length)];
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Obstacle")
        {
            var materialManager = collision.gameObject.GetComponent<MaterialManager>();

            if (materialManager.CanDoDamage && !theSameMaterial(collision.gameObject.GetComponent<MeshRenderer>().material))
            {
                var instanceOfFracturedSphere = Instantiate(fracturedSphere, transform.position, transform.rotation);
                instanceOfFracturedSphere.GetComponent<MaterialManager>().SetMaterialToFracts(meshRenderer.material);
                foreach (Rigidbody rb in instanceOfFracturedSphere.GetComponentsInChildren<Rigidbody>())
                {
                    rb.velocity = moving.PlayerRigidbody.velocity;
                }
                Destroy(gameObject);
                Time.timeScale = 0.2f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ColorLine")
        {
            var instanceOfFracturedSphere = Instantiate(fracturedSphere, transform.position, transform.rotation);
            instanceOfFracturedSphere.GetComponent<MaterialManager>().SetMaterialToFracts(meshRenderer.material);
            foreach (Rigidbody rb in instanceOfFracturedSphere.GetComponentsInChildren<Rigidbody>())
            {
                rb.velocity = moving.PlayerRigidbody.velocity;
            }

            meshRenderer.material = other.GetComponent<MeshRenderer>().material;
        }
    }

    bool theSameMaterial(Material material)
    {
        return material.color == meshRenderer.material.color;
    }
}
