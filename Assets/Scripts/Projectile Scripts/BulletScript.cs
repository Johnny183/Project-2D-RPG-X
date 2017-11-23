using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

	public float bulletSpeed = 5f;
	public float destroyTime = 6f;
	public int minDamage = 10;
	public int maxDamage = 15;

	private int randomDamage;

	void Start(){
		randomDamage = Random.Range(minDamage, maxDamage+1);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(0, bulletSpeed * Time.deltaTime, 0);
		StartCoroutine(DestroyObject(destroyTime));
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.tag == "Food" || other.gameObject.tag == "Floor" || other.gameObject.tag == "Projectile") return;
		if(other.gameObject.tag == "Player"){
			PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
			playerHealth.TakeDamage(randomDamage);
		} else if(other.gameObject.tag == "Enemy"){
			other.GetComponent<AIEnemyBase>().TakeDamage(other.gameObject, randomDamage);
		}
		Destroy(gameObject);
	}

	private IEnumerator DestroyObject(float destroyTime){
		yield return new WaitForSeconds(destroyTime);
		Destroy(gameObject);
	}
}
