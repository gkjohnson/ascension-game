using UnityEngine;
using System.Collections;

//creates a destroyed tile animation
public class BustedTile : MonoBehaviour {
	
	const int MAX_TIMER=50;
	int timer=MAX_TIMER+75;
	
	// rotate randomly and set velocities for shards of tile
	void Start () {
		int angle=Mathf.FloorToInt(Random.Range(0,6));
		angle%=6;
		angle*=60;
		this.transform.Rotate(new Vector3(0,angle,0));
		
		Rigidbody[] r = this.GetComponentsInChildren<Rigidbody>();
		for(int i=0;i<r.Length;i++){
			Vector3 v = -this.transform.position + r[i].transform.position;
			v.y*=3.5f*5*((float)Random.Range(1f,1.25f));
			v.x*=10;
			v.z*=10;
			r[i].velocity=v;
			
			v.y=0;
			v.x=0;
			v*=.6f;
			r[i].angularVelocity=v;
			
		}
		
	}
	
	//Updates the transparency of the pieces based on a timer
	void Update(){
		//fade the children with transparency
		timer--;
		Rigidbody[] r = this.GetComponentsInChildren<Rigidbody>();
		
		Color c=r[0].renderer.sharedMaterial.color;
		if(timer<MAX_TIMER)c.a=(float)timer/MAX_TIMER;

		for(int i=0;i<r.Length;i++){
			r[i].renderer.sharedMaterial.color=c;
		}
		
		if(timer==0){
			Destroy(this.gameObject);
		}
	}
	
	//sets a tile that will serve as a reference for color etc
	public void ReferenceTile(Tile t){

		Color c = t.renderer.sharedMaterial.color;
		
		c = 2*c/3 + new Color(.45f,.45f,.45f);
		
		for(int i=0;i<transform.GetChildCount();i++){
			Color c2 =new Color(Random.Range(-1.0f,1.0f),Random.Range(-1.0f,1.0f),Random.Range(-1.0f,1.0f));
			transform.GetChild(i).renderer.material.color = c  + .025f*c2;
		}
		
		this.transform.parent=t.transform.parent;
	}
	//Scale the pieces to get a new depth for them
	public void setScale(int s){
		Rigidbody[] r = this.GetComponentsInChildren<Rigidbody>();
		for(int i=0; i<r.Length; i++){
			Vector3 v = r[i].transform.localScale;
			v.y*=s;
			r[i].transform.localScale=v;
		}
	}
}
