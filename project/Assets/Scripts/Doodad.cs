using UnityEngine;
using System.Collections;

// a randomly retrieved surface model added to tiles for visual flair
public class Doodad : MonoBehaviour {
	
	static int HIGH_NUM=0;
	static int MID_NUM=2;
	static int LOW_NUM=2;
	
	/*void OnTriggerEnter(Collider c){
		//if it collides with a tile, set that to its parent and get the random doodad
		if(c.gameObject.tag=="Tile"){
			transform.position=c.gameObject.transform.position;
			transform.parent=c.gameObject.transform;
			Destroy(GetComponent("SphereCollider"));
			fetchDoodad();
		}
	}*/
	void Update(){
		//disable the component immediately so it doesnt cycle -- allows for the trigger to be called
		fetchDoodad();
		Destroy(GetComponent("SphereCollider"));
		enabled=false;	
	}
	
	bool fetchDoodad(){
		//Gets a random doodad from the folder based on the plane
		if(!transform.parent)return false;
		if(((MeshFilter)GetComponent("MeshFilter")).mesh==null)return false;
		int r=Mathf.FloorToInt(Random.Range(0,1000));
		
		string doodadResource="doodads/";
		if(transform.parent.parent.tag=="HighPlane"){
			if(HIGH_NUM==0){
				Destroy(this.gameObject);
				return false;
			}
			doodadResource+="highplaneDoodad_"+(r%HIGH_NUM+1).ToString();
		}else if(transform.parent.parent.tag=="MidPlane"){
			if(MID_NUM==0){
				Destroy(this.gameObject);
				return false;
			}
			doodadResource+="midplaneDoodad_"+(r%MID_NUM+1).ToString();
		}else if(transform.parent.parent.tag=="LowPlane"){
			if(LOW_NUM==0){
				Destroy(this.gameObject);
				return false;	
			}
			doodadResource+="lowplaneDoodad_"+(r%LOW_NUM+1).ToString();
		}
		((MeshFilter)GetComponent("MeshFilter")).mesh=(Mesh)Resources.Load(doodadResource,typeof(Mesh));
		
		this.transform.RotateAroundLocal(new Vector3(0,1,0),Random.Range(0,2*Mathf.PI));
		
		return true;
	}
}
