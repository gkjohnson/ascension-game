using UnityEngine;
using System.Collections;

//called every turn change to see if the unearthed fragment should be reset
public class UnearthedFragmentTurnChange : TurnChange {
	
	//if the unearthed fragment is not activated, reset it with a certain chance
	public override void OnTurnChange ()
	{
		UnearthedFragment uf = ((UnearthedFragment)GetComponent("UnearthedFragment"));
		
		if(!uf.isActive() && Random.Range(0.0f,1.0f) >.80f)uf.reset();
	}
}
