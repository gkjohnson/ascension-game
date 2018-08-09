using UnityEngine;
using System.Collections;

//called only when a turn has changed, this component ascends units when applicable
public class ShrineTurnChange : TurnChange{
	
	//public int maxAscentions = 6;	
	Unit u;
	
	void Start(){
		GM = GeneralManager.GenMan;
		u=(Unit)GetComponent("Unit");
	}
	
	//on turn change, ascent all units with the appropriate number of souls to the next level
	override public void OnTurnChange(){
		int toAscend = ((Shrine)GetComponent("Shrine")).SOULS_TO_ASCEND;
		
		Tile[] t = u.CurrentTile._adjacentTiles;
		
		for(int i=0;i<t.Length ; i++){
			if(!t[i] || !t[i].Above)continue;
			if(t[i].Above.Resident)continue;
			
			Unit r = t[i].Resident;
			if(r){
				//if the residents turn just passed and it has a soul
				if(r.Player==GM.PlayerMan.GetPrevTurn() &&
					r.Player == u.Player &&
					r.GetComponent("Soul")){
					
					
					//if there are enought souls to ascend
					Soul s = ((Soul)r.GetComponent("Soul"));
					if(s.Souls>=toAscend){
						
						//asend then soul and play the ascend animation
						if(((Shrine)GetComponent("Shrine")).glowLight) ((Shrine)GetComponent("Shrine")).glowLight.intensity=5;
						
						
						Tile prevTile = r.CurrentTile;
						
						r.MoveToTile(t[i].Above);
						r.AscendMode();
						
						//if there are souls left over, create a new unit
						if(s.Souls%toAscend!=0){
							Soul newS = ((Soul)GM.PlayerMan.CreateUnit(prevTile, GM.PlayerMan.instance_BasicSoul,false).GetComponent("Soul"));
							newS.SetSouls(s.Souls%toAscend);
							((Unit)newS.GetComponent("Unit")).Player=r.Player;
						}
						
						//turn the amount of souls needed to ascend into one soul each
						s.SetSouls((int)s.Souls/toAscend);
						
						/*
						maxAscentions-= s.Souls;
						if(maxAscentions==0){
							Exhausted();
							break;
						}
						*/
					}
				}
			}
		}
	}
	
	void Exhausted(){
		((Building)GetComponent("Building")).Exhausted();
	}
	
}
