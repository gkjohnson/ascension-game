using UnityEngine;
using System.Collections;

//Template for any component that needs to do something every turn change
abstract public class TurnChange : MonoBehaviour {
	
	protected GeneralManager GM;
	
	// Use this for initialization
	void Start () {
		GM = GeneralManager.GenMan;
		
		enabled=false;
	}
	
	//called every turn change
	abstract public void OnTurnChange();
	
}
