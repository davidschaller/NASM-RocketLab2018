using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This is the base class for all floating origin spawners, providing a useful methods for spawning and handling prefabs.</summary>
	[RequireComponent(typeof(SgtFloatingObject))]
	public abstract class SgtFloatingSpawner : MonoBehaviour
	{
		/// <summary>The camera must be within this range for this spawner to activate.</summary>
		public SgtLength Range = new SgtLength(1.0, SgtLength.ScaleType.AU);

		/// <summary>If you want to define prefabs externally, then you can use the SgtSpawnList component with a matching Category name.</summary>
		public string Category;

		/// <summary>If you aren't using a spawn list category, or just want to augment the spawn list, then define the prefabs you want to spawn here.</summary>
		public List<SgtFloatingObject> Prefabs;

		private static List<SgtFloatingObject> prefabs = new List<SgtFloatingObject>();

		[SerializeField]
		private List<SgtFloatingObject> instances;

		[SerializeField]
		private bool inside;

		[System.NonSerialized]
		private SgtFloatingObject cachedObject;

		[System.NonSerialized]
		private bool cachedObjectSet;

		/// <summary>The <b>SgtFloatingObject</b> component alongside this component.</summary>
		public SgtFloatingObject CachedObject
		{
			get
			{
				if (cachedObjectSet == false)
				{
					cachedObject    = GetComponent<SgtFloatingObject>();
					cachedObjectSet = true;
				}

				return cachedObject;
			}
		}

		protected virtual void OnEnable()
		{
			CachedObject.OnDistance += HandleDistance;
		}

		protected virtual void OnDisable()
		{
			cachedObject.OnDistance -= HandleDistance;

			DespawnAll();
		}

		protected bool BuildSpawnList()
		{
			if (instances == null)
			{
				instances = new List<SgtFloatingObject>();
			}

			prefabs.Clear();

			if (string.IsNullOrEmpty(Category) == false)
			{
				var spawnList = SgtSpawnList.FirstInstance;

				while (spawnList != null)
				{
					if (spawnList.Category == Category)
					{
						BuildSpawnList(spawnList.Prefabs);
					}

					spawnList = spawnList.NextInstance;
				}
			}

			BuildSpawnList(Prefabs);

			return prefabs.Count > 0;
		}

		protected SgtFloatingObject SpawnAt(SgtPosition position)
		{
			if (prefabs.Count > 0)
			{
				var index  = Random.Range(0, prefabs.Count);
				var prefab = prefabs[index];

				if (prefab != null)
				{
					var oldSeed        = prefab.Seed;
					var oldPosition    = prefab.Position;
					var oldPositionSet = prefab.PositionSet;

					prefab.Seed        = Random.Range(int.MinValue, int.MaxValue);
					prefab.Position    = position;
					prefab.PositionSet = true;

					var instance = Instantiate(prefab, SgtFloatingRoot.Root);

					prefab.Seed        = oldSeed;
					prefab.Position    = oldPosition;
					prefab.PositionSet = oldPositionSet;

					instances.Add(instance);

					instance.InvokeOnSpawn();

					return instance;
				}
			}

			return null;
		}

		protected abstract void SpawnAll();

		private void HandleDistance(double distance)
		{
			var floatingCamera = SgtFloatingCamera.Instances.First.Value;
			var sqrDistance    = SgtPosition.SqrDistance(ref CachedObject.Position, ref floatingCamera.Position);
			var newInside      = distance <= (double)Range;

			if (inside != newInside)
			{
				inside = newInside;

				if (inside == true)
				{
					SpawnAll();
				}
				else
				{
					DespawnAll();
				}
			}
		}

		private void DespawnAll()
		{
			if (instances != null)
			{
				for (var i = instances.Count - 1; i >= 0; i--)
				{
					var instance = instances[i];

					if (instance != null)
					{
						SgtHelper.Destroy(instance.gameObject);
					}
				}

				instances.Clear();
			}
		}

		private static void BuildSpawnList(List<SgtFloatingObject> floatingObjects)
		{
			if (floatingObjects != null)
			{
				for (var i = floatingObjects.Count - 1; i >= 0; i--)
				{
					var floatingObject = floatingObjects[i];

					if (floatingObject != null)
					{
						prefabs.Add(floatingObject);
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace SpaceGraphicsToolkit
{
	public abstract class SgtFloatingSpawner_Editor<T> : SgtEditor<T>
		where T : SgtFloatingSpawner
	{
		protected override void OnInspector()
		{
			if (SgtFloatingRoot.InstanceCount == 0)
			{
				if (HelpButton("Your scene contains no SgtFloatingRoot component, so all spawned SgtFloatingSpawnable prefabs will be placed in the scene root.", MessageType.Warning, "Add", 35.0f) == true)
				{
					new GameObject("Floating Root").AddComponent<SgtFloatingRoot>();
				}

				Separator();
			}

			var missing = true;

			if (Any(t => string.IsNullOrEmpty(t.Category) == false))
			{
				missing = false;
			}

			if (Any(t => t.Prefabs != null && t.Prefabs.Count > 0))
			{
				missing = false;
			}

			Draw("Range", "The camera must be within this range for this spawner to activate.");
			BeginError(missing);
				Draw("Category", "If you want to define prefabs externally, then you can use the SgtSpawnList component with a matching Category name.");
				Draw("Prefabs", "If you aren't using a spawn list category, or just want to augment the spawn list, then define the prefabs you want to spawn here.");
			EndError();
		}
	}
}
#endif