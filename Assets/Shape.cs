using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    private Vector3 scale;
    private Rigidbody rb;
    private Material material;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        material = gameObject.GetComponent<MeshRenderer>().material;
    }

    public ShapeData ToShapeData()
    {
        return new ShapeData(
            Vector3Serializable.FromVector(transform.localScale), ColorRGB.FromColor(material.color),
            Vector3Serializable.FromVector(transform.position), Vector3Serializable.FromVector(rb.velocity),
            Vector3Serializable.FromVector(rb.angularVelocity),
            Vector3Serializable.FromVector(transform.rotation.eulerAngles));
    }
}