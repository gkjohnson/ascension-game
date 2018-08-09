using UnityEngine;
using System.Collections;

public class SaveNetworkedGame : MonoBehaviour {
	const string NETWORKED_SAVED_GAMEID_LIST = "networked_saved_games";
	const string NETWORKED_SAVED_GAME_PREFIX = "networked_save_game_N";
	const int MAX_SAVE_GAMES = 5;
	
	//Manage locally saved network games
	public static bool SaveGameStateLocally(byte[] gamestate, string id)
	{
		if( id == "" )
		{
			
		}
		return false;
	}
	public static byte[] LoadLocalGameState( string id )
	{
		return null;
	}
	public static bool DeleteSaveGame( string id )
	{
		return false;
	}
	
	//Manage network saved games
	public static bool SaveNetworkedGameState( byte[] gamestate, string id )
	{
		return false;
	}
	public static byte[] LoadNetworkedGameState( string id )
	{
		return null;
	}
	
}
