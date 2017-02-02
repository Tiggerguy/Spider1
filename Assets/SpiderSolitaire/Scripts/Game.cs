using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using Holoville.HOTween;
using Holoville.HOTween.Plugins;
/**
 Project Name : Spider Solitaire
 Author : FlashTang 
*/

public class Game : MonoBehaviour {
	// Use this for initialization
	float dock_shadow_height=10.0f/100.0f;

	GameObject game_mc;
	GameObject back_cards_container;
	GameObject poker;
	Vector2 basePoint;

	float gap = 15.0f/100.0f;
	public static float baseX,baseY;
	public static float start_x = 16.0f/100.0f;
	public static float start_y = 12.0f/100.0f;

	public static float ori_card_wid = 100.0f/100.0f;
	public static float ori_card_hei = 150.0f/100.0f;

	public static float card_wid = 100.0f/100.0f;
	public static float card_hei = 150.0f/100.0f;

	public static float hei_750 = 750.0f/100.0f;
	public static float game_wid;

	ArrayList cardsCreatedArr = new ArrayList ();
	 
	ArrayList init_54;
	ArrayList rc_array_104=null;

	int level = 1;
	bool show_dock_start = true;
	//int prevDragIndex;
	int currentHitIndex;

	//float memoryZOfFirstMoveCard;
	//float memoryPosYOfFirstMoveCard;
	bool isDrag=false;
	int dargStartIndex;

	int closedIndex;
	float minDis;
	Vector2 offset;
	ArrayList dragList = new ArrayList ();

	bool settings_visible=false;
	GameObject dock_mc;
	GameObject n_game_win;
	GameObject background_mc;
	public static GameObject you_win_window;
	ArrayList groupObjectArr=new ArrayList();
	GameObject right_btns,settings_btn;

	private bool test = false;
	ArrayList testData;

	public static bool isRemoving = false;

	ArrayList completedInfoArr=null;

	public static int completedCount=0;
	public static float card_scale=1.0f;
	private GameObject replay_btn;

	void Start () {
		if(completedInfoArr==null){
			completedInfoArr = new ArrayList ();
		}

		testData = new ArrayList ();
		ArrayList data = new ArrayList{
			1,2,3,4,5,6,7,8,9,10,11,12,13,
			1,2,3,4,5,6,7,8,9,10,11,12,13,
			1,2,3,4,5,6,7,8,9,10,11,12,13,
			1,2,3,4,5,6,7,8,9,10,11,12,13,
			1,2,3,4,5,6,7,8,9,10,11,12,13,
			1,2,3,4,5,6,7,8,9,10,11,12,13,
			1,2,3,4,5,6,7,8,9,10,11,12,13,
			1,2,3,4,5,6,7,8,9,10,11,12,13
		};

		for (int s = 0; s < data.Count; s++) {
			testData.Add (new ArrayList{  (int)data [s],"Spade" });
		}
		 
		baseX = -(((float)Screen.width)/((float)Screen.height))*750.0f/100.0f/2.0f;
		baseY = -(750.0f / 100.0f / 2.0f);
		basePoint = new Vector2(baseX,baseY);

		game_wid = Mathf.Abs (baseX * 2.0f);

		/////////////////////////////////////////////////////////////////////////////
		//resize cards for different screen size
		float gap_100 = 15.0f;
		float ori_wid_100 = 100.0f;
		float card_wid_100 = 100.0f;
		float game_wid_100 = game_wid * 100.0f;
		float start_x_100 = start_x * 100.0f;

		float rest_ori = game_wid_100 - (start_x_100 + (card_wid_100 + gap_100) * 10);
		float min_gap_100 = 8.0f;
		while(rest_ori<card_wid_100*1.5f){
			card_wid_100 -= 1.0f;
			gap_100 = gap_100 * card_wid_100 / ori_wid_100;
			if(gap_100<min_gap_100){
				gap_100 = min_gap_100;
			}
			rest_ori = game_wid_100 - (start_x_100 + (card_wid_100 + gap_100) * 10);
		}

		///////////////////////////////////////////////////////////////////////////////
	 
		card_wid = card_wid_100 / 100.0f;
		card_scale = card_wid_100 / ori_wid_100;
		card_hei = card_hei * card_scale;
		gap = gap_100 / 100.0f;

		GameObject.Find ("windows").transform.position = Vector3.zero;

		GameObject bg = GameObject.Find ("background") as GameObject;
		bg.transform.localScale = new Vector2((game_wid+0.02f)/(bg.GetComponent<SpriteRenderer>().bounds.size.x),bg.transform.localScale.y);
		 
		game_mc = GameObject.Find ("game_mc");
		game_mc.transform.position = basePoint;

		replay_btn = GameObject.Find ("replay_btn");
		replay_btn.SetActive (false);

		back_cards_container = GameObject.Find ("back_cards_container");
		back_cards_container.transform.position = basePoint;

		background_mc = GameObject.Find ("background");
		poker = GameObject.Find ("poker");
		back_pos_stay_x = game_wid - card_wid / 2.0f - start_x;
		back_pos_stay_y = dock_hei + card_hei / 2.0f + dock_back_dis_y;

		you_win_window = GameObject.Find ("you_win_window");
		you_win_window.SetActive (false);

		n_game_win = GameObject.Find ("new_game_window");
		n_game_win.SetActive (false);

		s1 = GameObject.Find ("Toggle_1");
		s2 = GameObject.Find ("Toggle_2");
		s3 = GameObject.Find ("Toggle_3");

		settings_window = GameObject.Find ("settings_window");
	    settings_window.SetActive (false);
	 
		dock_mc = GameObject.Find ("dock_sprite");
		complete_glow = GameObject.Find ("complete_glow");
		create_groups ();

		GameObject.Find ("dock_bg_img").transform.localScale = new Vector3 ((game_wid*100+2)/1334.0f,1.0f,1.0f);
		if (show_dock_start) {
			dock_visible = true;
			background_mc.GetComponent<BoxCollider>().center =  new Vector3(0.0f,dock_hei+dock_shadow_height,0.0f);
		} else {
			toggleDockVisibility ();
		}

		right_btns = GameObject.Find ("right_btns");
		right_btns.transform.position = new Vector3 (game_wid / 2.0f, right_btns.transform.position.y, right_btns.transform.position.z);
		settings_btn = GameObject.Find ("settings_btn");
		settings_btn.transform.position = new Vector3 (-game_wid / 2.0f+0.5f, settings_btn.transform.position.y, settings_btn.transform.position.z);

		hint_mc = GameObject.Find ("hint_cmc");
		hint_mc.transform.parent = game_mc.transform;

		undo_temp_mc = GameObject.Find ("undo_temp_mc");
		undo_temp_mc.transform.parent = game_mc.transform;
	}

