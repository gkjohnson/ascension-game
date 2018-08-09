using UnityEngine;
using System.Collections;

public class PlaneManager : MonoBehaviour {
	
	//self reference
	public static PlaneManager PM;
	
	// references to the various planes
	public GameObject HighPlane=null;
	public GameObject MidPlane=null;
	public GameObject LowPlane=null;
	
	//plane constants
	public const int HIGHPLANE=2;
	public const int MIDPLANE=1;
	public const int LOWPLANE=0;
	
	//default color of the background
	Color ORIG_BG_COLOR;
	
	//camera that records the background image
	public Camera bgCamera;
	
	//noise that plays on turn change
	public AudioClip TurnChangeNoise;
	
	
	SoulVessel Win = null;
	
	//original camera position
	Vector3 ORIG_CAMERA_SPOT;
	Quaternion ORIG_CAMERA_ROTATION;
	const float ORIG_ORTHOSIZE = 65;
	const float ORIG_CAMDISTANCE = 100;
	
	//which plane the user is trying to view
	int destPlane=0;
	
	//destination camera pos and ortho
	Vector3 _cameraDest;
	
	//whether the user is trying to view all planes or not
	public bool viewAllPlanes=false;
	
	//the average of the world space
	Vector3 _centerpoint = Vector3.zero;
	
	
	//camera values
	public float _camAngle = 45;
	public float _camDistance = 100;
	
	Camera _camera;
	Transform _cameraParent;
	
	void Awake(){
		PM=this;
	}
		
	// Use this for initialization
	void Start () {
		
		HighPlane=GameObject.FindGameObjectWithTag("HighPlane");
		MidPlane=GameObject.FindGameObjectWithTag("MidPlane");
		LowPlane=GameObject.FindGameObjectWithTag("LowPlane");
				
		_cameraDest=Camera.main.transform.position;
		ORIG_CAMERA_SPOT = _cameraDest;
		ORIG_CAMERA_ROTATION = Camera.main.transform.rotation;
		
		if(!bgCamera) bgCamera=Camera.main;
		
		ORIG_BG_COLOR = bgCamera.backgroundColor;
		
		_camera = Camera.main;
		
		Vector3 targetPos = _camera.transform.position;
		targetPos.y = LowPlane.transform.position.y + 65;
		_camera.transform.position = targetPos;
		
		_camera.orthographicSize = 65.0f;
	}
	
	// reassociates the tiles with the ones above and below them
	//TODO : Move the Tile Manager
	public void ReassociateTiles()
	{
		//Get list of Tiles
		Tile[] LowTiles= LowPlane.transform.GetComponentsInChildren<Tile>();
		Tile[] MidTiles= MidPlane.transform.GetComponentsInChildren<Tile>();
		Tile[] HighTiles= HighPlane.transform.GetComponentsInChildren<Tile>();
		
		//This list matches each tile to its corresponding tile in each plane
		//Cycle through middle tiles
		for(int i=0;i<MidTiles.Length;i++){
			Transform t1=MidTiles[i].transform;
			
			//Cycle through and match high tiles
			for(int j=0;j<HighTiles.Length;j++){
				Transform t2=HighTiles[j].transform;
				if(Mathf.FloorToInt(t1.position.x-t1.parent.position.x)==Mathf.FloorToInt(t2.position.x-t2.parent.position.x)&&
				   Mathf.FloorToInt(t1.position.z-t1.parent.position.z)==Mathf.FloorToInt(t2.position.z-t2.parent.position.z)){
					MidTiles[i].Above=HighTiles[j];
					HighTiles[j].Below=MidTiles[i];
				}
			}
			
			//Cycle through and match low tiles
			for(int j=0;j<LowTiles.Length;j++){
				Transform t2=LowTiles[j].transform;
				if(Mathf.FloorToInt(t1.position.x-t1.parent.position.x)==Mathf.FloorToInt(t2.position.x-t2.parent.position.x)&&
				   Mathf.FloorToInt(t1.position.z-t1.parent.position.z)==Mathf.FloorToInt(t2.position.z-t2.parent.position.z)){
					MidTiles[i].Below=LowTiles[j];
					LowTiles[j].Above=MidTiles[i];
				}
			}
		}
		
		_centerpoint = getAveragePosition( MidPlane.transform.parent.gameObject );
		
		
		_cameraParent = new GameObject("CameraParent").transform;
		_cameraParent.transform.position = _centerpoint;
		_camera.transform.parent = _cameraParent.transform;
		
		_camera.transform.localRotation = Quaternion.identity;
		_camera.transform.localPosition = -Vector3.forward * _camDistance;
		
		Vector3 lowCenter = _centerpoint;
		lowCenter.y = LowPlane.transform.position.y + 50;
		
		_cameraParent.position = lowCenter;
		
	}
	
