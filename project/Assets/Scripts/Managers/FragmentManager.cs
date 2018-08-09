using UnityEngine;
using System.Collections;


public enum FragType{
	NONE = 0,
	//Fragment used to build stuff
	MAKE_BUILDING,
	
	//Build Fragments
	WOOD,
	SOUL_GEM,
	STONE,
	
	//Active Fragments
	KINGS_RING,
	NORMALIZE_PLANE,
	TOWN_STAFF,
	WOOD_PIPE,
	BURNING_EMBER,
	GEM_OF_SIGHT,
	FERTILIZE,
	SOUL_RELIC,
	EARTHQUAKE,
	DRAG_TRENCH,
	DRAG_WALL,
	SWAP_SPOTS
}; 

// DONE //
//TODO LIST:
// X store the next up fragment in the tiles and dig it up
// X store the "richness factor" in a tile
// X Dig up/mine and display amount of build Fragments
// X allow building of buildings with build frags
// X allow ascention of souls via buildings
// > implement the tile invalidation setup so it is not run every frame
// > implement the camera panning
// X implement a foundation tile type that can be used to build on
// X implement more fragments
// X implement BASIC sound
// X implement ability to only get specific items for one level
// X implement ability to dig under church / shrine / soul to deplete/ hurt them
// X implement min and max height of the tile

// DONE //
// X Implement sound for raising tiles
// X Implement better sound for digging tiles
// X implement other sounds as well
// X Implement method by which unearthed fragment can choose a new tile (fixed random tiles?)
// X Implement function that would increase probability of tile normalizing on turn change
// X Implement the probability that richness will increase with normalization / raising
// - Implement top layer
	// X Final soul vessel that must be merged into.
// X Design the level more completely
// X Implement the flame animations and tree burning functionality further

// > Tutorial
// > Title Screen
// X Display Both Players items at all times
// X When hovering over an item/selecting it, display a description and name
// X Have an indicater dictating whether or not a character has already moved/dug or not
// - MORE Powers:
// 		X Power to randomize the terrain
//		X Drag to make Wall Power
//		X Drag to make Trench Power
//		X Swap places power
//		- Teleport power
//		- Convert unit power
//		- Wind power that pushes units back based on distance from tile power was used on
//		-? Create a fire storm power
//		-? Delayed Powers? ie place a bomb and it goes off 3 turns later etc
//		-? Create Trees power?
//		-? Randomly kill a unit? (including your own)
//		-? Full level Earthquake
// X indicater indicating whether or not there are powers left to be used for the turn or if the QUEUE is full
//		- Graphically Enhance the indicater
// X indicater showing how many units are on each plane and how many need to be moved yet
//		X Stacked level views with count of units that have not been moved yet
// X include transitions in between player's turns
// X require less to ascend
// X require less to win
// X improve interface graphically
// -? max height delta is less intense on the lower levels?
// -? add new/different mechanics per level?
// X indicate where the unit will be "beamed" to
// -? different mechanics per layer - rivers / cloud spots???
// -? ability to build other things with your build frags
// -? tile height limit too high?


// X Fix line items not being able to select valid tiles
// -? "Haven't moved all people yet" warning
// -? Better visual indicater for when trees are fully grown
// X Temple not appealing (5, 3, 2)
// X Rebuild on top of old shrines/temples
// X make fertilizer less frequent / lesser radius
// -? Trees take too long to grow?
// X Fix inability to collect surface fragment
// X Soul relic stuns enemy unless it has soul > 1
// X Too much ember
// X MORE Earthquake
// X More swapping?
// X On second realm
//		X digging powers (no dig powers on first realm?)
//		X soul relic only on second realm



// CURRENT ORDERS OF BUSINESS //