	private GameObject undo_temp_mc;
	private GameObject s1, s2, s3;

	private ArrayList undoArr=new ArrayList();
	private bool can_undo=false;
	private bool isUnDoing=false;

	public void undo()
	{
		try
		{
			undoing();
		}
		finally 
		{
			isUnDoing = false;
		}
	}

	public void undoing(){
		//Debug.Log (undoArr.Count);
		if(undoArr.Count>0&&can_undo&&!isUnDoing&&!isRemoving){
			isUnDoing = true;
			if(isHintting){
				force_stop_hint_action ();
			}

			ArrayList childArr = undoArr[undoArr.Count-1] as ArrayList;

			int fromGroupIndex = -1;
			int toGroupIndex= -1;
			int count= -1;
			bool hasOpen=false;
			bool isComplete=false;
			string compType="";
			bool compTakeLastOpen=false;
			bool isDeal = false;

			GameObject group = null;
			GameObject targetGroup = null;
			ArrayList arr = null;
			ArrayList infoArrForUndomCompFunc = new ArrayList{arr,count,group,targetGroup,hasOpen};

			int n_999 = 999;
			if (((int)childArr [0])==n_999) {
				isDeal = true;
			} else {
				fromGroupIndex = (int)childArr [0];
				toGroupIndex = (int)childArr [1];
				count = (int)childArr [2];
				hasOpen = (bool)childArr [3];
				isComplete = (bool)childArr [4];
				compType = (string)childArr [5];
				if (childArr.Count > 6)
					compTakeLastOpen = (bool)childArr [6];

				group = groupObjectArr [fromGroupIndex] as GameObject;
				targetGroup = groupObjectArr [toGroupIndex] as GameObject;
				arr = group.GetComponent<Group> ().pokerArr;

				infoArrForUndomCompFunc = new ArrayList{arr,count,group,targetGroup,hasOpen};
			}

			if (isComplete) {
				//Debug.Log ("completedInfoArr.Count" + completedInfoArr.Count.ToString ());
				string card_type = completedInfoArr [completedInfoArr.Count - 1] as string;

				//Debug.Log (card_type);
				//GameObject firstAdded = null;

				if (compTakeLastOpen) {
					if (group.GetComponent<Group> ().getLastPoker () != null) {
						group.GetComponent<Group> ().getLastPoker ().GetComponent<Poker> ().close (null, 0);
					}
				}
				ArrayList cardToAddArr = new ArrayList ();
				for (int j = 13; j > 0; j--) {
					
					GameObject remake_card = GameObject.Instantiate (poker);

					remake_card.GetComponent<Poker> ().isInstaniated = true;
					//if (j == 13) {
					//	firstAdded = remake_card;
					//}

					cardsCreatedArr.Add (remake_card);

					remake_card.transform.FindChild ("glow").gameObject.SetActive (false);
					remake_card.transform.parent = game_mc.transform;
					remake_card.transform.localScale = new Vector3 (Game.card_scale, Game.card_scale, Game.card_scale);
					//Game.complete_glow.transform.position.y-Game.back_next_dis_normal/2.0f*Game.completedCount*Game.card_scale
					remake_card.transform.position = new Vector3 (complete_glow.transform.position.x, complete_glow.transform.position.y - Game.back_next_dis_normal / 2.0f * (Game.completedCount - 1) * Game.card_scale, complete_glow.transform.position.z);
					remake_card.GetComponent<Poker> ().isOpen = true;
					remake_card.GetComponent<Poker> ().num = j;
					remake_card.GetComponent<Poker> ()._type = compType;
					string str = j.ToString ();
					if (str.Length < 2) {
						str = "0" + str;
					}
					Sprite the_sprite = AssetsManager.getSpriteByFile ("images/cards/" + card_type + "00" + str);
					remake_card.GetComponent<Poker> ().transform.FindChild ("textureSprite").gameObject.GetComponent<SpriteRenderer> ().sprite = the_sprite;
					remake_card.GetComponent<Poker> ().transform.FindChild ("textureSprite").gameObject.GetComponent<SpriteRenderer> ().sortingOrder = 5000 + j;

					//GameObject last = null;
					//last = targetGroup.GetComponent<Group> ().getLastPoker ();
					GameObject _lastPoker = group.GetComponent<Group> ().getLastPoker ();

					float targetPosY;
					if (_lastPoker != null) {
						if (compTakeLastOpen) {
							targetPosY = group.transform.localPosition.y + _lastPoker.transform.localPosition.y - Game.back_next_dis * ((float)(14 - j)) * Game.card_scale;

						} else {
							targetPosY = group.transform.localPosition.y + _lastPoker.transform.localPosition.y - Game.back_next_dis_normal * ((float)(14 - j)) * Game.card_scale;
						}
					} else {
						targetPosY = group.transform.localPosition.y;
					}

					//Poker p = remake_card.GetComponent<Poker>();

					cardToAddArr.Add (remake_card);
					TweenParms parms = new TweenParms ();
					parms.Prop ("localPosition", new Vector3 (group.transform.localPosition.x, targetPosY, group.transform.localPosition.z - z_space)); 
					 
					parms.Ease (EaseType.Linear); 
					parms.Delay (0.05f * (float)(13 - j));
					parms.OnComplete (onUndoCompComp, remake_card, (14 - j), infoArrForUndomCompFunc, cardToAddArr);
					parms.OnStart (onUndoCompStart, remake_card, (14 - j), infoArrForUndomCompFunc, cardToAddArr);
					HOTween.To (remake_card.transform, 0.15f, parms);
				}
				can_undo = false;
			} else if (isDeal) {
				for (int q = 0; q < 10; q++) {
					GameObject gp = groupObjectArr [q] as GameObject;
					gp.GetComponent<Group> ().removeLastPoker ();
				}

				can_undo = true;
				//isUnDoing = false;
				 
				if(undoArr.Count>0){
					undoArr.RemoveAt (undoArr.Count-1);
				}

				deal_limit_num++;
				for (int i = 0; i < backArr.Count; i++) {
					if (i > (4-deal_limit_num)) {
						(backArr [i] as GameObject).SetActive (true);
					} else {
						(backArr [i] as GameObject).SetActive (false);
					}
				}
				 
				deal_num -= 10;
				if(deal_num<54){
					deal_num = 54;
				}
			} else {
				can_undo = false;
				func (arr, count, group, targetGroup, hasOpen);
			}
			//isUnDoing = false;
		}
	}

