using UnityEngine;
using System.Collections;

public class Generator0_1 : GenerateLevel {
	
	const byte HEIGHT_MASK = 31;
	const byte RICHNESS_MASK = 224;
	
	const byte UNIT_MASK = 31;
	const byte PLAYER_MASK = 224;
	
	const byte SOULMOVED_MASK = 128;
	const byte SOULCOUNT_MASK = 127;
	
	const byte FRAGMENT_MASK = 31;
	const byte SURFFRAG_MASK = 7;
		
	#region Generate Map
	//GENERATE MAP
	static public bool Generator( byte[] state, out Tile[,] lowarray, out Tile[,] midarray, out Tile[,] higharray, Transform lowplane, Transform midplane, Transform highplane)
	{		
		// PARSE HEADER
		
		// byte array converted to string
		string statestring = System.Text.Encoding.ASCII.GetString( state );
		
		// header variables
		int w = 0;
		int h = 0;
		string woodstr = "";
		string gemstr = "";
		string stonestr = "";
		int players = 0;
		string datatype = "";
		
		// get the variables in the header
		foreach( string s in statestring.Split('\n') )
		{
			string[] spl = s.Split(' ');
			switch( spl[0] )
			{
			case "WIDTH":
				w = int.Parse(spl[1]);
				break;
			case "HEIGHT":
				h = int.Parse(spl[1]);
				break;
			case "PLAYERS": 
				players = int.Parse(spl[1]);
				break;
			case "FRAGS":
				GenerateLevel.ParsePlayersFragments( s );
				print(s);
				break;
			case "WOOD":
				woodstr = s;
				break;
			case "STONE":
				stonestr = s;
				break;
			case "GEMS":
				gemstr = s;
				break;
			case "CURRENTTURN":
				PlayerManager.PM.CurrTurn = int.Parse( spl[1] );
				break;
			case "DATA":
				datatype = ( spl.Length > 1 )? spl[1] : "";
				break;
			}
			
			if( spl[0] == "DATA" ) break;
		}
		
		// Set wood
		string[] tempspl = woodstr.Split(' ');
		for( int i = 1; i < tempspl.Length && i < players + 1; i ++)
		{
			FragmentManager.FM.SetWood( int.Parse(tempspl[ i ]) , i );
		}
		// Set stone
		tempspl = stonestr.Split(' ');
		for( int i = 1; i < tempspl.Length && i < players + 1; i ++)
		{
			FragmentManager.FM.SetStone( int.Parse(tempspl[ i ]) , i );
		}
		// Set stone
		tempspl = gemstr.Split(' ');
		for( int i = 1; i < tempspl.Length && i < players + 1; i ++)
		{
			FragmentManager.FM.SetGems( int.Parse(tempspl[ i ]) , i );
		}
		
		//PARSE BODY DATA
		
		//make the arrays more easily accesible via layers
		lowarray = new Tile[w,h];
		midarray = new Tile[w,h];
		higharray = new Tile[w,h];
		Tile[][,] tilearrays = { lowarray, midarray, higharray };
		Transform[] transarray = { lowplane, midplane, highplane };
		
		// point at which the binary data starts
		int dataStart = statestring.IndexOf("\n", statestring.IndexOf("DATA")) + 1;
				
		//copy data out to decompress it
		if( datatype == "binary_compressed" )
		{
			byte[] boarddata = new byte[ state.Length - dataStart ];
			System.Buffer.BlockCopy( state, dataStart, boarddata, 0, state.Length - dataStart );
			
			boarddata = CompressionHelper0_1.Decompress( boarddata );
			
			state = boarddata;
			dataStart = 0;
		}
		
		//generate the level
		for( int i = dataStart ; i < state.Length ; i ++ )
		{
			//get the x and y and layer for the tile
			int y = (i-dataStart) / (6 * w);
			int x = (((i-dataStart) / 6) - y * w)%w;
			int layer = y / h;
			y %= h;
			
			// if the layer is too big, run
			if( layer > 2 ) break;
			
			//store the recent tile
			Tile t = null;
			
			if( state[i] != 0 )
			{	 
				const float rad = 4.31f;

				//create the tile
				t = Instantiate( Resources.Load("prefabs/Tile", typeof(Tile)) ) as Tile;
			
				//place and store the layer
				t.transform.parent = transarray[ layer ];
				tilearrays[layer][x , y] = t;
				
				t.transform.localPosition = new Vector3( (x - w/2) * rad * 3.45f + rad * (y%2) * 1.725f ,0, (y - h/2) * rad * 1);
				
				// BYTE 1
				t.ORIG_HEIGHT = state[i] - 1 - 10; // original height
				// BYTE 2
				t.setHeight( (state[i + 1] & HEIGHT_MASK) -1 - 10 ); // current height
				t.setRichness( ((state[i + 1] & RICHNESS_MASK) >> 5) - 1); // current richness
				
				// BYTE 3 & 4
				ByteToUnit( state[ i + 2 ], state[ i + 3 ] , t ); //create unit on the tile
				
				// BYTE 5
				ByteToSurfFrag( state[ i + 4 ] , t );
				
				// BYTE 6
				t.setFragment( (FragType) (state[i + 5] & FRAGMENT_MASK) ); //sets the internal fragment of the tile
				
				
			}
			
			
			i += 5;
		}
		
		return false;
	}
	
