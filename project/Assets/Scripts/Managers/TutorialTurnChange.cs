using UnityEngine;
using System.Collections;

//Because the tutorial requires that the first player always be in control, this ensures that the turn is always set back to the first player
public class TutorialTurnChange : TurnChange {
	
	bool Change = false;
	
	public void Start(){
		GM = GeneralManager.GenMan;
		
		enabled=true;
	}
	//change turns if needed
	 public void Update(){
		if(Change){
			Change = false;
			GM.PlayerMan.NextTurn();
		}
	}
	
	//if it is not the first players turn, ensure that the component is flagged to change the turn again
	public override void OnTurnChange (){
		if(GM.PlayerMan.CurrTurn != 0){
			//GM.PlayerMan.NextTurn();
			Change = true;
		}
	}
}
