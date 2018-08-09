using UnityEngine;
using System.Collections;

/*
 * How did I construct the narrative and mythology behind this. Inspiration/context. Where did it come from.
 * 
 * 15+ page documentation
 * 
 * Playtest on Tuesday
 * 
 */

//TUTORIAL
//Class to hand the tutorial elements of the game

public class Tutorial : MonoBehaviour {
	
	enum States {
		EXPLAIN,
		SELECT,
		MOVEMENT,
		ELEVATION,
		ELEVATION_LOW,
		DIGGING,
		TURN_CHANGE,
		SOUL_GENERATION,
		TILE_FLATTENING,
		USE_ITEMS,
		TILE_RICHNESS,
		COLLECT_GEMS,
		RESOURCE_LIST,
		BUILD,
		MERGING,
		MERGE_FOUR,
		MOVE_TO_GLOW,
		ASCEND,
		POST_ASCEND,
		EXPLAIN_STONE,
		COLLECT_TREE,
		EXPLAIN_TEMPLE,
		SCROLL,
		WIN,
		MAIN_MENU,
		ADVANCED_TACTIC,
		PRE_KILL_SOUL,
		KILL_SOUL,
		PRE_KILL_SHRINE,
		KILL_SHRINE,
		END_ADVANCED
		
	};
	
	States tState = States.EXPLAIN;
	
	GeneralManager GM;
	// Use this for initialization
	void Start () {
		GM = GeneralManager.GenMan;
		OnStateChange();
	}
	
	SoulHost firstUnit = null;
	// For Frag Count Check
	int fragStartAmount = 0;
	// For Merge Check
	int MergeCount = 0;
	// Tile used to demonstrate the elevation to tiles
	public Tile ElevationTile;
	// tile use to spawn unit on last plane
	public Tile GoalSpawnTile;
	
	//for GUI
	//current tutorial description
	string TutDesc = "";
	//title for the current state
	string Title="";
	//what state the next button should go to
	int nextButton = -1;
	
