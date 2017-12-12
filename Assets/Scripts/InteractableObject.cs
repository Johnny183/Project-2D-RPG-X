using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
	public int pickupAddHealth = 0;
	public int pickupAddExperience = 0;
	public int pickupTakeDamage = 0;
	
	public Text text;
	public Canvas canvas;
	public LayerMask blockingLayer;

	protected GameObject player;
	protected float interactDistance;

	void Awake(){
		player = GameObject.FindGameObjectWithTag("Player");

		if(GameManager.instance != null){
			interactDistance = GameManager.instance.playerIneractDist;
		}
	}

	public virtual void UpdateUI(){
		GetComponent<BoxCollider2D>().enabled = false;
		RaycastHit2D target = Physics2D.Linecast(transform.position, player.transform.position, blockingLayer);
		GetComponent<BoxCollider2D>().enabled = true;

		if(target.distance < interactDistance){
			if(target.transform.tag == "Player"){
				Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.x));
				text.transform.position = screenPos;
				text.gameObject.SetActive(true);
			} else {
				text.gameObject.SetActive(false);
			}
		} else {
			text.gameObject.SetActive(false);
		}
	}

	public virtual void Interact(Transform other){

		if(pickupAddHealth != 0){
			other.GetComponent<PlayerHealth>().AddHealth(pickupAddHealth);
		}
		
		if(pickupTakeDamage != 0){
			other.GetComponent<PlayerHealth>().TakeDamage(pickupTakeDamage);
		}

		if(pickupAddExperience != 0){
			other.GetComponent<PlayerExperience>().AddExp(pickupAddExperience);
		}

		Destroy(gameObject);
	}
}
