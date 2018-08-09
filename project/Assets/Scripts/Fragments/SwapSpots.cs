using UnityEngine;
using System.Collections;

public class SwapSpots : Fragment {
	
	SoulHost first=null;
	
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		SoulHost[] sh = (SoulHost[])Component.FindObjectsOfType(typeof(SoulHost));
		
		if(!first){
			for(int i = 0 ; i < sh.Length; i++){
				((Unit)sh[i].GetComponent("Unit")).CurrentTile.Validate();
			}
		}else{
			string tag = ((Unit)first.GetComponent("Unit")).CurrentTile.transform.parent.tag;
			
			Unit fu = ((Unit)first.GetComponent("Unit"));
			//fu.CurrentTile.Validate();
			
			for(int i = 0 ; i < sh.Length; i++){
				Unit u =((Unit)sh[i].GetComponent("Unit"));
				
				if(fu.Player==u.Player)continue;
				
				Tile t = u.CurrentTile;
				if(t.transform.parent.tag == tag){
					t.Validate();
				}
			}
			
		}

		return true;
	}
	
	//called when the host is selected and valid tile is selected
	override public bool SelectValidTile(Tile t){
		if(first ==(SoulHost)t.Resident.GetComponent("SoulHost")){
			first = null;
			return true;
		}
		
		if(!first){
			first = (SoulHost)t.Resident.GetComponent("SoulHost");
		}else{
			Tile f = ((Unit)first.GetComponent("Unit")).CurrentTile;
			Tile s = t;
			
			Unit _first = f.Resident;
			Unit second = t.Resident;
			
			
			f.Resident=null;
			s.Resident=null;
			
			second.MoveToTile(f);
			second.ClickToTile(f);
			
			_first.MoveToTile(s);
			_first.ClickToTile(s);
			
			((GameObject)Instantiate(Resources.Load("Prefabs/TurretBulletExplosion",typeof(GameObject)),s.transform.position,s.transform.rotation)).transform.parent=s.transform;
			((GameObject)Instantiate(Resources.Load("Prefabs/TurretBulletExplosion",typeof(GameObject)),f.transform.position,f.transform.rotation)).transform.parent=f.transform;
			
			second.CurrentTile=f;
			_first.CurrentTile=s;
			
			f.Resident=second;
			s.Resident=_first;
			
			_first.Moved=false;
			second.Moved=false;
			
			s.audio.PlayOneShot(s.DigNoise);
			f.audio.PlayOneShot(f.DigNoise);
			
			second.Hover = false;
			
			Use();
		}
		return true;
	}
	
	public void LateUpdate(){
		if(first){
			first.renderer.material.color=new Color(0,0,1,1);
		}
	}
	public override string getName ()
	{
		return "Pair of Exotic Earrings";
	}
	
	public override string getDescription ()
	{
		return "Swap any two souls from different factions by clicking the first soul, then the second to swap. Select the first soul again to deselect it.";
	}
	
	//called when the unit is deselected - used to reset the host to before it is clicked on
	override public void Clear(){
		first = null;
	}
	
	/* DEFUNCT */
	//called when the host is selected and another unit within range is clicked on
	override public bool ActOnUnit(Unit u){return true;}
	//called when the unit is rolled over and when it is being placed
	override public void MarkTiles(Tile t){}
}