// Festival Feedback:
// X Drag powers only work on click
// X rolling pin does not flatten COMPLETELY - allow it to leave some bumbs - too powerful
// -? highlighting color while using fragment should be different when using fragments vs units
// T people dont know its two player
// -? people dont know how to build shit - more blinking when something can be built?
// -? maybe too overpowerd that merging can string out over long distances
// X Gem of sight should stay around for longer
// X item models do not read very well when being collected - "Gray Things"
// X middle mouse scroll to change levels
// X make sure that the item descriptions are clear enough

// Next Up:
// - make the narrative clear to players
// > optimizing tile validation code (only update when something has changed etc)
// > optimize code for tile coloring (only update when something has changed etc)
// D design/model trees
// D design/model shrine/temple
// - design/model/animate units
// - design/model soul well
// - design/model soul vessel
// > Level loading from image map
// > Consider Camera Panning
// > Consider Perspective Camera
// - clear the game of the unity GUI visual
// X get rid of the timer for turns / make it an option
// - allow for the ability to get rid of the post effects and other graphics depending on the quality level
// X Test Hoe/Drag objects especially when clicking on another unit to start the object
// X Redo a lot of the tutorial 
//		- More Steps - less wordy
//		- A LOT of hand holding - dont let them do anything I don't want them to. More arrows
// X different indicators for the amount 
// - show whose turn it is much better and clearly
// *** - add much better indication for which units have moved/dug
// - Add particles and noise for the merging animation
// - change movement animation

// Powers:
// - Digging Ring
// - Radiactive soil - depletes soil richness
// - Fertilizer grows trees
// - stun enemies in a radius power
// - Wind Poer that pushes units back based on distance from tile power was used on

// Optimizations:
// - material manipulation is a HUGE bottleneck - units/tiles
// - Anything that is loaded from resource should be saved in a static variable if possible
// - Item camera draw -> make space smaller
// - Consider using a stack for the tile validation (fastest method for validating)
// - allow for switching for display mode

public class FragmentManager : MonoBehaviour {
	private Fragment Selected = null;
	private int SelectedNum = -1;
	
	public GeneralManager GM = null;
	
	public const int MAX_FRAGS=5;
	public Fragment[,] PlayerFrags;
	
	public int[] StoneFrags;
	public int[] GemFrags;
	public int[] WoodFrags;
	public static string[] TypeToFrag;
	
	public int[,] LowerFrags;
	public int[,] MiddleFrags;
	public int[,] UpperFrags;
	
	public static FragmentManager FM;
	
	public bool drawGUI = true;
	