	//convert the byte to unit and souls to the tile
	static Unit ByteToUnit( byte b , byte souls, Tile t)
	{
		byte player = (byte)(( PLAYER_MASK & b) >> 5); // player number assigned
		byte unit = (byte)(UNIT_MASK & b); // unit number
				
		Unit u = null;
		
		string name = "";
		
		switch( unit )
		{
		case (byte)GenerateLevel.UnitTypes.OBELISK:
			name = "Prefabs/buildings/SoulVessel";
			break;
		case (byte)GenerateLevel.UnitTypes.SHRINE:
			name = "Prefabs/buildings/Shrine";
			break;
		case (byte)GenerateLevel.UnitTypes.SOUL:
			name = "Prefabs/BasicSoul";
			break;
		case (byte)GenerateLevel.UnitTypes.SPAWNER:
			name = "Prefabs/buildings/SoulWell";
			break;
		case (byte)GenerateLevel.UnitTypes.TEMPLE:
			name = "Prefabs/buildings/Temple";
			break;
		default :
			return null;
		}
				
		u = (Instantiate( Resources.Load(name), Vector3.zero, Quaternion.identity ) as GameObject).GetComponent<Unit>();
				
		if( u )
		{
			u.transform.parent = null;
			u.Player = player;
			if( t ) u.SetTile( t );			
			if(u.GetComponent<SoulHost>()){
				int s = souls & SOULCOUNT_MASK;
				bool didmove = (souls & SOULMOVED_MASK) != 0;
				u.GetComponent<Soul>().SetSouls( s );
				u.Moved = didmove;
			}
		}
		
		return u;
	}
	
	//converts the byte to surface fragment
	static SurfaceFragment ByteToSurfFrag( byte b , Tile t)
	{
		
		byte param = (byte)((b>>3) & FRAGMENT_MASK); //fragment type number 
		byte surfnum = (byte)(b & SURFFRAG_MASK); //surface fragment number
		
		SurfaceFragment sf = null;
		
		switch( surfnum )
		{
		case (byte)GenerateLevel.SurfaceFragmentTypes.FOUNDATIONTILE:
			t.Foundation = true;
			return null;
		case (byte)GenerateLevel.SurfaceFragmentTypes.TREE:
			sf = Instantiate( Resources.Load("Prefabs/SurfaceFragments/Tree", typeof(SurfaceFragment)) ) as SurfaceFragment;
			(sf as TreeFragment).Growth = param;
			break;
		case (byte)GenerateLevel.SurfaceFragmentTypes.SURFFRAG:
			sf = Instantiate( Resources.Load("Prefabs/SurfaceFragments/UnearthedFragment", typeof(SurfaceFragment)) ) as SurfaceFragment;
			(sf as UnearthedFragment).currFrag = FragmentManager.CreateFragment( (FragType) param );
			break;
		}
		
		if( sf ) sf.SetTile( t );
	
		return sf;
	}
	#endregion
	
