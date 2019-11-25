using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ShowGizmoType
{
    All,Vertices,CurvePath,Handles,Bounds,None
}

public class Backups
{
    public string TimeCreated;
    public float[,] Heights;
    public float[, ,] Splats;
    public SplatPrototype[] Splattextures;
    public Backups(string Timestamp,float[,] HeightMap, float[, ,] SplatMaps,SplatPrototype[] SplatProtos)
    {
        TimeCreated = Timestamp;
        Heights = HeightMap;
        Splats = SplatMaps;
        Splattextures = SplatProtos;
    }
}

[System.Serializable]
[AddComponentMenu("Enviroment/Path Generator")]
public class PathGenerator : MonoBehaviour 
{
    private Vector3[] Path = new Vector3[0];
    public List<Vector3> Points = new List<Vector3>();
    GameObject MeshObject;
    public Texture2D MeshTexture;
    public bool MeshX = true;

    public int Smoothness = 10;
    public int UVSmoothness = 1;

    public float SplatmapAlpha = 1;

    public int MeshHeightScale = 1;
    public int MeshWidthScale = 1;
    public bool TerriainRaise = false;
    public int TerriainRaiseHeight = 5;

    public int TrenchDepth = 0;

    public string MeshName = "Generated Mesh";

    public List<Backups> Backsups = new List<Backups>();

    public ShowGizmoType GizmoShown = ShowGizmoType.All;

    public Texture2D SplatmapTexture = null;

    public void AddPoint(Vector3 NewPoint)
    {
        Points.Add(NewPoint);
        if (Points.Count > 1)
        {
                Path = CreateCurve(Points, Smoothness);
        }
    }

    public void ClearPoints()
    {
        Points.Clear();
        Path = new Vector3[0];
    }

    public void RemovePoint(int index)
    {
        Points.RemoveAt(index);
        Remake();
    }

    public void Remake()
    {
        if (Points.Count > 1)
        {
                Path = CreateCurve(Points, Smoothness);
        }
    }

    public Vector3[] CreateCurve(List<Vector3> Points, int Steps)
    {

        Vector3[] PointList = new Vector3[Points.Count + 2];
        PointList[0] = Points[0];
        for (int i = 1; i < Points.Count + 1; i++)
        {
            PointList[i] = Points[i - 1];
        }
        PointList[PointList.Length - 1] = Points[Points.Count - 1];

        if (PointList.Length < 4)
        {
            return new Vector3[0];
        }

        List<Vector3> results = new List<Vector3>();

        for (int p = 1; p < PointList.Length - 2; p++)
        {
            for (int i = 0; i < Steps; i++)
            {
                results.Add(GetCurvePoint(PointList[p - 1], PointList[p], PointList[p + 1], PointList[p + 2], (1f / Steps) * i));
            }
        }
        results.Add(PointList[PointList.Length - 2]);
        return results.ToArray();
    }

    public Vector3 GetCurvePoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 result = new Vector3();

        float t0 = ((-t + 2f) * t - 1f) * t * 0.5f;
        float t1 = (((3f * t - 5f) * t) * t + 2f) * 0.5f;
        float t2 = ((-3f * t + 4f) * t + 1f) * t * 0.5f;
        float t3 = ((t - 1f) * t * t) * 0.5f;

        result.x = p0.x * t0 + p1.x * t1 + p2.x * t2 + p3.x * t3;
        result.y = p0.y * t0 + p1.y * t1 + p2.y * t2 + p3.y * t3;
        result.z = p0.z * t0 + p1.z * t1 + p2.z * t2 + p3.z * t3;

