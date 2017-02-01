using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;
/**
 Project Name : Spider Solitaire
 Author : FlashTang
*/
public class Poker : MonoBehaviour {
	public string _type="Spade";
	public int num=0;
	public int g_index;
	public string image_path;

	public bool isDown = false;
	public bool isInstaniated = false;

	public int row;
	public int column;

	private Sprite textureSprite;

	public bool isOpen=false;
	// Use this for initialization
	void Start () {
	}

	void init(){
	}

	public delegate void Del(GameObject go);
	Del funcAfterOpen=null;
	Del funcAfterClose=null;

	public Poker open(Del func,float delay=0) {
		scaleType = "open";
		if(isOpen){
			return this;
		}
		funcAfterOpen = func;
		// C#
		TweenParms parms = new TweenParms();
		//parms.Prop("localPosition", new Vector3((float)j*(card_wid+gap)+card_wid/2.0f+start_x,hei_750-start_y-card_hei/2.0f-((float)i)*back_next_dis,0)); // Position tween
		//parms.Prop("localPosition", new Vector3(0,-0.3f,0));
		parms.Ease(EaseType.Linear); // Easing type
		parms.Delay(delay); // Initial delay
		//parms.OnStart(onStart,card);
		parms.Prop("localScale",new Vector3(0f,1f*Game.card_scale,1f*Game.card_scale));
		parms.OnComplete(scale0Complete,this.gameObject.name);
		HOTween.To(this.gameObject.transform, 0.1f, parms);

		return this;
	}

	private string scaleType=null;
	public Poker close(Del func,float delay=0){
		scaleType = "close";

		if(!isOpen){
			return this;
		}
		funcAfterClose = func;
		// C#
		TweenParms parms = new TweenParms();
		//parms.Prop("localPosition", new Vector3((float)j*(card_wid+gap)+card_wid/2.0f+start_x,hei_750-start_y-card_hei/2.0f-((float)i)*back_next_dis,0)); // Position tween
		parms.Ease(EaseType.Linear); // Easing type
		parms.Delay(delay); // Initial delay
		//parms.OnStart(onStart,card);
		parms.Prop("localScale",new Vector3(0f,1f*Game.card_scale,1f*Game.card_scale));
		parms.OnComplete(scale0Complete,this.gameObject.name);
		HOTween.To(this.gameObject.transform, 0.1f, parms);

		return this;
	}

	private void scale0Complete(TweenEvent e){
		if (scaleType == "open") {
			textureSprite = AssetsManager.getSpriteByFile (image_path);
		} else {
			textureSprite = AssetsManager.getSpriteByFile ("images/cards/back");
			//Debug.Log (textureSprite);
		}
		this.gameObject.transform.FindChild ("textureSprite").GetComponent<SpriteRenderer> ().sprite = textureSprite;

		///////////////////////////////////////////////////////////
		TweenParms parms = new TweenParms();
		parms.Ease(EaseType.Linear); // Easing type
		parms.Prop("localScale",new Vector3(1f*Game.card_scale,1f*Game.card_scale,1f*Game.card_scale));
		parms.OnComplete (scale1Complete, this.gameObject);
		HOTween.To(this.gameObject.transform, 0.1f, parms);
		///////////////////////////////////////////////////////////
	}

	private void scale1Complete(TweenEvent e){ 
		if (scaleType == "open") {
			isOpen = true;
			if(funcAfterOpen!=null){
				funcAfterOpen (this.gameObject);
			}
		} else {
			isOpen = false;
			if(funcAfterClose!=null){
				funcAfterClose (this.gameObject);
			}
		}		 
	}
		
	public float _alpha=1.0f;
	 
	public float alpha{
		get { 
			return _alpha; 
		}
		set { 
			_alpha = value; 
			gameObject.transform.FindChild("textureSprite").gameObject.GetComponent<SpriteRenderer>().material .color= 
				new Color(1.0f,1.0f,1.0f,_alpha);

			gameObject.transform.FindChild("glow").gameObject.GetComponent<SpriteRenderer>().material .color= 
				new Color(1.0f,1.0f,1.0f,_alpha);
		}
	}

	public void fadeOutAlphaAndDistroy(float delay=0.3f,float time=0.3f,bool addToKillList=true){

		//Debug.Log ("alpha:"+alpha.ToString());
		///////////////////////////////////////////////////////////
		TweenParms parms = new TweenParms();
		parms.Ease(EaseType.Linear); // Easing type
		parms.Prop("alpha",0.0f);
		parms.Delay (delay);
		parms.OnComplete (fadeAlphaComplete, 0);
		if (addToKillList) {
			Game.tweensToKillArr.Add (HOTween.To (this, time, parms));
		} else {
			HOTween.To (this, time, parms);
		}

		///////////////////////////////////////////////////////////
		
	}

	private void fadeAlphaComplete(TweenEvent e){

		//Debug.Log ("Destroy:: Type:"+_type+" num:"+num.ToString());
		GameObject.Destroy(this.gameObject);
	}

	// Update is called once per frame
	void Update () {

	}
}