	public Tile[] FoundationTiles;
	public SoulVessel FinalVessel;
	
	
	
	
	//Check the tutorial state and act accordingly
	void Update () {
		GM.FragMan.Building_Shrine.Gem=1;
		
		SoulHost[] sh = (SoulHost[])Component.FindObjectsOfType(typeof(SoulHost));
		Unit[] units = (Unit[])Component.FindObjectsOfType(typeof(Unit));
		Soul[] souls = (Soul[])Component.FindObjectsOfType(typeof(Soul));
		
		if(tState == States.EXPLAIN){
			// start at showing all planes and explaining them
			GM.UnitMan.Deselect();
			GM.PlaneMan.setDestPlane(0);
			GM.PlaneMan.viewAllPlanes = true;
			
			if(!firstUnit){
				for(int i=0;i<sh.Length;i++){
					if(sh[i].getUnit().CurrentTile.transform.parent.tag == "LowPlane"){
						firstUnit = sh[i];
						break;
					}
				}
			}
		// SELECT TUTORIAL //
		} else if(tState == States.SELECT){
			//move on when a soul has been selected
			GM.PlaneMan.viewAllPlanes = false;
			GM.PlaneMan.setDestPlane(0);
			if(GM.UnitMan.getSelected() && GM.UnitMan.getSelected().Player==0){
				setState(States.MOVEMENT);
			}
		// MOVEMENT TUTORIAL //
		}else if(tState == States.MOVEMENT){
			//continue when a soul has been moved
			GM.PlaneMan.setDestPlane(0);
			if(!GM.UnitMan.getSelected() || GM.UnitMan.getSelected().Player!=0)setState(States.SELECT);
			
			if(GM.UnitMan.getSelected() && GM.UnitMan.getSelected().isMoving())setState (States.ELEVATION);
		//ELEVATION TUTORIAL //
		}else if(tState == States.ELEVATION){
			//shows how elevation effects movement
			//creates a unit to be placed at the tile
			if(!ElevationTile.Resident){
				GM.PlayerMan.CreateUnit(ElevationTile,GM.PlayerMan.instance_BasicSoul);
			}else{
				if(GM.UnitMan.getSelected()!=ElevationTile.Resident)GM.UnitMan.Select(ElevationTile.Resident);
				ElevationTile.setHeight(0);
				for(int i=0;i<ElevationTile._adjacentTiles.Length/2;i++){
					ElevationTile._adjacentTiles[i].setHeight(2);
					ElevationTile._adjacentTiles[i+3].setHeight(-2);
				}
				//ElevationTile._adjacentTiles[0].setHeight(0);
			}
			
		}else if(tState == States.ELEVATION_LOW){
			//shows how low tiles do not affect the movement of souls
			if(GM.UnitMan.getSelected()!=ElevationTile.Resident)GM.UnitMan.Select(ElevationTile.Resident);
			ElevationTile.setHeight(0);
			for(int i=0;i<ElevationTile._adjacentTiles.Length/2;i++){
				ElevationTile._adjacentTiles[i].setHeight(1);
				ElevationTile._adjacentTiles[i+3].setHeight(-1);
			}
			//ElevationTile._adjacentTiles[0].setHeight(0);
			
		// DIGGING TUTORIAL //
		}else if(tState == States.DIGGING){
			//continue once a tile has been dug into
			GM.PlaneMan.setDestPlane(0);
			
			if(GM.UnitMan.getSelected())firstUnit = GM.UnitMan.getSelected().GetComponent<SoulHost>();
				
			//if no item was found, create one
			if(firstUnit){
				Tile t = firstUnit.GetComponent<Unit>().CurrentTile;
				if(t.getHeight() != t.ORIG_HEIGHT){
					int fragCount = GM.FragMan.getFragCount(0) + GM.FragMan.CountGem(0);
	
					if(fragCount == 0){
						Fragment f = FragmentManager.CreateFragment(t.PeekFragment());
						GM.FragMan.Collect(f);
						t.CreateFragRemains(f);
					}
					
					setState(States.TURN_CHANGE);
				}
			}

		// TURN CHANGE TUTORIAL //
		}else if(tState == States.TURN_CHANGE){
			//if the turn has been changed, move forward
			GM.PlaneMan.setDestPlane(0);
			if(GM.PlayerMan.CurrTurn != 0){
				setState(States.SOUL_GENERATION);
			}
		// USE ITEMS TUTORIAL //
		}else if(tState == States.SOUL_GENERATION){
			GM.PlaneMan.setDestPlane(0);
		}else if(tState == States.TILE_FLATTENING){
			GM.PlaneMan.setDestPlane(0);
		}else if(tState == States.USE_ITEMS){
			GM.PlaneMan.setDestPlane(0);
			
			//if an itme has been used, move forward
			if(GM.FragMan.getFragCount(0) < fragStartAmount){
				setState(States.TILE_RICHNESS);
			}
			fragStartAmount = GM.FragMan.getFragCount(0);
		// COLLECT GEMS TUTORIAL //
		}else if(tState == States.TILE_RICHNESS){
			GM.PlaneMan.setDestPlane(0);
		}else if(tState == States.COLLECT_GEMS){
			GM.PlaneMan.setDestPlane(0);
			
			//if a soul can still be moved, do not reset their move and dig variables
			bool found = false;
			for(int i = 0; i <sh.Length;i++){
				if(sh[i].getUnit () && sh[i].getUnit().Player==0 && !sh[i].Dug){
					found=true;
					break;
				}
			}
			
			//otherwise, if one cant be found, allow all souls to move and dig again
			if(!found){
				for(int i = 0; i <sh.Length;i++){
					sh[i].getUnit().Moved=false;
					sh[i].Dug=false;
				}			
			}
			
			//once a gem has been found, move on
			if(GM.FragMan.CountGem(0) >= GM.FragMan.Building_Shrine.getGem()){
				setState(States.RESOURCE_LIST);
			}
			
		// BUILD SHRINE TUTORIAL //
		}else if(tState == States.BUILD){
			GM.PlaneMan.setDestPlane(0);
			
			//if a shrine has been built, move on
			if(Component.FindObjectsOfType(typeof(Shrine)).Length>0){
				MergeCount = souls.Length;
				setState(States.MERGING);
			}
		// MERGE TUTORIAL //
		}else if(tState == States.MERGING){
			//once a soul has been merged, move on
			GM.PlaneMan.setDestPlane(0);

			int newCount = souls.Length;
			
			if(newCount<MergeCount){
				setState(States.MERGE_FOUR);
			}
			MergeCount=newCount;
		// ASCEND TUTORIAL //
		}else if(tState == States.MERGE_FOUR){
			//once a unit has been merged with four souls, move on
			GM.PlaneMan.setDestPlane(0);
			for(int i=0;i<souls.Length;i++){
				if(souls[i].Souls>=4)setState(States.MOVE_TO_GLOW);
			}
		}else if(tState == States.MOVE_TO_GLOW){
			//once a unit with four souls has been move to the ascend tiles around the shrine, continue on
			GM.PlaneMan.setDestPlane(0);
			for(int i=0;i<sh.Length;i++){
				if(sh[i].getSoul() &&sh[i].getSoul().Souls>=4){
					if(sh[i].getUnit().CurrentTile.GetComponentInChildren(typeof(AscendLight)))setState(States.ASCEND);
				}
				sh[i].getUnit().Moved=false;
				sh[i].Dug=false;
			}
		}else if(tState == States.ASCEND){
			GM.PlaneMan.setDestPlane(0);
			
			//once a soul has been ascended, move on
			for(int i = 0 ; i<units.Length;i++){
				if(units[i].CurrentTile.transform.parent.tag == "MidPlane"){
					setState(States.POST_ASCEND);
					break;
				}
			}
			
		// GOAL TUTORIAL //
		}else if(tState == States.POST_ASCEND){
			if(GM.FragMan.getSelected() && GM.FragMan.getSelected().GetType() == typeof(MakeBuilding) && ((MakeBuilding)GM.FragMan.getSelected()).GetBuilding().GetType() == typeof(Shrine))GM.FragMan.Deselect();
			GM.PlaneMan.setDestPlane(1);
		}else if(tState == States.EXPLAIN_STONE){
			if(GM.FragMan.getSelected() && GM.FragMan.getSelected().GetType() == typeof(MakeBuilding) && ((MakeBuilding)GM.FragMan.getSelected()).GetBuilding().GetType() == typeof(Shrine))GM.FragMan.Deselect();
			GM.PlaneMan.setDestPlane(1);
			for(int i=0;i<sh.Length;i++){
				if(sh[i].Dug){
					sh[i].getUnit().Moved=false;
					sh[i].Dug=false;
				}
			}
			
			if(GM.FragMan.CountStone(0)>0)setState(States.COLLECT_TREE);
			
		}else if(tState == States.COLLECT_TREE){
			if(GM.FragMan.getSelected() && GM.FragMan.getSelected().GetType() == typeof(MakeBuilding) && ((MakeBuilding)GM.FragMan.getSelected()).GetBuilding().GetType() == typeof(Shrine))GM.FragMan.Deselect();
			GM.PlaneMan.setDestPlane(1);
			
			//once some wood has been found, move on
			if(GM.FragMan.CountWood(0)!=0)setState(States.EXPLAIN_TEMPLE);
			
			//make sure all souls can move and dig always
			for(int i=0;i<sh.Length;i++){
				sh[i].getUnit().Moved=false;
				sh[i].Dug=false;
			}
		}else if(tState == States.EXPLAIN_TEMPLE){
			//once a temple has been built, move on
			if(GM.FragMan.getSelected() && GM.FragMan.getSelected().GetType() == typeof(MakeBuilding) && ((MakeBuilding)GM.FragMan.getSelected()).GetBuilding().GetType() == typeof(Shrine))GM.FragMan.Deselect();
			
			if(Component.FindObjectOfType(typeof(Temple)))setState(States.SCROLL);
			
			GM.PlaneMan.setDestPlane(1);
		}else if(tState == States.SCROLL){
			if(GM.PlaneMan.getDestPlane()==2)setState(States.WIN);
		}else if(tState == States.WIN){
			if(GM.PlayerMan.CheckWin()==0)setState(States.MAIN_MENU);
		}
	}
	
