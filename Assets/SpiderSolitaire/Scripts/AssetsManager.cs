using UnityEngine;
using System.Collections;
/**
 Project Name : Spider Solitaire
 Author : FlashTang 
*/
public class AssetsManager : MonoBehaviour {

	public static GameObject allMC; 
	 
	public static Texture2D getTexture2DByFile(string _fileStr){
		Texture2D texture = Resources.Load (_fileStr, typeof(Texture2D)) as Texture2D;
		return texture;
	}

	public static Sprite getSpriteByFile(string _fileStr){
		Sprite sprite = Resources.Load (_fileStr, typeof(Sprite)) as Sprite;
		return sprite;
	}


	public static GameObject createGameObjectBySpriteFile(string _fileStr, string _name=null){
		GameObject gameObject = new GameObject ();

		gameObject.AddComponent<SpriteRenderer> ();
		gameObject.GetComponent<SpriteRenderer> ().sprite = AssetsManager.getSpriteByFile (_fileStr);
	 	
		if(_name!=null){
			gameObject.name = _name;
		}
		return gameObject;

	}
}
