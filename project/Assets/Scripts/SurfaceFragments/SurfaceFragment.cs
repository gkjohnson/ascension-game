using UnityEngine;
using System.Collections;

//A template for a "fragment" that sits on the surface of the tiles
public abstract class SurfaceFragment : MonoBehaviour {

	public Tile AttachedTile=null;
	public static GeneralManager GM;
	
	virtual public void Start(){
		GM = GeneralManager.GenMan;
		findTile();
	}
	
	//sets the tile to stick to for the surface fragment and sets the appropriate varibles in the tile
	public void SetTile(Tile t){
		if(AttachedTile){
			AttachedTile.SurfaceFrag = null;
		}
		t.SurfaceFrag = this;
		AttachedTile = t;
		
		transform.position = AttachedTile.transform.position;
		transform.parent = AttachedTile.transform;
	}
	
	virtual public void LateUpdate(){}
	
	//removes the surface fragment
	public void Remove(){
		Destroy(this.gameObject);
	}
	
	/*void OnTriggerEnter(Collider c){
		if (c.gameObject.tag=="Tile" && AttachedTile==null){
			SetTile((Tile)c.gameObject.GetComponent("Tile"));
			Destroy(GetComponent("SphereCollider"));
		}
	}*/
	
	//if true, it means the frag was harvested successfully and when digging, souls will not dig down into the tiles
	abstract public bool Harvest();
	
	//new function that uses raycasting to find the tiles its on top of
	void findTile(){
		RaycastHit hit;
		Vector3 dir = new Vector3(0,-30,0);
		if (Physics.Raycast(transform.position, dir, out hit,20)){
			SetTile(hit.transform.GetComponent<Tile>());
		}
	}
	
}