	//Updates for the Advanced Tactics portion
	void LateUpdate(){
		//set all souls to not be able to move around the 
		if(tState == States.ADVANCED_TACTIC || 
			tState == States.PRE_KILL_SOUL ||
			tState == States.KILL_SOUL ||
			tState == States.PRE_KILL_SHRINE ||
			tState == States.KILL_SHRINE ||
			tState == States.END_ADVANCED
			){
			
			GM.PlaneMan.setDestPlane(0);
			SoulHost[] u = (SoulHost[])Component.FindObjectsOfType(typeof(SoulHost));
			for(int i=0; i < u.Length; i++){
				u[i].getUnit().CanMove=false;
			}
		}
		
		// if the proper tile has been dug into, move on
		if(tState == States.ADVANCED_TACTIC){
			if(ElevationTile.getHeight() < ElevationTile.ORIG_HEIGHT)setState(States.PRE_KILL_SOUL);
		
		//These states allways look at the bottom realm
		}else if(tState == States.PRE_KILL_SOUL){
			GM.PlaneMan.setDestPlane(0);
		}else if(tState == States.KILL_SOUL){
			GM.PlaneMan.setDestPlane(0);
			if(ElevationTile.getHeight() < ElevationTile.ORIG_HEIGHT)setState(States.PRE_KILL_SHRINE);
		}else if(tState == States.PRE_KILL_SHRINE){
			GM.PlaneMan.setDestPlane(0);
		}else if(tState == States.KILL_SHRINE){
			GM.PlaneMan.setDestPlane(0);
			if(ElevationTile.getHeight() < ElevationTile.ORIG_HEIGHT)setState(States.END_ADVANCED);
		}else if(tState == States.END_ADVANCED){
			GM.PlaneMan.setDestPlane(0);
		}	
	}
	
	//function used to set the state
	void setState(States ts){
		tState=ts;
		OnStateChange();
	}
	
