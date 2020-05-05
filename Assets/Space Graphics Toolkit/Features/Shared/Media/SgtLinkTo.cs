using UnityEngine;
using UnityEngine.EventSystems;

namespace SpaceGraphicsToolkit
{
	/// <summary>This component turns a UI element into a button.</summary>
	[ExecuteInEditMode]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Link To")]
	public class SgtLinkTo : MonoBehaviour, IPointerClickHandler
	{
		public enum LinkType
		{
			PreviousScene,
			NextScene
		}

		/// <summary>The actions this button will perform.</summary>
		public LinkType Link { set { link = value; } get { return link; } } [SerializeField] private LinkType link;

		protected virtual void Update()
		{
			switch (link)
			{
				case LinkType.PreviousScene:
				case LinkType.NextScene:
				{
					var group = GetComponent<CanvasGroup>();

					if (group != null)
					{
						var show = GetCurrentLevel() >= 0 && GetLevelCount() > 1;

						group.alpha          = show == true ? 1.0f : 0.0f;
						group.blocksRaycasts = show;
						group.interactable   = show;
					}
				}
				break;
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			switch (link)
			{
				case LinkType.PreviousScene:
				{
					var index = GetCurrentLevel();

					if (index >= 0)
					{
						if (--index < 0)
						{
							index = GetLevelCount() - 1;
						}

						LoadLevel(index);
					}
				}
				break;

				case LinkType.NextScene:
				{
					var index = GetCurrentLevel();

					if (index >= 0)
					{
						if (++index >= GetLevelCount())
						{
							index = 0;
						}

						LoadLevel(index);
					}
				}
				break;
			}
		}

		private static int GetCurrentLevel()
		{
			var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			var index = scene.buildIndex;

			if (index >= 0)
			{
				if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(index).path != scene.path)
				{
					return -1;
				}
			}

			return index;
		}

		private static int GetLevelCount()
		{
			return UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
		}

		private static void LoadLevel(int index)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(index);
		}
	}
}