	void func(ArrayList arr,int count,GameObject group,GameObject targetGroup,bool hasOpen){
		GameObject firstCardTomove = arr[arr.Count-count] as GameObject;
		undo_temp_mc.transform.localPosition = new Vector3 (group.transform.localPosition.x,group.transform.localPosition.y+firstCardTomove.transform.localPosition.y,group.transform.localPosition.z-z_space);
		ArrayList removeArr = new ArrayList ();
		for (int i = arr.Count-count; i < arr.Count; i++) {
			GameObject moveCard = arr [i] as GameObject;
			moveCard.transform.parent = undo_temp_mc.transform;
			removeArr.Add (moveCard);
		}
		int remove_count = count;
		while(remove_count>0){
			group.GetComponent<Group> ().pokerArr.RemoveAt (group.GetComponent<Group> ().pokerArr.Count-1);
			remove_count--;
		}

		GameObject last = null;
		last = targetGroup.GetComponent<Group> ().getLastPoker ();

		float val;
		if (last != null) {
			if (hasOpen) {
				val = last.transform.localPosition.y - back_next_dis * Game.card_scale;
			} else {
				val = last.transform.localPosition.y - back_next_dis_normal * Game.card_scale;
			}
		} else {
			val = 0.0f;
		}

		if(hasOpen){
			targetGroup.GetComponent<Group> ().getLastPoker ().GetComponent<Poker> ().close(null,0);
		}

		TweenParms parms = new TweenParms();
		parms.Prop("localPosition", new Vector3(targetGroup.transform.localPosition.x,targetGroup.transform.localPosition.y+val,targetGroup.transform.localPosition.z-z_space)); 
		parms.Ease(EaseType.EaseInOutSine); 
		parms.Delay (0f);
		parms.OnComplete (undoComp, targetGroup,removeArr);
		HOTween.To(undo_temp_mc.transform, 0.25f, parms);

	}

	void onUndoCompComp(TweenEvent e){
		if (e.parms != null) {
			int id = (int)e.parms [1];
			if(id==13){
				ArrayList infoArr = e.parms [2] as ArrayList;
				GameObject group = infoArr [2] as GameObject;
				group.GetComponent<Group> ().addPokers ( e.parms [3] as ArrayList);
				
				func (infoArr[0] as ArrayList,(int)infoArr[1],infoArr[2] as GameObject,infoArr[3] as GameObject,(bool)infoArr[4]);

				//can_undo = true;
			}
		}
	}

	void onUndoCompStart(TweenEvent e){
		if (e.parms != null) {
			if(((int)e.parms[1]) == 13){
				GameObject.Destroy (Game.completeBigK_array[Game.completeBigK_array.Count-1] as GameObject);
				Game.completeBigK_array.RemoveAt(Game.completeBigK_array.Count-1);
				Game.completedCount--;
			}
			GameObject go = e.parms [0] as GameObject;
			go.transform.FindChild ("textureSprite").gameObject.GetComponent<SpriteRenderer> ().sortingOrder = 5000 + ((int)e.parms [1]);
		}
	}

	//func (e.parms [2] as ArrayList,(int)e.parms [3],e.parms [4] as GameObject,e.parms [5] as GameObject,(bool)e.parms [6]);
	private void undoComp(TweenEvent e){
		if (e.parms != null) {
			ArrayList arr = e.parms [1] as ArrayList;
			GameObject go = e.parms [0] as GameObject;
			for(int i = 0;i<arr.Count;i++){
				GameObject card = arr [i] as GameObject;
				card.transform.parent = go.transform;
			}

			go.GetComponent<Group> ().addPokers (arr);

			undoArr.RemoveAt (undoArr.Count-1);

			//isUnDoing = false;
			can_undo = true;
		}
	}

	private int deal_limit_num = 5;
	private ArrayList hintArr = null;

	private int currentHintIndex;
	private int hintLength;

	private GameObject hint_mc;

	private ArrayList allHintCardCopyArray=new ArrayList();
 
	private bool isHintting=false;

