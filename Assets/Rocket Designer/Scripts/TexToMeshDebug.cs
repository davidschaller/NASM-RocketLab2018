using UnityEngine;
using System.Collections;

public class TexToMeshDebug : MonoBehaviour
{
    public int textureSize = 256;

    public MeshRenderer target;

    RenderTexture rtexCompare;
    // Use this for initialization
    Material newMat;


    void Start()
    {
        rtexCompare = new RenderTexture(textureSize, textureSize, 32);
        RenderTexture.active = rtexCompare;

        tex2D = new Texture2D(rtexCompare.width, rtexCompare.height);

        GetComponent<Camera>().GetComponent<Camera>().targetTexture = rtexCompare;
        GetComponent<Camera>().Render();

        Material mat = target.GetComponent<Renderer>().material;

        Texture2D mainTex = mat.mainTexture as Texture2D;

        newMat = new Material(Shader.Find("Decal"));

        newMat.SetTexture("_MainTex", mainTex);
        target.GetComponent<Renderer>().material = newMat;

        
    }

    Texture2D tex2D;

    // Update is called once per frame
    void Update()
    {
        GetComponent<Camera>().Render();
        newMat.SetTexture("_DecalTex", rtexCompare);
        target.GetComponent<Renderer>().material = newMat;
    }
}