	void Start(){
		
		GM = GeneralManager.GenMan; 
		PlayerManager p = GameObject.FindObjectOfType(typeof(PlayerManager)) as PlayerManager;
		
		PlayerFrags = new Fragment[p.PlayerCount , MAX_FRAGS];
		
		StoneFrags = new int[p.PlayerCount];
		GemFrags = new int[p.PlayerCount];
		WoodFrags = new int[p.PlayerCount];
		for(int i=0;i<p.PlayerCount;i++){
			StoneFrags[i]=0;
			GemFrags[i]=0;
			WoodFrags[i]=0;
		}
		
		//* Lower Plane Fragments + Probabilities *//
		LowerFrags = new int[,] {
			{ (int) FragType.KINGS_RING ,	 	8 },
			{ (int) FragType.SOUL_GEM ,		 	21 },
			{ (int) FragType.NORMALIZE_PLANE , 	5 },
			{ (int) FragType.TOWN_STAFF ,		7 },
			{ (int) FragType.WOOD_PIPE ,		6 },
			{ (int) FragType.BURNING_EMBER , 	1 },
			{ (int) FragType.GEM_OF_SIGHT ,		5 },
			{ (int) FragType.FERTILIZE ,		1 },
			{ (int) FragType.SOUL_RELIC,		2 },
			{ (int) FragType.DRAG_TRENCH,		2},
			{ (int) FragType.DRAG_WALL,			3},
			//{ (int) FragType.SWAP_SPOTS,		2},
			//{ (int) FragType.EARTHQUAKE,		1000}, 
		};
		
		//* Middle Plane Fragments + Probabilities *//
		MiddleFrags = new int[,]{
			{ (int) FragType.KINGS_RING,		5},
			{ (int) FragType.TOWN_STAFF,		3},
			{ (int) FragType.WOOD_PIPE ,		9 },
			{ (int) FragType.STONE,				22},
			{ (int) FragType.SOUL_RELIC,		1 },
			{ (int) FragType.NORMALIZE_PLANE , 	4 },
			{ (int) FragType.GEM_OF_SIGHT ,		8 },
			{ (int) FragType.FERTILIZE ,		3 },
			{ (int) FragType.EARTHQUAKE,		9},
			{ (int) FragType.DRAG_TRENCH,		7},
			{ (int) FragType.DRAG_WALL,			3},
			{ (int) FragType.BURNING_EMBER , 	0 },
			{ (int) FragType.SWAP_SPOTS,		6},

		};
		
		//* High Plane Fragments + Probabilities *//
		UpperFrags = new int[,]{
			{ (int) FragType.KINGS_RING,		10},
			{ (int) FragType.TOWN_STAFF,		10},
			{ (int) FragType.NORMALIZE_PLANE , 	5 },
			{ (int) FragType.WOOD_PIPE ,		8 },
			{ (int) FragType.EARTHQUAKE ,		10 },
			{ (int) FragType.NORMALIZE_PLANE , 	4 },
			{ (int) FragType.DRAG_TRENCH,		7},
			{ (int) FragType.DRAG_WALL,			7},
		};
		
		// Array that converts the fragType enums to the strings
		TypeToFrag = new string[ 17 ]; //TODO: make this the minimal size
		
		TypeToFrag[ (int) FragType.DRAG_TRENCH ] 		= "DragTrench";
		TypeToFrag[ (int) FragType.NONE] 				= "";
		TypeToFrag[ (int) FragType.DRAG_WALL ] 			= "DragWall";
		TypeToFrag[ (int) FragType.EARTHQUAKE ] 		= "Earthquake";
		TypeToFrag[ (int) FragType.FERTILIZE ] 			= "Fertilize";
		TypeToFrag[ (int) FragType.GEM_OF_SIGHT ] 		= "GemOfSight";
		TypeToFrag[ (int) FragType.KINGS_RING ] 		= "KingsRing";
		TypeToFrag[ (int) FragType.MAKE_BUILDING ] 		= "MakeBuilding";
		TypeToFrag[ (int) FragType.NORMALIZE_PLANE ] 	= "NormalizePlane";
		TypeToFrag[ (int) FragType.SOUL_GEM ] 			= "SoulGemFragment";
		TypeToFrag[ (int) FragType.SOUL_RELIC ] 		= "SoulRelic";
		TypeToFrag[ (int) FragType.STONE ] 				= "StoneFragment";
		TypeToFrag[ (int) FragType.BURNING_EMBER ]	 	= "BurningEmber";
		TypeToFrag[ (int) FragType.SWAP_SPOTS ] 		= "SwapSpots";
		TypeToFrag[ (int) FragType.TOWN_STAFF ] 		= "TownStaff";
		TypeToFrag[ (int) FragType.WOOD ] 				= "WoodFragment";
		TypeToFrag[ (int) FragType.WOOD_PIPE ] 			= "WoodenPipe";
		
		FM=this;
		
	}
	
	public void Update(){
		if(Input.GetKeyDown(KeyCode.Space)){
			Fragment f = CreateFragment( GetRandomFrag());
			f.Fresh=false;
			Collect(f);
		}
	}
	
	public int getFragCount(int player){
		int c = 0;
		
		for(int i=0; i<MAX_FRAGS; i++){
			if(PlayerFrags[player,i] != null){
				c ++;
			}
		}
		return c;
	}
	
	//Build Fragment Info / manipulation
	//WOOD
	public void UseWood(int n){
		WoodFrags[GM.PlayerMan.CurrTurn]-=n;
	}
	public int CountWood(int p){
		if(p<0 || p>GM.PlayerMan.PlayerCount){
			throw new UnityException();
		}
		return WoodFrags[p];
	}
	public bool SetWood(int n, int p)
	{
		if( p < 0 || p >= WoodFrags.Length )return false;
		WoodFrags[p] = n;
		return true;
	}
	
