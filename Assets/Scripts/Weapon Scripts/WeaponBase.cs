using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WeaponBase : MonoBehaviour {
	
	protected PlayerExperience playerExperience;
	protected PlayerController playerController;
	protected GameObject player;
	protected Text specialText;
	protected Text specialTime;
	protected Image specialImage;
	protected Image specialBlur;
	protected Sprite weaponSpecialSprite;

	private bool canFireSpecial = false;
	private bool alertActive = false;
	private float specialCooldown;

	public void Awake(){
		LoadWeaponBase();
	}

	public void LoadWeaponBase(){
		if(GameObject.FindGameObjectWithTag("Player") != null){
			player = GameObject.FindGameObjectWithTag("Player");
			playerExperience = player.GetComponent<PlayerExperience>();
			playerController = player.GetComponent<PlayerController>();
			specialTime = GameObject.Find("SpecialTime").GetComponent<Text>();
			specialBlur = GameObject.Find("SpecialBlur").GetComponent<Image>();
			specialImage = GameObject.Find("SpecialImage").GetComponent<Image>();
		}
	}

	void Update(){
		if(!alertActive){
			specialBlur.fillAmount -= 1 / specialCooldown * Time.deltaTime;
		}
	}

	protected void SetSpecialImage(Sprite image){
		specialImage.sprite = image;
	}

	protected bool isSpecialAvailable(){
		if(canFireSpecial){
			return true;
		} else {
			return false;
		}
	}

	protected void SpecialCooldownStart(int countDown){
		canFireSpecial = false;
		alertActive = false;
		specialBlur.fillAmount = 1;
		specialCooldown = countDown;
		StartCoroutine(SpecialVisualCountdown(countDown));
	}

	private IEnumerator SpecialVisualCountdown(int countDown){
		specialTime.text = "" + countDown;
		while(countDown != 0){
			yield return new WaitForSeconds(1);
			countDown--;
			specialTime.text = "" + countDown;
		}
		canFireSpecial = true;
		alertActive = true;
		specialTime.text = "";
	}
}

public interface CustomWeaponInterface {
	void FirePrimary(Vector3 position, Vector3 rotation);
	void FireSpecial(Vector3 position, Vector3 rotation);
}
