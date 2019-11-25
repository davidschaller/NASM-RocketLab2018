using UnityEngine;
using UnityEditor;

public class CreatePrimitiveMacros
{
	[MenuItem("GameObject/Create Other/Adv. Capsule", false, 2700)]
	public static void CreateCapsule()
	{
		GameObject gameObject = new GameObject("Capsule");
		gameObject.AddComponent<CapsulePrimitive>();
		Selection.objects = new GameObject[1] { gameObject };
	}

	[MenuItem("GameObject/Create Other/Adv. Cube", false, 2700)]
	public static void CreateCube()
	{
		GameObject gameObject = new GameObject("Cube");
		gameObject.AddComponent<CubePrimitive>();
		Selection.objects = new GameObject[1] { gameObject };
	}

	[MenuItem("GameObject/Create Other/Adv. Cylinder", false, 2700)]
	public static void CreateCylinder()
	{
		GameObject gameObject = new GameObject("Cylinder");
		gameObject.AddComponent<CylinderPrimitive>();
		Selection.objects = new GameObject[1] { gameObject };
	}

	[MenuItem("GameObject/Create Other/Adv. Disk", false, 2700)]
	public static void CreateDisk()
	{
		GameObject gameObject = new GameObject("Disk");
		gameObject.AddComponent<DiskPrimitive>();
		Selection.objects = new GameObject[1] { gameObject };
	}

	[MenuItem("GameObject/Create Other/Adv. Plane", false, 2700)]
	public static void CreatePlane()
	{
		GameObject gameObject = new GameObject("Plane");
		gameObject.AddComponent<PlanePrimitive>();
		Selection.objects = new GameObject[1] { gameObject };
	}

	[MenuItem("GameObject/Create Other/Adv. Pyramid", false, 2700)]
	public static void CreatePyramid()
	{
		GameObject gameObject = new GameObject("Pyramid");
		gameObject.AddComponent<PyramidPrimitive>();
		Selection.objects = new GameObject[1] { gameObject };
	}

	[MenuItem("GameObject/Create Other/Adv. Slope", false, 2700)]
	public static void CreateSlope()
	{
		GameObject gameObject = new GameObject("Slope");
		gameObject.AddComponent<SlopePrimitive>();
		Selection.objects = new GameObject[1] { gameObject };
	}

	[MenuItem("GameObject/Create Other/Adv. Sphere", false, 2700)]
	public static void CreateSphere()
	{
		GameObject gameObject = new GameObject("Sphere");
		gameObject.AddComponent<SpherePrimitive>();
		Selection.objects = new GameObject[1] { gameObject };
	}

	[MenuItem("GameObject/Create Other/Adv. Torus", false, 2700)]
	public static void CreateTorus()
	{
		GameObject gameObject = new GameObject("Torus");
		gameObject.AddComponent<TorusPrimitive>();
		Selection.objects = new GameObject[1] { gameObject };
	}
}
