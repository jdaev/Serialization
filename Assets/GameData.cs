using System.Xml.Serialization;

[System.Serializable]
[XmlRoot("GameData")]
public class GameData
{
    public ShapeData[] spheres;
    public ShapeData[] cubes;
    public GameData(){}
    public GameData(ShapeData[] spheres, ShapeData[] cubes)
    {
        this.spheres = spheres;
        this.cubes = cubes;
    }
}