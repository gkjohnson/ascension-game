using UnityEngine;
using System.Collections;

public class DragTrench : Fragment {
	
	protected const int HDELTA = 2;
	
	int maxTimeCount = 25;
	int timeCount = 0;
	
	int maxDig=15;
	
	Tile prevTile=null;
	
	bool Started=false;
	
	public bool validaterNormalize(Tile t, Tile f, int dir , int dDelta , int hDelta){
		return true;//dir==2;
	}
	
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		
		if(HoverTile && Input.GetMouseButtonDown(0))Started=true;
		
		if(Started && Input.GetMouseButton(0)){			
			timeCount--;
			
			if(timeCount <=0 && HoverTile || HoverTile && prevTile!=HoverTile){
				Do ();
	
				timeCount=maxTimeCount;
				prevTile=HoverTile;
				maxDig--;
			}
		}else if(Started && !Input.GetMouseButton(0)){
			timeCount=0;
		}
						
		if(maxDig==0)Use();
		
		//if(HoverTile)GM.TileMan.Validate(HoverTile,5,5,new TileManager.validater(validaterNormalize));
		//if(HoverTile)HoverTile.Validate();

		return true;
	}
	
	public virtual void Do(){
		HoverTile.Dig(HDELTA,false);
	}
	
	
	
	//called when the host is selected and valid tile is selected
	override public bool SelectValidTile(Tile t){
		SelectTile(t);
		return true;
	}
	
	
	public override string getName ()
	{
		return "Hoe";
	}
	public override string getDescription ()
	{
		return "Click, hold, and drag from a tile to create a trench in an arbitrary path.";
	}
	
	//called when the unit is deselected - used to reset the host to before it is clicked on
	override public void Clear(){
		if(prevTile!=null)Use();
	}
	
	/* DEFUNCT */
	//called when the host is selected and another unit within range is clicked on
	override public bool ActOnUnit(Unit u){return true;}
	//called when the unit is rolled over and when it is being placed
	override public void MarkTiles(Tile t){}
}
