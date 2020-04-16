using UnityEngine;
using System.Collections;

namespace Problematic.StarfieldGenerator.Demo
{
	public class DemoController : MonoBehaviour
	{
		public Skybox skybox;
		public StarfieldGenerator generator;

		void OnGUI ()
		{
			GUI.Label (new Rect (10, 10, 300, 50), "You can either generate a skybox and assign it to the Demo Character Look Cam's skybox component, or click the button below!");
			if (GUI.Button (new Rect (10, 75, 250, 75), "Create Skybox")) {
				generator.Generate ();
				skybox.material = generator.GenerateSkybox ();
				generator.Clear ();
			}
		}
	}
}
