using UnityEngine;
using System.Collections;


public class TileManager : MonoBehaviour {
	
	public GeneralManager GM;
	
	
	public float tileHeightDelta=1.0f;
	
	//TODO: use this to determine if tiles need to be refreshed
	private bool _invalidTiles=true;
	
	public static TileManager TM;
	
	Tile[] _alltiles = null;
	
	void Awake(){
		TM=this;	
	}
	// Use this for initialization
	void Start () {
		GM = GeneralManager.GenMan;
	}
	
	
	public bool stoploop = false;
	// Update is called once per frame
	void Update () {
		if( _alltiles == null || _alltiles.Length == 0 ) _alltiles = Component.FindObjectsOfType( typeof( Tile ) ) as Tile[];
		
		if(transferToUnit){
			Ray myray = new Ray();
			myray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (!Physics.Raycast(myray))
				transferToUnit=null;
		}
		
		if(resetActTile)ActiveTile=null;
		if(!resetActTile)resetActTile=true;
	}
		
	void LateUpdate(){
		
		if(stoploop) return;

		//TODO: Only do this if the tiles have been invalidated because of a move etc
		InvalidateAll();

		UpdateValidatedTiles();

		_invalidTiles = false;

	}
	
	public void Invalidate(){
		_invalidTiles = true;
	}
	
	//invalidate all tiles that exist
	void InvalidateAll(){
		for( int i=0 ; i < _alltiles.Length ; i++ ){
			if( _alltiles[i]==null ) break;
			_alltiles[i].Invalidate();
			_alltiles[i].marked=false;
		}
	}
	
	//Normalize Terrain eases tiles toward their original spot again
	
	public void NormalizeTerrain(){
		
		Tile[] t = Component.FindObjectsOfType(typeof(Tile)) as Tile[];
		
		for(int i=0;i<t.Length;i++){
			if(t[i].getHeight() == t[i].ORIG_HEIGHT)continue;
			
			if(t[i].Fresh){
				t[i].Fresh=false;
				continue;
			}
			
			t[i].Normalize();
			
		}
	}

	/*////////////////////////
	// VALIDATION FUNCTIONS //
	////////////////////////*/
	//Validate teh tiles a certain distance out
	public delegate bool validater(Tile t , Tile f , int dir , int dDelta , int hDelta);
	
	public void Validate(Tile t, int dist, int height, validater func){
		
		Validate(t,dist,height,func,dist,height);
	}
	
	//TODO: Stop it from stopping at a tile that has a unit on it. This should be handled in the validater
	private void Validate(Tile t, int dist, int height, validater func, int origDist, int origHeight){
		
		//If the distance has been reached, return
		if(dist<=0)return;
		
		//loop through all surrounding tiles
		for(int i=0;i<t._adjacentTiles.Length;i++){
			if(t._adjacentTiles[i]==null)continue;
			
			//check if it's allowed to be validated
			if(
			   Mathf.Abs(t.getHeight() - t._adjacentTiles[i].getHeight()) <= height &&
			   func(
			        t._adjacentTiles[i],
			        t,
			        i,
			        origDist-dist,
			        origHeight-height
			        ))
			{
				t._adjacentTiles[i].Validate();
			}else{
				continue;
			}

			//recurse
			if(Mathf.Abs(t.getHeight() - t._adjacentTiles[i].getHeight()) <= height){
				Validate(t._adjacentTiles[i],dist-1,height, func, origDist,origHeight);
			}
		//END LOOP	
		}
	}
	
	public void Validate(Tile t, int dist, int heightDelta){
		Validate(t,dist,heightDelta,true);
	}
	public void Validate(Tile t,int dist,int heightDelta, bool blockable){
		Validate(t,dist,heightDelta,blockable,0);
	}
	
	public const int TYPE_VALID=0;
	public const int TYPE_MARKED=1;
	
	public void Validate(Tile t, int dist, int heightDelta, bool blockable, int type){
		Validate(t,dist,heightDelta,blockable,type,t.getHeight());
		if(type==TYPE_VALID)t.Invalidate();
		if(type==TYPE_MARKED)t.marked=false;
	}
	void Validate(Tile t,int dist,int heightDelta,bool blockable, int type, int height){
		if(dist<0){return;}
		if(t==null){return;}
		
		bool tempValid=false;
		bool tempMarked=false;
		
		if(type==TYPE_VALID)tempValid=true;
		if(type==TYPE_MARKED)tempMarked=true;
		
		if(tempValid)t.SetValid(tempValid);
		if(tempMarked)t.marked=tempMarked;
		//if(t.Resident!=null)return;
		for(int i=0;i<t._adjacentTiles.Length;i++){
			if(t._adjacentTiles[i]==null)continue;
			if(Mathf.Abs(height-t._adjacentTiles[i].getHeight())>heightDelta)continue;
			if((t._adjacentTiles[i].Resident!=null)&&dist>0){
				if(tempValid)t._adjacentTiles[i].SetValid( tempValid );
				if(tempMarked)t._adjacentTiles[i].marked=tempMarked;
				
				if(!t._adjacentTiles[i].Resident.GetComponent("Soul")){
					if(tempValid)t._adjacentTiles[i].SetValid( tempValid );
					if(tempMarked)t._adjacentTiles[i].marked=tempMarked;
				}
				if(blockable)continue;
			}
			Validate(t._adjacentTiles[i],dist-1,heightDelta,blockable,type, height);
		}
	}
	//END VALIDATION FUNCTIONS
	
