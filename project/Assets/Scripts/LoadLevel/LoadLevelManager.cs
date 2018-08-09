using UnityEngine;
using System.Collections;
using System.IO;

// manages the creation of the map
public class LoadLevelManager : MonoBehaviour {
	
	static LoadLevelManager _self = null;
	public static LoadLevelManager LLM {
		get{ return _self; }
	}
	
	[SerializeField]
	Transform _lowPlane;
	[SerializeField]
	Transform _midPlane;
	[SerializeField]
	Transform _highPlane;
	
	// stores the type of game that was loaded
	StateType _type = (StateType)(-1);
	// stores the game name or ID of the currently loaded map
	string _gameNameId = "";
	
	// tiles to store the game state
	Tile[,] _lowTiles = null;
	Tile[,] _midTiles = null;
	Tile[,] _highTiles = null;
	
	//prepare the instance
	void Awake()
	{
		_self = this;
	}
	
	// get the state and store the the type of game that is being dealt with
	void Start ()
	{
		
		StateToLoad level = Component.FindObjectOfType(typeof(StateToLoad)) as StateToLoad;
		if( !level ) return;
		
		level.FetchData();
		
		_type = level.GetStateType();
		_gameNameId = level.GetNameID();

		//create the level with the data from the state
		GenerateLevel.Generate( level.GetByteData(), out _lowTiles, out _midTiles, out _highTiles, _lowPlane, _midPlane, _highPlane);
		
		//set up each tile
		foreach(Tile t in Component.FindObjectsOfType(typeof(Tile)) as Tile[])
		{
			t.GetNeighbors();
		}
		PlaneManager.PM.ReassociateTiles();
		
		Destroy(level.gameObject);
	}
	
	//returns the id of the game so it may be saved out again to its appropriate locale
	public string GetGameID()
	{
		return _gameNameId;
	}
	
	//returns information about the current game and wwhether or not it is a new game or not or networked or not
	public bool IsNetworked()
	{
		return _type == StateType.LOADED_NETWORKED || _type == StateType.NEW_NETWORKED;
	}
	public bool IsNew()
	{
		return _type == StateType.NEW_LOCAL || _type == StateType.NEW_NETWORKED;
	}
	
	
	public byte[] GetGameState()
	{
		return GenerateGameState.GenerateState( _lowTiles, _midTiles, _highTiles );
	}
	
	
	/*
	void Update()
	{
		
		//print( (FragType)FragmentManager.FM.GetRandomFrag(true));
		
		if( Input.GetKeyDown (KeyCode.P) )
		{
			File.WriteAllBytes(Application.dataPath + "/Resources/Maps/new.bytes", GenerateGameState.GenerateState(_lowTiles, _midTiles, _highTiles));
			print( "generated" );
		}

	}*/
	
	
	float updateInterval = 0.5f;
 
	private float accum = 0.0f; // FPS accumulated over the interval
	private int frames = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	void Update()
	{
	    timeleft -= Time.deltaTime;
	    accum += Time.timeScale/Time.deltaTime;
	    ++frames;
	 
	    // Interval ended - update GUI text and start new interval
	    if( timeleft <= 0.0 )
	    {
	        // display two fractional digits (f2 format)
	        timeleft = updateInterval;
	        accum = 0.0f;
	        frames = 0;
	    }
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(200,200,200,200), "" + (accum/frames).ToString("f2"));
	}
	


}
