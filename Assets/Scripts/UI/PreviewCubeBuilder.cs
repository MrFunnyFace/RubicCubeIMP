//using UnityEngine;
//using UnityEngine.UI;

//public class PreviewCubeBuilder : MonoBehaviour
//{
//    [Header("UI")]
//    public RawImage targetImage;

//    [Header("Preview Objects")]
//    public Camera previewCamera;
//    public RubikCubeGenerator cubeGenerator;

//    [Header("Render")]
//    public int textureSize = 256;

//    private RenderTexture renderTexture;

//    public void BuildFromSave(SaveData data)
//    {
//        // RenderTexture
//        renderTexture = new RenderTexture(textureSize, textureSize, 16);
//        renderTexture.Create();

//        previewCamera.targetTexture = renderTexture;
//        targetImage.texture = renderTexture;

//        // Генерация куба
//        cubeGenerator.cubeSize = data.cubeSize;
//        cubeGenerator.GenerateCube();

//        // Применяем сохранённое состояние
//        ApplyCubies(data.cubeData);

//        // Камера
//        var camCtrl = previewCamera.GetComponent<UICubeCameraController>();
//        camCtrl.RepositionCamera();
//    }

//    void ApplyCubies(CubeSaveData data)
//    {
//        foreach (var c in data.cubies)
//        {
//            Cubie cubie = cubeGenerator.cubies
//                .Find(x => x.Index == new Vector3Int(c.x, c.y, c.z));

//            if (cubie == null) continue;

//            cubie.transform.localPosition = c.localPosition;
//            cubie.transform.localRotation = c.localRotation;
//            cubie.SetFaceColors(c.faceColors);
//        }
//    }

//    private void OnDestroy()
//    {
//        if (renderTexture != null)
//            renderTexture.Release();
//    }
//}