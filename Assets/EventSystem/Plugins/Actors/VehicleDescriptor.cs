using UnityEngine;
using System.Collections;

public class VehicleDescriptor : MonoBehaviour
{
	public NPCController[] engines;
	public float engineHorizontalSpacing = 2;
	public float engineForwardDistance = 3;
	public string frontWheelsMeshName;
	public string rearWheelsMeshName;
}
