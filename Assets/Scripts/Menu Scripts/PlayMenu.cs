using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayMenu : MonoBehaviour {

	public AnimationClip pointerEnterClip;
	public AnimationClip pointerLeaveClip;
	public Button continueGoButton;

	public void NewGame(){
		GameManager.instance.SetupDefaults();
		continueGoButton.onClick.Invoke();
	}

	public void MouseOverLevel(GameObject caller){
		caller.GetComponent<Animation>().clip = pointerEnterClip;
		caller.GetComponent<Animation>().Play();
	}

	public void MouseLeaveLevel(GameObject caller){
		caller.GetComponent<Animation>().clip = pointerLeaveClip;
		caller.GetComponent<Animation>().Play();
	}

	public void InvokeClick(Button caller){
		caller.transform.localScale = new Vector3(1, 1, 1);
		caller.onClick.Invoke();
	}
}
