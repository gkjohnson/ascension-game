using UnityEngine;
using System.Collections;

//structure that represents a building and the resources that are needed to build it
public abstract class Building : MonoBehaviour {
	//Required Resources
	public int Gem;
	public int Stone;
	public int Wood;
		
	public int getGem(){return Gem;}
	public int getStone(){return Stone;}
	public int getWood(){return Wood;}
	
	virtual public void Update(){
		enabled=false;
	}
	
	abstract public void Exhausted();
}
