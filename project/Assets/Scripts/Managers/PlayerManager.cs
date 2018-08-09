using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	
	public int StartMinerals=0;
	public int ResetAP=10;
	public int MineralIncome=0;
	
	public int PlayerCount=2;
	public int[] _soulCap;
	public int[] _soulAmount;
	
	public int CurrTurn=0;
	
	const int MAX_TIME=250;
	float timer=MAX_TIME;
	
	public GeneralManager GM = null;
	public static PlayerManager PM;
	
	
	public Color[] PlayerColors = new Color[5];
	
	SoulVessel[] _vessels = null;
	
	public enum Move{
		NONE, CREATE_SOUL, CREATE_HOST
	}
	public Unit SelectedHost=null;
	public Move CurrentMove=Move.NONE;
	
	void Awake(){
		PM=this;
	}
	
	// Use this for initialization
	void Start () {	
		_soulCap=new int[PlayerCount];
		_soulAmount=new int[PlayerCount];
		
		_vessels = Component.FindObjectsOfType( typeof( SoulVessel ) ) as SoulVessel[];
		
		GM = (GeneralManager)GameObject.FindObjectOfType(typeof(GeneralManager));
	}
	
	void Update () {
		if(CheckWin()==-1){
		}else{
			SoulVessel[] sw = (SoulVessel[])Component.FindObjectsOfType(typeof(SoulVessel));
			for(int i=0;i<GM.PlayerMan.PlayerCount;i++){
				if(((Unit)sw[i].GetComponent("Unit")).Player == CheckWin()){
					sw[i].setWin();
					break;
				}
			}
		}
	}
	
	public Color getColor(int i){
		if(PlayerColors.Length<=i)return new Color(1,0,1,0);
		return PlayerColors[i];
	}
	
	public int getSoulCount(int p){
		return _soulCap[p];
	}
	
	public int SoulsToWin = 1;
	
	//checkes the win condition every turn - if there is only one player remaining - he wins
	public int CheckWin(){
		
		int WIN_SOUL_COUNT = SoulsToWin;
		
		for(int i=0; i < _vessels.Length; i++){

			Soul s = (Soul)_vessels[i].GetComponent("Soul");
			if(s.Souls >= WIN_SOUL_COUNT){
				return ((Unit)s.GetComponent("Unit")).Player;
			}
		}
		
				
		return -1;
	}
	//moves to the next turn for the next available player
	public int NextTurn(){
		if(CheckWin()!=-1)return CurrTurn;
		
		CurrTurn++;
		CurrTurn %=PlayerCount;
		
		UnitManager.UM.Deselect();
		FragmentManager.FM.Deselect();
		
		UnitManager.UM.OnChangeTurn();
		FragmentManager.FM.OnChangeTurn();
		PlaneManager.PM.OnChangeTurn();
		
		ClearMove();
				
		timer=MAX_TIME;
		
		TileManager.TM.NormalizeTerrain();
		
		Object[] changeTurnComp=Object.FindObjectsOfType(typeof(TurnChange));
		for(int i=0;i<changeTurnComp.Length;i++){
			((TurnChange)changeTurnComp[i]).OnTurnChange();
		}
		
		return CurrTurn;
	}
	
	public int GetPrevTurn(){
		int tempTurn = CurrTurn - 1;
		if(tempTurn == -1){
			tempTurn = PlayerCount-1;
		}
		return tempTurn;
	}
	
	public GUISkin gs;
		
	//Draws player information on the UI
	void OnGUI() {
		//Don't Draw anything if it's paused
		if(GM.Paused)return;
		
		
		if (GUI.Button(new Rect(Screen.width-60, Screen.height-90, 50, 50), (Texture2D)Resources.Load("GUITexture/nextTurn",typeof(Texture2D)),""))NextTurn();

		//Display which players turn it is
		//GUI.Label(new Rect(Screen.width-300,Screen.height-60,100,100),"Player's Turn: "+CurrTurn.ToString());
		
		//TODO: Make the buy unit/host buttons unclickable if the funds are not available
		//Display available moves
		if(CheckWin()!=-1){
			if(Input.GetButtonDown("Space"))Application.LoadLevel("StartScreen");
			
			//GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/2 - 100, 200, 100), "Player " + CheckWin() + " Wins!!!");
			GUI.DrawTexture(new Rect(Screen.width/2 - 231, Screen.height/2 - 100, 462, 200),(Texture2D)Resources.Load("GUITexture/Winner")); 
		}
		
		// TIMER CODE TO DISPLAY TIMER //
		//displays the timer for the turn
		//GUI.Box(new Rect(Screen.width-300,Screen.height-30,290 * ((float)timer/MAX_TIME),20),Mathf.FloorToInt(timer).ToString());
    }
	
	void StartCreateHostMove(Unit h){
		if(CurrentMove==Move.CREATE_HOST && SelectedHost==h){
			CurrentMove=Move.NONE;
			return;
		}

		SelectedHost=h;
		StartMove(Move.CREATE_HOST);
	}
	
	//Set the move that thecurrent player is using
	void StartMove(Move m){
		if(CurrentMove==m && CurrentMove!=Move.CREATE_HOST){
			CurrentMove=Move.NONE;
			return;
		}
		GM.UnitMan.Deselect();	
		CurrentMove=m;
		
	}
	//Clear the move and set it back to none
	public void ClearMove(){
		CurrentMove=Move.NONE;
	}
	
	//instance of the units
	public Unit instance_BasicSoul;
	public AudioClip spawnUnitNoise;
	
	public Unit CreateUnit(Tile t, Unit u){
		return CreateUnit(t,u,true);
	}
	public Unit CreateUnit(Tile t, Unit u, bool explosion){
		Unit g;
		//if there is a unit on the tile
		if(t.Resident!=null){
			return null;
		//if the tile is empty
		}else{
			//make a new unit at the tile
			g=(Unit)Instantiate(u);
			g.CurrentTile=t;
			g.SetTile(t);
			g.ClickToTile(t);
			//g.Moved=true;
			g.Player=CurrTurn;
						
			if(explosion)((GameObject)Instantiate(Resources.Load("Prefabs/TurretBulletExplosion",typeof(GameObject)),g.transform.position,g.transform.rotation)).transform.parent=g.CurrentTile.transform;
			
			//* Aesthetics for the spawning *//
			t.audio.PlayOneShot(spawnUnitNoise);
			
			AscendLight al = (AscendLight)((GameObject)Instantiate(Resources.Load("prefabs/AscendLight"))).GetComponent("AscendLight");
			al.setTile(t);
			al.OneShot();

		}
				
		return g;
	}
	
}
