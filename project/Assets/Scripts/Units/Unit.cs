using UnityEngine;
using System.Collections;

//The basic component for something can reside on a tile and move between them

public class Unit : MonoBehaviour {
	//stores tiles for the unit
	public Tile CurrentTile = null;
	public Tile TempTile = null;
	
	public UnitManager _unitManager;
	
	//which player this is owned by
	public int Player;
	public bool Moved=false;
	
	//how can the unit can move
	public int MOVE_DIST=0;
	public int MOVE_HEIGHT=0;
	
	//is being hovered over
	public bool Hover=false;
	
	//is the unit purely a soul
	public bool Naked;
	
	int started=2;
	
	public int MineralCost;
	
	public GeneralManager GM;
	
	public AudioClip AscendNoise;
	
	public bool CanMove = true;
	
	void Start(){
		//Start by snapping the unit to the tile below
		if(!CurrentTile) findTile();
		if(CurrentTile != null)ClickToTile(CurrentTile);
		
		_unitManager=UnitManager.UM;
		
		gameObject.layer = LayerMask.NameToLayer("Unit");
		
		this.transform.RotateAround(new Vector3(0,1,0), Random.Range(0,6)*60);
	
		GM = GeneralManager.GenMan;
				
	}
	//update the highlight of the unit
	void Update(){
		
		
		if(started>0){
			started--;
			ClickToTile(CurrentTile);
		}
		UpdateHighlight();
	}
	
	float val=1.5f;
	void FixedUpdate(){
		
		float temp = Time.realtimeSinceStartup;
		
		//update the movement to a new tile if it has not already stuck to it
		if(!stick)UpdateMove();
		
		//make the unit bounce up and down if selected
		float s = this.transform.localScale.x;
		
		//bounce the unit if it is selected
		if(GetComponent("Host") && GetComponent("Host").GetType() == typeof(SoulHost)){
			
			if(stick && GM.UnitMan.getSelected() == this){
				
				Vector3 goal = transform.parent.position + new Vector3(0,Mathf.Sin(val)+1,0);
				transform.position -= (transform.position - goal)/5;
				
				if(GM.UnitMan.getSelected()==this)val+=.1f;
				else val+=.1f;
			
			
				this.transform.localScale = new Vector3(1,Mathf.Sin(val)*.25f + .75f,1);
				this.transform.localScale*=s;
			}
		}
		if(GM.UnitMan.getSelected()!=this){
			//Vector3 goal = new Vector3(1,1,1)*s;			
			transform.position -= (transform.position - transform.parent.position)/10;
			transform.localScale -= (transform.localScale - new Vector3(1,1,1)*s)/10;
			val=1.5f;
		}
		
	}
	
	//Update the units color based off of variables/scenarios
	public void UpdateHighlight(){
		//TODO: Make it so this is not called every frame
		
		//if(player==-1)return;
		if(_unitManager.getSelected()==this){
			renderer.material.color=Color.blue;
		//}else if(Hover){
			//renderer.material.color=Color.blue;
		}else if(Moved){
			renderer.material.color=Color.yellow;
		}else{
			renderer.material.color=Color.white;
			Soul s = (Soul)GetComponent("Soul");
			Host h = (Host)GetComponent("Host");
			if(Naked||!(h&&s&&s.Souls==0)){
				/*if(Player==0){
					renderer.material.color=new Color(1,.3f,.1f);
				}else if(Player==1){
					renderer.material.color=new Color(0,1,.9f);
				}*/
				renderer.material.color = GM.PlayerMan.getColor(this.Player);
			}
		}
		
		//RANDOM COLORS//
		//renderer.material.color = new Color(Random.Range(0.0f, 1.0f),Random.Range(0.0f, 1.0f),Random.Range(0.0f, 1.0f));
	}
	
	//is the unit stuck to a tile
	bool stick=false;
	
	//movement speeds
	const int BASERATIO=12;
	const int ASCENDRATIO=7;
	int _moveRatio=5;
	
	public bool isMoving(){return  !stick;}
	
	//mode when ascending -- emits particles and moves quickly
	public void AscendMode(){
		_moveRatio=ASCENDRATIO;
		particleSystem.enableEmission=true;
		audio.PlayOneShot(AscendNoise);
	}
	
	//Move the unit closer to the tile it is currently on
	void UpdateMove(){
		//if the object is moveable - ease it to the tile
		Tile t=CurrentTile;
		if(!t)return;
		if(TempTile)t=TempTile;
		
		Vector3 dist = CurrentTile.transform.position-this.transform.position;
		
		//click if the unit is stuck to the tile
		if(stick||TempTile){
			ClickToTile(t); 
			stick=true;
			//return;
		}else if(!stick){
			if(dist.magnitude<.5f)stick=true;
			//print(dist.magnitude);
		}
		
		if(!TempTile){
			Vector3 v=t.transform.position-transform.position;
			v=v/_moveRatio;
			//((CharacterController)this.GetComponent("CharacterController")).Move(v);
			transform.position=transform.position + v;
		//otherwise click it there
		}else{
			ClickToTile(t);
		}
		
		if(_moveRatio==ASCENDRATIO && dist.magnitude<1){
			transform.position=CurrentTile.transform.position-new Vector3(0,3,0);
			MoveToTile(CurrentTile);
		}
		//print (this.particleSystem.enableEmission);
		
		TempTile=null;
	}
	
	//if moused over the unit, call mouse over on the tile it is on
	public void OnMouseOver(){
		CurrentTile.OnMouseOver();
	}

	public void OnMouseExit(){
		CurrentTile.OnMouseExit();
	}
	
	//snaps the the unit to the tile instantly, no easing
	public void ClickToTile(Tile t){
		if(t==null)return;
		this.transform.position=t.transform.position;
		this.transform.parent=t.transform;
		//make sure that there is no resident left on the tile
		//and change the current tile to t
		if(t.Resident==null){
			//CurrentTile=t;
			//t.Resident=this;
		}
	}
	
	//set the current tile to the passed tile t and acts as a move action for the unit
	public void MoveToTile(Tile t){
		if(t.Resident && t.Resident!=this)return;
		
		_moveRatio = BASERATIO;
		particleSystem.enableEmission=false;
		
		CurrentTile.Resident=null;
		CurrentTile=t;
		CurrentTile.Resident=this;
		MakeMove();
		
		this.transform.parent = t.transform;
	}
	
	//sets the tile to have moved for that turn
	void MakeMove(){Moved=true;stick=false;}
	
	
	//sets the current tile to the passed tile t
	public bool SetTile(Tile t){
		if(t.Resident)return false;
		CurrentTile=t;
		t.Resident=this;
		ClickToTile(t);
		return true;
	}
	
	//new function that uses raycasting to find the tiles its on top of
	void findTile(){
		RaycastHit hit;
		Vector3 dir = new Vector3(0,-30,0);
		if (Physics.Raycast(transform.position, dir, out hit,20)){
			SetTile(hit.transform.GetComponent<Tile>());
		}
	}
}
