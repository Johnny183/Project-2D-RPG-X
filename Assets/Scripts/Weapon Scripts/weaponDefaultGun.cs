using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class weaponDefaultGun : WeaponBase, CustomWeaponInterface {
	public GameObject primaryFire;
	public GameObject specialFire;
	public int specialCountDown = 30;
	
	void Start() {
		specialText = GameObject.FindWithTag("SpecialText").GetComponent<Text>();
		SpecialCooldownStart(specialCountDown);
		//transform.SetParent(GameObject.FindGameObjectWithTag("WeaponSlot").transform);
	}

	void FixedUpdate() {
		transform.position = player.transform.position;
	}

	public void FirePrimary(Vector3 position, Vector3 rotation){
		var firedPrefab = (GameObject)Instantiate(primaryFire, position, Quaternion.identity);
		firedPrefab.transform.Rotate(rotation);
	}

	public void FireSpecial(Vector3 position, Vector3 rotation){
		if(base.isSpecialAvailable()){
			Vector3 newPosition;
			if(playerController.facingDirection == "RIGHT") {
				newPosition = new Vector3(position.x + 0.15f, position.y, position.z);
				rotation = new Vector3(0, 0, 0);
			} else if(playerController.facingDirection == "LEFT") {
				newPosition = new Vector3(position.x - 0.15f, position.y, position.z);
				rotation = new Vector3(0, 0, -180);
			} else if(playerController.facingDirection == "UP") {
				newPosition = new Vector3(position.x, position.y + 0.15f, position.z);
				rotation = new Vector3(0, 0, 90);
			} else {
				newPosition = new Vector3(position.x, position.y - 0.15f, position.z);
				rotation = new Vector3(0, 0, -90);
			}
			var firedPrefab = (GameObject)Instantiate(specialFire, newPosition, Quaternion.identity);
			firedPrefab.transform.Rotate(rotation);
			SpecialCooldownStart(specialCountDown);
		}
	}
}
