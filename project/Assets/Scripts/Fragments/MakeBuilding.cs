using UnityEngine;
using System.Collections;

public class MakeBuilding : Fragment {
	
	Building type = null;
	
	public override void StartAux ()
	{
	}
	
	public void Build(Building b){
		type=b;
	}
	public Building GetBuilding(){
		return type;
	}
	public static bool CanBuild(Building b){
		
		GeneralManager nGM = Component.FindObjectOfType(typeof(GeneralManager)) as GeneralManager;
		
		int g = nGM.FragMan.CountGem(nGM.PlayerMan.CurrTurn);
		int s = nGM.FragMan.CountStone(nGM.PlayerMan.CurrTurn);
		int w = nGM.FragMan.CountWood(nGM.PlayerMan.CurrTurn);
			
		if(b.getGem()<=g &&
		b.getStone()<=s&&
		b.getWood()<=w){
			return true;
		}
		return false;
	}
	
	
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		
		if(!CanBuild(type))return true;
		
		Tile[] t = (Tile[])Component.FindObjectsOfType(typeof(Tile));
		
		for(int i=0 ; i < t.Length ; i++){
			if(t[i].Foundation && (!t[i].Resident || t[i].Resident && t[i].Resident.Player == GM.PlayerMan.CurrTurn) && !t[i].SurfaceFrag)t[i].Validate ();
		}
	
		return true;
	}
	
	//called when the host is selected and valid tile is selected
	override public bool SelectValidTile(Tile t){		
		
		GM.FragMan.UseGems(type.getGem());
		GM.FragMan.UseStone(type.getStone());
		GM.FragMan.UseWood(type.getWood());
		
		if(t.Resident && t.Resident.GetComponent("Building")){
			((Building)t.Resident.GetComponent("Building")).Exhausted();
		}
		
		
		GM.PlayerMan.CreateUnit(t,(Unit)type.GetComponent("Unit"),true);
		
		Use();
		return true;
	}
	
	public override string getName ()
	{
		return "Build " + type.GetType();
	}
	public override string getDescription ()
	{
		Shrine s = (Shrine)type.GetComponent("Shrine");
		if(s){
			return "Build a spiritual " + type.GetType() + " that allows any unit with " + s.SOULS_TO_ASCEND + " merged souls";
		}
		
		return "";
	}
	
	
	//called when the unit is deselected - used to reset the host to before it is clicked on
	override public void Clear(){}
	
	/* DEFUNCT */
	//called when the host is selected and another unit within range is clicked on
	override public bool ActOnUnit(Unit u){return true;}
	//called when the unit is rolled over and when it is being placed
	override public void MarkTiles(Tile t){}
	
	
}