	//STONE
	public void UseStone(int n){
		StoneFrags[GM.PlayerMan.CurrTurn]-=n;
	}
	public int CountStone(int p){
		if(p<0 || p>GM.PlayerMan.PlayerCount){
			throw new UnityException();
		}
		return StoneFrags[p];
	}
	public bool SetStone(int n, int p)
	{
		if( p < 0 || p >= StoneFrags.Length )return false;
		StoneFrags[p] = n;
		return true;
	}
	//GEMS
	public void UseGems(int n){
		GemFrags[GM.PlayerMan.CurrTurn]-=n;
	}
	public int CountGem(int p){
		if(p<0 || p>GM.PlayerMan.PlayerCount){
			throw new UnityException();
		}
		return GemFrags[p];
	}
	public bool SetGems(int n, int p)
	{
		if( p < 0 || p >= GemFrags.Length )return false;
		GemFrags[p] = n;
		return true;
	}
	
	public void ClearFragments(){
		for(int i=0;i<GM.PlayerMan.PlayerCount;i++){
			ClearFragments(i);
		}
	}
	public void ClearFragments(int p){
		for(int i=0;i<MAX_FRAGS;i++){
			if(PlayerFrags[p,i]){
				Destroy(PlayerFrags[p,i]);
				PlayerFrags[p,i]=null;
			}
		}
	}
	
	// "Collect" the fragment passed
	public void Collect(Fragment f)
	{
		Collect(f, GM.PlayerMan.CurrTurn);
	}
	public void Collect(Fragment f, int player){
		int p = player;
		
		if(f==null)return;
		
		
		//if collecting a build fragment, collect it and move on
		if(f.GetType() == typeof(SoulGemFragment)){
			Destroy(f.gameObject);
			GemFrags[GM.PlayerMan.CurrTurn]++;
			return;
		}else if(f.GetType() == typeof(StoneFragment)){
			Destroy(f.gameObject);
			StoneFrags[GM.PlayerMan.CurrTurn]++;
			return;
		}else if(f.GetType() == typeof(WoodFragment)){
			Destroy(f.gameObject);
			WoodFrags[GM.PlayerMan.CurrTurn]++;
			return;
		}
		
				
		if(PlayerFrags[p , 0] != null){
			//shift all the fragments to the right if adding a new one and there's no space, deleting the final fragment
			Destroy(PlayerFrags[p , MAX_FRAGS-1]);
			for(int i = MAX_FRAGS-1; i > 0; i--){
				PlayerFrags[p , i] = PlayerFrags[p , i-1];
			}
		}
		
		//find the first place that is null to use that as the place to store the fragment
		for(int i=1 ; i<MAX_FRAGS ; i++){
			if(PlayerFrags[p , i] != null){
				PlayerFrags[p , i-1] = f;
				break;
			}
		}
		//if the last place is still null (first fragment) remove
		if(PlayerFrags[p, MAX_FRAGS-1]==null){
			PlayerFrags[p, MAX_FRAGS-1]=f;
		}
		
	}
	