	//get the average position of all the tiles.
	Vector3 getAveragePosition(GameObject g){
		Tile[] _children=g.GetComponentsInChildren<Tile>();
		
		Vector3 avg=new Vector3(0,0,0);
		for(int i=0;i<_children.Length;i++){
			avg+=_children[i].transform.position;
		}
		
		return avg/_children.Length;
	}
	
	// Update is called once per frame
	void Update(){
		
		UpdateCamera();
		
		destTileDelta = 2.0f;
		destTileHeight = (Tile.MIN_HEIGHT-2)*TileManager.TM.tileHeightDelta;
		
		//Tile.minHeight+=(-Tile.minHeight+destTileHeight)/2;
		//TileManager.TM.tileHeightDelta+=(-TileManager.TM.tileHeightDelta+destTileDelta)/10;
		TileManager.TM.tileHeightDelta = destTileDelta;
		//Tile.minHeight = destTileHeight;
		
		prevmousepos = Input.mousePosition;
	}
	
	bool _resetZoom = false;
	//Update the camera position etc
	void UpdateCamera()
	{
		if(Camera.main.transform.parent == null) return;
		
		//TODO: Make these movements work with touch input
				
		//CHANGE LAYER WITH SWIPING
		if(rotateWithMouse) mouseDelta += Input.mousePosition - prevmousepos;
		else mouseDelta = Vector3.zero;
		
		//TODO: consider checking how long the full swipe took to execute
		//the mouse was released and it's motion was in a relatively straight line, and it ended quickly, then change layers
		if(
			Input.GetMouseButtonUp(0) && 
			Mathf.Abs( mouseDelta.y ) > .15f * Screen.height &&
			Mathf.Abs( mouseDelta.x ) < Screen.width * .05f &&
			(Input.mousePosition - prevmousepos).sqrMagnitude > 30.0f * 30.0f
		){
			
			int prevDestPlane = destPlane;	
			destPlane -= (int) (mouseDelta.y / Mathf.Abs(mouseDelta.y));
			
			destPlane = Mathf.Clamp( destPlane, 0, 2 );
			
			if(destPlane != prevDestPlane) _resetZoom = true;
		}

		if(destPlane < 0) destPlane = 0;
		else if(destPlane > 2) destPlane = 2;
		
		
		
		// MOUSE CAMERA ROTATION //
		if(Input.GetMouseButton(0) && rotateWithMouse){
			
			float heightmult = 1;// -((Input.mousePosition.y) - Screen.height / 2) / (Screen.height/2);
						
			rotationVel.x = (Input.mousePosition.x - prevmousepos.x) * 15f * heightmult;
			rotationVel.y = (Input.mousePosition.y - prevmousepos.y) * 15f;
		}
		if(Input.GetMouseButtonDown(0))
		{
			//check if the raycast hit a tile and see if it's valid
			RaycastHit hit;
			if(Physics.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out hit))
			{
				Tile t = null;
				if( hit.transform.GetComponent<Tile>() ) t = hit.transform.GetComponent<Tile>();
				if( hit.transform.GetComponent<TileBase>() ) t = hit.transform.GetComponent<TileBase>().transform.parent.GetComponent<Tile>();
				
				if( t && t.IsValid() == false && !t.Resident && !FragmentManager.FM.getSelected()) rotateWithMouse = true;
			}
			else
			{
				rotateWithMouse = true;
			}
		}
		else if(Input.GetMouseButtonUp(0))
		{
			rotateWithMouse = false;
		}
		
		//update the rotational inertia
		Camera.main.transform.parent.RotateAround( Camera.main.transform.parent.position, new Vector3(0,1,0), rotationVel.x*Time.deltaTime );
		//rotate Y with clamped rotation values
		_camAngle -= rotationVel.y * Time.deltaTime;
		_camAngle = Mathf.Clamp( _camAngle, 15, 50);
		
		_cameraDest= new Vector3(Camera.main.transform.position.x,_cameraDest.y,Camera.main.transform.position.z);	
		rotationVel *= .9f;		
		
		// EASE TO PROPER HEIGHT
		float yDest = 0;
		if( !viewAllPlanes ){
			
			switch( destPlane )
			{
			case LOWPLANE:
				yDest = LowPlane.transform.position.y;
				break;
			case MIDPLANE:
				yDest = MidPlane.transform.position.y;
				break;
			case HIGHPLANE:
				yDest = HighPlane.transform.position.y;
				break;
			}
		}
		else
		{
			yDest = MidPlane.transform.position.y;
		}
		
