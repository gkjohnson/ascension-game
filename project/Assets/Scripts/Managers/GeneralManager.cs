using UnityEngine;
using System.Collections;

//Main purpose of this class is to provide easy access to the other managers and handle other game related functions
public class GeneralManager : MonoBehaviour {
	
	//exposes access to the other managers
	public static GeneralManager GenMan;
	public UnitManager UnitMan;
	public PlayerManager PlayerMan;
	public TileManager TileMan;
	public PlaneManager PlaneMan;
	public FragmentManager FragMan;
	
	static GUIStyle genStyle;
	
	void Awake() {
		GenMan = this;
	}
	
	// find other managers
	void Start () {		
		UnitMan = (UnitManager)GameObject.FindObjectOfType(typeof(UnitManager));
		PlayerMan = (PlayerManager)GameObject.FindObjectOfType(typeof(PlayerManager));
		TileMan = (TileManager)GameObject.FindObjectOfType(typeof(TileManager));
		PlaneMan = (PlaneManager)GameObject.FindObjectOfType(typeof(PlaneManager));
		FragMan = (FragmentManager)GameObject.FindObjectOfType(typeof(FragmentManager));
	}
	
	
	// check if the player paused the game
	public bool Paused = false;
	void Update(){
		if(Input.GetKeyDown(KeyCode.Escape)){
			PauseGameManager.PGM.Pause();
		}
	}
	
	//if the game is paused, draw the pause screen
	void OnGUI(){
		/*if(Paused){
			GUI.skin = getStyle();
			GUI.color= new Color(0,0,0,1);
			//draw background boxes (multiple to enhance the color)
			GUI.Box(new Rect(0,0,Screen.width,Screen.height),"");
			GUI.Box(new Rect(0,0,Screen.width,Screen.height),"");
			GUI.Box(new Rect(0,0,Screen.width,Screen.height),"");
			
			//draw buttons that unpause, quit, and go to main menu
			GUI.color= new Color(1,1,1,1);
			if(GUI.Button(new Rect(Screen.width/2 - 150, Screen.height/2 - 25 - 55, 300, 50), "Resume")){
				Paused=false;
			}else if(GUI.Button(new Rect(Screen.width/2 - 150, Screen.height/2 - 25, 300, 50), "Quit")){
				Application.LoadLevel("StartScreen");
				//Application.LoadLevel(0);
			}else if(GUI.Button(new Rect(Screen.width/2 - 150, Screen.height/2 - 25 + 55, 300, 50), "Exit Game")){
				Application.Quit();
			}
		}*/
	}
	
	static GUISkin gs = null;
	//returns the general guistyle created for the game
	public static GUISkin getStyle(){
		if(!gs){
			gs=(GUISkin)Resources.Load("guiSkins/GameSkin");
		}
		return gs;
	}
	
}
