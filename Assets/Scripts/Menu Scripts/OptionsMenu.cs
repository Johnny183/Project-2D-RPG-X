using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {
	
	public Slider musicSlider;
	public Slider gameSlider;
	public AnimationClip pointerEnterClip;
	public AnimationClip pointerLeaveClip;

	void Awake(){
		musicSlider.value = GameManager.instance.musicVolume;
		gameSlider.value = GameManager.instance.gameVolume;
	}

	public void SetGameVolume(){
		SoundManager.instance.SetGameVolume(gameSlider.value);
		GameManager.instance.gameVolume = gameSlider.value;
	}

	public void SetMusicVolume(){
		SoundManager.instance.SetMusicVolume(musicSlider.value);
		GameManager.instance.musicVolume = musicSlider.value;
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
