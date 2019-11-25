using UnityEngine;
using System.Collections;

public class TextToTexture : MonoBehaviour
{
    public static TextToTexture Main { get; private set; }

    [System.Serializable]
    public class TextureDetails
    {
        public string path;
        public Vector3 pos;
        public Vector3 rot;
        public float cameraSize;
        public bool reflective;
    }

    public Transform rocketParent;

    public int textureSize = 256;

    public TextureDetails[] textureParams;

    public Cubemap cubemap;
    public Color mainColor,
                 reflectionColor;

    RenderTexture rtexCompare;

    Transform text3d;

    void Awake()
    {
        if (!Main)
            Main = this;
    }

    void Start()
    {
        if (!rocketParent)
            Debug.LogError("Set rocket's parent, please", transform);

        text3d = transform.GetChild(0);

        if (!text3d)
            Debug.LogError("Put 3d text inide, please", transform);
    }

    public void ApplyText(string text)
    {
        GetComponent<Camera>().enabled = true;
        text3d.GetComponent<TextMesh>().text = text;

        if (rocketParent)
        {
            rtexCompare = new RenderTexture(textureSize, textureSize, 32);
            RenderTexture.active = rtexCompare;
            GetComponent<Camera>().GetComponent<Camera>().targetTexture = rtexCompare;

            for (int i = 0; i < textureParams.Length; i++)
            {
                TextureDetails item = textureParams[i];

                if (!string.IsNullOrEmpty(item.path))
                {
                    Transform targetMesh = rocketParent.Find(item.path);

                    if (targetMesh)
                    {
                        text3d.localPosition = item.pos;
                        text3d.localEulerAngles = item.rot;
                        GetComponent<Camera>().orthographicSize = item.cameraSize;
                        GetComponent<Camera>().Render();

                        Material mat = targetMesh.GetComponent<Renderer>().material;

                        Texture2D mainTex = mat.mainTexture as Texture2D;
                        Texture2D newtex = new Texture2D(rtexCompare.width, rtexCompare.height);
                        newtex.ReadPixels(new Rect(0, 0, rtexCompare.width, rtexCompare.height), 0, 0);
                        newtex.Apply();

                        if (item.reflective)
                        {
							if (Shader.Find("Custom/ReflectiveDecal") != null) {
								Material newMat = new Material(Shader.Find("Custom/ReflectiveDecal"));
								newMat.SetTexture("_MainTex", mainTex);
								newMat.SetTexture("_DecalTex", newtex);
								newMat.SetTexture("_Cube", cubemap);
								newMat.SetColor("_Color", mainColor);
								newMat.SetColor("_ReflectColor", reflectionColor);
								targetMesh.GetComponent<Renderer>().material = newMat;
							}
                        }
                        else
                        {
                            Material newMat = new Material(Shader.Find("Decal"));
                            newMat.SetTexture("_MainTex", mainTex);
                            newMat.SetTexture("_DecalTex", newtex);

                            targetMesh.GetComponent<Renderer>().material = newMat;
                        }
                    }
                    else
                        Debug.LogError("Couldn't find child with path: " + item.path + " inside " + rocketParent.name, rocketParent);

                    //Debug.Break();
                }
            }

            GetComponent<Camera>().enabled = false;
            RenderTexture.active = null;
            rtexCompare.Release();
            Destroy(rtexCompare);
        }
    }
    /*
    public bool test = false;

    void Update()
    {
        if (test)
        {
            test = false;
            ApplyText("test text");
            
        }
    }
     */
}