	//dictates that the currently selected fragment should be "used" (Destroyed and removed from the QUEUE)
	public void UseFragment(Fragment f){
		int player=-1;
		for(int i=0;i<GM.PlayerMan.PlayerCount;i++){
			for(int j=0;j<MAX_FRAGS;j++){
				if(PlayerFrags[i,j] == f){
					player = i;
				}
			}
		}
		if(player==-1)player = GM.PlayerMan.CurrTurn;
		
		
		if(f!=Selected){
			if(f){
				for(int i=MAX_FRAGS-1; i >= 0 ; i--){

					if(f == PlayerFrags[player, i]){
						PlayerFrags[player,i]=null;
					}
					
					if(PlayerFrags[player, i]==null && i>0){
						PlayerFrags[player, i] = PlayerFrags[player, i-1];
						PlayerFrags[player, i-1] = null;
					}
				}
			}
			Destroy(f);
			return;
		}
		
		if( Selected == null || SelectedNum == -1){
			if(Selected && SelectedNum==-1 || !Selected && SelectedNum!=-1){
				//throw new UnityException("Selection of fragment issue");
			}
			if(Selected){
				Destroy(Selected.gameObject);
			}
			Deselect();
			return;
		}
		
		int p = GM.PlayerMan.CurrTurn;
		
		Destroy(Selected);
		PlayerFrags[p , SelectedNum] = null;
		
		for(int i=SelectedNum ; i > 0 ; i--){
			PlayerFrags[p, i] = PlayerFrags[p, i-1];
		}
		PlayerFrags[p,0] = null;
		Deselect();
	}
	
	// Exposed fucntions for selecting a current fragment to use etc
	public void Select(int num){
		if(SelectedNum==num){
			Deselect();
			return;
		}
		
		if(num>MAX_FRAGS || num<0){
			throw new UnityException("Trying to select int number" + num);
		}
		
		Select(PlayerFrags[GM.PlayerMan.CurrTurn , num]);
		SelectedNum = num;
	}
	public void Select(Fragment f){
		Deselect();
		GM.UnitMan.Deselect();
		
		Selected = f;
		
		//* set the selected fragment to be the info fragment *//
		setInfoFragment(f);
	}
	//Deselect the Fragments
	public void Deselect(){
		
		Fragment tempSelect = Selected;
		
		Selected = null;
		
		if(tempSelect)tempSelect.Clear();
		
		if(tempSelect && SelectedNum == -1){
			Destroy(tempSelect.gameObject);
		}
		
		SelectedNum = -1;

		infoDisplayFrag=null;
		
		//* clear any fragment from being info displayed *//
		clearInfoFragment();
	}
	//Return the selected Fragments
	public Fragment getSelected(){	
		return Selected;
	}
	//Retrun the Fragment asked for
	public Fragment GetFragment(int i)
	{
		return GetFragment(i, GM.PlayerMan.CurrTurn);	
	}
	public Fragment GetFragment(int i, int player)
	{		
		return PlayerFrags[player , MAX_FRAGS - i - 1];
	}
	
	//convert the fragtypes to component strings and vice versa
	public static int FragToInt( string s )
	{
		if( TypeToFrag == null ) return -1;
		for( int i = 0 ; i < TypeToFrag.Length ; i ++)
		{
			if( TypeToFrag[ i ] == s ) return i;
		}
		return -1;
	}
	public static string IntToFrag( FragType ft )
	{
		if( TypeToFrag == null ) return "";
		if( (int) ft > TypeToFrag.Length ) return "";
		
		return TypeToFrag[ (int) ft ];
	}
	
	//Creates an instance for the type of fragment passed
	public static Fragment CreateFragment(FragType ft){
		
		return CreateFragment( IntToFrag(ft) );	
	}
	//Create a fragment based off the string
	public static Fragment CreateFragment(string FragName){
		if(FragName=="" || FragName==null)return null;
				
		Fragment f = (Fragment)(new GameObject()).AddComponent(FragName);
		return f;
	}
		
	public Building Building_Shrine;
	public Building Building_Church;
	
	int maxFragGlowTime = 15;
	int fragGlowTime = 0;
	
	
	//Functions for displaying the selected items information //
	Fragment infoDisplayFrag = null;
	void setInfoFragment(Fragment f){
		if(infoDisplayFrag == f){	
			clearInfoFragment();

			return;
		}
				
		infoDisplayFrag = f;
		
		InfoFragDisplay fd = (InfoFragDisplay)(Component.FindObjectOfType(typeof(InfoFragDisplay)));
		MeshFilter mf = (MeshFilter)fd.GetComponent("MeshFilter");
		mf.mesh = f.getModel();
	}
	void clearInfoFragment(){
		infoDisplayFrag = null;

		InfoFragDisplay fd = (InfoFragDisplay)(Component.FindObjectOfType(typeof(InfoFragDisplay)));
		MeshFilter mf = (MeshFilter)fd.GetComponent("MeshFilter");
		mf.mesh = null;
	}
	
