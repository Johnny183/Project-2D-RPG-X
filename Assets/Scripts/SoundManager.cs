using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

	public AudioSource efxSource;
	public AudioSource musicSource;
	public static SoundManager instance = null;

	public float lowPitchRange = .95f;
	public float highPitchRange = 1.05f;

	void Awake () {
		if (instance == null){
			instance = this;
		} else if (instance != this){
			Destroy (gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	void Start(){
		efxSource.volume = GameManager.instance.gameVolume;
		musicSource.volume = GameManager.instance.musicVolume;
	}

	public void SetGameVolume(float value){
		efxSource.volume = value;
	}

	public void SetMusicVolume(float value){
		musicSource.volume = value;
	}

	public void PauseUnpauseMusic(){
		if(musicSource.isPlaying){
			musicSource.Pause();
		} else {
			musicSource.UnPause();
		}
	}

	public void RestartMusic(){
		musicSource.time = 0;
	}

	public void SetMusic(AudioClip clip){
		musicSource.clip = clip;
		musicSource.Play();
	}

	public void PlaySingle(AudioClip clip){
		efxSource.clip = clip;
		efxSource.PlayOneShot(efxSource.clip);
	}

	public void RandomizeSfx(params AudioClip[] clips){
		int randomIndex;
		if(clips.Length > 1) {
			randomIndex = Random.Range(0, clips.Length); 
		} else { 
			randomIndex = 0; 
		}
		
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);

		efxSource.pitch = randomPitch;
		efxSource.clip = clips[randomIndex];
		efxSource.PlayOneShot(efxSource.clip);
	}
}