	public static ArrayList tweensToKillArr;
	public void start_show_hint(){
		if(isUnDoing){
			return;
		}
		if(tweensToKillArr==null){
			tweensToKillArr = new ArrayList ();
		}
		force_stop_hint_action ();

		if (currentHintIndex == -1) {
			currentHintIndex = 0;
		} else {
			currentHintIndex++;
		}

		hintLength = hintArr.Count;
		if(hintArr.Count>0&&currentHintIndex<hintArr.Count){
			isHintting = true;
			int grpIndex = (int)((ArrayList)(hintArr[currentHintIndex]))[0];
			ArrayList movelist = (ArrayList)((ArrayList)(hintArr[currentHintIndex]))[1];
			Vector3 targetPos = (Vector3)((ArrayList)(hintArr[currentHintIndex]))[2];
			int targetGroupIndex = (int) ((ArrayList)(hintArr[currentHintIndex]))[3];

			GameObject currentGroup = groupObjectArr [grpIndex] as GameObject;
			GameObject targetGroup = groupObjectArr [targetGroupIndex] as GameObject;

			//Debug.Log ("grpIndex"+grpIndex.ToString());
			//Debug.Log ("targetGroupIndex:"+targetGroupIndex.ToString());
			for(int i=0;i<movelist.Count;i++){

				GameObject cardToCopy = movelist [i] as GameObject;

				if(i==0){
	             	hint_mc.transform.localPosition= new Vector3 (currentGroup.transform.localPosition.x,currentGroup.transform.localPosition.y+cardToCopy.transform.localPosition.y,currentGroup.transform.localPosition.z-z_space);
				}
				 
				GameObject copyCrad = GameObject.Instantiate (cardToCopy);

				copyCrad.transform.FindChild ("textureSprite").GetComponent<SpriteRenderer> ().sortingOrder = 2000 + i;
				//copyCrad.transform.FindChild ("glow").GetComponent<SpriteRenderer> ().sortingOrder = 2000 + i;
				copyCrad.transform.parent = hint_mc.transform;
				copyCrad.transform.localPosition = new Vector3(0.0f,-back_next_dis_normal * Game.card_scale*((float)i),-z_space*i);

				copyCrad.transform.FindChild ("glow").gameObject.SetActive (true);

				allHintCardCopyArray.Add (copyCrad);

			}

			TweenParms parms = new TweenParms();
			parms.Prop("localPosition", new Vector3(targetGroup.transform.localPosition.x,targetGroup.transform.localPosition.y+targetPos.y-back_next_dis_normal*Game.card_scale,targetGroup.transform.localPosition.z-z_space)); 
			parms.Ease(EaseType.EaseInOutSine); 
			parms.Delay (0f);
			parms.OnComplete (hint1StepCompleted, currentHintIndex);
			tweensToKillArr.Add(HOTween.To(hint_mc.transform, 0.5f, parms));
		}
	}

	private void hint1StepCompleted(TweenEvent e){
		for(int i=0;i<allHintCardCopyArray.Count;i++){
			GameObject go = allHintCardCopyArray [i] as GameObject;
			go.GetComponent<Poker> ().fadeOutAlphaAndDistroy ();
		}

		TweenParms parms = new TweenParms();
		parms.Prop("localPosition", Vector3.zero); 
		parms.Ease(EaseType.Linear); 
		parms.Delay (0.5f);
		parms.OnComplete (delayComp,currentHintIndex);
		tweensToKillArr.Add(HOTween.To(GameObject.Find("delay_mc").transform, 0.5f, parms));
	}

	private void delayComp(TweenEvent e){
		if (currentHintIndex < hintLength) {
			start_show_hint ();

		} else {
			isHintting = false;
		}
	}

	public void hint(){
		if(!isRemoving){
			currentHintIndex = -1;
			hintArr = getHintArray ();
			start_show_hint ();
		}
	}

	public void force_stop_hint_action(){
		while(tweensToKillArr!=null&&tweensToKillArr.Count>0){
			Tweener t = (Tweener)tweensToKillArr [0];
			if(t!=null){
				t.Pause ();
				t.Kill ();
			}
			tweensToKillArr.RemoveAt (0);
			isHintting = false;
		}

		while(allHintCardCopyArray!=null&&allHintCardCopyArray.Count>0){
			GameObject goToRemoe = allHintCardCopyArray [0] as GameObject;
			if(goToRemoe!=null){
				GameObject.Destroy (goToRemoe);
			}
			allHintCardCopyArray.RemoveAt (0);
		}
	}

	private ArrayList getHintArray(){
		ArrayList __hintArr = new ArrayList();

		for (int i = 0; i < 10; i++) {
			GameObject group = groupObjectArr [i] as GameObject;
			int moveAbleIndex = group.GetComponent<Group> ().getLastMoveAbleIndex ();
			ArrayList moveList = group.GetComponent<Group> ().getLastMoveAbleArray ();
			if(moveAbleIndex!=-1){
				GameObject firstCardOfCradstoMove = moveList[0] as GameObject;
				for (int k = 0; k < groupObjectArr.Count; k++) {
					if(i!=k){
						GameObject group2 = groupObjectArr [k] as GameObject;
						//int moveAbleIndex_2 = group2.GetComponent<Group> ().getLastMoveAbleIndex ();
						GameObject theLast =  group2.GetComponent<Group> ().getLastPoker ();
						if(theLast!=null){

							if(firstCardOfCradstoMove.GetComponent<Poker>().num==theLast.GetComponent<Poker>().num-1){

								ArrayList arr = new ArrayList ();
								arr.Add (i);
								arr.Add (moveList);
								arr.Add (theLast.transform.localPosition);
								arr.Add (k);
							    __hintArr.Add (arr);

//								Debug.Log (i);
//								Debug.Log (moveList.Count);
//								Debug.Log (theLast.GetComponent<Poker>().num);
							}
						}
					}
				}
			}
		}
		return __hintArr;
	}

	GameObject settings_window;
	public void replay(){
		if (rc_array_104!=null&&rc_array_104.Count > 1) {
			create_game (true);
		} else {
			create_game ();
		}
		n_game_win.SetActive (false);
	}

	public void create_game(bool replay=false){
		deal_limit_num = 5;
		replay_btn.SetActive (true);
		undoArr = new ArrayList ();
		try_clear_game ();
		create_back ();
		if(!replay){
			rc_array_104 = getCardsArray (level);
		}
		create_first_set_cards ();
	}

