using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MainScript : MonoBehaviour
{
    public static MainScript Instance;

    public Collider worldBounds;

    public int shapeCount = 10;

    private Shape sphereResource;
    private Shape cubeResource;

    private List<Shape> spheres = new List<Shape>();
    private List<Shape> cubes = new List<Shape>();

    private void Awake()
    {
        sphereResource = Resources.Load<Shape>("Prefabs/Sphere");
        cubeResource = Resources.Load<Shape>("Prefabs/Cube");
    }

    void Start()
    {
        RandomSpawn();
    }

    private void RandomSpawn()
    {
        for (int i = 0; i < shapeCount; i++)
        {
            float r = Random.Range(-1, 1);

            Shape sp = SpawnShape(r < 0 ? sphereResource : cubeResource, RandomPositionInBounds(worldBounds.bounds),
                Quaternion.identity,
                Random.Range(0, 5), Random.ColorHSV());
            while (IsOverlapping(sp))
            {
                sp.transform.position = RandomPositionInBounds(worldBounds.bounds);
            }

            if (r < 0)
                spheres.Add(sp);
            else cubes.Add(sp);
        }
    }

    public void RandomRespawn()
    {
        foreach (Shape cube in cubes)
        {
            GameObject.Destroy(cube);
        }

        foreach (Shape sphere in spheres)
        {
            GameObject.Destroy(sphere);
        }
        cubes.Clear();
        spheres.Clear();
        RandomSpawn();
    }

    private Shape SpawnShape(Shape shapeResource, Vector3 spawnPosition, Quaternion rotation, float scale, Color color)
    {
        Shape sp = GameObject.Instantiate(shapeResource, spawnPosition,
            rotation);
        Material spMaterial = sp.GetComponent<MeshRenderer>().material;
        spMaterial.color = color;
        sp.transform.localScale = Vector3.one * scale;

        return sp;
    }

    private bool IsOverlapping(Shape shape)
    {
        Collider shapeCollider = shape.gameObject.GetComponent<Collider>();
        foreach (Shape cube in cubes)
        {
            Collider cubeCollider = cube.GetComponent<Collider>();
            if (shapeCollider.bounds.Intersects(cubeCollider.bounds))
            {
                return true;
            }
        }

        foreach (Shape sphere in spheres)
        {
            Collider sphereCollider = sphere.GetComponent<Collider>();
            if (shapeCollider.bounds.Intersects(sphereCollider.bounds))
            {
                return true;
            }
        }

        return false;
    }

    private Vector3 RandomPositionInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    void Update()
    {
    }
}