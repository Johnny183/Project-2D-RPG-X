using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour {
	
	public int projectileSpeed;
	public float destroyTime;
	[HideInInspector] public int damage;
	public int additionDamageMultiplier;
	public float timesDamageMultiplier = 1;

	void Awake(){
		StartCoroutine(DestroyObject(destroyTime));
	}

	// Calculates total damage for a given projectile
	public int CalculateDamage(){
		return Mathf.RoundToInt((damage + additionDamageMultiplier) * timesDamageMultiplier);
	}

	// Destroy Projectile after a given amount of time
	private IEnumerator DestroyObject(float destroyTime){
		yield return new WaitForSeconds(destroyTime);
		Destroy(gameObject);
	}
}

interface SpecialWeaponInterface{
	int GetSpecialUIRotationZ();
}
