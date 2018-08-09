using UnityEngine;
using System.Collections;

//The base of the tile with stripes on it
public class TileBase : MonoBehaviour {
	
	public Color col_offset=new Color(0,0,0,0);
	
	//choose a random color change for the tile base
	void Awake(){
		col_offset.r=(float)Random.Range(-0f,1f);
		col_offset.g=(float)Random.Range(-.5f,.25f);
		col_offset.b=Random.Range(-1f,-.5f);//(float)Random.Range(-.1f,.1f);
		
		col_offset*=Random.Range(.1f,.2f);
		
		//col_offset+=-1*new Color(1,1,1,1)+transform.parent.renderer.material.color;
		
		//col_offset*=.75f;
		//col_offset=new Color(0,0,0,0);
	}
	
	//add a collision box so they can be hovered over
	void Start(){
		gameObject.AddComponent("BoxCollider");
		BoxCollider b=((BoxCollider)GetComponent("BoxCollider"));
		//b.isTrigger=true;
		b.size=new Vector3(7,1,7);
		b.center=new Vector3(0,.5f,0);
		enabled=false;
	}
	
	void OnMouseOver(){
		((Tile)transform.parent.GetComponent("Tile")).OnMouseOver();
	}
	void OnMouseExit(){
		((Tile)transform.parent.GetComponent("Tile")).OnMouseExit();
	}
	
}
