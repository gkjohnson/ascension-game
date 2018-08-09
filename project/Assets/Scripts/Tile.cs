using UnityEngine;
using System.Collections;

//TILE
//Component for each tile that handles height changes, residing units, and powerups within them

public class Tile : MonoBehaviour{
	int _height = -10;
	public Tile[] _adjacentTiles= new Tile[6];
	public TileManager _tileManager;
	public static GeneralManager GM;
	
	//points to the tiles above and below
	public Tile Above=null;
	public Tile Below=null;
	
	//flags for the tiles state
	bool valid=false;
	public bool marked=false;
	public bool _mouseOver=false;
	public bool Foundation = false;
	
	public const int MAX_HEIGHT=10;
	public const int MIN_HEIGHT=-10;
	
	//units or objects residing on the tile
	public Unit Resident=null;
	public SurfaceFragment SurfaceFrag= null;
	
	public GameObject instance_TileBase;
	public GameObject TileBase;
		
	public static float minHeight = -20;
	
	//Fragment information
	FragType NextFrag = FragType.KINGS_RING;
	
	//Richness
	int _richness=-1;
	public const int MAX_RICHNESS=5;
	
	public Texture2D[] RichnessTextures = new Texture2D[MAX_RICHNESS+1];
	
	//dictates the original height
	public int ORIG_HEIGHT;
	
	//for dictationg whether or not the tile has been freshly changed
	public bool Fresh = false;
	
	//noises for digging, finding, and changing height
	public AudioClip DigNoise;
	public AudioClip PickupNoise;
	public AudioClip RaiseNoise;
	
	//stores the original color of the tile
	Color _origColor = new Color(-1,-1,-1);

	
	//flag about whether or not to refresh the material and height
	bool _refreshMat = true;
	bool _updateHeight = true;
	
	void Awake(){
		if(_height==0)setHeight(getImpliedHeight());
		audio.Stop();
	}
	
	// Use this for initialization
	void Start () {
		GM = GeneralManager.GenMan;
		
		//find the neighbors of the tile
		//GetNeighbors();
		
		_tileManager = TileManager.TM;
		
		//creata base
		TileBase = ((GameObject)Instantiate(instance_TileBase));
		TileBase.transform.parent = this.transform;
		TileBase.transform.localPosition = Vector3.zero;
		
		//load the appropriate material
		string toLoad = "";
		if( transform.parent.tag == "LowPlane" ) renderer.sharedMaterial = TileMaterialManager.TMM.LowMat;
		else if ( transform.parent.tag == "MidPlane" ) renderer.sharedMaterial = TileMaterialManager.TMM.MidMat;
		else if ( transform.parent.tag == "HighPlane" ) renderer.sharedMaterial = TileMaterialManager.TMM.HighMat;
				
		//get a doodad to add visual flair
		int r=Mathf.FloorToInt(Random.Range(0,6));
		//if(r==0)((GameObject)Instantiate(Resources.Load("doodad"),transform.position,transform.rotation)).transform.parent = this.transform;
		
		//set the richness of the tile
		if( _richness == -1)
		{
			if(Random.Range(0.0f, 1.0f) < 0.25f){
				_richness = Random.Range(0, 2);	
			}else{	
				_richness = Random.Range(0, MAX_RICHNESS+1);
			}
		}
		
		//get a new fragment for the tile
		RefreshFragment();
		
		//rotate the tiles randomly to give more interested to anything on them
		this.transform.RotateAround(new Vector3(0,1,0), Mathf.PI/3 * Random.Range(0,6));
		UpdateMaterial();
	}
	
	void Update () {
		//if the material needs updating
		if(_refreshMat || true) UpdateMaterial();
		//update the height of the tile
		if(_updateHeight) UpdateHeight();
	}
	