	//called when the state has been changed. Changes the description and sets the state of the world/tiles based on teh state
	void OnStateChange(){
		Tile[] tiles = (Tile[])Component.FindObjectsOfType(typeof(Tile));
		SoulHost[] units = (SoulHost[])Component.FindObjectsOfType(typeof(SoulHost));
		for(int i=0;i<units.Length;i++){
			Unit u = (Unit)units[i].GetComponent("Unit");
			u.Moved=false;
			units[i].Dug=false;
		}
		
		if(tState == States.EXPLAIN){
			nextButton = (int)States.SELECT;
			Title = "Ascension";
			TutDesc = "Ascension takes place on a series of parallel planes. \n\n\tYou control a civilization of souls whose goal it is to ascend through these dimensions to become a god. Here is how to control your people.";
		}else if(tState == States.SELECT){
			nextButton = -1;
			Title = "Selecting Units";
			TutDesc = "Left click a Soul unit to Select it.";
			for(int i = 0; i < tiles.Length; i++){
				tiles[i].setFragment(FragType.TOWN_STAFF);
			}
		}else if(tState == States.MOVEMENT){
			Title = "Moving Units";
			TutDesc = "Left click one of the highlighted tiles around the Soul to MOVE the unit to that tile." +
				"\n\n\tPress A and D to rotate the world to get a better view.\n\n\tMOVE a soul to one of the available tiles around it to continue.";
			for(int i = 0; i < tiles.Length; i++){
				tiles[i].setFragment(FragType.TOWN_STAFF);
			}
		}else if(tState == States.ELEVATION){
			nextButton = (int)States.ELEVATION_LOW;
			Title = "Tile Elevation";
			TutDesc = "Souls can only move up and down one tile at a time. As you can see, the selected soul that is surrounded cannot move onto the tiles around that have a height difference of 2 or more.";
		}else if(tState == States.ELEVATION_LOW){
			nextButton = (int)States.DIGGING;
			Title = "Tile Elevation";
			TutDesc = "They CAN move over tiles with a height difference of one. \n\n\tThe stripes on the side of the tiles dictate the height relative to tiles around it.";
		}else if(tState == States.DIGGING){
			
			for(int i=0;i<tiles.Length;i++){
				tiles[i].setHeight(tiles[i].ORIG_HEIGHT);
			}
			GM.UnitMan.Deselect();
			Destroy(ElevationTile.Resident.gameObject);
			ElevationTile.Resident=null;
			
			nextButton = -1;
			Title = "Digging and Collecting Fragments";
			TutDesc = "After selecting a soul, RIGHT CLICK on the selected Soul to DIG into the tile below it. After a soul has dug, nothing more can be done with it.\n\n\tSelect a soul and dig into the tile below by right clicking it.";
		}else if(tState == States.TURN_CHANGE){
			Title = "Changing the Turn";
			TutDesc = "The circular arrow in the bottom right changes the game to the next Player's turn. It is switched a second time automatically here for the sake of the tutorial. \n\n\tClick the arrow in the bottom right to switch the turn.";
		}else if(tState == States.SOUL_GENERATION){
			nextButton = (int)States.USE_ITEMS;

			Title = "Soul Generation";
			TutDesc = "Two new souls are spawned from each player's SOUL WELL at the beginning of every turn as long as there is space around the SOUL WELL.";
		}else if(tState == States.USE_ITEMS){
			nextButton = -1;
			
			if(GM.FragMan.getFragCount(0)==0){
				Fragment f = FragmentManager.CreateFragment(FragType.TOWN_STAFF);
				f.Fresh = false;
				GM.FragMan.Collect(f);
			}else{
				GM.FragMan.GetFragment(4).Fresh=false;
			}
			
			Title = "Using Fragments";
			TutDesc = "The icons in the upper right represent the FRAGMENTS that your souls have collected. Fragments may only be used on the NEXT turn after they have been found. Click on the icon to begin using the FRAGMENT." +
				"\n\n\tThe instructions on how to use the FRAGMENT are displayed on the right.\n\n\tUse the FRAGMENT on the terrain to continue.";
			fragStartAmount = GM.FragMan.getFragCount(0);
		}else if(tState == States.TILE_RICHNESS){
			nextButton = (int)States.COLLECT_GEMS;
			Title = "Tile Richness";
			TutDesc ="The concentric circles on each of the tiles dictate the RICHNESS of a tile, or the chance that it will yield a new item.\n\n\tThe labeled tiles with three rings are the most rich.";
		}else if(tState == States.COLLECT_GEMS){
			nextButton = -1;
			Title = "Collecting Resources";
			TutDesc = "Soul Gems are a resource used to build churches and shrines. They are gathered the same way as other Fragments, by digging, but can only be found on the first plane."+
				"\n\n\tFind one Soul Gem to continue.";
			for(int i = 0; i < tiles.Length; i++){
				tiles[i].setFragment(FragType.SOUL_GEM);
			}
		}else if(tState == States.RESOURCE_LIST){
			nextButton = (int)States.BUILD;
			Title = "Resources";
			TutDesc = "The amount of collected SOUL GEMS is displayed on the right side of the screen above the Fragment icons, along with resource amounts for WOOD and STONE (which can be found on the second plane).";
		}else if(tState == States.BUILD){
			nextButton = -1;
			Title = "Building Shrines and Temples";
			TutDesc = "In order to progress to the next plane, a SHRINE must be built. The button to build a shrine is on the left side of the screen along with the amount of required resources to create it. Shrines can " +
				"only be built on gray FOUNDATION TILES." +
				"\n\n\tBuild a SHRINE to continue.";
		}else if(tState == States.MERGING){
			Title = "Merging Units";
			TutDesc = "To MERGE two souls together, select a SOUL, and RIGHT CLICK on the soul within range that you would like to merge it in to." +
				"\n\n\tTo see how many souls a unit has within it, hover over the unit.\n\n\tMerge two souls to continue.";
		}else if(tState == States.MERGE_FOUR){
			Title = "Preparing To Ascend";
			TutDesc = "Good. Now merge FOUR souls into a single entity.";
		}else if(tState == States.MOVE_TO_GLOW){
			Title = "Preparing To Ascend";
			TutDesc = "Move the unit with four souls to a glowing tile around your shrine.";
		}else if(tState == States.ASCEND){
			Title = "Ascending Units";
			TutDesc = "Click the arrow progress the turn and Ascend the soul to the next realm.";
		}else if(tState == States.POST_ASCEND){
			nextButton = (int)States.EXPLAIN_STONE; 
			Title = "Second Plane";
			TutDesc = "This is the second plane of reality. A few different items are available from this area. From here, your souls need to ascend to the next plane in order achieve their goal of becoming a god.";
			if(GM.FragMan.CountGem(0)==0)GM.FragMan.Collect(FragmentManager.CreateFragment(FragType.SOUL_GEM));
		}else if(tState == States.EXPLAIN_STONE){
			for(int i = 0; i < tiles.Length; i++){
				tiles[i].setFragment(FragType.STONE);
			}
			nextButton = -1;
			Title = "Collecting Stone";
			TutDesc = "Collecting STONE on this realm works the same as collecting SOUL GEMS on the first: by digging. Stone can be used in conjunction with gems and wood to create a TEMPLE.\n\n\tFind one piece of stone to continue.";
		}else if(tState == States.COLLECT_TREE){
			nextButton = -1;
			Title = "Collecting Wood";
			TutDesc = "Trees can be collected by digging on a tile with a grown, blossoming tree on it.\n\n\tCollect wood from a fully grown tree to continue.";
			
			//Code to replace all items in all tiles */
			/*for(int i = 0; i < tiles.Length; i++){
				int[,] f = null;
				switch(tiles[i].transform.parent.tag){
				case "MidPlane":
					f=GM.FragMan.MiddleFrags;
					break;
				case "LowPlane":
					f = GM.FragMan.LowerFrags;
					break;
				case "HighPlane":
					f = GM.FragMan.UpperFrags;
					break;
				}
				
			}*/
			
		}else if(tState == States.EXPLAIN_TEMPLE){
			nextButton = -1;
			if(GM.FragMan.CountGem(0)==0)GM.FragMan.Collect(FragmentManager.CreateFragment(FragType.SOUL_GEM));
			Title = "Temple";
			TutDesc = "Temples work nearly identically to Shrines, but require stone and wood to build. They only require 2 merged souls in order to ascend, meaning it takes much less time to get to the next plane if you have one." +
				"\n\n\tTemples can be built from the button on the left, same as a Shrine.";
		}else if(tState == States.SCROLL){
			nextButton = -1;

			Title = "Controlling Planes";
			TutDesc = "In order to win, a player must control all realms/planes as best they can to win. In order to switch which plane is being viewed, roll the scroll wheel UP or DOWN OR press S or D." +
				"\n\n\tScroll UP to the next plane to continue.";
		}else if(tState == States.WIN){
			Title = "Astral Plane";
			TutDesc = "This is the final, ASTRAL plane. In order to attain the status of a god, a single soul must be merged into the SOUL VESSEL." +
				"\n\n\tSelect the unit and MERGE into the Soul Vessel to win the game.";
		}else if(tState == States.MAIN_MENU){
			Title = "Astral Plane";
			TutDesc = "And that's how you play. Select 'START' from the main menu to begin a two player game.";
		}else if(tState == States.ADVANCED_TACTIC){
			Title = "Paired Digging";
			TutDesc = "SELECT a SOUL, then DIG using RIGHT CLICK between the two souls to Dig between the two souls.";
			SoulVessel[] sv = (SoulVessel[])Component.FindObjectsOfType(typeof(SoulVessel));
			for(int i=0;i<sv.Length;i++){
				Soul s = (Soul)sv[i].GetComponent("Soul");
				s.SetSouls(0);
			}
			
			SoulHost[] sh = (SoulHost[])Component.FindObjectsOfType(typeof(SoulHost));
			for(int i=0; i<sh.Length;i++){
				Destroy(sh[i].gameObject);
			}
			
			ShrineTurnChange[] stc = (ShrineTurnChange[])Component.FindObjectsOfType(typeof(ShrineTurnChange));
			for(int i=0;i<stc.Length;i++){
				stc[i].GetComponent<Shrine>().Exhausted();
			}
			
			for(int i=0;i<tiles.Length;i++){
				tiles[i].setHeight(tiles[i].ORIG_HEIGHT);
			}
			
			GM.PlaneMan.RemoveWin();
			
			GM.PlayerMan.CreateUnit(ElevationTile._adjacentTiles[0],GM.PlayerMan.instance_BasicSoul);
			GM.PlayerMan.CreateUnit(ElevationTile._adjacentTiles[3],GM.PlayerMan.instance_BasicSoul);
			ElevationTile.setHeight(ElevationTile.ORIG_HEIGHT);
		
			
		}else if(tState == States.PRE_KILL_SOUL){
			nextButton=(int)States.KILL_SOUL;
			Title = "Paired Digging";
			TutDesc = "No FRAGMENTS are awarded in this case, regardless of the tile's RICHNESS.";
		}else if(tState == States.KILL_SOUL){
			nextButton=-1;
			Title = "Killing and Damaging Souls";
			TutDesc = "The more souls that are around the tile, the deeper the hole that will be dug. If more than two souls are used to dig a hole, enemy souls can be damaged in the process, or destroyed if they only have 1 soul merged to them." +
				"\n\n\tDestroy the opposing soul to continue.";
			ElevationTile.setHeight(ElevationTile.ORIG_HEIGHT);
			GM.PlayerMan.CreateUnit(ElevationTile._adjacentTiles[1],GM.PlayerMan.instance_BasicSoul);
			GM.PlayerMan.CreateUnit(ElevationTile._adjacentTiles[4],GM.PlayerMan.instance_BasicSoul);
			GM.PlayerMan.CreateUnit(ElevationTile,GM.PlayerMan.instance_BasicSoul).Player=1;
			
		}else if(tState == States.PRE_KILL_SHRINE){
			nextButton=(int)States.KILL_SHRINE;
			Title = "Killing and Damaging Souls";
			TutDesc = "The amount of SOULS that are depleted from the enemy unit and depth of the hole created when this is done is directly related to the amount of your souls surrounding the tile.";

		}else if(tState == States.KILL_SHRINE){
			nextButton=-1;
			Title = "Destroying Shrines and Temples";
			TutDesc = "Finally, this can be used to destroy enemy SHRINES and TEMPLES. \n\n\tDig between the souls to destroy the SHRINE.";
			ElevationTile.setHeight(ElevationTile.ORIG_HEIGHT);
			GM.PlayerMan.CreateUnit(ElevationTile._adjacentTiles[2],GM.PlayerMan.instance_BasicSoul);
			GM.PlayerMan.CreateUnit(ElevationTile._adjacentTiles[5],GM.PlayerMan.instance_BasicSoul);
			GM.PlayerMan.CreateUnit(ElevationTile,(Unit)GM.FragMan.Building_Shrine.GetComponent("Unit")).Player=1;

		}else if(tState == States.END_ADVANCED){
			Title = "End";
			TutDesc = "That's the end of the advanced tactics! Press the button continue to the main menu.";
		}
	}
	
