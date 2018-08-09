using UnityEngine;
using System.Collections;

//an icon to show what icon is within or has been found within a tile
public class FragmentRemains : MonoBehaviour {

	Tile tile;
	
	int timer = 85;
	int moveDelta =20;
	
	Vector3 dVector = new Vector3(0,20,0);
	
	void Awake(){
		transform.RotateAround(new Vector3(1,0,0), Mathf.PI/4);
		renderer.material.color= new Color(2,2,2);
	}
	// rotate amd move the model
	void FixedUpdate () {
		
		Vector3 goal = tile.transform.position + dVector;
		
		transform.position += (goal - transform.position)/moveDelta;
		
		timer --;
		if(timer==0 && dVector == new Vector3(0,-10,0))Destroy(this.gameObject);
		else if(timer==0){
			dVector= new Vector3(0,-10,0);
			timer=50;
			moveDelta=10;
		}
		transform.RotateAround(new Vector3(0,1,0), .025f);
		
	}
	
	//set the tile that it's associate with
	public void SetTile(Tile t){
		tile=t;
		this.transform.position=t.transform.position - new Vector3(0,5,0);
		transform.parent=t.transform;
	}
	
	//set which fragment it represents and get the model
	public void SetFragment(Fragment f){
		if(!f)return;
		((MeshFilter)GetComponent("MeshFilter")).mesh=f.getModel();
	}
	//set the model that is used
	public void SetModel(Mesh m){
		((MeshFilter)GetComponent("MeshFilter")).mesh=m;
	}
	//set the direction that it is moving in
	public void SetDeltaVector(Vector3 v){
		dVector = v;
	}
	public void SetTimer(int t){
		timer=t;
	}
}
