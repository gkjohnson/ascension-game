using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class SaveLocalGame : MonoBehaviour {
	const string LOCAL_SAVED_GAMEID_LIST = "local_saved_games";
	const string LOCAL_SAVED_GAME_PREFIX = "local_game_save_L";
	const int MAX_SAVE_GAMES = 5;
	const char SPLIT_CHAR = ' ';
	
	//Load and save local games
	public static bool SaveGameState(byte[] gamestate, string id)
	{
		List<string> list = GetGameIDs();
				
		id = id.Trim();
		if( id == "")
		{
			//find a new id for the thing
			for( int i = 0 ; i < MAX_SAVE_GAMES ; i ++ )
			{
				if( !list.Contains( i.ToString() ) )
				{
					id = i.ToString();
					break;
				}
			}	
		}
		
		//if adding the id will overflow the max amount of allowed games
		if( !list.Contains( id ) && list.Count >= MAX_SAVE_GAMES ) return false;
		
		// add the id and save the game
		string key = LOCAL_SAVED_GAME_PREFIX + id;
		PlayerPrefs.SetString( key , Convert.ToBase64String( gamestate ) );
		PlayerPrefs.Save();
		
		if( !list.Contains( id ) ) list.Add(id);
		SetGameIDs( list );
		
		return true;
	}
	//returns the game state associated with the given id
	public static byte[] LoadGameState(string id)
	{
		string key = LOCAL_SAVED_GAME_PREFIX + id;
		
		if( !PlayerPrefs.HasKey(key) ) return null;
		
		return Convert.FromBase64String(PlayerPrefs.GetString(key));
	}
	//removes the passed save game
	public static bool DeleteSaveGame(string id)
	{
		string key = LOCAL_SAVED_GAME_PREFIX + id;
		if( !PlayerPrefs.HasKey(key) ) return false;
		
		PlayerPrefs.DeleteKey( key );
		PlayerPrefs.Save();
		
		//remove the id from the current list	
		List<string> ls = GetGameIDs();
		ls.Remove( id );
		SetGameIDs( ls );
		
		return true;
	}
	
	//returns a list of the ids as string
	public static List<string> GetGameIDs()
	{
		List<string> ls = new List<string>();
		string[] list = PlayerPrefs.GetString( LOCAL_SAVED_GAMEID_LIST , "" ).Split( SPLIT_CHAR );
		
		foreach( string s in list )
		{
			if( s != "" ) ls.Add( s );
		}
		
		return ls;
	}
	
	//sets the ids
	static void SetGameIDs( List<string> list )
	{
		string s = "";
		for( int i = 0 ; i < list.Count ; i ++ )
		{
			s += list[i];
			if( i != list.Count - 1 ) s += SPLIT_CHAR;
		}
		
		print(" set : " + s );
		
		PlayerPrefs.SetString( LOCAL_SAVED_GAMEID_LIST, s );
		PlayerPrefs.Save();
	}
	
	
			
			
}
