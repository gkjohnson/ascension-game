using UnityEngine;
using System.Collections;

public class SoulRelic : Fragment {
	
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		
		SoulHost[] sh = (SoulHost[])Component.FindObjectsOfType(typeof(SoulHost));
		
		for(int i=0; i<sh.Length; i++){
			Unit u = (Unit)sh[i].GetComponent("Unit");
			u.CurrentTile.Validate();
		}

		return true;
	}
	
	//called when the host is selected and valid tile is selected
	override public bool SelectValidTile(Tile t){
		
		Soul s = (Soul)t.Resident.GetComponent("Soul");
		Unit u = (Unit)t.Resident.GetComponent("Unit");
		
		SoulHost sh = (SoulHost)t.Resident.GetComponent("SoulHost");
		
		if(u.Player != GM.PlayerMan.CurrTurn){
			sh.Stun();
			//s.RemoveSouls(1);
		}else{
			s.AddSouls(1);
		}
		
		t.audio.PlayOneShot(t.DigNoise);
		
		Instantiate(Resources.Load("Prefabs/TurretBulletExplosion"),t.transform.position, t.transform.rotation);
		
		Use();
		return true;
	}
	
	public override string getName ()
	{
		return "Soul Food";
	}
	
	public override string getDescription ()
	{
		return "Empower your own souls by increasing the amount of souls merged in the selected unit, or stun those of your enemy for one turn.";
	}
	
	//called when the unit is deselected - used to reset the host to before it is clicked on
	override public void Clear(){}
	
	/* DEFUNCT */
	//called when the host is selected and another unit within range is clicked on
	override public bool ActOnUnit(Unit u){return true;}
	//called when the unit is rolled over and when it is being placed
	override public void MarkTiles(Tile t){}
}
