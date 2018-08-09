using UnityEngine;
using System.Collections;

public class DragWall : DragTrench {
	public override void Do ()
	{
		HoverTile.incHeight(HDELTA);
	}
	
	public override string getName ()
	{
		return "Pail";
	}
	public override string getDescription ()
	{
		return "Click, hold, and drag from a tile to create a wall in an arbitrary path.";
	}
}
