using UnityEngine;
using System.Collections;

namespace Problematic.StarfieldGenerator.Demo
{
	public class DemoCharacter : MonoBehaviour
	{
		public Camera lookCam;
		public float moveSpeed = 10f;
		public float lookSpeed = 100f;

		void Update ()
		{
			var move = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0f, Input.GetAxisRaw ("Vertical")).normalized;
			transform.Translate (move * moveSpeed * Time.deltaTime);

			transform.Rotate (new Vector2 (0, Input.GetAxis ("Mouse X")) * lookSpeed * Time.deltaTime);
			lookCam.transform.Rotate (new Vector2 (-Input.GetAxis ("Mouse Y"), 0) * lookSpeed * Time.deltaTime);
		}
	}
}
