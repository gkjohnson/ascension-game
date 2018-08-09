using UnityEngine;
using System.Collections;

public class LoadNewLocalGame : StateToLoad {
	
	TextAsset _levelstate;
	
	//returns the data as a string
	public override string GetStringData ()
	{
		if( !_levelstate ) return null;
		return _levelstate.text;
	}
	//returnsthe state data as a byte array
	public override byte[] GetByteData ()
	{
		if( !_levelstate ) return null;
		return _levelstate.bytes;
	}
	
	//fetches the state data
	public override bool FetchData ()
	{
		try
		{
			_levelstate = Resources.Load( "Maps/" + _nameid , typeof(TextAsset) ) as TextAsset;
		}
		catch( UnityException e )
		{
			return false;
		}
		
		if( _levelstate == null ) return false;
		
		return true;
	}
	
}
