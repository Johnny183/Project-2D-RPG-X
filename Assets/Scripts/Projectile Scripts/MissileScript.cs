﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour {

	public float turnSpeed = 200f;
	public float missileVelocity = 5f;
	public int minDamage = 30;
	public int maxDamage = 50;
	public GameObject explosionEffect;

	private int randomDamage;
	private Rigidbody2D rb2d;
	private GameObject[] targets;
	private bool canExplode = false;

	// Use this for initialization
	void Start () {
		randomDamage = Random.Range(minDamage, maxDamage+1);
		rb2d = gameObject.GetComponent<Rigidbody2D>();
		StartCoroutine(delayExplosion(0.2f));
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
		rb2d.velocity = transform.right * missileVelocity;
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.tag == "Food" || other.gameObject.tag == "Floor" || other.gameObject.tag == "Projectile" || !canExplode) return;
		if(other.gameObject.tag == "Player"){
			PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
			playerHealth.TakeDamage(randomDamage);
		} else if(other.gameObject.tag == "Enemy"){
			other.GetComponent<AIEnemyBase>().TakeDamage(other.gameObject, randomDamage);
		}
		Instantiate(explosionEffect, transform.position, transform.rotation);
		Destroy(gameObject);
	}
}
