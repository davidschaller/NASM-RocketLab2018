using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VP_DebugDisplayLinkedList : MonoBehaviour {
	
	public VP_EmitterLinkedList[] Emitters;
	
	static Material lineMaterial;
	static void CreateLineMaterial() {
	    if( !lineMaterial ) {
	        lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
	            "SubShader { Pass { " +
	            "    Blend SrcAlpha OneMinusSrcAlpha " +
	            "    ZWrite Off Cull Off Fog { Mode Off } " +
	            "    BindChannels {" +
	            "      Bind \"vertex\", vertex Bind \"color\", color }" +
	            "} } }" );
	        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
	        lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
	    }
	}
	
	
	
	// Use this for initialization
	void Start () {
		CreateLineMaterial();
	}
	
	void OnGUI() {
		
		int total = 0;
		int live = 0;
		float dbg = 0f;
		
		foreach(VP_EmitterLinkedList e in Emitters){
			total += e.particleTotal;
			live += e.particleLive;
			dbg += (float)(e.debugTime*1000);
		}
		
		GUILayout.Space(20);
		GUILayout.Label(live.ToString()+"/"+total.ToString());
		
		GUI.color = Color.white;
		GUILayout.Label(dbg.ToString("f2")+"ms");
	}
	
	void OnPostRender() {
		if(Emitters != null){
			if(Emitters.Length>0){
	
				lineMaterial.SetPass( 0 );
				Vector3 pos;
				float size;
			    GL.Begin( GL.LINES );
				foreach(VP_EmitterLinkedList e in Emitters){
					int i = 0;
					for (LinkedListNode<ParticleSystem.Particle> current = e.particles.First; current != null; current = current.Next){
						if(current.Value.startLifetime > 0){
							GL.Color(current.Value.color);
							pos = current.Value.position;
							size = current.Value.size;
							GL.Vertex( pos );
			    			GL.Vertex( pos + Vector3.up*size );
							GL.Vertex( pos );
							GL.Vertex( pos + Vector3.right*size );
							GL.Vertex( pos );
							GL.Vertex( pos + Vector3.forward*size );
						}
						i++;
						if(i>=e.particleLive) break;
					}
				}
			    GL.End();
			}
		}
	}
	
	
}
