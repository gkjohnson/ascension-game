using UnityEngine;
using System.Collections;

public class KingsRing : Fragment {
	
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		if(HoverTile==null)return true;
		
		//HoverTile.Validate();
		for(int i = 0; i < HoverTile._adjacentTiles.Length; i++){
			if(HoverTile._adjacentTiles[i])HoverTile._adjacentTiles[i].Validate();
		}
		return true;
	}
	
	//called when the host is selected and valid tile is selected
	override public bool SelectTile(Tile t){
		for(int i = 0; i < HoverTile._adjacentTiles.Length; i++){
			//HoverTile._adjacentTiles[i].Dig(4);
			if(HoverTile._adjacentTiles[i])HoverTile._adjacentTiles[i].incHeight(2);
			//if(HoverTile._adjacentTiles[i])HoverTile._adjacentTiles[i].Dig(3,false);

		}
		Use();
		return true;
	}
	
	//called when the unit is deselected - used to reset the host to before it is clicked on
	override public void Clear(){}
	
	override public string getName(){
		return "King's Ring";
	}
	public override string getDescription ()
	{
		return "Create a ring shaped portrusion from the land at any point on the map.";
	}
	
	/* DEFUNCT */
	//called when the host is selected and another unit within range is clicked on
	override public bool ActOnUnit(Unit u){return true;}
	//called when the unit is rolled over and when it is being placed
	override public void MarkTiles(Tile t){}
	
}
