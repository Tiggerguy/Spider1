using UnityEngine;
using System.Collections;


using Holoville.HOTween;
using Holoville.HOTween.Plugins;
/**
 Project Name : Spider Solitaire
 Author : FlashTang 
*/
public class Group : MonoBehaviour {

	// Use this for initialization

	public ArrayList pokerArr = new ArrayList ();

	public Vector3 stayPos;

	public bool isSmall=false;

	public int row;
	 
	void Start () {
		 
	
	}


	public void addPoker(GameObject go){
		float next_dis= Game.back_next_dis;
		go.transform.parent = this.gameObject.transform;

		GameObject lastPoker = getLastPoker ();


		if (isSmall) {
			next_dis = Game.back_next_dis;
		} else {
			if(lastPoker != null){
				if (lastPoker.GetComponent<Poker> ().isOpen) {
					next_dis = Game.back_next_dis_normal;
				} else {
					next_dis = Game.back_next_dis;
				}
			}

		}
		if (pokerArr.Count > 0) {

				go.transform.localPosition = new Vector3 (0.0f,
				lastPoker.transform.localPosition.y - next_dis*Game.card_scale, lastPoker.transform.localPosition.z - Game.z_space);
		 
		
		} else {
			
			go.transform.localPosition = new Vector3 (0.0f,
				-0.0f, 0.0f);
		}
		 
		go.GetComponent<Poker> ().column = pokerArr.Count;
		go.GetComponent<Poker> ().row = row;
		go.transform.FindChild ("textureSprite").gameObject.GetComponent<SpriteRenderer>().sortingOrder = 100+pokerArr.Count;
		pokerArr.Add (go);
	}
	public void addPokers(ArrayList pokerList){
		for (int i = 0; i < pokerList.Count; i++) {
			addPoker ((GameObject)(pokerList[i]));
		}
	}

	public ArrayList removeCardsFrom(int startIndex){
		ArrayList removeList = new ArrayList();  
		for (int i = startIndex; i < pokerArr.Count; i++) {
			removeList.Add (pokerArr[i]);
		}

		return removeList;
	}

	public ArrayList getMoveCardsArray(int startIndex){
		ArrayList removeList = new ArrayList();  
		for (int i = startIndex; i < pokerArr.Count; i++) {
			removeList.Add (pokerArr[i]);
		}

		return removeList;
	}

 	
	public bool isDragAbleAtIndex(int startIndex){
 
		bool able = true;
		int memoryNum = -1;
		string memoryType = null;
		if(startIndex!=pokerArr.Count-1){
			for (int i = startIndex; i < pokerArr.Count; i++) {
				GameObject card = pokerArr[i] as GameObject;
				string nowType = card.GetComponent<Poker>()._type;

				if(memoryNum!=card.GetComponent<Poker> ().num+1&&memoryNum!=-1||(memoryType!=nowType&&i!=startIndex)){
			 
					able = false;
				}

				memoryNum = card.GetComponent<Poker> ().num;
				memoryType = card.GetComponent<Poker> ()._type;
			
			}
		}
		return able;
	}


	public int getLastMoveAbleIndex(){

		int index = -1;

		for (int i = 0; i < pokerArr.Count; i++) {
			 
			GameObject card = pokerArr [i] as GameObject;
			if(card.GetComponent<Poker>().isOpen){
				if(isDragAbleAtIndex (i)){
					index = i;
					break;
				}
			}

		}

		return index;
		
	}

	public ArrayList getLastMoveAbleArray(){
		ArrayList maArr = new ArrayList ();
		int moveAbleIndex = getLastMoveAbleIndex ();
		if(moveAbleIndex!=-1){

			for (int i = moveAbleIndex; i < pokerArr.Count; i++) {
				
				maArr.Add (pokerArr[i] as GameObject);

			}
		}
		//Debug.Log ("Group - "+row.ToString()+" moveAbleIndex:"+moveAbleIndex.ToString());
		return  maArr;

	}

	public GameObject getLastPoker(){
		if(pokerArr.Count>0){
			return pokerArr [pokerArr.Count - 1] as GameObject;
		}
		//Debug.Log ("No poker in this row!");
		return null;


	}

	public void goNextHighestDepth(){
		for (int i = 0; i < pokerArr.Count; i++) {
			GameObject go = pokerArr [i] as GameObject;
			GameObject text_sp = go.transform.FindChild ("textureSprite").gameObject;
			text_sp.GetComponent<SpriteRenderer> ().sortingOrder = 1000 + i;
		}
	}

