using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelectMenu : MonoBehaviour {
	
	public ScrollRect levelsScroll;
	public AnimationClip pointerEnterClip;
	public AnimationClip pointerLeaveClip;
	public GameObject levelsPanel;
	public GameObject[] levels;
	public GameObject[] levelsLocked;

	public void MenuSelected(){

		foreach(GameObject lvl in levelsLocked){
			lvl.SetActive(true);
		}

		if(GameManager.instance.currentGameLevel > 1){
			for(int i = 0; i < GameManager.instance.currentGameLevel; i++){
				levelsLocked[i].SetActive(false);
			}
		} else {
			levelsLocked[0].SetActive(false);
		}

		levels[GameManager.instance.currentGameLevel-1].GetComponent<Animation>().Play();
		levels[GameManager.instance.currentGameLevel-1].transform.localScale = new Vector3(1.1f, 1.1f, 1f);
	}

	public void MouseOverLevel(GameObject caller){
		if(!caller.transform.Find("Locked Panel").gameObject.activeSelf && !caller.GetComponent<Animation>().IsPlaying("TextSwing")){
			caller.GetComponent<Animation>().clip = pointerEnterClip;
			caller.GetComponent<Animation>().Play();
		}
	}

	public void MouseLeaveLevel(GameObject caller){
		if(!caller.transform.Find("Locked Panel").gameObject.activeSelf && !caller.GetComponent<Animation>().IsPlaying("TextSwing")){
			caller.GetComponent<Animation>().clip = pointerLeaveClip;
			caller.GetComponent<Animation>().Play();
		}
	}

	public void InvokeClick(Button caller){
		caller.transform.localScale = new Vector3(1, 1, 1);
		caller.onClick.Invoke();
	}
}