	//Update the height of the tile
	public void UpdateHeight(){
		//store the height difference for one layer
		float heightDelta = _tileManager.tileHeightDelta;
		//get the destination height
		float newHeight = _height * heightDelta;
		//get the current height
		float currHeight = transform.localPosition.y;
		
		float ease = .25f;
		
		//fix the info so that materials can be batched
		if( Mathf.Abs(newHeight - currHeight) < 0.001f )
		{
			_updateHeight = false;
			ease = 1.0f;
			
		}
		
		//ease a little from current height to dest height
		newHeight = currHeight+(newHeight - currHeight) * ease;
		
		//move and scale the tile to move with the ease
		transform.localPosition = new Vector3( transform.localPosition.x, newHeight, transform.localPosition.z);
		TileBase.transform.localScale = new Vector3(1,- (newHeight - minHeight),1);
		
		//change the tile UVs to scale with the height smoothly and with no stretching
		TileBase.renderer.material.mainTextureScale = new Vector2(2, -((TileBase.transform.localScale.y) / (heightDelta)) * .5f * .25f );
		TileBase.renderer.material.mainTextureOffset = new Vector2( 0 , .5f );
		
		if( !_updateHeight )
		{
			try
			{
				TileBase.renderer.sharedMaterial = TileMaterialManager.TMM.TileBaseMat[ _height - MIN_HEIGHT ];
			}
			catch( System.Exception ){}
		}
	}
	//Update the highlight of the tile based on variables
	void UpdateMaterial(){

		//mark the material as updated
		_refreshMat = false;
		
		if(Foundation){
			renderer.sharedMaterial = TileMaterialManager.TMM.foundationMat;
		}

		//shift the hover states
		int shift = 0;
		if( valid && _mouseOver ) shift = 3;
		else if( valid ) shift = 1;
		else if( _mouseOver ) shift = 2;
				
		Material[] m = renderer.sharedMaterials;//new Material[ 3 ];
		//m[0] = renderer.sharedMaterials[0];
		m[1] = TileMaterialManager.TMM.RichnessMat[ _richness ];
		m[2] = TileMaterialManager.TMM.HoverValidMat[ shift ];
		
		renderer.sharedMaterials = m ;
	}
	
	
	//click commands for when the tile is valid and when it is not
	public void OnMouseOver(){
		//if it changed to mouseover, refresh the materials
		if(!_mouseOver)_refreshMat=true;
		_mouseOver=true;
		
		if(Resident!=null){
			Resident.Hover=true;
		}
		
		//if the tile is valid, select it when clicked
		if(Input.GetMouseButtonDown(0)||Input.GetMouseButtonDown(1)||Input.GetMouseButtonDown(2)){
			_tileManager.SelectTile(this);
		}		
		//notify the manager that the tile has been hovered over
		_tileManager.TileHover(this);
		
		
		//set the plane manager to focus on this tile's plane when needed
		//_tileManager.GM.PlaneMan.SetFocusPlane(transform.parent.tag);
	}
	//negate the mouseover actions when the mouse exits the tile
	public void OnMouseExit(){
		if(_mouseOver)_refreshMat=true;

		_mouseOver=false;
		if(Resident!=null){
			Resident.Hover=false;
		}
	}
	
	public const int HIGHLIGHT_UP=0;
	public const int HIGHLIGHT_DOWN=1;
	public const int HIGHLIGHT_BOTH=2;
	//used to validate the tiles that are above and below the planes for the attached tile
	public void ValidatePlanes(int dir,int depth){
		//valid=true;
		if(depth<=0)return;
		if(dir==HIGHLIGHT_UP||dir==HIGHLIGHT_BOTH){
			if(Above!=null){
				Above.ValidatePlanes(HIGHLIGHT_UP,depth-1);
			}
		}
		if(dir==HIGHLIGHT_DOWN||dir==HIGHLIGHT_BOTH){
			if(Below!=null){
				Below.ValidatePlanes(HIGHLIGHT_DOWN,depth-1);
			}
		}
	}
	
	//calculates the height based on teh current y position of the tile relative to its parent (used initially to decide the height of the tile)
	int getImpliedHeight(){
		int planeHeight;
		if(transform.parent)planeHeight=Mathf.FloorToInt(transform.position.y-transform.parent.position.y);
		else planeHeight=0;
		return planeHeight;
	}
	
	// Check if the passed tile is adjacent
	bool isAdjacent(Tile t){
		for(int i=0;i<_adjacentTiles.Length;i++){
			if(_adjacentTiles[i]==t){
				return true;
			}
		}
		return false;
	}
	
	public int getHeight(){ return _height; }
	
	// Sets the height of the tile
	public int setHeight(int num){
		int h = _height;
		
		_height=Mathf.Max(Mathf.Min(num, MAX_HEIGHT), MIN_HEIGHT);
		h -= _height;
		
		Fresh=true;

		_updateHeight = true;
		
		return -h;
	}
	//increases the height by the number given
	public int incHeight(int num){
		return setHeight(_height+num);
	}
	//decreases the height by the passed number
	public int decHeight(int num){
		return setHeight(_height-num);
	}
	
	//validates the tile so it can be selected
	public void Validate(){
		if(!valid)
		{
			_refreshMat=true;
			//TileManager.TM.Invalidate();
		}
		valid=true;
	}
	//devalidates the tile so it can no longer selected
	public void Invalidate(){
		if(valid)
		{
			_refreshMat=true;
			//TileManager.TM.Invalidate();
		}
		valid=false;
	}
	public bool IsValid(){
		return valid;
	}
	public void SetValid(bool t)
	{
		if(t) Validate();
		else Invalidate();
	}
	//digs into the tile at a depth of 1, returns an item that was dug up, if there is one
	public Fragment Dig(){
		return Dig(1);
	}
	//digs into the tile the passed depth
	public Fragment Dig(int depth){
		return Dig(depth,true);
	}
	