		Vector3 targetPos = Camera.main.transform.parent.position;
		targetPos.y = yDest;
		//Camera.main.transform.position -= (Camera.main.transform.position - targetPos) * Time.deltaTime * 5;
		Camera.main.transform.parent.position = Vector3.Lerp(Camera.main.transform.parent.position, targetPos, 5 * Time.deltaTime);
		
		
		
		// EASE ORTHO TO PROPER SIZE 

		// Based on finger pinch, zoom in and out
		float scroll = Input.GetAxis("Mouse ScrollWheel") * 10;
		
		Camera.main.orthographicSize -= scroll*150*Time.deltaTime;
		Camera.main.orthographicSize = Mathf.Clamp( Camera.main.orthographicSize, 20, 105);
		
		_camDistance -= scroll* 150 * Time.deltaTime;
				
		if( _resetZoom && (Camera.main.orthographicSize < ORIG_ORTHOSIZE || _camDistance < ORIG_CAMDISTANCE) && scroll == 0.0f)
		{
			Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, ORIG_ORTHOSIZE, Time.deltaTime);
			_camDistance = Mathf.Lerp( _camDistance, ORIG_CAMDISTANCE, Time.deltaTime);
			
			if( 
				Mathf.Abs( Camera.main.orthographicSize - ORIG_ORTHOSIZE ) < .1f && 
				Mathf.Abs(_camDistance - ORIG_CAMDISTANCE) < .1f
			)
				_resetZoom = false;
		}
		else
		{
			_resetZoom = false;
		}
		
		
		
		//Pan the camera with right click
		mouseDelta = Input.mousePosition - prevmousepos;
		
		if( Input.GetMouseButton(1) )
		{
			float panX = -mouseDelta.x * .15f;
			float panY = -mouseDelta.y * .15f;
			
			_cameraParent.position += _cameraParent.forward*panY;
			_cameraParent.position += _cameraParent.right*panX;
		
		
			Vector3 planeCenter = _centerpoint;
			planeCenter.y = _cameraParent.position.y;
			
			Vector3 deltaPos = _cameraParent.position - planeCenter;
			
			if( deltaPos.sqrMagnitude > 25.0f * 25.0f ) deltaPos = deltaPos.normalized * 25.0f;
		
			_cameraParent.position = planeCenter + deltaPos;
		}
		

		
		
		
		// UPDATE THE BACKGROUND COLOR
		bgCamera.backgroundColor -= (bgCamera.backgroundColor - (ORIG_BG_COLOR + PlayerManager.PM.getColor(PlayerManager.PM.CurrTurn) * .04f)) * Time.deltaTime * 2;
		
		//update the ambient light color
		RenderSettings.ambientLight = 
			Color.Lerp( 
				RenderSettings.ambientLight, 
				(Color.white * .85f + PlayerManager.PM.getColor(PlayerManager.PM.CurrTurn) * .15f) * .15f, Time.deltaTime * 2
			);

		
		
		
		
		
		
		
		_camera.transform.localRotation = Quaternion.identity;
		_camera.transform.RotateAroundLocal(Vector3.right, _camAngle * Mathf.Deg2Rad);
		_camera.transform.position = _cameraParent.position - _camera.transform.forward * _camDistance;
		
		
		
		
		
		
	}
	
	//returns the plane that the user is trying to view
	public int getDestPlane(){
		return destPlane;
	}
	//sets the plane that the user is trying to view
	public void setDestPlane(int i){
		destPlane=i;
	}
	
	
	//remove the winner
	public void RemoveWin(){
		Win=null;
		_cameraDest = ORIG_CAMERA_SPOT;
		Camera.main.transform.rotation = ORIG_CAMERA_ROTATION;
	}
	
	//set the given SoulVessel as the one that had won
	public void SetWinner(SoulVessel sw){
		Win=sw;
	}
	
	Vector3 prevmousepos = Vector3.zero;
	//float rotationVel = 0.0f;
	Vector2 rotationVel = Vector2.zero;
	bool rotateWithMouse = false;
	
	float destTileHeight=-100;
	float destTileDelta=1.0f;
	
	Vector3 mouseDelta = Vector3.zero;

	//on turn change, set the background color
	public void OnChangeTurn(){
		bgCamera.backgroundColor = new Color(2,1.5f, 1.75f)*.15f;//ORIG_BG_COLOR * ORIG_BG_COLOR *100;
		Camera.main.audio.PlayOneShot(TurnChangeNoise);
	}
	
}