	public void button_random_click(){
		if(!isRemoving){
			force_stop_hint_action ();
			create_game ();
			n_game_win.SetActive (false);
		}
	}

	public void button_cancel_click(){
		if (!isRemoving) {
			force_stop_hint_action ();
			n_game_win.SetActive (false);
		}
	}

	public void button_winning_click(){
		if (!isRemoving) {
			force_stop_hint_action ();
			create_game ();
			n_game_win.SetActive (false);
		}
	}

	public void button_replay_click(){
		if (!isRemoving) {
			force_stop_hint_action ();
			create_game ();
			n_game_win.SetActive (false);
		}
	}

	public void show_new_game_window(){
		force_stop_hint_action ();
		n_game_win.SetActive (true);
	}

	public void show_settings_window(){
		force_stop_hint_action ();
		settings_visible = true;
	    settings_window.SetActive (true);
	}

	public void hide_settings_window(){
		if (!isRemoving) {
			force_stop_hint_action ();
			settings_visible = false;
			settings_window.SetActive (false);
		}
	}

	public void hide_you_win_window(){
		if (!isRemoving) {
			you_win_window.SetActive (false);
			show_new_game_window ();
		}
	}

	public void suitsClick1(){
		if(s1.GetComponent<Toggle> ().isOn){
			level = 1;
			//s1.GetComponent<Toggle> ().isOn = true;
			s2.GetComponent<Toggle> ().isOn = false;
			s3.GetComponent<Toggle> ().isOn = false;
		}
	}

	public void suitsClick2(){
		if (s2.GetComponent<Toggle> ().isOn) {
			level = 2;
			s1.GetComponent<Toggle> ().isOn = false;
			//s2.GetComponent<Toggle> ().isOn = false;
			s3.GetComponent<Toggle> ().isOn = false;
		}
	}

	public void suitsClick3(){
		if(s3.GetComponent<Toggle> ().isOn){
			level = 3;
			s1.GetComponent<Toggle> ().isOn = false;
			s2.GetComponent<Toggle> ().isOn = false;
			//s3.GetComponent<Toggle> ().isOn = false;
		}
	}

	public void toggleDockVisibility(){
		float dock_full_hei = dock_hei / 2.0f * 100.0f + dock_shadow_height / 2.0f * 100;
		TweenParms parms = new TweenParms();
		parms.Ease(EaseType.Linear); // Easing type
		if (!dock_visible) {
			background_mc.GetComponent<BoxCollider>().center =  new Vector3(0.0f,1.20f,0.0f);
		    parms.OnComplete(dockFadeComplete,0);
		} else {
			background_mc.GetComponent<BoxCollider>().center =  new Vector3(0.0f,0.0f,0.0f);
			dock_full_hei *= -1;
			parms.OnStart(dockFadeStart,0);
		}
		parms.Prop("localPosition", new Vector3(0.0f,(-750.0f/2.0f+dock_full_hei),dock_mc.transform.localPosition.z)); // Position tween
		HOTween.To(dock_mc.transform, 0.2f, parms);
		dock_visible = !dock_visible;
	}

	void dockFadeComplete(TweenEvent e){
		//dock_mc.SetActive (dock_visible);
	}

	void dockFadeStart(TweenEvent e){
		//dock_mc.SetActive (dock_visible);
	}

	private void create_groups(){
		float _x=-1, _y=-1;
		groupObjectArr = new ArrayList ();
		for (int i = 0; i < 10; i++) {
			GameObject group = Instantiate (GameObject.Find ("group"));
			group.transform.parent = game_mc.transform;
			group.transform.FindChild("empty_glow").gameObject.transform.localScale = Vector3.one * Game.card_scale;
			group.name = "group_" + i;
			group.GetComponent<Group> ().row = i;
			_x = start_x + (card_wid / 2.0f) + (gap+card_wid)*((float)i);
			_y = hei_750 - start_y - card_hei/2.0f;
			group.transform.localPosition = new Vector3 (_x, _y,0.0f);
			group.GetComponent<Group> ().stayPos = new Vector3 (group.transform.localPosition.x, group.transform.localPosition.y, group.transform.localPosition.z);
			groupObjectArr.Add (group);
		}

		complete_glow.transform.parent = game_mc.transform;
		Vector3 p = new Vector3 (_x+gap+Game.card_wid+Game.card_wid/2.0f,_y,0.0f);
		complete_glow.transform.localPosition = p;
		complete_glow.transform.localScale = Vector3.one * Game.card_scale;
	}

	public static GameObject complete_glow;
	public static ArrayList completeBigK_array = new ArrayList ();

	private void  try_clear_game(){
		memoryGroup = null;
		can_deal = false;
		while(cardsCreatedArr!=null&&cardsCreatedArr.Count>0){
			GameObject go = cardsCreatedArr [0] as GameObject;
			GameObject.Destroy (go);
			cardsCreatedArr.RemoveAt (0);
		}

		while(backArr.Count>0){
			GameObject go = backArr [0] as GameObject;
			GameObject.Destroy (go);
			backArr.RemoveAt (0);
		}

		if(groupObjectArr!=null){
			for (int i = 0; i < groupObjectArr.Count; i++) {
				GameObject grp = groupObjectArr [i] as GameObject;
				grp.GetComponent<Group> ().pokerArr = new ArrayList ();
			}
		}
	}

	ArrayList backArr = new ArrayList ();
	float dock_back_dis_y = 12.0f / 100.0f;
	float dock_hei = 110.0f/100.0f;
	float back_pos_stay_x = 0.0f;
	float back_pos_stay_y = 0.0f;
	float back_stay_dis = 20.0f / 100.0f;
	float game_cen_x;
	 
	public static float z_space = 10.0f / 100.0f;
	GameObject backSample;

