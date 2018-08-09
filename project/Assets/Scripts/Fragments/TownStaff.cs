using UnityEngine;
using System.Collections;

public class TownStaff : Fragment {
	
	Tile Initial = null;
	
	protected const int MAX_DIST = 7;
	protected const int HDELTA = 3;
	bool Raising = false;
	int Direction = 0;
	int Distance = 0;
	Tile lastTile = null;
	int count = 5000;
	
	//TODO: Test this delayed response thoroughly to ensure that nothing bad happens.
	void Update(){

		count--;
		if(Raising && count % 4 ==0){
			if(!lastTile){
				Use ();
				return;
			}
			lastTile = lastTile._adjacentTiles[Direction];
			Apply(lastTile);
			//lastTile.Dig(2,false);
			Distance--;
			if(Distance==-1)Use ();
		}
	}
	
	public virtual void Apply(Tile t){
		if(t==null)return;
		t.incHeight(HDELTA);
	}
	
	public bool validaterNormalize(Tile t, Tile f, int dir , int dDelta , int hDelta){
		return true;//dir==2;
	}
	
	//called when the current selected unit has a host - used to validate tiles
	override public bool UpdateValidatedTiles(){
		if(Raising)return true;
		
		//if(HoverTile)HoverTile.Validate();
		
		if(Initial)Initial.Validate();
		
		if(Initial && HoverTile){

			
			int dist = getDist(Initial, HoverTile);

			int dir = getDir(Initial,HoverTile);
			
			Tile t2 = Initial;
			for(int i=0; i<MAX_DIST ; i++){
				if(i>dist)break;
				
				t2 = t2._adjacentTiles[dir];
				if(t2)t2.Validate ();
				else break;
			}
			
		}
		return true;
	}
	
	int getDir(Tile fr, Tile to){
		
		Vector3 v = fr.transform.position - to.transform.position;
		float angle= Mathf.Atan2(v.x,v.z);
		int iAngle=(int)(Mathf.CeilToInt(Mathf.Rad2Deg*(angle))+180 + 30);
		int num=(iAngle/60)%6;
		return num;
	}
	
	int getDist(Tile fr, Tile to){
		return (int)(((fr.transform.position - to.transform.position).magnitude)/8 - 1);
	}
	
	public override bool SelectValidTile (Tile t)
	{
		SelectTile(t);
		return true;
	}
	
	//called when the host is selected and valid tile is selected
	override public bool SelectTile(Tile t){
		if(Raising)return true;
		
		if(Initial == t){
			Initial=null;
		}else if(Initial){
			
			int dir = getDir(Initial, t);
			int dist = getDist(Initial, t);
			
			Apply(Initial);
			
			Raising = true;
			lastTile = Initial;
			Distance= Mathf.Min(dist, MAX_DIST-1);
			Direction= dir;
			
		}else{
			Initial=t;
		}
		
		return true;
	}
	
	public override string getName ()
	{
		return "Ancient Staff";
	}
	public override string getDescription ()
	{
		return "Create a wall between two tiles up to " + MAX_DIST + " tiles out. Select the first tile, then select the second to indicate a path for the wall.";
	}
	//called when the unit is deselected - used to reset the host to before it is clicked on
	override public void Clear(){}
	
	/* DEFUNCT */
	//called when the host is selected and another unit within range is clicked on
	override public bool ActOnUnit(Unit u){return true;}
	//called when the unit is rolled over and when it is being placed
	override public void MarkTiles(Tile t){}
	
}