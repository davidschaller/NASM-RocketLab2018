using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtAtmosphereOuterMesh))]
	public class SgtAtmosphereOuterMesh_Editor : SgtEditor<SgtAtmosphereOuterMesh>
	{
		protected override void OnInspector()
		{
			var updateMesh  = false;
			var updateApply = false;

			DrawDefault("rings", ref updateMesh, "The amount of rings in the generated mesh.");
			DrawDefault("detail", ref updateMesh, "The amount of edges around each ring in the mesh.");

			if (updateMesh  == true) DirtyEach(t => t.UpdateMesh ());
			if (updateApply == true) DirtyEach(t => t.ApplyMesh());
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component allows you to generate the SgtAtmosphere.OuterMesh field.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(SgtAtmosphere))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtAtmosphereOuterMesh")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Atmosphere Outer Mesh")]
	public class SgtAtmosphereOuterMesh : SgtBehaviour
	{
		/// <summary>The amount of rings in the generated mesh.</summary>
		[FormerlySerializedAs("rings")] [Range(10, 256)] public int Rings = 8; public void SetRings(int value) { Rings = value; UpdateMesh(); }

		/// <summary>The amount of edges around each ring in the mesh.</summary>
		[FormerlySerializedAs("detail")] [Range(32, 256)] public int Detail = 50; public void SetDetail(int value) { Detail = value; UpdateMesh(); }

		[System.NonSerialized]
		private Mesh generatedMesh;

		[System.NonSerialized]
		private SgtAtmosphere cachedAtmosphere;

		[System.NonSerialized]
		private bool cachedAtmosphereSet;

		public Mesh GeneratedMesh
		{
			get
			{
				return generatedMesh;
			}
		}

#if UNITY_EDITOR
		/// <summary>This method allows you to export the generated mesh as an asset.
		/// Once done, you can remove this component, and set the <b>SgtAtmosphere</b> component's <b>OuterMesh</b> setting using the exported asset.</summary>
		[ContextMenu("Export Mesh")]
		public void ExportMesh()
		{
			if (generatedMesh != null)
			{
				SgtHelper.ExportAssetDialog(generatedMesh, "Atmosphere OuterMesh");
			}
		}
#endif

		[ContextMenu("Update Mesh")]
		public void UpdateMesh()
		{
			if (generatedMesh == null)
			{
				generatedMesh = SgtHelper.CreateTempMesh("Atmosphere OuterMesh (Generated)");

				ApplyMesh();
			}

			var positions  = new Vector3[Rings * Detail + 2];
			var indices    = new int[Rings * Detail * 6];
			var detailStep = (Mathf.PI * 2.0f) / Detail;
			var ringStep   = 1.0f / Rings;
			var ringOffset = 0.5f / Rings;

			positions[1] = new Vector3(0.0f, 0.0f, -1.0f);
			positions[0] = new Vector3(0.0f, 0.0f,  1.0f);

			for (var d = 0; d < Detail; d++)
			{
				var angle = d * detailStep;
				var x     = Mathf.Sin(angle);
				var y     = Mathf.Cos(angle);
				var o     = d * Rings + 2;

				for (var r = 0; r < Rings; r++)
				{
					var z = r * ringStep + ringOffset; // 0..1

					z = z * z;
					z = z * 2.0f - 1.0f; // -1..1

					var s = Mathf.Sin(Mathf.Acos(z));

					positions[o + r] = new Vector3(x * s, y * s, -z);
				}
			}

			var i = 0;

			for (var d = 0; d < Detail; d++)
			{
				var a = d * Rings + 2;
				var b = ((d + 1) % Detail) * Rings + 2;

				// Caps
				indices[i++] = 0;
				indices[i++] = b;
				indices[i++] = a;

				indices[i++] = 1;
				indices[i++] = a + Rings - 1;
				indices[i++] = b + Rings - 1;

				// Body
				for (var r = 0; r < Rings - 1; r++)
				{
					indices[i++] = b + r;
					indices[i++] = b + r + 1;
					indices[i++] = a + r;
					indices[i++] = b + r + 1;
					indices[i++] = a + r + 1;
					indices[i++] = a + r;
				}
			}

			generatedMesh.Clear(false);
			generatedMesh.vertices  = positions;
			generatedMesh.triangles = indices;
			generatedMesh.RecalculateBounds();
		}

		[ContextMenu("Apply Mesh")]
		public void ApplyMesh()
		{
			if (cachedAtmosphereSet == false)
			{
				cachedAtmosphere    = GetComponent<SgtAtmosphere>();
				cachedAtmosphereSet = true;
			}

			if (cachedAtmosphere.OuterMesh != generatedMesh)
			{
				cachedAtmosphere.SetOuterMesh(generatedMesh);
			}
		}

		protected virtual void OnEnable()
		{
			UpdateMesh();
			ApplyMesh();
		}

		protected virtual void OnDestroy()
		{
			if (quitting == false)
			{
				if (generatedMesh != null)
				{
					generatedMesh.Clear(false);

					SgtObjectPool<Mesh>.Add(generatedMesh);
				}
			}
		}
	}
}