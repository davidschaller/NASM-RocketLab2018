using UnityEngine;
using System.Collections;

public class McHenryCommander : MonoBehaviour
{
	public static McHenryCommander Main;
	public delegate void VoidCallback();
	public VoidCallback SaluteBack;
	public AnimationClip npcSaluteClip;
}
