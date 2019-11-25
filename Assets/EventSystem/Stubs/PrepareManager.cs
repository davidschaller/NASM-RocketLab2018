using UnityEngine;
using System.Collections;

public class PrepareProps
{
	public string commanderName;
}

public class PrepareManager : MonoBehaviour
{
	public static PrepareProps PrepareProperties
	{
		get; set;
	}
}
