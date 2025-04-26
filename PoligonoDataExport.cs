using OpenTK;
using OpenTK.Graphics;
using System.Collections.Generic;

public class PoligonoDataExport
{
    public List<Vector3Serializable> Vertices { get; set; }
    public Vector3Serializable Posicion { get; set; }
    public Vector3Serializable Escala { get; set; }
    public float RotacionX { get; set; }
    public float RotacionY { get; set; }
    public float RotacionZ { get; set; }
    public string Color { get; set; }  // Guardamos el color como string HEX o RGBA
}
public class Vector3Serializable
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Vector3Serializable(Vector3 v)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
    }
}
public class ColorSerializable
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public float A { get; set; }

    public ColorSerializable(Color4 color)
    {
        R = color.R;
        G = color.G;
        B = color.B;
        A = color.A;
    }
}