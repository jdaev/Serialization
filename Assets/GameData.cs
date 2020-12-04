using System.Xml.Serialization;

[System.Serializable]
[XmlRoot("GameData")]
public class GameData
{
    [XmlAttribute("spheres")] public ShapeData[] spheres;
    [XmlAttribute("cubes")] public ShapeData[] cubes;

    public GameData(ShapeData[] spheres, ShapeData[] cubes)
    {
        this.spheres = spheres;
        this.cubes = cubes;
    }
}