	Texture2D arrowTex;
	
	//draw the description and any needed buttons or arrows based on the required state
	void OnGUI(){
		//if the game is paused, do not draw the tutorial gui elements
		if(GM.Paused)return;
		
		//set the style for gui elements
		GUI.skin = GeneralManager.getStyle();
		if(!arrowTex)arrowTex = (Texture2D)Resources.Load("guiTex/arrow",typeof(Texture2D));
		
		GUI.depth=-1;
		
		GUIStyle gs = new GUIStyle();
		gs.alignment = TextAnchor.UpperLeft;
		gs.normal.textColor=new Color(1,1,1,1);
		gs.wordWrap=true;
		
		GUI.Box(new Rect(175,10,300,200),"");
		GUI.Label(new Rect(185,10,280,200),Title);
		GUI.Label(new Rect(185,10,280,200),"\n\n\t" + TutDesc,gs);
		
		
		if(tState == States.EXPLAIN){
			//create the next button to move onto the next step
			if(
				GUI.Button(new Rect(175,215,300,50),"Next")
			){
				setState(States.SELECT);
			}
		}else if(tState == States.SELECT){
			//point to the unit that should be selected
			if(firstUnit){
				SoulHost u = (SoulHost)firstUnit.GetComponent("SoulHost");
				Vector3 v = Camera.main.WorldToScreenPoint(u.transform.position);
			
				GUIUtility.RotateAroundPivot(180,new Vector2(v.x,Screen.height-v.y));
				GUI.DrawTexture(new Rect(v.x-95,Screen.height-v.y,100,100),arrowTex,ScaleMode.ScaleToFit);
			}
		}else if(tState == States.MOVEMENT){
		}else if(tState == States.ELEVATION || tState == States.ELEVATION_LOW){
			//point to the tile around with tiles have elevated
			drawArrow(ElevationTile.transform.position,new Vector2(100,100), 90);
		}else if(tState == States.DIGGING){
			//point to a unit in order to dig
			if(!GM.UnitMan.getSelected()&& firstUnit){
				SoulHost u = (SoulHost)firstUnit.GetComponent("SoulHost");
				Vector3 v = Camera.main.WorldToScreenPoint(u.transform.position);
			
				GUIUtility.RotateAroundPivot(180,new Vector2(v.x,Screen.height-v.y));
				GUI.DrawTexture(new Rect(v.x-95,Screen.height-v.y,100,100),arrowTex,ScaleMode.ScaleToFit);	
			}
		}else if(tState == States.TURN_CHANGE){
			//point to the turn change arrow in the bottom right
			Vector2 v = new Vector2(Screen.width-50, Screen.height-125);
			GUIUtility.RotateAroundPivot(90,new Vector2(v.x,v.y));
			GUI.DrawTexture(new Rect(v.x-95,v.y,150,150),arrowTex,ScaleMode.ScaleToFit);
			
		}else if(tState == States.SOUL_GENERATION){
			SoulWellTurnChange[] sw = (SoulWellTurnChange[])Component.FindObjectsOfType(typeof(SoulWellTurnChange));
			
			//find the users soul well where units are generated
			for(int i=0;i<sw.Length;i++){
				Unit u=sw[i].GetComponent<Unit>();
				if(u.Player==0){
					drawArrow(u.transform.position,new Vector2(100,100),90);
					Tile t = u.CurrentTile;
					int count =0;
					//point to two souls that were generated around the soul well
					for(int j=0; j<t._adjacentTiles.Length;j++){
						if(t._adjacentTiles[j] && t._adjacentTiles[j].Resident){
							drawArrow(t._adjacentTiles[j].Resident.transform.position + new Vector3(0,3,0),new Vector2(50,50),90);
							count++;
							if(count==2)break;
						}
					}
				}
			}
			
		}else if(tState == States.USE_ITEMS){
			//point the the items that the user has in the upper right
			Vector2 v = new Vector2(Screen.width-100, 45);
			GUIUtility.RotateAroundPivot(0,new Vector2(v.x,v.y));
			GUI.DrawTexture(new Rect(v.x-95,v.y,150,150),arrowTex,ScaleMode.ScaleToFit);
		}else if(tState == States.TILE_RICHNESS){
			//point to the most rich tiles in the world
			Tile[] tiles = (Tile[])Component.FindObjectsOfType(typeof(Tile));
			for(int i=0;i<tiles.Length;i++){
				if(tiles[i].getRichness() == Tile.MAX_RICHNESS){
					drawArrow(tiles[i].transform.position,new Vector2(50,50),90);
				}
			}
		}else if(tState == States.COLLECT_GEMS){
		}else if(tState == States.RESOURCE_LIST){
			//point to the list of resources in the upper right
			drawArrow(new Vector2(Screen.width-105,Screen.height - 15),new Vector2(100,100),0);
		}else if(tState == States.BUILD){
			//if the user needs to build, point to the build shrine button
			if(GM.FragMan.getSelected() && GM.FragMan.getSelected().GetType() != typeof(MakeBuilding) || !GM.FragMan.getSelected()){
				Vector2 v = new Vector2(35, 80);
				GUIUtility.RotateAroundPivot(-75,new Vector2(v.x,v.y));
				GUI.DrawTexture(new Rect(v.x-95,v.y,150,150),arrowTex,ScaleMode.ScaleToFit);
			//if the user has already selected to build, point to the foundation tiels where shrines can be built
			}else if(GM.FragMan.getSelected() && GM.FragMan.getSelected().GetType() == typeof(MakeBuilding)){
				for(int i=0;i<FoundationTiles.Length;i++){
					
					Tile t = FoundationTiles[i];
					Vector3 v = Camera.main.WorldToScreenPoint(t.transform.position);
					GUIUtility.RotateAroundPivot(75,new Vector2(v.x,Screen.height-v.y));
					GUI.DrawTexture(new Rect(v.x-95,Screen.height-v.y,100,100),arrowTex,ScaleMode.ScaleToFit);
					GUIUtility.RotateAroundPivot(-75,new Vector2(v.x,Screen.height-v.y));
				}
			}
		}else if(tState == States.MERGING || tState == States.MERGE_FOUR){
			
		}else if(tState == States.MOVE_TO_GLOW){
			Soul[] s = (Soul[])Component.FindObjectsOfType(typeof(Soul));
			
			for(int i=0; i<s.Length; i++){
				// if there is a unit with four souls
				if(s[i].Souls>=4){
					Shrine sh = (Shrine)Component.FindObjectOfType(typeof(Shrine));
					Unit u = (Unit)sh.GetComponent("Unit");
					Tile t2 = u.CurrentTile;
					//point to highlighted tiles around the shrines in the scene
					for(int j = 0; j<t2._adjacentTiles.Length; j++){
						Tile t = t2._adjacentTiles[j];
						Vector3 v = Camera.main.WorldToScreenPoint(t.transform.position);
						v.y+=30;
						v.x-=10;
						GUIUtility.RotateAroundPivot(75,new Vector2(v.x,Screen.height-v.y));
						GUI.DrawTexture(new Rect(v.x,Screen.height-v.y,30,30),arrowTex,ScaleMode.ScaleToFit);
						GUIUtility.RotateAroundPivot(-75,new Vector2(v.x,Screen.height-v.y));
					}
					return;
				}
			}
		}else if(tState == States.ASCEND){
			//draw an arrow pointing to the turn change button in the bottom right
			drawArrow(new Vector2(Screen.width-40,65),new Vector2(100,100),90);
		}else if(tState == States.COLLECT_TREE){
			
			//point to grown tress in the world to show what needs to be harvested
			TreeFragment[] t = (TreeFragment[]) Component.FindObjectsOfType(typeof(TreeFragment));
			
			for(int i=0;i<t.Length;i++){
				if(t[i].Growth>=TreeFragment.MAX_GROWTH){
					drawArrow(t[i].transform.position + new Vector3(0,10,0),new Vector2(25,25),90);
				}
			}
		}else if(tState == States.EXPLAIN_TEMPLE){
			
			// Point to the Build Temple button in the top left
			Vector2 v = new Vector2(35, 160);
			GUIUtility.RotateAroundPivot(-75,new Vector2(v.x,v.y));
			GUI.DrawTexture(new Rect(v.x-95,v.y,150,150),arrowTex,ScaleMode.ScaleToFit);
		}else if(tState == States.WIN){
			
			// point to the soul vessel of the first player, idicating that he needs to merge to it
			SoulVessel[] sv = (SoulVessel[])Component.FindObjectsOfType(typeof(SoulVessel));
			for(int i=0;i<sv.Length;i++){
				Unit u = (Unit)sv[i].GetComponent("Unit");
				if(u.Player==0){
					drawArrow(u.transform.position + new Vector3(0,12,0),new Vector2(100,100),180);
				}
			}
		}else if(tState == States.MAIN_MENU){
			
			//Draw buttons to go to advanced tutorial, or back to main menu
			if(
				GUI.Button(new Rect(175,215,300,50),"Main Menu")
			){
				Application.LoadLevel(0);
			}
			if(
				GUI.Button(new Rect(175,270,300,50),"Learn Advanced Tactics")
			){
				setState(States.ADVANCED_TACTIC);
			}
			
		}else if(tState == States.ADVANCED_TACTIC || 
			tState == States.KILL_SOUL ||
			tState == States.KILL_SHRINE
		){
			
			//point to the surrounding souls around the tile of interest
			Tile[] t = ElevationTile._adjacentTiles;
			bool selected = false;
			for(int i=0;i<t.Length;i++){
				if(t[i].Resident==GM.UnitMan.getSelected() && t[i].Resident!=null){
					selected=true;
					break;
				}
			}
			
			//if one of the units is already selected, do not point to it, and instead point to the center
			if(!selected){
				for(int i=0;i<t.Length;i++){
					if(t[i].Resident){
						drawArrow(t[i].transform.position + new Vector3(0,3,0),new Vector2(50,50),90);
					}
				}
			}else{
				drawArrow(ElevationTile.transform.position + new Vector3(0,1,0),new Vector2(100,100),90);
			}
		}else if(tState == States.END_ADVANCED){
			if(
				GUI.Button(new Rect(175,215,300,50),"Main Menu")
			){
				Application.LoadLevel(0);
			}
		}
		
		//drawn if a next button is needed
		if(nextButton!=-1){
			if(
				GUI.Button(new Rect(175,215,300,50),"Next")
			){
				setState((States)nextButton);
			}
		}
		GUI.depth=0;
	}
	
	//Draws an arrow pointing to position pos, with width and heith from size, and rotated around pos by degree angle
	void drawArrow(Vector3 pos, Vector2 size, float angle){
		Vector3 v =Camera.main.WorldToScreenPoint(pos);
		drawArrow(new Vector2(v.x,v.y),size,angle);
	}
	//function to point to a 2d point on the screen
	void drawArrow(Vector2 pos, Vector2 size, float angle){
		Vector3 v = pos;
			
		GUIUtility.RotateAroundPivot(angle,new Vector2(v.x,Screen.height-v.y));
		GUI.DrawTexture(new Rect(v.x-size.x,Screen.height-v.y,size.x,size.y),arrowTex,ScaleMode.ScaleToFit);
		GUIUtility.RotateAroundPivot(-angle,new Vector2(v.x,Screen.height-v.y));

	}

	
}