	private void create_back(){
		backArr = new ArrayList ();
		for (int i = 0; i < 5; i++) {
			GameObject back_mc = Instantiate (poker);
			 
			backSample = back_mc;

			back_mc.transform.parent = back_cards_container.transform;

			back_mc.transform.localScale = new Vector3 (Game.card_scale,Game.card_scale,Game.card_scale);
			game_cen_x = game_wid / 2.0f;
			back_mc.transform.localPosition = new Vector2 (game_cen_x,back_pos_stay_y);
			back_mc.transform.FindChild ("textureSprite").gameObject.GetComponent<SpriteRenderer>().sortingOrder = 100-i;
			//back_mc.SetActive (false);
			back_mc.transform.FindChild ("glow").gameObject.SetActive (false);
			backArr.Add(back_mc);
		}
	}

	private void back_go_stay_pos(){
		for (int i = 0; i < backArr.Count; i++) {
			GameObject go = backArr[i] as GameObject;
			TweenParms parms = new TweenParms();
			parms.Prop("localPosition", new Vector3(back_pos_stay_x,back_pos_stay_y+((float)i)*back_stay_dis* Game.card_scale,go.transform.localPosition.z)); // Position tween
			parms.Ease(EaseType.EaseInOutSine); // Easing type
			parms.Delay(((float)(i))*0.018f); // Initial delay
			 
			parms.OnComplete(backFadeComp,go,i);
			HOTween.To(go.transform, 0.3f, parms);
		}
	}

	private void backFadeComp(TweenEvent e){
		if((int)(e.parms[1])==4){
			for (int i = 0; i < groupObjectArr.Count; i++) {
				GameObject group = groupObjectArr [i] as GameObject;
				GameObject pokerToOpen = group.GetComponent<Group> ().getLastPoker ();
				float time = 0.07f*((float)i);
				if(i<groupObjectArr.Count-1){
					pokerToOpen.GetComponent<Poker> ().open (null, time);
				}
				else{
					pokerToOpen.GetComponent<Poker> ().open (funcAfterOpen, time);
				}
			}
		}
	}

	private void funcAfterOpen(GameObject theGo){
		can_deal = true;
	}

	bool can_deal = false;
	public static float back_next_dis = 14.0f/100.0f;
	public static float back_next_dis_normal = 48.0f/100.0f;
	GameObject memoryGroup=null;

	int deal_num;
	private void create_first_set_cards(){
		int num = 0;
		for (int i = 0; i < 6; i++) {
			for (int j = 0; j < 10; j++) {
				if(i==5&&j==4){
					break;
				}
			
				GameObject card = Instantiate (poker);
				card.transform.localScale = Vector3.one *card_scale;
				cardsCreatedArr.Add (card);
				 
				card.GetComponent<Poker> ().row = j;
				card.GetComponent<Poker> ().column = i;

				card.name = "card_" + num.ToString ();
				GameObject group = (GameObject)(groupObjectArr [j]);
				group.GetComponent<Group> ().pokerArr.Add (card);
				card.transform.parent = group.transform;

				card.GetComponent<Poker> ()._type = ((rc_array_104 [num] as ArrayList) [1]).ToString();
				card.GetComponent<Poker> ().num = (int)((rc_array_104 [num] as ArrayList) [0]);
				//Debug.Log (num.ToString()+":"+card.GetComponent<Poker> ().num.ToString());
				card.GetComponent<Poker> ().g_index = num;
				int id = card.GetComponent<Poker> ().num;
				card.GetComponent<Poker> ().isInstaniated = true;
				card.SetActive (false);
				card.transform.FindChild ("glow").gameObject.SetActive (false);
				card.transform.position =new Vector3 (backSample.transform.position.x,backSample.transform.position.y,-z_space*((float)i));
				card.transform.FindChild ("textureSprite").gameObject.GetComponent<SpriteRenderer>().sortingOrder = 100+i;
				string str = id.ToString();

				if(str.Length<2){
					str = "0" + str;
				}
				card.GetComponent<Poker> ().image_path = "images/cards/"+(string)(card.GetComponent<Poker> ()._type)+"00"+str;

				// C#
				TweenParms parms = new TweenParms();
				//group.GetComponent<Group> ().stayPos = new Vector3 (0.0f, -((float)i) * back_next_dis* Game.card_scale, card.transform.localPosition.z);

				parms.Prop("localPosition", new Vector3 (0.0f, -((float)i) * back_next_dis* Game.card_scale, card.transform.localPosition.z)); // Position tween
				parms.Ease(EaseType.EaseOutSine); // Easing type
				parms.Delay(((float)num)*0.02f+0.0f); // Initial delay
				parms.OnStart(onStart,card,((bool)(i==9)));
				parms.OnComplete(fadeCompleted,card);
				HOTween.To(card.transform, 0.15f, parms);

				num++;
			}
		}

		deal_num = num;
		//Debug.Log (num);
		///Debug.Log (deal_num);
	}

	private void onStart(TweenEvent e){
		//Debug.Log( e.tween ); // outputs the tweener or sequence that invoked this function
		if ( e.parms != null ) {
			//back_cards_container.SetActive (false);

			(e.parms [0] as GameObject).SetActive (true);
			if ((bool)(e.parms [1]) == true) {
				deal_limit_num--;
				for (int i = 0; i < backArr.Count; i++) {
					if (i > (4-deal_limit_num)) {
						(backArr [i] as GameObject).SetActive (true);
					} else {
						(backArr [i] as GameObject).SetActive (false);
					}
				}
				//backArr.RemoveAt (0);
			}
		}
	}
	 
	private void fadeCompleted( TweenEvent e ) {
		if ( e.parms != null ) {
			GameObject go = e.parms [0] as GameObject;
			if(go.GetComponent<Poker> ().g_index==53){
				back_go_stay_pos ();
			}
		}
	}