	public void restore(){
		//float next_dis;
		//if (isSmall) {
		//	next_dis = Game.back_next_dis;
		//} else {
		//	next_dis = Game.back_next_dis_normal;
		//}

		for (int i = 0; i < pokerArr.Count; i++) {
			GameObject go = pokerArr [i] as GameObject;
			GameObject prev=null;
			if(i>0){
				prev = pokerArr [i-1] as GameObject;
			}
 			
			if (prev == null) {
				go.transform.localPosition = new Vector3 (0.0f, 0.0f, -Game.z_space * i);
			} else {
				if (prev.GetComponent<Poker> ().isOpen) {
					go.transform.localPosition = new Vector3 (0.0f, prev.transform.localPosition.y - Game.back_next_dis_normal*Game.card_scale, -Game.z_space * i);
				} else {
					go.transform.localPosition = new Vector3 (0.0f, prev.transform.localPosition.y - Game.back_next_dis*Game.card_scale , -Game.z_space * i);
				}

			}
		}
	}

	public void removeLastPoker(){
		
		if(pokerArr.Count>0){
			GameObject lastpoker = pokerArr [pokerArr.Count - 1] as GameObject;

			GameObject.Destroy (lastpoker);

			pokerArr.RemoveAt (pokerArr.Count-1);
		}

	}
 
	public void stayOriPos (){

		for (int i = 0; i < pokerArr.Count; i++) {
			GameObject go = pokerArr [i] as GameObject;
			GameObject text_sp = go.transform.FindChild ("textureSprite").gameObject;
			text_sp.GetComponent<SpriteRenderer> ().sortingOrder = 100 + i;
		}
		//Debug.Log (stayPos);
		this.gameObject.transform.localPosition = new Vector3 (stayPos.x,stayPos.y,0.0f);
	}

	public void showLast(){


	}

	public bool tryRemoveCompletedCards(){
		bool hasComp = false;
		bool isLinked = true;
		int memoryNum=1;
		if(pokerArr.Count>=13){
			Poker first = getLastPoker ().GetComponent<Poker> ();
			if(first.num==1){
				string startType = first._type;
				for (int i = pokerArr.Count-2; i >= 0; i--) {
					GameObject go = pokerArr [i] as GameObject;
					Poker p = go.GetComponent<Poker>();
					if (p.num == memoryNum + 1 && p._type.Equals(startType)) {
						if (memoryNum == 12)
							break;
						memoryNum++;
					} else {
						isLinked = false;
						break;
					}

				}

				if(isLinked){
					hasComp = true;
					int d_time = 0;
					Game.isRemoving = true;
					for (int i = pokerArr.Count-1; i > pokerArr.Count-14; i--) {
						d_time++;
						GameObject go = pokerArr [i] as GameObject;
						// C#
						TweenParms parms = new TweenParms();

						Vector3 pos;// = new Vector3(Game.baseX+Game.game_wid - Game.card_wid/2.0f-Game.start_x,Game.baseY-Game.card_hei/2.0f-Game.start_y+Game.hei_750-Game.back_next_dis_normal/2.0f*Game.completedCount*Game.card_scale,go.transform.position.z);
						 
						pos = new Vector3(Game.complete_glow.transform.position.x,Game.complete_glow.transform.position.y-Game.back_next_dis_normal/2.0f*Game.completedCount*Game.card_scale,Game.complete_glow.transform.position.z);
						parms.Prop("position",pos); // Position tween
						parms.Ease(EaseType.Linear); // Easing type
						parms.Delay(((float)d_time)*0.05f); // Initial delay

						if (i == pokerArr.Count - 13) {
							parms.OnComplete (comp, 1,go);
						} else {
							parms.OnComplete (comp, 0,go);
						}

						HOTween.To(go.transform, 0.15f, parms);
				


					}

					int removeNum = 13;

					while(removeNum!=0){
						removeNum--;

						pokerArr.RemoveAt (pokerArr.Count-1);
						
					}

					Game.completedCount++;


				}

				 
			}


		}

		return hasComp;
	}

	private int comp_depth = 4000;
	private void comp(TweenEvent e){

		((GameObject)(e.parms [1])).transform.FindChild ("textureSprite").gameObject.GetComponent<SpriteRenderer>().sortingOrder = 100*Game.completedCount+comp_depth++;

		if (((int)(e.parms [0])) == 1) {
			if (pokerArr.Count > 0) {
				(pokerArr [pokerArr.Count - 1] as GameObject).GetComponent<Poker> ().open (null, 0);
			}
			Game.completeBigK_array.Add (e.parms [1] as GameObject);

			if(Game.completedCount==8){
				 
				Game.you_win_window.SetActive (true);
			}
			Game.isRemoving = false;

			 

		} else {

			((GameObject)(e.parms [1])).GetComponent<Poker> ().fadeOutAlphaAndDistroy (0.3f,0.3f,false);
		}



			 

	}


	public bool resize(){



		return true;
	}

	public void showHigh(){
	}

	public void showLow(){
	}



	// Update is called once per frame
	void Update () {
	
	}
}
