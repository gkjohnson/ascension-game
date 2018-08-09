using UnityEngine;
using System.Collections;

public class Earthquake : Fragment {
	public bool validaterNormalize(Tile t, Tile f, int dir , int dDelta , int hDelta){
		if(t.marked)return true;
			
		int delta = Random.Range(-2,2);
		
		if(delta < 0){
			t.Dig(Mathf.Abs(delta),false);
		}else{
			t.incHeight(delta);
		}
		
		t.incHeight(1);
		t.marked=true;
		return true;//dir==2;
	}
	
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		if(HoverTile)GM.TileMan.Validate(HoverTile,4,99,false);
		if(HoverTile)HoverTile.Validate();

		return true;
	}
	
	//called when the host is selected and valid tile is selected
	override public bool SelectValidTile(Tile t){
		if(HoverTile)GM.TileMan.Validate(HoverTile,4,99,new TileManager.validater(validaterNormalize));
		Use();
		return true;
	}
	
	public override string getName ()
	{
		return "Shattered Granite";
	}
	
	public override string getDescription ()
	{
		return "Create an earthquake at any point on the map, varying the height of the affected tiles randomly.";
	}
	
	//called when the unit is deselected - used to reset the host to before it is clicked on
	override public void Clear(){}
	
	/* DEFUNCT */
	//called when the host is selected and another unit within range is clicked on
	override public bool ActOnUnit(Unit u){return true;}
	//called when the unit is rolled over and when it is being placed
	override public void MarkTiles(Tile t){}
}
