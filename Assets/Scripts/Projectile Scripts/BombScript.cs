using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : ProjectileBase, SpecialWeaponInterface {

	public int specialUIRotationZ = 0;
	public float explosionRadius = 2f;
	public GameObject explosionEffect;

	private bool canExplode = false;

	// Use this for initialization
	void Start () {
		StartCoroutine(delayExplosion(1f));
	}

	public int GetSpecialUIRotationZ(){
		return specialUIRotationZ;
	}

	private IEnumerator delayExplosion(float delayTime){
		yield return new WaitForSeconds(delayTime);
		canExplode = true;
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.tag == "Food" || other.gameObject.tag == "Floor" || other.gameObject.tag == "Projectile" || !canExplode) return;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.NameToLayer("blockingLayer"));

		foreach (Collider2D nearbyObject in colliders) {
			float proximity = (transform.position - nearbyObject.transform.position).magnitude;
			int newDamage = Mathf.CeilToInt(CalculateDamage() - (proximity / explosionRadius));
			
			if(nearbyObject.CompareTag("Player")){
				PlayerHealth playerHealth = nearbyObject.gameObject.GetComponent<PlayerHealth>();
				playerHealth.TakeDamage(newDamage);
			} else if (nearbyObject.CompareTag("Enemy")){
				nearbyObject.GetComponent<AIEnemyBase>().TakeDamage(newDamage);
			}
		}

		GameObject effect = Instantiate(explosionEffect, transform.position, transform.rotation);
		Destroy(effect, 2f);
		Destroy(gameObject);
	}
}
