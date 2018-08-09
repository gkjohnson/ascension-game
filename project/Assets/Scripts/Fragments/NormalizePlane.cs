using UnityEngine;
using System.Collections;

public class NormalizePlane : Fragment {
	
	const int DIST = 2;
	
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		if(HoverTile==null)return true;
		
		GM.TileMan.Validate(HoverTile,DIST,999,false);
		HoverTile.Validate();

		return true;
	}
	
	
	public bool validaterNormalize(Tile t, Tile f, int dir , int dDelta , int hDelta){
		
		t.setHeight((int)(t.getHeight()*0.8f));
		
		return true;//dir==2;
	}
	
	//called when the host is selected and valid tile is selected
	override public bool SelectValidTile(Tile t){
		GM.TileMan.Validate(t,DIST,999,new TileManager.validater(validaterNormalize));
		Use();
		return true;
	}
	
	public override string getName ()
	{
		return "Rotting Rolling Pin";
	}
	public override string getDescription ()
	{
		return "Flatten the land closer to its original state.";
	}
	
	//called when the unit is deselected - used to reset the host to before it is clicked on
	override public void Clear(){}
	
	/* DEFUNCT */
	//called when the host is selected and another unit within range is clicked on
	override public bool ActOnUnit(Unit u){return true;}
	//called when the unit is rolled over and when it is being placed
	override public void MarkTiles(Tile t){}
}
