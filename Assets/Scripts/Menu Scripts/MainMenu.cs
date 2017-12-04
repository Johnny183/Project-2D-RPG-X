using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public AnimationClip pointerEnterClip;
	public AnimationClip pointerLeaveClip;

	void Awake(){
		Cursor.visible = true;
	}

	public void PlayGame(){
		GameManager.instance.LoadGameScene("Level1");
	}

	public void QuitGame(){
		Application.Quit();
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
		caller.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
		caller.onClick.Invoke();
	}
}
