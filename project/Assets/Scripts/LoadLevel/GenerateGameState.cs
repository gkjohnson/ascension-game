using UnityEngine;
using System.Collections;
using System.IO;

public abstract class GenerateGameState : MonoBehaviour {
		
	// Generates a Version 0.1 gamestate
	public static byte[] GenerateState(Tile[,] low, Tile[,] mid, Tile[,] high)
	{
		return Generator0_1.GenerateState( low, mid, high);
	}
	
	public static string GetFragList()
	{
		string s = "";
		for(int i = 0 ; i < PlayerManager.PM.PlayerCount ; i ++)
		{
			for(int f = 0 ; f < FragmentManager.FM.getFragCount( i ) ; f ++)
			{
				if( FragmentManager.FM.GetFragment(f,i) != null ) s += FragmentManager.FM.GetFragment(f,i).GetType().ToString() + " ";				
			}
			s+= "|";
		}
		return s;
	}
}
