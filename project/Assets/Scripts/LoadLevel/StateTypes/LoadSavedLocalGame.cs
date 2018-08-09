using UnityEngine;
using System.Collections;
using System;

public class LoadSavedLocalGame : StateToLoad {
	
	byte[] data = null;
	
	//returns the byte data for the state
	public override byte[] GetByteData ()
	{
		return data;
	}
	
	//returns the string data for the state
	public override string GetStringData ()
	{
		return Convert.ToBase64String(data);
	}
	
	// fetches the data for the state
	public override bool FetchData ()
	{
		data = SaveLocalGame.LoadGameState( _nameid );
		if( data == null ) return false;
		
		return true;
	}
}
