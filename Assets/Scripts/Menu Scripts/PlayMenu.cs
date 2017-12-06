using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayMenu : MonoBehaviour {

	public GameObject continuePanelLocked;
	public Button continueGoButton;
	public AnimationClip pointerEnterClip;
	public AnimationClip pointerLeaveClip;

	public void MenuSelected(){
		if(GameManager.instance.currentGameLevel == 1){
			continuePanelLocked.SetActive(true);
		} else {
			continuePanelLocked.SetActive(false);
		}
	}

	public void NewGame(){
		GameManager.instance.SetupDefaults();
		continueGoButton.onClick.Invoke();
	}

	public void MouseOverLevel(GameObject caller){
		if(caller.transform.Find("Locked Panel") != null){
			if(caller.transform.Find("Locked Panel").gameObject.activeSelf){
				return;
			}
		}

		caller.GetComponent<Animation>().clip = pointerEnterClip;
		caller.GetComponent<Animation>().Play();
	}

	public void MouseLeaveLevel(GameObject caller){
		if(caller.transform.Find("Locked Panel") != null){
			if(caller.transform.Find("Locked Panel").gameObject.activeSelf){
				return;
			}
		}

		caller.GetComponent<Animation>().clip = pointerLeaveClip;
		caller.GetComponent<Animation>().Play();
	}

	public void InvokeClick(Button caller){
		caller.transform.localScale = new Vector3(1, 1, 1);
		caller.onClick.Invoke();
	}
}
