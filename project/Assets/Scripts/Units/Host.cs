using UnityEngine;
using System.Collections;

//template for any unit that can be interacted with and needs to react to selected/hovered/validated tiles
abstract public class Host : MonoBehaviour {
	
	public static GeneralManager GM;
	
	void Start(){
		GM = (GeneralManager)GameObject.FindObjectOfType(typeof(GeneralManager));
		StartAux();
	}
	
	virtual public void StartAux(){}
	
	//For all bool functions, if the function returns false, the default is ignored
	//otherwise, the default is carried through with
	
	//called when the current selected unit has a host - used to validate tiles
	abstract public bool UpdateValidatedTiles();
	//called when the host is selected and called every time a valid tile is hovered over
	abstract public bool TileHover(Tile t);
	//called when the host is selected and valid tile is selected
	virtual public bool SelectValidTile(Tile t){return true;}
	//called when the host is selected and tile is selected
	virtual public bool SelectTile(Tile t){return false;}
	//called when the unit is deselected - used to reset the host to before it is clicked on
	abstract public void Clear();
	
	/* DEFUNCT */
	//called when the host is selected and another unit within range is clicked on
	abstract public bool ActOnUnit(Unit u);
	//called when the unit is rolled over and when it is being placed
	abstract public void MarkTiles(Tile t);
	
	virtual public void Selected(){}

}
