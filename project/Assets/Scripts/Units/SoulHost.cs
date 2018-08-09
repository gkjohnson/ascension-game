using UnityEngine;
using System.Collections;

//the host class for the basic soul unit so it can handle validating tiles and digging, merging, etc
public class SoulHost : Host {
	
	Unit u;
	Soul s;
	
	bool stun=false;
	public AudioClip mergeSound;
	
	//dictates whether or not hte unit can dig
	public bool Dug=false;
	
	//called on start
	override public void StartAux(){
		u=(Unit)GetComponent("Unit");
		s=(Soul)GetComponent("Soul");
	}
	
	//if stunned, set moved and dug to true (so it cannot do so) and set the color to blue if selected
	void LateUpdate(){
		//TODO: Make so this does not get called every frame
		if(GM.UnitMan.getSelected()!=u && Dug)renderer.material.color=new Color(1,1,1);
		
		if(GM.PlayerMan.CurrTurn==u.Player && stun){
			u.Moved=true;
			Dug=true;
			
			stun = false;
		}else if(stun){
			u.Moved=true;
			Dug=true;
		}
	}
	
	//set the unit to stunned so it cannot move during its next turn
	public void Stun(){
		stun=true;
	}
	
	//delegate used to validate the moveable tiles
	public bool validaterMove(Tile t, Tile f, int dir , int dDelta , int hDelta){
		return true;//dir==2;
	}
	
	//delegate function used to validate any tiles that can be dug into
	public bool validaterDig(Tile t, Tile f, int dir, int dDelta, int hDelta){
		
		if(t.getHeight() < Tile.MIN_HEIGHT)return false;
		
		if(dDelta>3)return false;
		if(t.Resident && t.Resident.Player == u.Player)return false;
		
		//if souls are across from an adjacent tile, allow the unit to dig
		if(t._adjacentTiles[dir] &&
		t._adjacentTiles[dir].Resident &&
		t._adjacentTiles[dir].Resident.Player == u.Player &&
		t._adjacentTiles[dir].Resident.GetComponent("SoulHost")){
			return true;
		}

		return false;
	}
	
	public const int POWER_SOUL_VAL = 4;
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		if(Dug)return true;
		
		//if the unit has enough souls, increase its movement range
		if(s.Souls >= POWER_SOUL_VAL){
			u.MOVE_DIST = 3;
			u.MOVE_HEIGHT=2;
		}else{
			u.MOVE_DIST = 2;
			u.MOVE_HEIGHT=1;
		}
		
		//if the unit can move, use the movement delegate function
		TileManager.validater f;
		int dist;
		int height;
		if(!u.Moved&&u.CanMove){
			f = new TileManager.validater(validaterMove);
			dist = u.MOVE_DIST;
			height = u.MOVE_HEIGHT;
			
		//otherwise use the dig function
		}else{
			f = new TileManager.validater(validaterDig);
			dist=1;
			height=2;
		}
		GM.TileMan.Validate(u.CurrentTile,dist,height,f);
		
		u.CurrentTile.Validate();
		
		return true;
	}
	
	//called when the host is selected and called every time a valid tile is hovered over
	override public bool TileHover(Tile t){
		return true;
	}
	
	//called when the host is selected and valid tile is selected
	override public bool SelectValidTile(Tile t){
		//if right clicked
		if(Input.GetMouseButtonDown(1)){
			//if digging below the current unit
			if(u.CurrentTile==t){
				if(t.getHeight() - Tile.MIN_HEIGHT > 0) Dig(t);
			//if merging to another unit
			}else if(t.Resident && t.Resident.Player==u.Player){
				if(!u.Moved){
					if(s.MergeTo(((Soul)t.Resident.GetComponent("Soul")))){
						MergeAnim ma = (MergeAnim)Instantiate((MergeAnim)Resources.Load("prefabs/mergeAnim",typeof(MergeAnim)),this.transform.position,this.transform.rotation);
						ma.setTarget(this.gameObject,t.Resident.gameObject);
						t.Resident.audio.PlayOneShot(mergeSound);
					}
				}	
			}else{
				//otherwise, look if its around the current tile
				for(int i=0;i<u.CurrentTile._adjacentTiles.Length;i++){
					Tile temp=u.CurrentTile._adjacentTiles[i];
					if(t==temp){
						//if there is a friendly unit directly across the tile
						if(temp._adjacentTiles[i] &&
							temp._adjacentTiles[i].Resident &&
							temp._adjacentTiles[i].Resident.Player==u.Player){
							//count how many friendly units are around the tile and dig
							int count=0;
							for(int j=0;j<temp._adjacentTiles.Length;j++){
								Tile temp2=temp._adjacentTiles[j];
								if(temp2 &&
								temp2.Resident && 
								temp2.Resident.Player==u.Player){
									count++;
								}
							}
							Dig(t,count-1,false);
						}
					}
				}
			}
		//if left clicked
		}else if(Input.GetMouseButton(0)){
			//move to the vaildated tile
			if(t!=u.CurrentTile && !u.Moved){
				((Unit)GetComponent("Unit")).MoveToTile(t);
			}
		}
		
		return true;
	}
	
	//dig into the tile
	void Dig(Tile t){
		Dig(t, 1);
	}
	//dig into a tile to depth num
	void Dig(Tile t, int num){
		Dig (t, num, true);
	}
	//dig into a time to depth num and give an item
	void Dig(Tile t, int num, bool giveFrag){
		if(Dug)return;
		
		//if the surface fragment could not be harvested for some reason
		if(!(t.SurfaceFrag && t.SurfaceFrag.Harvest())) t.Dig(num,giveFrag);
			
		Dug=true;
		
		//destroy buildings or deplete souls through digging
		Unit tu = t.Resident;
		if(tu){
			if(tu.GetComponent("Building")){
				((Building)tu.GetComponent("Building")).Exhausted();
			}else if(tu.GetComponent("Soul") && tu.Player != u.Player){
				((Soul)tu.GetComponent("Soul")).RemoveSouls(Mathf.Max(num-1,1));
			}
		}
			
		GM.UnitMan.Deselect();
	}
	
	//called when the unit is deselected - used to reset the host to before it is clicked on
	override public void Clear(){
	}
	
	//DEFUNCT
	
	//called when the host is selected and another unit within range is clicked on
	override public bool ActOnUnit(Unit u){
		return true;
	}
	//called when the unit is rolled over and when it is being placed
	override public void MarkTiles(Tile t){
	}
	
	public Unit getUnit(){return u;}
	public Soul getSoul(){return s;}
	
	public override void Selected ()
	{
		if(Dug || u.Player != GM.PlayerMan.CurrTurn)GM.UnitMan.Deselect();
	}

}
