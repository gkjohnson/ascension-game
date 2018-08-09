using UnityEngine;
using System.Collections;

//called every turn on every soul to update the move and dug status of each unit
public class SoulTurnChange : TurnChange {

	public override void OnTurnChange(){
		((Unit)GetComponent("Unit")).Moved=false;
		((SoulHost)GetComponent("Host")).Dug=false;
	}
}
