using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Class that only displays the start screen title and buttons
public class DisplayStartScreen : MonoBehaviour {

	void OnGUI(){
		//get the main style and apply it
		GUI.skin = GeneralManager.getStyle();
		
		//move title position up and down
		float pos = Mathf.Sin(Time.time/2)*15;
		//GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/2 - 100 + height, 200, 50), "Ascension");
		
		//draw the title with the up and down motion
		GUI.DrawTexture(new Rect(Screen.width/2 - 150, Screen.height/2 - 200 + pos, 300, 300),(Texture2D)Resources.Load("GUITextures/Title"));
		
		//draw the start button on teh screen that will start the game
		if(
			GUI.Button(new Rect(Screen.width/2 - 100, Screen.height/2 - 25 + 100, 200, 50), "Start")
		){
			StateToLoad.LoadLevel( "newtestmap_orig", StateType.NEW_LOCAL );
			
			Application.LoadLevel("LoadMap");
		}
		//draw button to start tutorial level
		if(
			GUI.Button(new Rect(Screen.width/2 - 100, Screen.height/2 - 25 + 155, 200, 50), "How To Play")
		){
			Application.LoadLevel("Tutorial");
		}
		
		//draw button to quit the game
		if(
			GUI.Button(new Rect(Screen.width/2 - 100, Screen.height/2 - 25 + 210, 200, 50), "Quit Game")
		){
			Application.Quit();
		}
		
		if(
			SaveLocalGame.GetGameIDs().Count > 0 && GUI.Button(new Rect(Screen.width/2 - 100, Screen.height/2 - 25 + 260, 200, 50), "Load Game")
		){
						StateToLoad.LoadLevel( SaveLocalGame.GetGameIDs()[0], StateType.LOADED_LOCAL);
			
			Application.LoadLevel("LoadMap");
		}
		
		
		
		//pulse the vignetting effect on the screen
		Vignetting v = (Vignetting)Camera.main.GetComponent("Vignetting");
		if(v){
			v.intensity = (0.5f *  Mathf.Sin(Time.time/2) + 0.5f)*2 + .5f;
		}
		
		//center the label text for name at the bottom
		var centeredStyle = GUI.skin.GetStyle("Label");
   		centeredStyle.alignment = TextAnchor.UpperCenter;
		centeredStyle.fontSize= 100;
		
		GUI.color = new Color(1,1,1,.25f);
		
		GUI.Label(new Rect(Screen.width/2 - 150, Screen.height - 25 - 40, 300, 50), "Created by Garrett Johnson \n\n Music by Mr.Spastic (Absolution Instrumental)",centeredStyle);
	}
}
