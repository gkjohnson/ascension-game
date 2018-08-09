using UnityEngine;
using System.Collections;

//Called on every turn change so the tree can grow when the turn is swapped and count the turns it's been burning
public class TreeTurnChange : TurnChange {
	
	int burnTurnCount=1;
	
	public void Start(){
		enabled=true;
	}
	//"Start" code moved here to ensure that the tile has been found by the tree surface fragment component
	public void Update(){
		//rotate the tree and move away from center to add interest
		TreeFragment tf = (TreeFragment)GetComponent("TreeFragment");
		
		const float shiftVal = 1.75f;
		this.transform.position += new Vector3(shiftVal * Mathf.Sin(Random.Range(0.0f, 2*Mathf.PI)), 0 , shiftVal * Mathf.Cos(Random.Range(0.0f, 2*Mathf.PI)));
		
		this.transform.parent = tf.AttachedTile.transform;

		transform.Rotate(new Vector3(0,Random.Range(0.0f,360.0f),0));
		enabled = false;	
	}
	//decrements the burning counter, every turn if it's burning
	public override void OnTurnChange ()
	{
		TreeFragment tf=((TreeFragment) GetComponent("TreeFragment"));
		
		tf.Grow();
		
		//if the tree has been burning
		if(tf.PrevBurning){
			//try to spread the fire to any surrounding trees with a random chance
			for(int i=0; i<tf.AttachedTile._adjacentTiles.Length;i++){
				if(!tf.AttachedTile._adjacentTiles[i])continue;
				if(tf.AttachedTile._adjacentTiles[i].SurfaceFrag &&
					tf.AttachedTile._adjacentTiles[i].SurfaceFrag.GetType() == typeof(TreeFragment) &&
					((TreeFragment)tf.AttachedTile._adjacentTiles[i].SurfaceFrag).Growth == TreeFragment.MAX_GROWTH &&
					Random.Range(0.0f,1.0f) < .55f){
					
						((TreeFragment)tf.AttachedTile._adjacentTiles[i].SurfaceFrag).Burning=true;
				}
			}
		}
		
		//if the tree has been burning for a turn
		if(burnTurnCount==0){
			//shrivel the tree and stop it from burning
			if(tf.Burning){	
				tf.Growth=0;
				tf.Burning=false;
				tf.PrevBurning=false;
			}
			//reset the burn counter
			burnTurnCount=1;
		}
		
		if(tf.Burning){
			burnTurnCount--;
		}
	}
}
