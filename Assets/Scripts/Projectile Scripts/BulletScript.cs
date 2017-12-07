using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : ProjectileBase {
	
	// Update is called once per frame
	void Update () {
		transform.Translate(0, projectileSpeed * Time.deltaTime, 0);
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.tag == "Food" || other.gameObject.tag == "Floor" || other.gameObject.tag == "Projectile") return;
		
		int newDamage = CalculateDamage();

		if(other.gameObject.tag == "Player"){
			other.gameObject.GetComponent<PlayerHealth>().TakeDamage(newDamage);
		} else if(other.gameObject.tag == "Enemy"){
			other.GetComponent<AIEnemyBase>().TakeDamage(newDamage);
		}

		Destroy(gameObject);
	}
}
