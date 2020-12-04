using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
[XmlRoot("ShapeData")]
public class ShapeData
{
    [XmlAttribute("scale")] public Vector3Serializable Scale;
    [XmlAttribute("color")] public ColorRGB Color;
    [XmlAttribute("position")] public Vector3Serializable Position;
    [XmlAttribute("velocity")] public Vector3Serializable Velocity;
    [XmlAttribute("angularVelocity")] public Vector3Serializable AngularVelocity;
    [XmlAttribute("rotation")] public Vector3Serializable Rotation;

    public ShapeData(Vector3Serializable scale, ColorRGB color, Vector3Serializable position,
        Vector3Serializable velocity, Vector3Serializable angularVelocity,
        Vector3Serializable rotation)
    {
        Scale = scale;
        Color = color;
        Position = position;
        Velocity = velocity;
        AngularVelocity = angularVelocity;
        Rotation = rotation;
    }
}

[System.Serializable]
public class Vector3Serializable
{
    [XmlAttribute("x")] public float x;
    [XmlAttribute("y")] public float y;
    [XmlAttribute("z")] public float z;

    public Vector3Serializable(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Vector3Serializable FromVector(Vector3 vector)
    {
        return new Vector3Serializable(vector.x, vector.y, vector.z);
    }

    public Vector3 ToVector3()
    {
        return new Vector3(this.x, this.y, this.z);
    }
}

[System.Serializable]
public class ColorRGB
{
    [XmlAttribute("r")] public float r;
    [XmlAttribute("g")] public float g;
    [XmlAttribute("b")] public float b;
    [XmlAttribute("a")] public float a;

    public ColorRGB(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public static ColorRGB FromColor(Color color)
    {
        return new ColorRGB(color.r, color.g, color.b, color.a);
    }

    public Color ToUnityColor()
    {
        return new Color(r, g, b, a);
    }
}