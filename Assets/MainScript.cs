using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Newtonsoft.Json;
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
                Random.Range(0, 5) * Vector3.one, Random.ColorHSV());
            int recheckCount = 10;
            while (IsOverlapping(sp) && recheckCount>0)
            {
                sp.transform.position = RandomPositionInBounds(worldBounds.bounds);
                recheckCount--;
            }

            if (r < 0)
                spheres.Add(sp);
            else cubes.Add(sp);
        }
    }

    public void RandomRespawn()
    {
        ClearShapes();
        RandomSpawn();
    }

    private void ClearShapes()
    {
        foreach (Shape cube in cubes)
        {
            GameObject.Destroy(cube.gameObject);
        }

        foreach (Shape sphere in spheres)
        {
            GameObject.Destroy(sphere.gameObject);
        }

        cubes.Clear();
        spheres.Clear();
    }

    private Shape SpawnShape(Shape shapeResource, Vector3 spawnPosition, Quaternion rotation, Vector3 scale,
        Color color)
    {
        Shape sp = GameObject.Instantiate(shapeResource, spawnPosition,
            rotation);
        Material spMaterial = sp.GetComponent<MeshRenderer>().material;
        spMaterial.color = color;
        sp.transform.localScale = scale;

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

    private GameData GenerateSaveData()
    {
        ShapeData[] spheresData = new ShapeData[spheres.Count];
        ShapeData[] cubesData = new ShapeData[cubes.Count];
        for (int i = 0; i < spheresData.Length; i++)
        {
            Shape sphere = spheres[i];
            
            spheresData[i] = sphere.ToShapeData();
        }

        for (int i = 0; i < cubesData.Length; i++)
        {
            Shape cube = cubes[i];
            
            cubesData[i] = cube.ToShapeData();
        }

        return new GameData(spheresData, cubesData);
    }

    private void LoadGameFromGameData(GameData gameData)
    {
        ClearShapes();
        foreach (var t in gameData.spheres)
        {
            Vector3 scale = t.Scale.ToVector3();
            Color color = t.Color.ToUnityColor();
            Vector3 position = t.Position.ToVector3();
            Vector3 velocity = t.Velocity.ToVector3();
            Vector3 angularVelocity = t.AngularVelocity.ToVector3();
            Quaternion rotation = Quaternion.Euler(t.Rotation.ToVector3());
            Shape sp = SpawnShape(sphereResource, position,
                rotation,
                scale, color);
            Rigidbody rb = sp.GetComponent<Rigidbody>();
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
            spheres.Add(sp);
        }

        foreach (var t in gameData.cubes)
        {
            Vector3 scale = t.Scale.ToVector3();
            Color color = t.Color.ToUnityColor();
            Vector3 position = t.Position.ToVector3();
            Vector3 velocity = t.Velocity.ToVector3();
            Vector3 angularVelocity = t.AngularVelocity.ToVector3();
            Quaternion rotation = Quaternion.Euler(t.Rotation.ToVector3());
            Shape sp = SpawnShape(cubeResource, position,
                rotation,
                scale, color);
            Rigidbody rb = sp.GetComponent<Rigidbody>();
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
            cubes.Add(sp);
        }
    }

    public void SaveToBinary()
    {
        GameData gameData = GenerateSaveData();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + "/save_binary.dat");

        bf.Serialize(fileStream, gameData);
        fileStream.Close();

    }

    public void LoadFromBinary()
    {
        if (!File.Exists(Application.persistentDataPath + "/save_binary.dat")) return;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(Application.persistentDataPath + "/save_binary.dat", FileMode.Open);
        GameData gameData = (GameData) bf.Deserialize(fileStream);
        fileStream.Close();
        LoadGameFromGameData(gameData);
    }

    public void SaveToJSON()
    {
        GameData gameData = GenerateSaveData();
        string json = JsonUtility.ToJson(gameData);

        StreamWriter sw = File.CreateText(Application.persistentDataPath +"/save.json"); 
        sw.Close();

        File.WriteAllText(Application.persistentDataPath +"/save.json", json);
    }

    public void LoadFromJSON()
    {
        string json = File.ReadAllText(Application.persistentDataPath +"/save.json"); 

        LoadGameFromGameData(JsonUtility.FromJson<GameData>(json));
    }

    public void SaveToXML()
    {    
        GameData gameData = GenerateSaveData();
        FileStream fileStream = File.Create(Application.persistentDataPath + "/save.xml");
        var serializer = new XmlSerializer(typeof(GameData));
        serializer.Serialize(fileStream, gameData);
        fileStream.Close();
    }

    public void LoadFromXML()
    {
        if (!File.Exists(Application.persistentDataPath + "/save.xml")) return;
        var serializer = new XmlSerializer(typeof(GameData));
        FileStream fileStream = File.Open(Application.persistentDataPath + "/save.xml", FileMode.Open);
        var gameData = serializer.Deserialize(fileStream) as GameData;
        fileStream.Close();
        LoadGameFromGameData(gameData);
    }


    void Update()
    {
    }
}