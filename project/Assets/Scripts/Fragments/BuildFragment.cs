using UnityEngine;
using System.Collections;

public abstract class BuildFragment : Fragment {
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		return true;
	}
	
	//called when the host is selected and valid tile is selected
	override public bool SelectValidTile(Tile t){
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
