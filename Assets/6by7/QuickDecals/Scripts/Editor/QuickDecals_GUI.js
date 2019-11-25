class QuickDecals_GUI extends QuickDecals_Base 
{
	@MenuItem("Window/6by7/QuickDecals (v1.0)")
    static function Init()
	{
        window = GetWindow(QuickDecals_GUI, false, "QuickDecals");
        window.Show();
    }
	
	function OnDisable()
	{
		window = null;
	}
	
	function OnGUI()
	{
		useRandom = EditorGUILayout.Toggle("Random?", useRandom);
		//randomRtn = EditorGUILayout.Toggle("Random Rotation?", randomRtn);
		if(!useRandom)
		{
			decalMat = EditorGUILayout.ObjectField("",  decalMat, typeof(Material), false);
		}
		else
		{
			matNum = EditorGUILayout.IntField("# of Decals: ", matNum);
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			if(matNum != 0)
			{
				for(var i : int = 0;i < matNum;i++)
				{
					if(i+1 > matArray.Count)
					{
						matArray.Add(null);
					}
					matArray[i] = EditorGUILayout.ObjectField("",  matArray[i], typeof(Material), false);
				}
			}
			EditorGUILayout.EndScrollView();
		}
		
		// use delegate to pass sceneview info to our own sceneview
		if(SceneView.onSceneGUIDelegate != this.OnSceneGUI)
		   SceneView.onSceneGUIDelegate = this.OnSceneGUI;
	}
}