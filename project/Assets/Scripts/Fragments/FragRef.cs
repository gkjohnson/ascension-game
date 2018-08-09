using UnityEngine;
using System.Collections;

public class FragRef : Fragment {

	public bool validaterNormalize(Tile t, Tile f, int dir , int dDelta , int hDelta){
		return true;//dir==2;
	}
	
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		if(HoverTile)GM.TileMan.Validate(HoverTile,5,5,new TileManager.validater(validaterNormalize));
		if(HoverTile)HoverTile.Validate();

		return true;
	}
	
	//called when the host is selected and valid tile is selected
	override public bool SelectValidTile(Tile t){
		Use();
		return true;
	}
	
	//called when the unit is deselected - used to reset the host to before it is clicked on
	override public void Clear(){}
	
	/* DEFUNCT */
	//called when the host is selected and another unit within range is clicked on
	override public bool ActOnUnit(Unit u){return true;}
	//called when the unit is rolled over and when it is being placed
	override public void MarkTiles(Tile t){}
}