	//digs into the tile the passed depth, and returns an item depending on "giveFrag"
	public Fragment Dig(int depth, bool giveFrag){
		
		//if the height could not be changed, do not dig
		if(decHeight(depth) == 0){
			return null;
		}
		
		//create the destroyed tile object
		GameObject g = (GameObject)Instantiate(Resources.Load("Prefabs/BustedTile"),this.transform.position - new Vector3(0,depth,0),this.transform.rotation);
		BustedTile b = ((BustedTile)g.GetComponent("BustedTile"));
		
		b.ReferenceTile(this);		
		
		b.setScale(depth);
		
		audio.PlayOneShot(DigNoise);
		
		//try to give fragment, if applicable
		if(giveFrag){
			//Gives a random chance that no fragment will be dropped based off of richness
			
			float r = Random.Range(0.0f,1.0f);
			
			if(r > ((float)_richness)/MAX_RICHNESS * .9f){
				return null;
			}
			
			Fragment f = GetFragment();
			GM.FragMan.Collect(f);
			CreateFragRemains(f);
			return f;
		}else{
			return null;
		}
		
		audio.PlayOneShot(RaiseNoise);
	}
	
	//create the fragment icon that floats above the tile
	public void CreateFragRemains(Fragment f){
		FragmentRemains fragr = ((FragmentRemains)((GameObject)Instantiate(Resources.Load("Prefabs/FragmentRemains"))).GetComponent("FragmentRemains"));
		fragr.SetTile(this);
		fragr.SetFragment(f);
		
		audio.PlayOneShot(PickupNoise);
	}
	
	//sets the fragment held in the tile
	public void setFragment(FragType ft){
		NextFrag = ft;
	}
	
	//gets a different random fragment basedon which plane the tile is on
	void RefreshFragment(){
		PlaneManager pm = PlaneManager.PM;
		
		GameObject p = transform.parent.gameObject;
		if( p == pm.HighPlane){
			NextFrag = FragmentManager.FM.GetRandomFrag( FragmentManager.FM.UpperFrags);
		}else if( p== pm.MidPlane){
			NextFrag = FragmentManager.FM.GetRandomFrag( FragmentManager.FM.MiddleFrags);
		}else if( p== pm.LowPlane){
			NextFrag = FragmentManager.FM.GetRandomFrag( FragmentManager.FM.LowerFrags);
		}
	}
	
	//increase the richness of the tile by one
	public void incRichness(){
		changeRichness(1);
	}
	//decrease the richness of the tile by one
	public void decRichness(){
		changeRichness(-1);
	}
	//changes the richness of the tile by the amount passed
	public void changeRichness(int val){
		_richness = Mathf.Min(Mathf.Max(_richness + val, 0), MAX_RICHNESS);
	}
	
	//check what item is held within the tile
	public FragType PeekFragment(){
		return NextFrag;
	}
	
	//returns the fragment within the tile, and has a chance of lowering the richness of the tile
	Fragment GetFragment(){
		Fragment f = FragmentManager.CreateFragment(NextFrag);
		
		RefreshFragment();
		
		if(Random.Range(0.0f,1.0f)<.75){
			decRichness();
		}
		
		return f;
	}
	
	//returns the richness of the tile
	public int getRichness(){
		return _richness;
	}
	//setst he richness of the tile
	public void setRichness(int r){
		_richness=r;
	}
	
	//Normalize the tile to get closer to the original position and alter the richness
	public void Normalize(){
		if(_height == ORIG_HEIGHT)return;		
		
		
		//potential to normalize based off of the distance from its origin (inverted -- low number = high chance)
		int potential = (int)(((1.0f-((float)Mathf.Abs(_height-ORIG_HEIGHT)/MAX_HEIGHT))*5));
		
		//try to change the tile based on a random value and potential
		int r=Random.Range(0,potential);
		if(r==0){
			
			//potentially increase the richness of the tile
			if(Random.Range(0.0f, 1.0f) > ((float)_richness/MAX_RICHNESS)*0.75f + 0.25f){
				_richness++;
			}
			
			int dir=_height - ORIG_HEIGHT;
			
			//depending on the height of the tile, increase the amount that it can change.
			int x = Mathf.CeilToInt((float)Mathf.Abs((_height-ORIG_HEIGHT)/(MAX_HEIGHT/5)));
			int delta = Random.Range(1, x);
						
			if(dir>0){
				decHeight(delta);
			}else if(dir<0){
				incHeight(delta);
			}
			Fresh=false;
		}
	}
	
	//New function to raycast to find the neighboring tiles
	public void GetNeighbors(){
		//raycast out to each direction
		int sides = 6;
		for(int i=0;i<sides;i++){
			//if it hits something, add its tile component to array of tiles
			RaycastHit hit;
			Vector3 dir = new Vector3(8*Mathf.Sin(i*(2*Mathf.PI/sides)),0,8*Mathf.Cos(i*(2*Mathf.PI/sides)));
			if (Physics.Raycast(transform.position, dir, out hit, 8)){
				_adjacentTiles[i] = hit.transform.GetComponent<Tile>();
			}

		}
	}
	
}