	//* GUI Display *//
	void OnGUI() {
		if( !drawGUI ) return;
		
		GUI.skin = GeneralManager.getStyle();
		
		//Don't Draw anything if it's paused
		if(GM.Paused)return;
		
		
		//* Display Alert for items *//
		if(getFragCount(GM.PlayerMan.CurrTurn) >= MAX_FRAGS){
			fragGlowTime++;
			if(fragGlowTime>maxFragGlowTime*2)fragGlowTime=0;
		}else{
			fragGlowTime=0;
		}
		
		GUI.color = new Color(1,0,.15f,(1 - (float)fragGlowTime/maxFragGlowTime)*.45f);
		
		const int sizeDelta=550;
		float f = (float)fragGlowTime / maxFragGlowTime;
		//f = Mathf.Sqrt(f);
		
		GUI.DrawTexture(new Rect(Screen.width-(sizeDelta/2)*f - sizeDelta/4,(-sizeDelta/4.0f)*f,sizeDelta * f,(sizeDelta/(2.0f))*f), (Texture2D)Resources.Load("GUITextures/glowDot",typeof(Texture2D)),ScaleMode.StretchToFill,true);
		//GUI.DrawTexture(new Rect(-sizeDelta/2,0,sizeDelta * f,(sizeDelta/2)*f), (Texture2D)Resources.Load("guiTex/glowDot",typeof(Texture2D)),ScaleMode.ScaleToFit,true);
		GUI.color = new Color(1,1,1);
		
		
		GUIStyle s2= GUI.skin.GetStyle("Label");
		s2.alignment=TextAnchor.UpperLeft;
		
		GUIStyle butStyle = GUI.skin.GetStyle("Button");
		butStyle.padding = new RectOffset(2,2,2,2);
		
		//* Draw Player Boxes *//
		for(int i=0;i < GM.PlayerMan.PlayerCount; i ++){
			//* Define box postions and heights *//
			int w = 250;
			int h = 75;
			
			int x = Screen.width - w-5;
			int y = 5 + i*h +10*i;
			int fragDim=50;
			
			Color c = GUI.color;
			//* Make the player's box more opaque if it is that user's ture *//
			if(i==GM.PlayerMan.CurrTurn){
				c.a = 1.0f;
			}else{
				c.a = .4f;
			}
			GUI.color=c;
			
			//* Draw the background for the box*//
			GUI.DrawTexture(
				new Rect(x-5,y,w+10,h+5),
				(Texture2D)Resources.Load("GUITextures/PlayerInfoBox"));
			
			
			//* Draw the player label *//
			GUI.Label(
				new Rect(x,y+2, w/2,20),
				"Player " + (i+1));
		
			//* Draw the icon labels *//
			GUI.DrawTexture(
				new Rect(x+w-w/2, y,w/2, 25),
				(Texture2D)Resources.Load("GUITextures/BuildFragIcons"));
			
			//* Draw the values for the frags *//
			GUI.Label(
				new Rect(x+w-w/2 + 25, y+2,w/2, 20),
				GemFrags[i]+"");
			GUI.Label(
				new Rect(x+w-w/2 + 66, y+2,w/2, 20),
				WoodFrags[i]+"");
			GUI.Label(
				new Rect(x+w-w/2 + 107, y+2,w/2, 20),
				StoneFrags[i]+"");			
			//get the gui color
			//* Draw the frag buttons/icons *//
			for(int j = 0; j < MAX_FRAGS; j ++){
				
				Fragment tempFrag = PlayerFrags[i, j];
				
				// make it transparent
				c.a = .5f;
				//if its selected make it solid
				if(getSelected() == tempFrag){
					c.a = 1.0f;
				}
				GUI.color=c;
				
				if(tempFrag){
					Texture2D icon = (Texture2D)Resources.Load("GUITextures/FragmentIcons/" + tempFrag.GetType(),typeof(Texture2D));
					
						
					if(tempFrag.Fresh && i == GM.PlayerMan.CurrTurn){
						c.a = .25f;
						GUI.color=c;
						
						if(icon){
							GUI.DrawTexture(
								new Rect(x + fragDim*j, y +25,fragDim,fragDim),
								icon,ScaleMode.ScaleToFit);
						}else{
							GUI.Label(
								new Rect(x + fragDim*j, y +25,fragDim,fragDim),
								tempFrag.GetType().ToString());
						}
					}else{

						if((new Rect(x + fragDim*j, Screen.height-(y +25) - fragDim,fragDim,fragDim)).Contains(Input.mousePosition)){
							if(c.a!=1.0f)c.a=.75f;
							if(Input.GetMouseButton(0))c.a=.6f;
							
						}else if(c.a!=1.0f){
							c.a=.6f;
						}					
						
						GUI.color=c;
						
						bool buttonDo=false;
						
						if(icon){
							buttonDo = GUI.Button(
								new Rect(x + fragDim*j, y +25,fragDim,fragDim),
								icon);
						}else{
							buttonDo = GUI.Button(
								new Rect(x + fragDim*j, y +25,fragDim,fragDim),
								tempFrag.GetType().ToString());
						}
						
						if(buttonDo){
							if(GM.PlayerMan.CurrTurn == i){
								Select(j);
							}
							else {
								if(Selected){
									Deselect();
								}
								setInfoFragment(PlayerFrags[i,j]);
							}
						}
					}
				}
			}
			//always make the color 1 again
			c.a = 1.0f;
			GUI.color=c;
		}
		
		//* Display name and instructions of current selected fragment *//
		if(infoDisplayFrag){
			GUI.Label(new Rect(Screen.width-250,220, 250, 25), infoDisplayFrag.getName(),s2);
			GUI.Label(new Rect(Screen.width-250,247, 250, 200), infoDisplayFrag.getDescription(),s2);
		}

		
		// TODO: Display the buttons as transparent if they cannot be built (or something like that)
		
		if(MakeBuilding.CanBuild(Building_Shrine) || true){
			//GUI.Label(new Rect(85,5,110,50),"SG: " + Building_Shrine.getGem() + "\nW: " + Building_Shrine.getWood() + "\nS: " + Building_Shrine.getStone());
			GUI.Label(new Rect(100,5,110,50),CountGem(GM.PlayerMan.CurrTurn) + "/" + Building_Shrine.getGem() + "\n" + CountWood(GM.PlayerMan.CurrTurn) + "/" + Building_Shrine.getWood() + "\n" + CountStone(GM.PlayerMan.CurrTurn) + "/" + Building_Shrine.getStone());
			GUI.DrawTexture(new Rect(80,10,20,40),(Texture2D)Resources.Load("GUITextures/BuildFragIconsButton"));
			

			if(GUI.Button(new Rect(5, 5, 75,50), "Build\nShrine") /*&& MakeBuilding.CanBuild(Building_Shrine)*/){
				BeginBuild(Building_Shrine);
			}
		}
		if(MakeBuilding.CanBuild(Building_Church) || true){
			//GUI.Label(new Rect(85,60,110,50),"SG: " + Building_Church.getGem() + "\nW: " + Building_Church.getWood() + "\nS: " + Building_Church.getStone());
			
			GUI.Label(new Rect(100,60,110,50),CountGem(GM.PlayerMan.CurrTurn) + "/" + Building_Church.getGem() + "\n" + CountWood(GM.PlayerMan.CurrTurn) + "/" + Building_Church.getWood() + "\n" + CountStone(GM.PlayerMan.CurrTurn) + "/" + Building_Church.getStone());
			GUI.DrawTexture(new Rect(80,65,20,40),(Texture2D)Resources.Load("GUITextures/BuildFragIconsButton"));
			
			if(GUI.Button(new Rect(5, 60, 75,50), "Build\nTemple") /*&& MakeBuilding.CanBuild(Building_Church)*/){
				//GM.FragMan.Collect (CreateFragment(FragType.KINGS_RING));
				BeginBuild(Building_Church);
			}
		}
		//GUI.Button(new Rect(65, 10, 100,100), "Shrine");
		
		//* Display current plane along with amount of souls per level *//
		SoulHost[] sh = (SoulHost[])Component.FindObjectsOfType(typeof(SoulHost));
		int[] soulCount= new int[3] {0,0,0};
		
		for(int i=0;i<sh.Length;i++){
			Unit uh = (Unit)sh[i].GetComponent("Unit");
			if(uh.Player == GM.PlayerMan.CurrTurn && !sh[i].Dug){
				if(uh.CurrentTile.transform.parent.tag == "HighPlane"){
					soulCount[2]++;
				}else if(uh.CurrentTile.transform.parent.tag == "MidPlane"){
					soulCount[1]++;
				}else if(uh.CurrentTile.transform.parent.tag == "LowPlane"){
					soulCount[0]++;
				}
			}
		}
		
		//* Display for which Plane is currently being viewed *//
		for(int i=0;i<3;i++){
			int delta = 0;
			if(GM.PlaneMan.getDestPlane() < i){
				delta=45;
			}
			if(GM.PlaneMan.getDestPlane()!= i){
				GUI.color=new Color(.375f,.35f,.4f);
			}
			GUI.DrawTexture(new Rect(10,Screen.height- 100 - i*30 - delta,125,125), (Texture2D)Resources.Load("GUITextures/GuiPlane",typeof(Texture2D)),ScaleMode.ScaleToFit,true);
			//GUI.Button(new Rect(10,300 - i*20 - delta,100,45), (i+1)+"");
			GUI.color=new Color(1.0f,1.0f,1.0f);
			
			GUI.Label(new Rect(140,Screen.height- 100 - i*30 - delta + 50,125,125), soulCount[i] + "");
			
		}
	}
	
