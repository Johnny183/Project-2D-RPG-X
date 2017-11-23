using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WeaponBase : MonoBehaviour {
	
	protected Text specialText;
	protected PlayerExperience playerExperience;
	protected PlayerController playerController;
	protected GameObject player;
	private bool canFireSpecial = false;
	private bool alertActive = false;

	public void Awake(){
		LoadWeaponBase();
	}

	public void LoadWeaponBase(){
		if(GameObject.FindGameObjectWithTag("Player") != null){
			player = GameObject.FindGameObjectWithTag("Player");
			playerExperience = player.GetComponent<PlayerExperience>();
			playerController = player.GetComponent<PlayerController>();
		}
	}

	void Update(){
		if(alertActive){
			if(playerExperience.playerLevelingUp){
				specialText.gameObject.SetActive(false);
			} else {
				specialText.gameObject.SetActive(true);
			}

			Vector3 screenPos;
			if(playerController.isPlayerSpeaking){
				screenPos = Camera.main.WorldToScreenPoint(new Vector3(player.transform.position.x, player.transform.position.y + 1.2f, player.transform.position.x));
			} else {
				screenPos = Camera.main.WorldToScreenPoint(new Vector3(player.transform.position.x, player.transform.position.y + 1f, player.transform.position.x));
			}
			specialText.transform.position = screenPos;
		}
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
		specialText.gameObject.SetActive(false);
		StartCoroutine(SpecialCountDown(countDown));
	}

	private IEnumerator SpecialCountDown(int countDown){
		yield return new WaitForSeconds(countDown);
		Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(player.transform.position.x, player.transform.position.y + 1.2f, player.transform.position.x));
		specialText.transform.position = screenPos;
		specialText.text = "Special Ready!";
		specialText.color = Color.yellow;
		specialText.gameObject.SetActive(true);
		canFireSpecial = true;
		alertActive = true;
	}
}

public interface CustomWeaponInterface {
	void FirePrimary(Vector3 position, Vector3 rotation);
	void FireSpecial(Vector3 position, Vector3 rotation);
}
