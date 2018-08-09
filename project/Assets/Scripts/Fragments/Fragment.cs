using UnityEngine;
using System.Collections;

//template for the many fragments
abstract public class Fragment : Host {
	
	//exposes the hovered tile so that it can be accessed easily
	protected Tile HoverTile=null;
	private bool hovered=false;	
	
	public bool Fresh=true;
	
	//called when the fragment is selected and a tile is hovered over -- automatically exposes the hovered over tile
	public override bool TileHover (Tile t)
	{
		HoverTile=t;
		hovered=true;
		return true;
	}
	//check if the tile is being hovered over
	void Update(){
		if(!hovered){
			HoverTile=null;
		}
		hovered=false;
	}
	
	void OnDestroy(){
		Use();
		Destroy(this.gameObject);
	}
	
	//called when the fragment has been used, removes from the player's item queue
	protected void Use(){
		GM.FragMan.UseFragment(this);
	}
	
	//get the associated model
	public Mesh getModel(){
		//print(this.GetType());
		Mesh o = ((Mesh)Resources.Load("FragmentMeshes/"+this.GetType(),typeof(Mesh)));

		return o;
	}
	
	//return the name of the fragment
	public virtual string getName(){
		return "No Name";
	}
	//return the description of the fragment
	public virtual string getDescription(){
		return "No Description";
	}
}