	void BeginBuild(Building b){
		if(Selected && Selected.GetType() == typeof(MakeBuilding) && ((MakeBuilding)Selected).GetBuilding().GetType() == b.GetType()){
			Deselect();
		}else{
			MakeBuilding f = (MakeBuilding)CreateFragment("MakeBuilding");
			f.Build(b);
			//if(MakeBuilding.CanBuild(b)){
			Select(f);
			//}else{
			//	setInfoFragment(f);
			//	Destroy(f.gameObject);
			//}
		}
	}
	
	public FragType GetRandomFrag(){
		return GetRandomFrag(false);
	}
	public FragType GetRandomFrag(bool tangible){
		int beg = (int) FragType.MAKE_BUILDING + 1;	//start after MakeBuilding
		if(tangible)
		{
			beg = 5; //start after Make Building and the build fragments
		}
		
		//Needs to return a random type for a specific tier
		return (FragType)Random.Range(beg,FragType.GetValues(typeof(FragType)).Length);
	}
	
	
	public FragType GetRandomFrag(int[,] fragList){		
		
		int totalNum = 0;
		for(int i = 0; i < fragList.Length/2; i++){
			totalNum+=fragList[i,1];
		}
		
		int rnum = Random.Range(0,totalNum+1);
		
		totalNum=0;
		
		for(int i=0; i < fragList.Length/2 ; i++){
			totalNum += fragList[i,1];
			if(rnum <= totalNum) return (FragType)fragList[i,0];
		}
		
		
		return (FragType)(-1);
	}
	
	public void OnChangeTurn(){
		for(int i=0;i<PlayerFrags.Length/GM.PlayerMan.PlayerCount;i++){
			if(GetFragment(i))GetFragment(i).Fresh=false;
		}
	}
}
