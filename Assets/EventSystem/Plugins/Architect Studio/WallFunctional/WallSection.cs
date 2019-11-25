using UnityEngine;

public class WallSection
{
    private Vector3 m_pos;
    public Vector3 Pos
    {
        get
        {
            return m_pos;
        }
        set
        {
            m_pos = value;
        }
    }

    private string interiorMaterialName;
    public string InteriorMaterialName
    {
        get
        {
            return interiorMaterialName;
        }
        set
        {
            interiorMaterialName = value;
        }
    }

    private string exteriorMaterialName;
    public string ExteriorMaterialName
    {
        get
        {
            return exteriorMaterialName;
        }
        set
        {
            exteriorMaterialName = value;
        }
    }

    public WallSection(Vector3 p_pos, string p_interiorMaterialName, string p_exteriorMaterialName)
    {
        m_pos = p_pos;
        interiorMaterialName = p_interiorMaterialName;
        exteriorMaterialName = p_exteriorMaterialName;
    }

    internal string Serialize()
    {
        string result = string.Format("{0};{1};{2}",
            Common.Vector3ToString(m_pos), interiorMaterialName, exteriorMaterialName);

        return result;
    }

    public WallSection()
    {

    }

    public WallSection Deserialize(string p_input)
    {
        WallSection result = new WallSection();

        result.Pos = Common.StringToVector3(p_input.Split(';')[0]);
        result.interiorMaterialName = p_input.Split(';')[1];
        result.exteriorMaterialName = p_input.Split(';')[2];

        return result;
    }

    private bool rotate90 = false;
    public bool Rotate90
    {
        get
        {
            return rotate90;
        }
        set
        {
            rotate90 = value;
        }

    }
}

