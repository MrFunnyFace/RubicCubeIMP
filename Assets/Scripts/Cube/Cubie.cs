using System.Collections.Generic;
using UnityEngine;

public class Cubie : MonoBehaviour
{
    public int ID { get; private set; } // уникальный ID
    public Vector3Int Index { get; private set; }

    [System.Serializable]
    public class Face
    {
        public CubeFace face;
        public Renderer renderer;
    }

    public List<Face> faces = new();

    public void Init(int id, Vector3Int index)
    {
        ID = id;
        SetIndex(index);
    }
    public Face GetFace(CubeFace faceType)
    {
        return faces.Find(f => f.face == faceType);
    }

    public void SetIndex(Vector3Int index)
    {
        Index = index;
        name = $"Cubie [{index.x},{index.y},{index.z}]";
    }
    public Material GetMaterial(CubeFace face)
    {
        var f = faces.Find(x => x.face == face);
        return f != null ? f.renderer.material : null;
    }

    public void SetFaceColor(CubeFace face, Material mat)
    {
        foreach (var f in faces)
            if (f.face == face)
                f.renderer.material = mat;
    }

    public void Select()
    {
        foreach (var f in faces)
            if (f.renderer != null)
                f.renderer.material.EnableKeyword("_EMISSION");
    }

    public void Deselect()
    {
        foreach (var f in faces)
            if (f.renderer != null)
                f.renderer.material.DisableKeyword("_EMISSION");
    }

}





