using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : ProjectileBase, SpecialWeaponInterface {

	public int specialUIRotationZ = 90;
	public float turnSpeed = 200f;
	public float explosionRadius = 2f;
	public GameObject explosionEffect;

	private Rigidbody2D rb2d;
	private GameObject[] targets;
	private bool canExplode = false;

	// Use this for initialization
	void Start () {
		rb2d = gameObject.GetComponent<Rigidbody2D>();
		StartCoroutine(delayExplosion(0.1f));
	}

	public int GetSpecialUIRotationZ(){
		return specialUIRotationZ;
	}
	
	private IEnumerator delayExplosion(float delayTime){
		yield return new WaitForSeconds(delayTime);
		canExplode = true;
	}

	void FixedUpdate () {
		GameObject target = null;
		targets = GameObject.FindGameObjectsWithTag("Enemy");
		for(int i = 0; i < targets.Length; i++){
			if(target != null){
				if(Vector2.Distance(transform.position, targets[i].transform.position) < Vector2.Distance(transform.position, target.transform.position)){
					target = targets[i];
				}
			} else {
				target = targets[i];
			}
		}

		if(target != null){
			Vector2 direction = (Vector2)target.transform.position - rb2d.position;
			direction.Normalize();

			float rotateAmount = Vector3.Cross(direction, transform.right).z;

			rb2d.angularVelocity = -rotateAmount * turnSpeed;
		}
		rb2d.velocity = transform.right * projectileSpeed;
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
