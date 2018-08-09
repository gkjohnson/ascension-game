using UnityEngine;
using System.Collections;

//A surface that represents an item fragment and can be seen and collected on the surface of a tile
public class UnearthedFragment : SurfaceFragment {
	public Fragment currFrag;
	
	public bool Active=false;
	
	//Start the particle system and reset the fragment
	public override void Start () {
		base.Start();
		
		particleSystem.enableEmission=true;
		((MeshRenderer)GetComponent("MeshRenderer")).enabled=false;
		
		if(currFrag == null) reset();

		transform.RotateAround(new Vector3(1,0,0), Mathf.PI/4);		
	}
	
	//rotate the object
	void Update () {
		transform.RotateAround(new Vector3(0,1,0), .1f * Time.deltaTime);
	}
	//stick to the attached tile
	public override void LateUpdate ()
	{
		transform.position = AttachedTile.transform.position + new Vector3(0,2,0);
		
		transform.localScale += (new Vector3(.75f,.75f,.75f) - transform.localScale)/5;
		
	}
	//when harvested, add the item to the current player's stash and turn everything off
	public override bool Harvest ()
	{
		if(!currFrag)return false;
		
		AttachedTile.audio.PlayOneShot(AttachedTile.PickupNoise);
		
		((MeshRenderer)GetComponent("MeshRenderer")).enabled=false;
		particleSystem.enableEmission=false;
		
		GM.FragMan.Collect (currFrag);
		currFrag=null;
				
		Active=false;
		
		AttachedTile.SurfaceFrag=null;
		
		return true;
	}
	//returns whether or not it is activated currently
	public bool isActive(){
		return Active;
	}
	//resets the unearthed fragment to a new tile, item, and turns it on
	public void reset(){
		
		currFrag = FragmentManager.CreateFragment( FragmentManager.FM.GetRandomFrag(true));
				
		Tile[] t = (Tile[])Component.FindObjectsOfType(typeof(Tile));
		
		//TODO: Change this so it only goes through one plane
		
		int r = t.Length / 3;
		r = Random.Range(0, r);
		
		//cycle through all tiles on the lowest plane and choose a tile to attach to, or none if one is not chosen in the given cycle
		for(int i=0; i<t.Length; i++){
			if(!t[i].Resident && !t[i].SurfaceFrag && !t[i].Foundation && t[i].transform.parent.tag=="LowPlane"){
				if(i <= r){
					if(t[i]==AttachedTile || t[i]==null)return;
					this.SetTile(t[i]);
				}
			}
		}
		
		//get mesh of the chosen frag
		Mesh newMesh = currFrag.getModel();
		((MeshFilter)GetComponent("MeshFilter")).mesh = newMesh;
		
		//activate it
		if(AttachedTile && AttachedTile.SurfaceFrag==this){
			Active = true;
		
			((MeshRenderer)GetComponent("MeshRenderer")).enabled=true;
			particleSystem.enableEmission=true;
			transform.localScale = new Vector3(0,0,0);
		}
		
		print( FragmentManager.FragToInt( currFrag.GetType().ToString() ));
		
	}
}
