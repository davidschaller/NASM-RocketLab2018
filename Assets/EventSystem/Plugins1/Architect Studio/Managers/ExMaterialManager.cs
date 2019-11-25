using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class ExMaterialManager
{
    private static List<ExMatDefinition> exMaterialList;
    public static List<ExMatDefinition> ExMaterialList
    {
        get
        {
            return exMaterialList;
        }
        private set { }
    }

    public static void Init()
    {
        exMaterialList = new List<ExMatDefinition>();

        ExMatDefinition[] exMaterials = 
            (ExMatDefinition[])GameObject.FindObjectsOfType(typeof(ExMatDefinition));

        if (exMaterials != null)
        {
            exMaterialList.AddRange(exMaterials);
            exMaterialList.Sort(delegate(ExMatDefinition p1, ExMatDefinition p2)
            { return p1.clientSortKey.CompareTo(p2.clientSortKey); });
        }
    }


    internal static ExMatDefinition GetMaterialById(int p_clientSortKey)
    {
        if (exMaterialList == null)
            Init();

        ExMatDefinition result = exMaterialList.Find(
            delegate(ExMatDefinition p)
            {
                return p.clientSortKey == p_clientSortKey;
            });

        return result ?? exMaterialList[0];
    }
}
