using UnityEngine;
using System.Collections;

//Called on every soul well when the turn is changed
public class SoulWellTurnChange : TurnChange {
	
	//add two souls to the surrounding tiles of the current player
	public override void OnTurnChange(){
		Unit u=((Unit)GetComponent("Unit"));
		
		if(GM.PlayerMan.CurrTurn==u.Player){
			//add souls until 2 have been added around the soul well
			int count=0;
			for(int i=0;i<u.CurrentTile._adjacentTiles.Length;i++){
								
				Tile t = u.CurrentTile._adjacentTiles[i];
				
				if(t && !t.Resident){
					count++;
					GM.PlayerMan.CreateUnit(t,GM.PlayerMan.instance_BasicSoul);
				}
				
				if(count==2)break;
			}
		}
	}
	
}
