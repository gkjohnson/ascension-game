using UnityEngine;
using System.Collections;

public class BurningEmber : Fragment {
	
	GameObject explosion;
	
	void Update(){
		if(explosion && !((ParticleSystem)explosion.GetComponent("ParticleSystem")).IsAlive()){
			Destroy(explosion);
			Use();
		}
	}
	public bool validaterBurn(Tile t, Tile f, int dir , int dDelta , int hDelta){
		if(t.SurfaceFrag &&
		t.SurfaceFrag.GetType() == typeof(TreeFragment)){
			((TreeFragment)t.SurfaceFrag).Burning=true;
		}
		
		return false;//dir==2;
	}
	
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		if(explosion)return true;
		
		if(HoverTile)GM.TileMan.Validate(HoverTile,1,2,false);
		if(HoverTile)HoverTile.Validate();
		return true;
	}
	
	//called when the host is selected and valid tile is selected
	override public bool SelectValidTile(Tile t){
		GM.TileMan.Validate(HoverTile,1,2,new TileManager.validater(validaterBurn));
		validaterBurn(t,null,0,0,0);
		
		
		explosion = (GameObject)Instantiate ( Resources.Load("Effects/BurningEmberExplosion"));
		explosion.transform.position=t.transform.position;
		
		return true;
	}
	
	//called when the unit is deselected - used to reset the host to before it is clicked on
	override public void Clear(){}
	
	//* Description and Name *//
	public override string getName ()
	{
		return "Burning Ember";
	}
	
	public override string getDescription ()
	{
		return "Start a forest fire among the trees to destroy the opponent's wood supply.";
	}
	
	
	
	/* DEFUNCT */
	//called when the host is selected and another unit within range is clicked on
	override public bool ActOnUnit(Unit u){return true;}
	//called when the unit is rolled over and when it is being placed
	override public void MarkTiles(Tile t){}
}