	private ArrayList getCardsArray(int _level=1){
		if(test){
			return testData;
		}
		ArrayList allArr = new ArrayList ();

		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 13; j++) {

				ArrayList arr = new ArrayList ();
				arr.Add (j+1);

				if(_level==1){
					arr.Add ("Spade");
				}
				else if(_level==2){
					if (i <= 3) {
						arr.Add ("Spade");
					} else {
						arr.Add ("Heart");
					}
					 
				}
				 
				else if(_level==3){
					if(i==0||i==1){
						arr.Add ("Spade");
					}
					if(i==2||i==3){
						arr.Add ("Heart");
					}
					if(i==4||i==5){
						arr.Add ("Club");
					}
					if(i==6||i==7){
						arr.Add ("Diamond");
					}
				}

				allArr.Add (arr);
			}
		}

		ArrayList gameArr = new ArrayList ();

		int length = allArr.Count;
		for (int k = 0; k < length; k++) {
			int r_n = (int)Random.Range (0,allArr.Count);
			gameArr.Add (allArr[r_n]);
			allArr.RemoveAt (r_n);
		}
	 
		return gameArr;
	}

	public void deal(){
		force_stop_hint_action ();
		if(!can_deal){
			return;
		}

		can_undo = false;
		ArrayList undoChildArr = new ArrayList ();
		int n_999 = 999;
		undoChildArr.Add (n_999);
		ArrayList pArr = new ArrayList ();
		can_deal = false;

		for (int i = 0; i < 10; i++) {
			GameObject card = Instantiate (poker);
			card.transform.localScale = Vector3.one *card_scale;

			cardsCreatedArr.Add (card);

			card.name = "card_" + deal_num.ToString ();
			GameObject group = (GameObject)(groupObjectArr [i]);

			card.GetComponent<Poker> ().row = i;
			card.GetComponent<Poker> ().column = group.GetComponent<Group> ().pokerArr.Count;
			 
			card.transform.parent =group.transform;
			//Debug.Log (rc_array_104.IndexOf(0));

			string ___type = ((rc_array_104 [deal_num] as ArrayList) [1]).ToString();
			int ___num = (int)((rc_array_104 [deal_num] as ArrayList) [0]);
			card.GetComponent<Poker> ()._type = ___type;
			card.GetComponent<Poker> ().num =___num;
			pArr.Add (new ArrayList{___type,___num});

			card.GetComponent<Poker> ().g_index = deal_num;
			int id = card.GetComponent<Poker> ().num;
			card.GetComponent<Poker> ().isInstaniated = true;

			card.SetActive (false);
			card.transform.FindChild ("glow").gameObject.SetActive (false);
			card.transform.position =new Vector3 (backSample.transform.position.x,backSample.transform.position.y,-z_space*((float)i));
			string str = id.ToString();

			if(str.Length<2){
				str = "0" + str;
			}
			card.GetComponent<Poker> ().image_path = "images/cards/"+(string)(card.GetComponent<Poker> ()._type)+"00"+str;

			GameObject last = null;
			if (group.GetComponent<Group> ().getLastPoker ()) {
				last = group.GetComponent<Group> ().getLastPoker ();
			}
			Vector3 pos;
			if (last != null) {
				card.transform.FindChild ("textureSprite").gameObject.GetComponent<SpriteRenderer>().sortingOrder = 100+group.GetComponent<Group>().pokerArr.Count;
				if (last.GetComponent<Poker> ().isOpen) {
					pos = new Vector3 (0.0f, last.transform.localPosition.y - back_next_dis_normal*card_scale, last.transform.localPosition.z - z_space);
				} else {
					pos = new Vector3 (0.0f, last.transform.localPosition.y - back_next_dis*card_scale, last.transform.localPosition.z - z_space);
				}
			} else {
				//Debug.Log ("No poker on the group");
				pos = new Vector3 (0.0f, 0.0f, 0.0f);
			}
			Vector3 pos2 = new Vector3 (pos.x,pos.y,pos.z);

			// C#
			TweenParms parms = new TweenParms();
			//group.GetComponent<Group> ().stayPos = pos;

			parms.Prop ("localPosition", pos2); // Position tween
			parms.Ease(EaseType.Linear); // Easing type
			parms.Delay(((float)i)*0.02f); // Initial delay
			parms.OnStart(onStart,card,((bool)(i==9)));
			parms.OnComplete(fadeCompleted_deal,((bool)(i==9)));
		
			HOTween.To(card.transform, 0.15f, parms);
			group.GetComponent<Group> ().pokerArr.Add (card);
			deal_num++;
		}
		undoChildArr.Add (pArr);
		undoArr.Add (undoChildArr);
	}

	private void lastDealStart(TweenEvent e){
		if (e.parms != null) {
		}
	}

	private void fadeCompleted_deal( TweenEvent e ) {
		if (e.parms != null) {
			if((bool)(e.parms[0])==true){
				for (int i = 0; i < groupObjectArr.Count; i++) {
					GameObject group = groupObjectArr [i] as GameObject;
					GameObject pokerToOpen = group.GetComponent<Group> ().getLastPoker ();
					float time = 0.07f*((float)i);
					if(i<groupObjectArr.Count-1){
						pokerToOpen.GetComponent<Poker> ().open (null, time);
					}
					else{
						pokerToOpen.GetComponent<Poker> ().open (funcAfterOpen_deal, time);
					}
				}
			}
		}
	}

	bool dock_visible;
	private void funcAfterOpen_deal(GameObject theGo){
		can_deal = true;
		can_undo = true;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			{
				if (hit.collider !=null)
				{
					//Debug.Log (Input.mousePosition);
					//Debug.Log (hit.collider.gameObject.GetComponent<Poker>());
					GameObject hitObject = hit.collider.gameObject;

					if(hitObject.name == "background"){
						//Debug.Log ("sssss");
						if(!GameObject.Find("new_game_window")&&!GameObject.Find("you_win_window")&&settings_visible==false){
							toggleDockVisibility ();
							force_stop_hint_action ();
						}
						//Debug.Log (hitObject.name);
					}
					Poker poker = hitObject.GetComponent<Poker> ();
					if(poker&&!poker.isInstaniated&&!isUnDoing){
						deal ();
					}
					if(poker&&poker.isInstaniated
						&&poker.isOpen&&!isUnDoing){

						force_stop_hint_action ();

						GameObject card = hit.collider.gameObject;
						card.GetComponent<Poker> ().isDown = true;
						GameObject group = groupObjectArr[card.GetComponent<Poker>().row] as GameObject;
						if(memoryGroup!=null){
							memoryGroup.GetComponent<Group>().stayOriPos ();
						}
						memoryGroup = group;
						dargStartIndex = card.GetComponent<Poker> ().column;
						//prevDragIndex = group.GetComponent<Group> ().row;
												 
						if (group.GetComponent<Group> ().isDragAbleAtIndex (dargStartIndex)) {
							Vector3 mp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
							offset = new Vector2 (mp.x - card.transform.position.x, mp.y - card.transform.position.y);

							group.transform.localPosition = new Vector3 (group.transform.localPosition.x, group.transform.localPosition.y, -0.5f);
							group.GetComponent<Group> ().goNextHighestDepth ();

							dragList = group.GetComponent<Group> ().getMoveCardsArray (dargStartIndex);
							//if (dragList.Count > 0) {
								//GameObject fGo = dragList [0] as GameObject;
								//memoryPosYOfFirstMoveCard = fGo.transform.localPosition.y;
								//memoryZOfFirstMoveCard = fGo.transform.localPosition.z;
							//}
							if (dragList.Count > 0) {
								isDrag = true;
							}
						} 
						else {
							isDrag = false;
						}
					}
				}
			}
		}
		//GameObject firstCard=null;

		if (Input.GetMouseButton(0)) {
			if(isDrag){
				//if(dragList.Count>0){
				//	firstCard = dragList [0] as GameObject;
				//}
				for (int k = 0; k < dragList.Count; k++) {
					GameObject targetGo = dragList [k] as GameObject;

					Vector3 p = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					targetGo.transform.position = new Vector3(p.x - offset.x,p.y-offset.y-back_next_dis_normal*((float)k)*Game.card_scale,targetGo.transform.position.z);
				}
			}
		}

		if (Input.GetMouseButtonUp (0)&&!isUnDoing&&!isHintting) {
			minDis = 99999999.9f;
			if (dragList.Count > 0&&isDrag){
				GameObject firstCrad = null;
				closedIndex = -1;
				if (dragList.Count > 0) {
					firstCrad = dragList [0] as GameObject;
					GameObject grp;
					for (int i = 0; i < groupObjectArr.Count; i++) {
						grp = groupObjectArr [i] as GameObject;
						float abs = Mathf.Abs (firstCrad.transform.position.x - grp.transform.position.x);
						if (minDis > abs) {
							minDis = abs;
							closedIndex = i;
						}
					}

					if (closedIndex != -1) {
						bool hasOpen = false;
						grp = groupObjectArr [closedIndex] as GameObject;
						GameObject last = grp.GetComponent<Group> ().getLastPoker();
						if (memoryGroup.GetComponent<Group> ().row != grp.GetComponent<Group> ().row
							&&(last==null||(last!=null&&last.GetComponent<Poker>().num==firstCrad.GetComponent<Poker>().num+1))) {
							while (memoryGroup.GetComponent<Group> ().pokerArr.Count > dargStartIndex) {
								memoryGroup.GetComponent<Group> ().pokerArr.RemoveAt (memoryGroup.GetComponent<Group> ().pokerArr.Count - 1);
							}
							grp.GetComponent<Group> ().addPokers (dragList);

							if (memoryGroup.GetComponent<Group> ().getLastPoker () != null) {
								if(!memoryGroup.GetComponent<Group> ().getLastPoker ().GetComponent<Poker>().isOpen){
									hasOpen = true;
								}
								memoryGroup.GetComponent<Group> ().getLastPoker ().GetComponent<Poker> ().open (null, 0);
							}

							ArrayList undoChildArr = new ArrayList ();
							ArrayList cardIDList = new ArrayList ();
							ArrayList cardTypeList = new ArrayList ();
							for (int p = 0; p < dragList.Count; p++) {
								GameObject _c = dragList [p] as GameObject;
								cardIDList.Add (_c.GetComponent<Poker>().num);
								cardTypeList.Add (_c.GetComponent<Poker>().num);
							}

							undoChildArr.Add (closedIndex);//from
							undoChildArr.Add (memoryGroup.GetComponent<Group>().row);//to
							undoChildArr.Add(dragList.Count);
							undoChildArr.Add (hasOpen);
							bool hasComplete = grp.GetComponent<Group> ().tryRemoveCompletedCards ();

							if(hasComplete){
								Game.isRemoving = true;
							}

							completedInfoArr.Add (((GameObject)dragList[0]).GetComponent<Poker>()._type);

							//Debug.Log (hasComplete);

							undoChildArr.Add (hasComplete);
							string completeType = firstCrad.GetComponent<Poker> ()._type;
							undoChildArr.Add (completeType);

							if (hasComplete) {

								GameObject newLastAfterRemove = grp.GetComponent<Group> ().getLastPoker ();
								if (newLastAfterRemove != null) {
									undoChildArr.Add (!newLastAfterRemove.GetComponent<Poker> ().isOpen);
								}
							} else {
								undoChildArr.Add (false);
							}
							undoArr.Add (undoChildArr);
							can_undo = true;
						} else {
							memoryGroup.GetComponent<Group> ().restore ();
						}
						memoryGroup.GetComponent<Group> ().stayOriPos ();
					}
				}
			}
			
			isDrag = false;
			while(dragList.Count>0){
				dragList.RemoveAt (0);
			}
		}
	}

	void Awake () {
		 //if you want to let spider works on 60fps , just descoment this two lines of code.
		 QualitySettings.vSyncCount = 0; 
		 Application.targetFrameRate = 60;
	}
}
