using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWeapon : MonoBehaviour, WeaponBaseInterface {
	public GameObject primaryProjectile;
	public GameObject specialProjectile;
	public float specialCooldownTime;

	public void FirePrimary(GameObject caller, string facingDirection, Vector3 position, int damage){
		Vector3 rotation = new Vector3(0, 0, 0);
		switch(facingDirection){
			case "LEFT":
				rotation = new Vector3(0, 0, 90);
				break;
			case "RIGHT":
				rotation = new Vector3(0, 0, -90);
				break;
			case "DOWN":
				rotation = new Vector3(0, 0, 180);
				break;
			default:
				rotation = new Vector3(0, 0, 0);
				break;
		}

		GameObject firedPrefab = Instantiate(primaryProjectile, position, Quaternion.identity);
		firedPrefab.transform.Rotate(rotation);
		firedPrefab.GetComponent<ProjectileBase>().damage = damage;
	}


	public void FireSpecial(GameObject caller, string facingDirection, Vector3 position, int damage){
		Vector3 rotation = new Vector3(0, 0, 0);
		switch(facingDirection){
			case "LEFT":
				rotation = new Vector3(0, 0, 180);
				break;
			case "RIGHT":
				rotation = new Vector3(0, 0, 0);
				break;
			case "DOWN":
				rotation = new Vector3(0, 0, -90);
				break;
			default:
				rotation = new Vector3(0, 0, 90);
				break;
		}

		GameObject firedPrefab = Instantiate(specialProjectile, position, Quaternion.identity);
		firedPrefab.transform.Rotate(rotation);
		firedPrefab.GetComponent<ProjectileBase>().damage = damage;
	}

	public Sprite GetSpecialImage(){
		return specialProjectile.GetComponent<SpriteRenderer>().sprite;
	}

	public int GetSpecialUIRotationZ(){
		return specialProjectile.GetComponent<SpecialWeaponInterface>().GetSpecialUIRotationZ();
	}

	public float GetSpecialCooldownTime(){
		return specialCooldownTime;
	}
}