	#region Generate State
		// Generates a Version 0.1 gamestate
	public static byte[] GenerateState(Tile[,] low, Tile[,] mid, Tile[,] high)
	{
		int w = low.GetLength(0);
		int h = low.GetLength(1);
		int players = PlayerManager.PM.PlayerCount;
		
		string fragstr = "";
		string woodstr = "";
		string stonestr = "";
		string gemstr = "";
		
		int currTurn = PlayerManager.PM.CurrTurn;
		
		string datatype = "binary_compressed";
		
		//create the build fragment strings
		for( int i = 0 ; i < PlayerManager.PM.PlayerCount ; i ++)
		{
			woodstr += " " + FragmentManager.FM.CountWood( i );
			stonestr += " " + FragmentManager.FM.CountStone( i );
			gemstr += " " + FragmentManager.FM.CountGem( i );
		}
		
		fragstr = GenerateGameState.GetFragList();
		
		//Generate the header
		string header =
		"VERSION 0.1\n" +
		"WIDTH " + w + "\n" +
		"HEIGHT " + h + "\n" +
		"PLAYERS " + players + "\n" +
		
		"FRAGS " + fragstr + "\n" +
		"WOOD" + woodstr + "\n" +
		"STONE" + stonestr + "\n" +
		"GEMS" + gemstr + "\n" +
		
		"NETWORKGAMEID " + "" + "\n" +
		"CURRENTTURN " + currTurn + "\n" +
		"DATA "+ datatype + "\n";
		
		//create the byte array
		byte[] b = new byte[ w * h * 6 * 3 ];
		for( int y = 0 ; y < h * 3 ; y ++ )
		{
			for( int x = 0 ; x < w ; x ++)
			{
				
				int index = ( y * w + x ) * 6;
				int layer = y/h;
				
				Tile[,] t = low;
				if( layer == 1) t = mid;
				else if( layer == 2) t = high;
				
				int newY = y%h;
				int newX = x;
				
				b[index + 0] = 0;
				b[index + 1] = 0;
				b[index + 2] = 0;
				b[index + 3] = 0;
				b[index + 4] = 0;
				b[index + 5] = 0;
				
				if(t[newX, newY] != null)
				{
					//BYTE 1
					b[index + 0] = (byte)(((byte)(t[newX, newY].ORIG_HEIGHT + 10 + 1)) & HEIGHT_MASK); // original Height
					//BYTE 2
					b[index + 1] = (byte)((byte)(((byte)(t[newX, newY].getHeight() + 10 + 1)) & HEIGHT_MASK) | // current height and richness
						(byte)(((byte)(t[newX,newY].getRichness() + 1) << 5) & RICHNESS_MASK));
					//BYTE 3 & 4
					UnitToBytes( out b[index + 2], out b[index + 3], t[newX,newY].Resident );
					
					//BYTE 5
					SurfaceFragmentToBytes( out b[index + 4], t[newX, newY]);
					
					//BYTE 6
					b[index + 5] = (byte)t[newX, newY].PeekFragment();
				}
			}
		}
		
		//compress the byte data
		if( datatype == "binary_compressed" ) b = CompressionHelper0_1.Compress( b );
		
		//join the byte arrays together
		byte[] final = new byte[ b.Length + header.Length ];
		System.Buffer.BlockCopy( b, 0, final, header.Length, b.Length );
		
		b = System.Text.Encoding.ASCII.GetBytes(header);
		System.Buffer.BlockCopy( b, 0, final, 0, b.Length);
		
		
		//print( Application.dataPath );
		//File.WriteAllBytes( Application.dataPath + "/newtestmapfromunity.txt" , final );
		
		
		return final;
	}
	
	static void UnitToBytes( out byte unit, out byte soul, Unit u )
	{
		soul = 0;
		if ( u == null )
		{
			unit = 0;
			return;
		}
		
		byte player = (byte)(((u.Player) << 5 ) & PLAYER_MASK);
		byte unitnum = 0;
		
		if( u.GetComponent<SoulHost>() )
		{
			unitnum = (byte) GenerateLevel.UnitTypes.SOUL;
			soul = (byte)((((byte) Mathf.Min(u.GetComponent<Soul>().Souls,127)) & SOULCOUNT_MASK) | ((u.GetComponent<Unit>().Moved)? SOULMOVED_MASK : (byte)0));

				//(byte)((((byte) Mathf.Min(u.GetComponent<Soul>().Souls,127)) & SOULCOUNT_MASK) | ((u.GetComponent<Unit>().Moved)? SOULMOVED_MASK : 0));
		}
		else if( u.GetComponent<SoulVessel>() )
		{
			unitnum = (byte) GenerateLevel.UnitTypes.OBELISK;
		}
		else if( u.GetComponent<Shrine>() )
		{
			unitnum = (byte) GenerateLevel.UnitTypes.SHRINE;
		}
		else if( u.GetComponent<Temple>() )
		{
			unitnum = (byte) GenerateLevel.UnitTypes.TEMPLE;
		}
		else if( u.GetComponent<SoulWellTurnChange>() )
		{
			unitnum = (byte) GenerateLevel.UnitTypes.SPAWNER;
		}
		
		unit = (byte)((byte)player | (byte)(unitnum));
	}
	
	static void SurfaceFragmentToBytes( out byte surf , Tile t)
	{
		if( !t || (!t.SurfaceFrag && !t.Foundation) )
		{
			surf = 0;
			return;
		}
		
		byte param = 0;
		byte frag = 0;
		
		if(t.Foundation)
		{
			frag = (byte)GenerateLevel.SurfaceFragmentTypes.FOUNDATIONTILE;
		}
		else if( t.SurfaceFrag.GetComponent<TreeFragment>() )
		{
			frag = (byte) GenerateLevel.SurfaceFragmentTypes.TREE;
			param = (byte) t.SurfaceFrag.GetComponent<TreeFragment>().Growth;
		}
		else if( t.SurfaceFrag.GetComponent<SurfaceFragment>() )
		{
			frag = (byte)GenerateLevel.SurfaceFragmentTypes.SURFFRAG;
			param = (byte)FragmentManager.FragToInt(t.SurfaceFrag.GetComponent<UnearthedFragment>().currFrag.GetType().ToString());
		}
		
		frag = (byte)(frag & SURFFRAG_MASK);
		param = (byte)(param << 3);
		param = (byte)(param & FRAGMENT_MASK);
		
		surf = (byte)(frag | param);
		
	}
	#endregion
}
