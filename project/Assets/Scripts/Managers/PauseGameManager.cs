using UnityEngine;
using System.Collections;

public class PauseGameManager : MonoBehaviour {
	
	//is game paused
	bool _paused = false;
	
	//reference to self
	static PauseGameManager _self = null;
	public static PauseGameManager PGM {
		get{ return _self; }
	}
	
	//prepare the self reference
	void Awake()
	{
		_self = this;
	}
	
	//returns if the game is paused or not
	public bool IsPaused(){ return _paused; }
	public void Pause()
	{
		_paused = true;
		Time.timeScale = 0.0f;
		enabled = true;
	}
	public void Unpause()
	{
		_paused = true;
		Time.timeScale = 1.0f;
		enabled = false;
	}
	
	//draws the pause screen
	void OnGUI()
	{
		if(!_paused) return;
		
		if( LoadLevelManager.LLM.IsNetworked() ) DrawNetworkedGUI();
		else DrawLocalGUI();
	}
	
	//draw the networked version of the menu
	void DrawNetworkedGUI()
	{
		//Resume
		//Unpause();
		//Options
		
		//Quit and Save Turn
		
		//End Turn, upload, and quit
		
		//Quit to Menu and lost state
	}
	//draw the local version of the menu
	void DrawLocalGUI()
	{
		//Resume
		//Unpause();
		//Options
		
		//Quit and Save Game
		if( GUI.Button(new Rect( 10, 10, 200,200), "SAVE AND CLOSE") )
		{
			Unpause();
			Application.LoadLevel("Start");
			print( "unpause");
			
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
			
			if( LoadLevelManager.LLM.IsNew() )SaveLocalGame.SaveGameState( LoadLevelManager.LLM.GetGameState() , "");
			else SaveLocalGame.SaveGameState( LoadLevelManager.LLM.GetGameState() , LoadLevelManager.LLM.GetGameID() );
		}
		
		//Quit To Menu and lose state
	}
}
