using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Material))]
public class Obstacle : MonoBehaviour
{
    string colorName;

    // Start is called before the first frame update
    void Start()
    {
        colorName = GetComponent<MeshRenderer>().material.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
