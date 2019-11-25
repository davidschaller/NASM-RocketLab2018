using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public enum PathBaseType
{
    Mesh,Terrain
}

public enum PathType
{
    Mesh, SplatMap
}

[System.Serializable]
[CustomEditor(typeof(PathGenerator))]
public class PathGeneratorEditor : Editor 
{
    PathGenerator PathGen;
    Ray MouseRay;
    bool PointPlacement;
    int tempSmooth = 0;
    int MenuSelected = 0;
    PathBaseType BaseType = PathBaseType.Mesh;
    PathType PathType = PathType.Mesh;
    Vector3 PointSelected;

    void Init()
    {
        if (PathGen == null)
        {
            PathGen = (PathGenerator)target as PathGenerator;
        }
    }

    public override void OnInspectorGUI()
    {
        Init();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Path Points"))
        {
            MenuSelected = 1;
        }

        if (GUILayout.Button("Settings"))
        {
            MenuSelected = 2;
        }

        if (GUILayout.Button("Backups"))
        {
            MenuSelected = 3;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        #region "PathPoints"
        if (MenuSelected == 1)
        {
            if (GUILayout.Button("Add Path Point"))
            {
                PointPlacement = true;
                Event.current.Use();
            }

            if (GUILayout.Button("Clear Path Point"))
            {
                PathGen.ClearPoints();
            }

            switch (BaseType)
            {
                case PathBaseType.Mesh:
                    PathGen.MeshName = EditorGUILayout.TextField("Mesh Name:", PathGen.MeshName);
                    break;
                case PathBaseType.Terrain:
                    switch (PathType)
                    {
                        case PathType.Mesh:
                            PathGen.MeshName = EditorGUILayout.TextField("Mesh Name:", PathGen.MeshName);
                            break;
                        case PathType.SplatMap:
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            if (GUILayout.Button("Finish"))
            {

                Backups NewBackUp = new Backups("", null, null,null);
                TerrainData terrain = Terrain.activeTerrain.terrainData;
                NewBackUp.TimeCreated = DateTime.Now.ToString();
                NewBackUp.Heights = terrain.GetHeights(0, 0, terrain.heightmapWidth, terrain.heightmapHeight);
                NewBackUp.Splats = terrain.GetAlphamaps(0, 0, terrain.alphamapWidth, terrain.alphamapHeight);
                NewBackUp.Splattextures = terrain.splatPrototypes;
                PathGen.Backsups.Insert(0, NewBackUp);
                if (PathGen.Backsups.Count == 6)
                {
                    PathGen.Backsups.RemoveAt(5);
                }

                switch (BaseType)
                {
                    case PathBaseType.Mesh:
                        PathGen.FinishMesh();
                        break;
                    case PathBaseType.Terrain:
                        switch (PathType)
	                    {
                            case PathType.Mesh:
                                PathGen.FinishMesh();
                                break;
                            case PathType.SplatMap:
                                bool SplatEffect = true;
                                if (PathGen.SplatmapAlpha == 0)
                                {
                                    SplatEffect = false;
                                }
                                PathGen.ApplySplatmap(SplatEffect,PathGen.SplatmapAlpha,PathGen.TerriainRaise);
                                break;
                            default:
                                break;
	                    }
                        break;
                    default:
                        break;
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (PathGen.Points.Count > 0)
            {
                for (int i = 0; i < PathGen.Points.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Path Point - " + i);
                    if (GUILayout.Button("Show Point"))
                    {
                        PointSelected = PathGen.Points[i];
                    }
                    if (GUILayout.Button("Remove Point"))
                    {
                        PathGen.RemovePoint(i);
                        PointSelected = Vector3.zero;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            #region "Point Placement"
            if ((Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.S) && (PointPlacement))
            {
                RaycastHit Hit;
                if (Physics.Raycast(MouseRay, out Hit))
                {
                    PathGen.AddPoint(Hit.point);
                }
                PointPlacement = false;
            }
            #endregion
        }
#endregion

        #region "Settings"
        if (MenuSelected == 2)
        {
            BaseType = (PathBaseType)EditorGUILayout.EnumPopup("Path Base", BaseType);

            if (BaseType == PathBaseType.Mesh)
            {
                PathGen.TerriainRaise = false;
                PathGen.Smoothness = EditorGUILayout.IntSlider("Mesh Smoothness: ", PathGen.Smoothness, 5, 20);
                if (tempSmooth != PathGen.Smoothness)
                {
                    tempSmooth = PathGen.Smoothness;
                    PathGen.Remake();
                }

                PathGen.MeshTexture = (Texture2D)EditorGUILayout.ObjectField("Mesh Texture:", PathGen.MeshTexture, typeof(Texture2D));

                PathGen.MeshX = EditorGUILayout.Toggle("3D Mesh", PathGen.MeshX);
                if (PathGen.MeshX)
                {
                    PathGen.MeshHeightScale = EditorGUILayout.IntSlider("Mesh Height:", PathGen.MeshHeightScale, 1, 10);
                }
                PathGen.MeshWidthScale = EditorGUILayout.IntSlider("Mesh Width:", PathGen.MeshWidthScale, 1, 10);
            }
            else
            {
                PathType = (PathType)EditorGUILayout.EnumPopup("Path Type", PathType);
                PathGen.Smoothness = EditorGUILayout.IntSlider("Path Smoothness: ", PathGen.Smoothness, 5, 20);
                if (tempSmooth != PathGen.Smoothness)
                {
                    tempSmooth = PathGen.Smoothness;
                    PathGen.Remake();
                }

                switch (PathType)
                {
                    case PathType.Mesh:
                        PathGen.SplatmapTexture = (Texture2D)EditorGUILayout.ObjectField("Mesh Texture:", PathGen.SplatmapTexture, typeof(Texture2D));
                        PathGen.MeshX = EditorGUILayout.Toggle("3D Mesh", PathGen.MeshX);
                        if (PathGen.MeshX)
                        {
                            PathGen.MeshHeightScale = EditorGUILayout.IntSlider("Mesh Height:", PathGen.MeshHeightScale, 1, 10);
                        }
                        PathGen.MeshWidthScale = EditorGUILayout.IntSlider("Mesh Width:", PathGen.MeshWidthScale, 1, 10);
                        PathGen.TerriainRaise = EditorGUILayout.Toggle("Raise Terrain To Path", PathGen.TerriainRaise);
                        if (PathGen.TerriainRaise)
                        {
                            PathGen.TerriainRaiseHeight = EditorGUILayout.IntSlider("Raise if below", PathGen.TerriainRaiseHeight, 0, PathGen.GetHighestPoint());
                        }
                        break;
                    case PathType.SplatMap:
                        PathGen.MeshX = false;
                        PathGen.SplatmapTexture = (Texture2D)EditorGUILayout.ObjectField("Splat Texture:", PathGen.SplatmapTexture, typeof(Texture2D));
                        PathGen.MeshWidthScale = EditorGUILayout.IntSlider("Path Width:", PathGen.MeshWidthScale, 1, 10);
                        PathGen.TerriainRaise = EditorGUILayout.Toggle("Raise Terrain To Path", PathGen.TerriainRaise);
                        PathGen.SplatmapAlpha = EditorGUILayout.Slider("Path Alpha:", PathGen.SplatmapAlpha, 0, 1);
                        break;
                    default:
                        break;
                }

            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            PathGen.GizmoShown = (ShowGizmoType)EditorGUILayout.EnumPopup("Gizmos Shown:", PathGen.GizmoShown);
            EditorGUILayout.Space();
        }
        #endregion

        #region "Backups"

        if (MenuSelected == 3)
        {

            for (int i = 0; i < PathGen.Backsups.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Backup " + i + " (" + PathGen.Backsups[i].TimeCreated + ")");
                if (GUILayout.Button("Restore"))
                {
                TerrainData terrain = Terrain.activeTerrain.terrainData;
                terrain.splatPrototypes = PathGen.Backsups[i].Splattextures;
                terrain.SetAlphamaps(0,0,PathGen.Backsups[i].Splats);
                terrain.SetHeights(0, 0, PathGen.Backsups[i].Heights);
                }
                EditorGUILayout.EndHorizontal();
                
            }

        }
        
        #endregion
    }

    void OnSceneGUI()
    {
        Init();
        HandleUtility.Repaint();
        MouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if ((PathGen.GizmoShown == ShowGizmoType.All) || (PathGen.GizmoShown == ShowGizmoType.Handles))
        {
            if (PathGen.Points.Count > 0)
            {
                for (int i = 0; i < PathGen.Points.Count; i++)
                {
                    PathGen.Points[i] = Handles.DoPositionHandle(PathGen.Points[i], Quaternion.identity);
                }
            }
        }

        if (PointSelected != Vector3.zero)
        {
            Handles.SphereCap(0, PointSelected, Quaternion.identity, 5F);
        }

        if (GUI.changed)
        {
            PathGen.Remake();
        }

    }
}
