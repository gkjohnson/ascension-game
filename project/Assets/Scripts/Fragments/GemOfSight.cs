using UnityEngine;
using System.Collections;

public class GemOfSight : Fragment {

	public bool validaterReveal(Tile t, Tile f, int dir , int dDelta , int hDelta){
		if(t.marked)return true;
		//if(t.PeekFragment()==FragType.WOOD ||t.PeekFragment()==FragType.STONE||t.PeekFragment()==FragType.SOUL_GEM){
		//	return true;
		//}
		
		
		if(t.Resident && t.Resident.GetComponent("Building"))return true;
		if(t.Resident && t.Resident.GetComponent("SoulWellTurnChange"))return true;
		if(t.Resident)return true;
		if(t.SurfaceFrag){
			if(!(t.SurfaceFrag.GetType() == typeof(TreeFragment) && ((TreeFragment)t.SurfaceFrag).Growth==0)){
				return true;
			}
		}
		Fragment frag = FragmentManager.CreateFragment(t.PeekFragment());
	
		FragmentRemains fr = (FragmentRemains)((GameObject)Instantiate(Resources.Load("Prefabs/FragmentRemains"))).GetComponent("FragmentRemains");
		fr.transform.position=t.transform.position;
		fr.SetFragment(frag);
		fr.SetTile(t);
		fr.SetDeltaVector(new Vector3(0,3,0));
		fr.transform.localScale*=.75f;
		fr.SetTimer(300);
		
		Destroy(frag);
		
		t.marked=true;
		return true;
	}
	
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		
		if(HoverTile)GM.TileMan.Validate(HoverTile,3,999,false);
		if(HoverTile)HoverTile.Validate();
		return true;
	}
	
	//called when the host is selected and valid tile is selected
	override public bool SelectValidTile(Tile t){
		GM.TileMan.Validate(HoverTile,3,999,new TileManager.validater(validaterReveal));
		validaterReveal(t,null,0,0,0);
		Use();
		return true;
	}
	
	public override string getName ()
	{
		return "Gem of Sight";
	}
	public override string getDescription ()
	{
		return "See within a set of tiles to reveal which fragments will be unearthed next. Only tiles with nothing on them can be revealed.";
	}
	
	//called when the unit is deselected - used to reset the host to before it is clicked on
	override public void Clear(){}
	
	/* DEFUNCT */
	//called when the host is selected and another unit within range is clicked on
	override public bool ActOnUnit(Unit u){return true;}
	//called when the unit is rolled over and when it is being placed
	override public void MarkTiles(Tile t){}
	
}
