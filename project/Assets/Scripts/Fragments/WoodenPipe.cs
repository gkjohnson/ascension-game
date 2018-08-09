using UnityEngine;
using System.Collections;

public class WoodenPipe : TownStaff {

	public override void Apply (Tile t)
	{
		if(t==null)return;
		t.Dig(HDELTA, false);
	}
	
	public override string getName ()
	{
		return "Wooden Pipe";
	}
	
	public override string getDescription ()
	{
		return "Create a trench between two tiles up to " + MAX_DIST + " tiles out. Select the first tile, then select the second to indicate a path for the trench.";
	}
}
