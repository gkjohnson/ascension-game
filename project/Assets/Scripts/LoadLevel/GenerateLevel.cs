using UnityEngine;
using System.Collections;

//generates the level from the give state
public abstract class GenerateLevel : MonoBehaviour {
	
	public enum UnitTypes
	{
		NONE = 0,
		SPAWNER = 1,
		SHRINE = 2,
		TEMPLE = 3,
		OBELISK = 4,
		SOUL = 5
	};
	
	public enum SurfaceFragmentTypes
	{
		NONE = 0,
		TREE = 1,
		SURFFRAG = 2,
		FOUNDATIONTILE = 3
	}
	
	
	//list of generators the for the given state with teh given version
	delegate bool _generateLevel( byte[] state, out Tile[,] lowarray, out Tile[,] midarray, out Tile[,] higharray, Transform lowplane, Transform midplane, Transform highplane);
	
	static _generateLevel[] _generators =
	{
		Generator0_1.Generator, // 0
		// 1
	};
	

	//generates the given state and places the tiles in the given arrays
	public static bool Generate( byte[] state, out Tile[,] low, out Tile[,] mid, out Tile[,] high, Transform lowplane, Transform midplane, Transform highplane )
	{
		//Get the first line which we expect to be at most 50 lines
		int i = 0 ;
		string s = "";
		while( state[i] != (byte)'\n' && i < 50)
		{
			s += (char)state[i];
			i ++;
		}
		
		low = null;
		mid = null;
		high = null;
		
		//see if it says version in it
		if( s.IndexOf("VERSION ") != -1 )
		{
			//get the secon part of the line to see what version the file format is
			string[] spl = s.Split(' ');
			
			switch( spl[1] )
			{
			case "0.1":
				return _generators[0](state, out low, out mid, out high, lowplane, midplane, highplane);
			case "0.2":
				break;
			}
			
		}
		
		return false;
	}
	
	//parses the string and gets the fragments for the players
	public static void ParsePlayersFragments(string s)
	{
		int start = 0;
		if( s.IndexOf("FRAGS") != -1 ) start = 1;
		
		//cut off the beginning "FRAGS "
		s = s.Substring(6 * start);
		string[] spl = s.Split('|');
		
		//go through the list of frags
		for(int i = 0 ; i < spl.Length ; i ++)
		{
			//go through the list of frags per player
			string[] frags = spl[i].Split(' ');
			for(int j = 0 ; j < frags.Length ; j ++)
			{
				if(frags[j] != "")
				{
					Fragment f = FragmentManager.CreateFragment( frags[j] );
					f.Fresh = false;
					FragmentManager.FM.Collect( f , i );
				}
			}
		}
	}
}