        return result;
    }

    public void FinishMesh()
    {
        if (MeshObject != null)
        {
            if (MeshName == MeshObject.name)
            {
                DestroyImmediate(MeshObject);
            }
        }
        MeshObject = new GameObject(MeshName, typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer));
        Mesh mesh = new Mesh();
        if (MeshX)
        {
            Mesh MeshTop = MakeMesh(new Vector3(0, MeshHeightScale, 0), false, 0, MeshHeightScale, MeshWidthScale);
            Mesh MeshBottom = MakeMesh(Vector3.zero, false, 0, MeshHeightScale, MeshWidthScale);
            Mesh MeshLeft = MakeMesh(Vector3.zero, true, 1, MeshHeightScale, MeshWidthScale);
            Mesh MeshRight = MakeMesh(Vector3.zero, true, 2, MeshHeightScale, MeshWidthScale);

            Mesh StartCap = new Mesh();
            StartCap.vertices = new Vector3[] { MeshTop.vertices[0], MeshTop.vertices[1], MeshBottom.vertices[0], MeshBottom.vertices[1] };
            StartCap.triangles = new int[] { 0, 1, 2, 1, 2, 3 };
            StartCap.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

            Mesh EndCap = new Mesh();
            EndCap.vertices = new Vector3[] { MeshTop.vertices[MeshTop.vertices.Length - 1], MeshTop.vertices[MeshTop.vertices.Length - 2], MeshBottom.vertices[MeshBottom.vertices.Length - 1], MeshBottom.vertices[MeshBottom.vertices.Length - 2] };
            EndCap.triangles = new int[] { 0, 1, 2, 1, 2, 3 };
            EndCap.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

            CombineInstance[] CombinedMeshes = new CombineInstance[6];
            CombinedMeshes[0].mesh = MeshBottom;
            CombinedMeshes[1].mesh = MeshTop;
            CombinedMeshes[2].mesh = MeshLeft;
            CombinedMeshes[3].mesh = MeshRight;
            CombinedMeshes[4].mesh = StartCap;
            CombinedMeshes[5].mesh = EndCap;

            mesh.CombineMeshes(CombinedMeshes, true, false);
            MeshObject.GetComponent<MeshFilter>().mesh = mesh;
        }
        else
        {
            mesh = MakeMesh(Vector3.zero, false, 0, MeshHeightScale, MeshWidthScale);
            MeshObject.GetComponent<MeshFilter>().mesh = mesh;
        }
        Material Texture = new Material(Shader.Find("PathGen MeshShader"));
        Texture.mainTexture = MeshTexture;
        MeshObject.GetComponent<Renderer>().sharedMaterial = Texture;
        MeshObject.GetComponent<Renderer>().sharedMaterial.shader = Shader.Find("PathGen MeshShader");

        if (TerriainRaise)
        {
            ModifyTerrain(TerriainRaiseHeight);
        }
    }

    public Mesh MakeMesh(Vector3 Offset,bool Vertical,int VerticalSide,int Height,int Width)
    {
        Mesh Mesh = new Mesh();
        Vector3[] MeshVertices = new Vector3[(Path.Length - 1) * 2];
        int VertIndex = 0;
        for (int i = 0; i < Path.Length; i++)
        {
            if ((i + 1) < Path.Length)
            {
            GameObject PlaceHolder = new GameObject();
            Transform PathPoint = PlaceHolder.transform;
            PathPoint.position = Path[i];
            PathPoint.LookAt(Path[i + 1]);
            if (Vertical == false)
            {
                MeshVertices[VertIndex] = PathPoint.position + ((PathPoint.TransformDirection(Vector3.left) / 2) * Width) + Offset;
                MeshVertices[VertIndex + 1] = PathPoint.position + -((PathPoint.TransformDirection(Vector3.left) / 2) * Width) + Offset;
            }
            else
            {
                if (VerticalSide == 1)
                {
                    MeshVertices[VertIndex] = PathPoint.position + ((PathPoint.TransformDirection(Vector3.left) / 2) * Width);
                    MeshVertices[VertIndex + 1] = PathPoint.position + ((PathPoint.TransformDirection(Vector3.left) / 2) * Width) + (Vector3.up * Height);
                }
                else if (VerticalSide == 2)
                {
                    MeshVertices[VertIndex] = PathPoint.position + -((PathPoint.TransformDirection(Vector3.left) / 2) * Width);
                    MeshVertices[VertIndex + 1] = PathPoint.position + -((PathPoint.TransformDirection(Vector3.left) / 2) * Width) + (Vector3.up * Height);
                }
            }
            DestroyImmediate(PlaceHolder);
            VertIndex += 2;
            }
        }
        Mesh.vertices = MeshVertices;

        int[] triangles = new int[Path.Length * 6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        for (int i = 3; i < triangles.Length - 1; i += 3)
        {
            if ((i + 2) < triangles.Length)
            {
                triangles[i] = (i / 3);
                triangles[i + 1] = (i / 3) + 1;
                triangles[i + 2] = (i / 3) + 2;
            }
        }
        int[] newtris = new int[triangles.Length - 12];
        for (int i = 0; i < triangles.Length - 12; i++)
        {
            newtris[i] = triangles[i];
        }

        Mesh.triangles = newtris;

        Vector2[] uvs = new Vector2[Mesh.vertices.Length];
        int pos = 0;
        for (var i = 0; i < uvs.Length; i += 2)
        {
            uvs[i] = new Vector2(0, pos * UVSmoothness);
            uvs[i + 1] = new Vector2(1, pos * UVSmoothness);
            pos++;
        }
        Mesh.uv = uvs;

        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();
        ;
        return Mesh;
    }

    public void ModifyTerrain(int Height)
    {
        TerrainData terrain = Terrain.activeTerrain.terrainData;
        int MapWidth = terrain.heightmapWidth;
        int MapHeight = terrain.heightmapHeight;
        float[,] Heights = terrain.GetHeights(0,0,MapWidth,MapHeight);


        Vector3[] TerrainModPath = CreateCurve(Points, 2048);
        for (int i = 0; i < TerrainModPath.Length; i++)
        {
            if ((i + 1) < TerrainModPath.Length)
            {
                Vector2 TerrainPos = ConvertWorldCor2TerrCor(new Vector3(TerrainModPath[i].x, TerrainModPath[i].z, 0));
                if (Mathf.Abs(terrain.GetInterpolatedHeight(TerrainPos.x, TerrainPos.y) - TerrainModPath[i].y) <= Height)
                {
                    GameObject PlaceHolder = new GameObject();
                    Transform PathPoint = PlaceHolder.transform;
                    PathPoint.position = TerrainModPath[i];
                    PathPoint.LookAt(TerrainModPath[i + 1]);
                    Vector3 left = PathPoint.position + ((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale);
                    Vector3 right = PathPoint.position + -((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale);
                    DestroyImmediate(PlaceHolder);

                    Vector2 LeftPos = ConvertWorldCor2TerrCor(new Vector3(left.x, left.z, 0));
                    Vector2 RightPos = ConvertWorldCor2TerrCor(new Vector3(right.x, right.z, 0));
                    float steps = 1.0F / 5;
                    for (float p = 0; p < 1; p += steps)
                    {
                        Vector2 LerpedPos = Vector2.Lerp(LeftPos, RightPos, p);
                        Heights[(int)LerpedPos.x, (int)LerpedPos.y] = ((TerrainModPath[i].y - 0.1F) / terrain.size.y);
                    }   
                }
            }
        }
        terrain.SetHeights(0, 0, Heights);
    }

    public void ApplySplatmap(bool Effect,float EffectAmount,bool ModifyTerrrainHeight)
    {
        if (ModifyTerrrainHeight)
        {
            ModifyTerrain(10000);
        }
        
        TerrainData terrain = Terrain.activeTerrain.terrainData;
        SplatPrototype[] Splats = new SplatPrototype[terrain.splatPrototypes.Length + 1];
        for (int i = 0; i < terrain.splatPrototypes.Length; i++)
        {
            Splats[i] = terrain.splatPrototypes[i];
        }
        Splats[Splats.Length - 1] = new SplatPrototype();
        Splats[Splats.Length - 1].texture = SplatmapTexture;
        Splats[Splats.Length - 1].tileOffset = new Vector2(0, 0);
        Splats[Splats.Length - 1].tileSize = new Vector2(15, 15);
        terrain.splatPrototypes = Splats;

        int MapWidth = terrain.alphamapWidth;
        int MapHeight = terrain.alphamapHeight;
        float[, ,] TextureMap = terrain.GetAlphamaps(0, 0, MapWidth, MapHeight);

        Vector3[] TerrainModPath = CreateCurve(Points, 512);
        for (int i = 0; i < TerrainModPath.Length; i++)
        {
            if ((i + 1) < TerrainModPath.Length)
            {
                GameObject PlaceHolder = new GameObject();
                Transform PathPoint = PlaceHolder.transform;
                PathPoint.position = TerrainModPath[i];
                PathPoint.LookAt(TerrainModPath[i + 1]);
                Vector3 left = PathPoint.position + ((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale);
                Vector3 right = PathPoint.position + -((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale);
                DestroyImmediate(PlaceHolder);
                if (Effect)
                {
                    Vector2 LeftPos = ConvertWorldCor2TerrCor(new Vector3(left.x, left.z, 0));
                    Vector2 RightPos = ConvertWorldCor2TerrCor(new Vector3(right.x, right.z, 0));
                    float steps = 1.0F / 5;
                    for (float p = 0; p < 1; p += steps)
                    {
                        Vector2 LerpedPos = Vector2.Lerp(LeftPos, RightPos, p);
                        if (Random.Range(0.0F, EffectAmount) < EffectAmount / 2)
                        {
                            for (int x = 0; x < terrain.splatPrototypes.Length - 1; x++)
                            {
                                TextureMap[(int)LerpedPos.x, (int)LerpedPos.y, x] = 0;
                            }
                            TextureMap[(int)LerpedPos.x, (int)LerpedPos.y, terrain.splatPrototypes.Length - 1] = 1;
                        }
                    }                   
                }
                else
                {
                    Vector2 LeftPos = ConvertWorldCor2TerrCor(new Vector3(left.x, left.z, 0));
                    Vector2 RightPos = ConvertWorldCor2TerrCor(new Vector3(right.x, right.z, 0));
                    float steps = 1.0F / 5;
                    for (float p = 0; p < 1; p += steps)
                    {
                        Vector2 LerpedPos = Vector2.Lerp(LeftPos, RightPos, p);
                        for (int x = 0; x < terrain.splatPrototypes.Length - 1; x++)
                        {
                            TextureMap[(int)LerpedPos.x, (int)LerpedPos.y, x] = 0;
                        }
                        TextureMap[(int)LerpedPos.x, (int)LerpedPos.y, terrain.splatPrototypes.Length - 1] = 1;
                    } 
                }
            }
        }
        terrain.SetAlphamaps(0, 0, TextureMap);
    }

    private Vector2 ConvertWorldCor2TerrCor(Vector3 wordCor)
    {
        Vector2 vecRet = new Vector2();
        Terrain ter = Terrain.activeTerrain;
        Vector3 terPosition = ter.transform.position;
        vecRet.y = ((wordCor.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
        vecRet.x = ((wordCor.y - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
        return vecRet;
    }

    public int GetHighestPoint()
    {
        int HighestPoint = int.MinValue;
        foreach (var item in Points)
        {
            if (item.y > HighestPoint)
            {
                HighestPoint = (int)item.y + 1;
            }
        }
        return HighestPoint;
    }

    void OnDrawGizmos()
    {
        if ((GizmoShown == ShowGizmoType.All) || (GizmoShown == ShowGizmoType.CurvePath))
        {
            if (Path.Length > 1)
            {
                for (int i = 0; i < Path.Length; i++)
                {
                    if ((i + 1) != Path.Length)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawLine(Path[i], Path[i + 1]);
                    }
                }
            }
        }

        if ((GizmoShown == ShowGizmoType.All) || (GizmoShown == ShowGizmoType.Vertices))
        {
            Gizmos.color = Color.green;
            int VertIndex = 0;
            for (int i = 0; i < Path.Length; i++)
            {
                if ((i + 1) < Path.Length)
                {
                    GameObject PlaceHolder = new GameObject();
                    Transform PathPoint = PlaceHolder.transform;
                    PathPoint.position = Path[i];
                    PathPoint.LookAt(Path[i + 1]);
                    Gizmos.DrawSphere(PathPoint.position + ((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale), 0.3F);
                    Gizmos.DrawSphere(PathPoint.position + -((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale), 0.3F);
                    if (MeshX)
                    {
                        Gizmos.DrawSphere(PathPoint.position + ((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale) + (Vector3.up * MeshHeightScale), 0.3F);
                        Gizmos.DrawSphere(PathPoint.position + -((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale) + (Vector3.up * MeshHeightScale), 0.3F);
                    }
                    DestroyImmediate(PlaceHolder);
                    VertIndex += 2;
                }
            }
        }

        if ((GizmoShown == ShowGizmoType.All) || (GizmoShown == ShowGizmoType.Vertices))
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < Path.Length; i ++)
            {
                if ((i + 2) < Path.Length)
                {
                    GameObject PlaceHolder = new GameObject();
                    Transform PathPoint = PlaceHolder.transform;
                    PathPoint.position = Path[i];
                    PathPoint.LookAt(Path[i + 1]);
                    GameObject PlaceHolder2 = new GameObject();
                    Transform PathPoint2 = PlaceHolder2.transform;
                    PathPoint2.position = Path[i + 1];
                    PathPoint2.LookAt(Path[i + 2]);
                    Gizmos.DrawLine(PathPoint.position + ((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale), PathPoint.position + -((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale));
                    Gizmos.DrawLine(PathPoint.position + ((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale), PathPoint2.position + ((PathPoint2.TransformDirection(Vector3.left) / 2) * MeshWidthScale));
                    Gizmos.DrawLine(PathPoint.position + -((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale), PathPoint2.position + -((PathPoint2.TransformDirection(Vector3.left) / 2) * MeshWidthScale));
                    if (MeshX)
                    {
                        Gizmos.DrawLine(PathPoint.position + ((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale) + (Vector3.up * MeshHeightScale), PathPoint.position + -((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale) + (Vector3.up * MeshHeightScale));
                        Gizmos.DrawLine(PathPoint.position + ((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale) + (Vector3.up * MeshHeightScale), PathPoint2.position + ((PathPoint2.TransformDirection(Vector3.left) / 2) * MeshWidthScale) + (Vector3.up * MeshHeightScale));
                        Gizmos.DrawLine(PathPoint.position + -((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale) + (Vector3.up * MeshHeightScale), PathPoint2.position + -((PathPoint2.TransformDirection(Vector3.left) / 2) * MeshWidthScale) + (Vector3.up * MeshHeightScale));

                        Gizmos.DrawLine(PathPoint.position + ((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale) + (Vector3.up * MeshHeightScale), PathPoint.position + ((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale));
                        Gizmos.DrawLine(PathPoint.position + -((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale) + (Vector3.up * MeshHeightScale), PathPoint.position + -((PathPoint.TransformDirection(Vector3.left) / 2) * MeshWidthScale));
                    }
                    DestroyImmediate(PlaceHolder);
                    DestroyImmediate(PlaceHolder2);
                }
            }
        }
    }
}
