using UnityEngine;
using System.Collections;

public class Fertilize : Fragment {

	public bool validaterFertilize(Tile t, Tile f, int dir , int dDelta , int hDelta){
		if(t.marked)return true;
		
		t.marked=true;
		
		t.incRichness();
		t.incRichness();
		t.incRichness();
		
		if(t.SurfaceFrag && t.SurfaceFrag.GetType() == typeof(TreeFragment) && !((TreeFragment)t.SurfaceFrag).Burning){
			((TreeFragment)t.SurfaceFrag).Grow();

		}
		
		return true;
	}
	
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		if(HoverTile)GM.TileMan.Validate(HoverTile,2,999,false);
		if(HoverTile)HoverTile.Validate();
		return true;
	}
	
	//called when the host is selected and valid tile is selected
	override public bool SelectValidTile(Tile t){
		GM.TileMan.Validate(t,2,999,new TileManager.validater(validaterFertilize));
		Use();
		return true;
	}
	
	public override string getName ()
	{
		return "Blooming Flower";
	}
	
	public override string getDescription ()
	{
		return "Fertilize the affected tiles, causing them to yield an unusually high amount of fragments.";
	}
	
	//called when the unit is deselected - used to reset the host to before it is clicked on
	override public void Clear(){}
	
	/* DEFUNCT */
	//called when the host is selected and another unit within range is clicked on
	override public bool ActOnUnit(Unit u){return true;}
	//called when the unit is rolled over and when it is being placed
	override public void MarkTiles(Tile t){}
}
