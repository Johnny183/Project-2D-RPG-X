using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionsMenu : MonoBehaviour {

	public GameObject playerQMenu;
	public Slider musicSlider;
	public Slider gameSlider;

	void Awake(){
		musicSlider.value = GameManager.instance.musicVolume;
		gameSlider.value = GameManager.instance.gameVolume;
	}

	public void LoadMenu(){
		Debug.Log("Loading Main Menu");
		playerQMenu.SetActive(false);
		GameManager.instance.LoadGameScene("MainMenu");
	}

	public void SetGameVolume(){
		SoundManager.instance.SetGameVolume(gameSlider.value);
		GameManager.instance.gameVolume = gameSlider.value;
	}

	public void SetMusicVolume(){
		SoundManager.instance.SetMusicVolume(musicSlider.value);
		GameManager.instance.musicVolume = musicSlider.value;
	}
}
