using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using JsonFx.Json;
using System.Text.RegularExpressions;


public abstract class AS3DLayoutGridBase : MonoBehaviour
{
    public int gridWidth = 35;
    public int gridHeight = 35;
    public Color lineColor = Color.white;

    protected Material lineMaterial;
    protected V3Line[] horizontalLines;
    protected V3Line[] verticalLines;
    protected float gridWorldY = 0;

    public void Awake()
    {
        lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
                                     "SubShader { Pass {" +
                                     "   BindChannels { Bind \"Color\",color }" +
                                     "   Blend SrcAlpha OneMinusSrcAlpha" +
                                     "   ZWrite Off Cull Off Fog { Mode Off }" +
                                     "} } }");
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
    }

    /// <summary>
    /// Load grid lines
    /// </summary>
    public void Start()
    {
        horizontalLines = new V3Line[gridHeight];
        verticalLines = new V3Line[gridWidth];

        float xBase = GetComponent<Camera>().transform.position.x - gridHeight / 2;
        float yBase = GetComponent<Camera>().transform.position.z - gridWidth / 2 + 3;
        gridWorldY = (GetComponent<Camera>().transform.position + GetComponent<Camera>().transform.TransformDirection(Vector3.forward)).y;

        for (int i = 0; i < gridWidth; i++)
        {
            verticalLines[i] = new V3Line();
            verticalLines[i].start = new Vector3(xBase + i, gridWorldY, yBase);

            verticalLines[i].end = new Vector3(xBase + i, gridWorldY, yBase + gridWidth - 1);
        }

        for (int i = 0; i < gridHeight; i++)
        {
            horizontalLines[i] = new V3Line();

            horizontalLines[i].start = new Vector3(xBase, gridWorldY, yBase + i);
            horizontalLines[i].end = new Vector3(xBase + gridHeight - 1, gridWorldY, yBase + i);
        }
    }

    /// <summary>
    /// Draw lines
    /// </summary>
    /// <param name="lines">Lines to draw</param>
    protected void DrawLines(V3Line[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            Vector3 v1 = lines[i].start;
            Vector3 v2 = lines[i].end;

            DrawLine(v1, v2, false);
        }
    }

    protected void DrawLine(Vector3 p_start, Vector3 p_end, bool p_bold)
    {
        GL.Vertex(p_start);
        GL.Vertex(p_end);

        if (p_bold)
        {
            Ray ray = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Vector3 target = ray.direction;
            Vector3 perpendicular = Vector3.Cross(target, (p_start - p_end)).normalized;

            int sign = 1;

            for (int j = 0; j < 4; j++)
            {
                Vector3 addWidth = perpendicular * 0.04f * (1 + j >> 1) * sign;
                GL.Vertex(p_start + addWidth);
                GL.Vertex(p_end + addWidth);
                sign = -sign;
            }
        }
    }

    protected void DrawGridLines()
    {
        if (horizontalLines == null || verticalLines == null)
            return;

        //float pWidth = (1.0f / Screen.width) * ((Screen.width + 0.0f) / Screen.height) * .22f;

        GL.Color(lineColor);
        DrawLines(horizontalLines);
        DrawLines(verticalLines);
    }

    protected void DrawDarkOutline(List<Vector3> p_markers)
    {
        if (BuildingManager.Picked == null || p_markers.Count == 0)
            return;

        int i = 0;
        for (i = 0; i < p_markers.Count - 1; i++)
        {
            Vector3 v1 = new Vector3(p_markers[i].x, gridWorldY, p_markers[i].z);
            Vector3 v2 = new Vector3(p_markers[i + 1].x, gridWorldY, p_markers[i + 1].z);

            DrawLine(v1, v2, true);

        }

        Vector3 vLast1 = new Vector3(p_markers[i].x, gridWorldY, p_markers[i].z);
        Vector3 vLast2 = new Vector3(p_markers[0].x, gridWorldY, p_markers[0].z);

        DrawLine(vLast1, vLast2, true);
    }

    /// <summary>
    /// Returns the nearest Point on the grid lines. Use this function to draw wall lines.
    /// </summary>
    /// <param name="p_cursor">To the cursor</param>
    /// <returns></returns>
    public Vector3 GetNearestIntersection(Vector3 p_cursor)
    {
        float closestVDist = 999;
        float closestHDist = 999;

        float closestX = 0;
        float closestZ = 0;

        foreach (V3Line l in verticalLines)
        {
            if (Mathf.Abs(p_cursor.x - l.start.x) < closestVDist)
            {
                closestVDist = Mathf.Abs(p_cursor.x - l.start.x);
                closestX = l.start.x;
            }
        }

        foreach (V3Line l in horizontalLines)
        {
            if (Mathf.Abs(p_cursor.z - l.start.z) < closestHDist)
            {
                closestHDist = Mathf.Abs(p_cursor.z - l.start.z);
                closestZ = l.start.z;
            }
        }

        return new Vector3(closestX, gridWorldY, closestZ);
    }

    protected virtual void OnPostRender()
    {
        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);

        DrawGridLines();

        // Temp line to draw walls
        GL.Color(Color.black);

        DrawDarkOutline(BuildingManager.Picked.OutsideWallMarkers);
        DrawDarkOutline(BuildingManager.Picked.InsideWallMarkers);

        GL.End();
    }

    protected void OnApplicationQuit()
    {
        DestroyImmediate(lineMaterial);
    }
}

