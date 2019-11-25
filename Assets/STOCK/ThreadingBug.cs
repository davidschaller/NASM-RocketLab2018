using UnityEngine;
using System.Collections;
using System.Collections.Generic;	

public class ThreadingBug : MonoBehaviour {
	
	List<UnityThreading.Task> taskList = new List<UnityThreading.Task>();
	private int _coresCount;
	private List<SortGrp> _sortGroups = new List<SortGrp>();
	private IDComp comp = new IDComp();
	double debugTime;
	
	int delay = 100;
	
	void Start () {
		_coresCount = SystemInfo.processorCount;
		SortGrp newGroup;
		SortID s;
		int i,j;
		
		for(i = 0 ; i < _coresCount ; i++){
			 newGroup = new SortGrp();
			for(j = 0 ; j < 1000 ; j++){
				s = new SortID();
				s.order = Random.Range(0f,100f);
				newGroup.items.Add(s);
			}
			_sortGroups.Add(newGroup);
		}
		
	}

	void Update () {
		
		if(delay < 0){
			debugTime = Time.realtimeSinceStartup;
			taskList.Clear();
			int i;
			for(i=0;i<_coresCount;i++){
				// crash occurs here !
				taskList.Add(UnityThreadHelper.TaskDistributor.Dispatch(() => _sortGroups[i].items.Sort(comp) ));
				
				// simpler example, same crash
				//taskList.Add(UnityThreadHelper.TaskDistributor.Dispatch( () => {return;} ) );
			}
			
			for(i = 0; i < taskList.Count ; i ++){
				taskList[i].Wait();
				taskList[i].Dispose();
			}
			taskList.Clear();
			debugTime = Time.realtimeSinceStartup - debugTime;
		}else{
			delay --;	
		}
	}
	
	void OnGUI() {
		if(delay > 0){
			GUILayout.Label("countdown = "+delay);
		}else{
			GUILayout.Label("time = "+(debugTime*1000).ToString("f2")+"ms  "+Time.realtimeSinceStartup);
		}
	}
}

public class SortID{
	public float order;
}

public class SortGrp{
	public List<SortID> items = new List<SortID>();
}

public class IDComp : IComparer<SortID>
{
	public int Compare(SortID a, SortID b)
	{
		return (a.order > b.order)?-1:1;
	}
}