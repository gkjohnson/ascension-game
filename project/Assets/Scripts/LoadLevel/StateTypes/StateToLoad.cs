using UnityEngine;
using System.Collections;

public enum StateType{
	NEW_LOCAL,
	LOADED_LOCAL,
	NEW_NETWORKED,
	LOADED_NETWORKED
}

// State that is to be loaded and generated into the current map
public abstract class StateToLoad : MonoBehaviour {
	
	protected string _nameid = "";
	protected StateType _type;
	
	//returns a component that stores everything needed to generate a level
	public static StateToLoad LoadLevel( string name_id, StateType type )
	{
		StateToLoad sl = null;
		GameObject go = new GameObject("LevelState");
		
		switch( type )
		{
		case StateType.NEW_LOCAL:
			sl = go.AddComponent<LoadNewLocalGame>();
			break;
		case StateType.NEW_NETWORKED:
			break;
		case StateType.LOADED_LOCAL:
			sl = go.AddComponent<LoadSavedLocalGame>();
			break;
		case StateType.LOADED_NETWORKED:
			break;
		}
		
		if( sl )
		{
			sl._nameid = name_id;
			sl._type = type;
			DontDestroyOnLoad( sl );
		}
		else
		{
			Destroy( go );
			go = null;
		}
		
		return sl;
	}
	
	//don't destroy this game object on load -- leave it to someone else
	void Awake()
	{
		DontDestroyOnLoad( gameObject );
	}
	
	//returns the type of level that is being played
	public StateType GetStateType()
	{
		return _type;
	}
	
	public string GetNameID()
	{
		return _nameid;
	}
	//allows the user to fetch and get data about the game state
	public abstract string GetStringData();
	public abstract byte[] GetByteData();
	
	// fetches the data that is needed to generate the level
	public abstract bool FetchData();
	
}
