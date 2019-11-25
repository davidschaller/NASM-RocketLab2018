#if !Crypto
using UnityEngine;
using System.Collections;


/*
 * THIS IS ONLY A STUB CLASS TO SATISFY COMPILATILATION DEPENDENCIES.
 */
public class CryptoGUI : MonoBehaviour
{
	public bool IsInterviewFinished;
	public enum GUIState
	{Interview, InterviewAnswer}
	public GUIState State;
	
	public static CryptoGUI main;
	
	public void ResetInterviewCounter() {}
	public void SelectLocation (string loc) {}
	public static string PlayerName;

}
#endif