	/*//////////////////////////////////
	// VALID TILE VALIDATION
	//////////////////////////////////*/
	
	//Updates the tiles that are valid to be clicked based ona series of parameters
	void UpdateValidatedTiles(){
		//if not using fragment
		if(GM.FragMan.getSelected()){
			GM.FragMan.getSelected().UpdateValidatedTiles();
		}else if(GM.UnitMan.getSelected()){
			if(GM.UnitMan.getSelected().Player!=GM.PlayerMan.CurrTurn)return;
			if(((Host)GM.UnitMan.getSelected().GetComponent("Host")))((Host)GM.UnitMan.getSelected().GetComponent("Host")).UpdateValidatedTiles();
		}
		
		return;
	}

	
	//this code is called every frame and the tile that is being hovered over currently
	//is passed to the function
	public int TerrainModDelta=0;
	public int TransferSouls=0;
	public Unit transferToUnit=null;
	//clears any variables that store information about acitons
	public void ClearActions(){
		TerrainModDelta=0;
		TransferSouls=0;
		transferToUnit=null;
	}
	
	/*//////////////////////////////////
	// TILE HOVER
	//////////////////////////////////*/
	int newSoulCount=1;
	Tile ActiveTile=null;
	bool resetActTile=false;
	//called every time a tile is hovered over - used to change the kind of actions that will be made
	public void TileHover(Tile t){
		//if not using fragment
		if(GM.FragMan.getSelected()){
			GM.FragMan.getSelected().TileHover(t);
		}else if(GM.UnitMan.getSelected()){
			if(GM.UnitMan.getSelected().Player!=GM.PlayerMan.CurrTurn)return;
			if(((Host)GM.UnitMan.getSelected().GetComponent("Host"))) ((Host)GM.UnitMan.getSelected().GetComponent("Host")).TileHover(t);
		}
		
		return;
	}
	
	/*//////////////////////////////////
	// VALID TILE SELECT
	//////////////////////////////////*/
	public void SelectTile(Tile t){
		if(Input.GetMouseButtonDown(0) && !GM.FragMan.getSelected()){
			//if(t.Resident==GM.UnitMan.getSelected())GM.UnitMan.Deselect();
			//else 
			if(t.Resident)GM.UnitMan.Select(t.Resident);
		}
		if(t.IsValid()){
			SelectValidTile(t);
		}else{
			SelectInvalidTile(t);
			//if(!t.Resident)GM.UnitMan.Deselect();
			
			
		}

		
	}
	//this code is used to do stuff to the tiles when theyre clicked
	public void SelectValidTile(Tile t){
		if(GM.FragMan.getSelected()){
			GM.FragMan.getSelected().SelectValidTile(t);
		}else if(GM.UnitMan.getSelected()){
			if(GM.UnitMan.getSelected().Player!=GM.PlayerMan.CurrTurn)return;
			((Host)GM.UnitMan.getSelected().GetComponent("Host")).SelectValidTile(t);
		}
	}
	//code that deploys events to units and fragments when a non validated tile is clicked
	public void SelectInvalidTile(Tile t){
		if(GM.FragMan.getSelected()){
			GM.FragMan.getSelected().SelectTile(t);
		}else if(GM.UnitMan.getSelected()){
			if(GM.UnitMan.getSelected().Player!=GM.PlayerMan.CurrTurn)return;
			if(GM.UnitMan.getSelected().GetComponent("Host") && !((Host)GM.UnitMan.getSelected().GetComponent("Host")).SelectTile(t)){
				if(!t.Resident)GM.UnitMan.Deselect();
			}
		}
	}
	
	
	void OnGUI(){
		GUIStyle s= GUI.skin.GetStyle("Label");
		s.alignment=TextAnchor.UpperCenter;

		if(GM.PlayerMan.CurrentMove==PlayerManager.Move.CREATE_SOUL&&ActiveTile&&ActiveTile.IsValid()){
			Vector3 p=Camera.main.WorldToScreenPoint(ActiveTile.transform.position);
			
			
			if(!ActiveTile.Resident)GUI.Label(new Rect(p.x-15,Screen.height-p.y-10,30,20),newSoulCount.ToString(),s);
			else{
				Soul sr = ActiveTile.Resident.GetComponent("Soul") as Soul;
				GUI.Label(new Rect(p.x-15,Screen.height-p.y-10,30,20),Mathf.Min(newSoulCount,sr.max_souls-sr.Souls).ToString(),s);
			}
		}if(transferToUnit){
			Vector3 p=Camera.main.WorldToScreenPoint(transferToUnit.transform.position);
			GUI.Label(new Rect(p.x-15,Screen.height-p.y-45,30,20),TransferSouls.ToString(),s);
			
		}/*else if(GhostTile.renderer.enabled){
			
			Vector3 p=Camera.main.WorldToScreenPoint(GhostTile.transform.position);
			GUI.Label(new Rect(p.x-15,Screen.height-p.y-40,30,20),TerrainModDelta.ToString(),s);
		}*/
	}
}
