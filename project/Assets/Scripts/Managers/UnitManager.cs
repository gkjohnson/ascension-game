using UnityEngine;
using System.Collections;

// This handles selection and managing of units
public class UnitManager : MonoBehaviour {
	
	private Unit Selected=null;
	static GeneralManager GM=null;
	public static UnitManager UM;
	
	//initialize reference to itself
	void Awake(){
		UM=this;	
	}
	
	//store access to the generalmanager
	void Start () {
		GM = (GeneralManager)GameObject.FindObjectOfType(typeof(GeneralManager));
	}
	
	//selects the passed unit, deselects it if it is already selected
	public bool Select(Unit u){
		//dont select if it has no player assigned
		if(u.Player==-1)return false;
		//dont select if the unit has no soul
		if(!(Soul)u.GetComponent("Soul"))return false;
		 
		//deselect any fragments
		GM.FragMan.Deselect();
		
		//dont select if it was already selected
		if(Selected==u){
			Deselect();
		}
		else{
			Deselect();
			Selected=u;
		}
		
		// notify the host if it exists that the unit was selected
		Host h = u.GetComponent<Host>();
		if(h){
			h.Selected();
		}
		
		//clear any waiting actions in the tile manager
		GM.TileMan.ClearActions();
		
		return true;
	}
	
	//returns the selected unit
	public Unit getSelected(){
		return Selected;
	}
	
	//deselects the currently selected unit
	public void Deselect(){
		if(!Selected)return;
		
		//clear the host's state if it exists
		Host h=(Host)Selected.GetComponent("Host");
		if(h)h.Clear();
		
		Selected=null;
		GM.PlayerMan.ClearMove();
	}
	
	//set all units to have not moved on change turn
	public void OnChangeTurn(){
		//cycle through every unit and set it to have not moved
		Unit[] units = (Unit[])GameObject.FindObjectsOfType(typeof(Unit));
		for(int i=0;i<units.Length;i++){
			units[i].Moved=false;
		}
	}
}
