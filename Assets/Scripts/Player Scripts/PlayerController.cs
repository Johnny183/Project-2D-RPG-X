using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[HideInInspector] public string facingDirection = "RIGHT";
	[HideInInspector] public bool isPlayerSpeaking = false;
	public float moveForce = 50f;
	public float maxSpeed = 2f;
	public Canvas playerUI;
	public LayerMask blockingLayer;
	public Text playerSpeakingText;

    private BoxCollider2D boxCollider;
	private Rigidbody2D rb2d;
	private float playerInteractDist;
	private bool playerIsShooting = false;
	private float shootingMovementCooldown = 0;

	void Awake(){
		Cursor.visible = false;
		playerUI.gameObject.SetActive(true);
		rb2d = GetComponent<Rigidbody2D>();
		boxCollider = GetComponent<BoxCollider2D>();
		playerInteractDist = GameManager.instance.playerIneractDist;
	}

	void Start(){
		EquipmentManager.instance.LoadPlayerEquipment();
	}
	
	// Called every game frame
	void Update(){
		// Stops the player from using WASD to control movement direction when shooting
		if(playerIsShooting){
			shootingMovementCooldown += Time.deltaTime;
			if(shootingMovementCooldown > 1){
				playerIsShooting = false;
			}
		}

		// If the player is speaking, set the position of our text component
		if(isPlayerSpeaking){
			Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.x));
			playerSpeakingText.transform.position = screenPos;
		}

		// Check if the player pressed the interact key
		if(Input.GetKeyDown(KeyCode.E)){
			CheckIfHitObject();
		}
	}

	// Called every physics frame
	void FixedUpdate(){
		// Player movement
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		if(h * rb2d.velocity.x < maxSpeed){
			rb2d.velocity = new Vector2(h * maxSpeed, rb2d.velocity.y);
		}
		if(Mathf.Abs(rb2d.velocity.x) > maxSpeed){
			rb2d.velocity = new Vector2 (Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);
		}
		if(v * rb2d.velocity.y < maxSpeed){
			rb2d.velocity = new Vector2(rb2d.velocity.x, v * maxSpeed);
		}
		if(Mathf.Abs(rb2d.velocity.y) > maxSpeed){
			rb2d.velocity = new Vector2 (rb2d.velocity.x, Mathf.Sign(rb2d.velocity.y) * maxSpeed);
		}

		if(!playerIsShooting){
			if (h > 0){
				facingDirection = "RIGHT";
				ChangeDirection();
			}
			if (h < 0){
				facingDirection = "LEFT";
				ChangeDirection();
			}
			if(v > 0){
				facingDirection = "UP";
				ChangeDirection();
			}
			if(v < 0){
				facingDirection = "DOWN";
				ChangeDirection();
			}
		}
	}

	// Stops the player from using WASD to control movement direction when shooting
	public void StartPlayerShootingCooldown(){
		shootingMovementCooldown = 0;
		playerIsShooting = true;
	}

	public void ChangeDirection(){
		switch(facingDirection){
			case "RIGHT":
				transform.eulerAngles = new Vector3(transform.rotation.x, 0, transform.rotation.x);
				break;
			case "LEFT":
				transform.eulerAngles = new Vector3(transform.rotation.x, 180, transform.rotation.x);
				break;
			case "UP":
				break;
			case "DOWN":
				break;
		}
	}

	// When we collide with another object that has a collider attached to the gameobject
	void OnCollisionEnter2D(Collision2D collision) {
		if(collision.transform.CompareTag("Food")){
			collision.transform.GetComponent<InteractableObject>().Interact(transform);
		} else if(collision.transform.CompareTag("ItemPickup")){
			collision.transform.GetComponent<InteractableItem>().PickUp(gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.transform.CompareTag("ItemPickup")){
			other.transform.GetComponent<InteractableItem>().PickUp(gameObject);
		}
	}

	// Called when an interactable object has been hit
	private void InteractWithObject(Transform other){

	}

	// Checks to see if we have hit an object that we can interact with
	private void CheckIfHitObject(){
		RaycastHit2D checkDirection;
		boxCollider.enabled = false;
		switch(facingDirection){
			case "RIGHT":
				checkDirection = Physics2D.Linecast(transform.position, new Vector2(transform.position.x + playerInteractDist, transform.position.y), blockingLayer);
				break;
			case "LEFT":
				checkDirection = Physics2D.Linecast(transform.position, new Vector2(transform.position.x - playerInteractDist, transform.position.y), blockingLayer);
				break;
			case "UP":
				checkDirection = Physics2D.Linecast(transform.position, new Vector2(transform.position.x, transform.position.y + playerInteractDist), blockingLayer);
				break;
			default:
				checkDirection = Physics2D.Linecast(transform.position, new Vector2(transform.position.x, transform.position.y - playerInteractDist), blockingLayer);
				break;
		}
		if(checkDirection.transform != null){
			InteractWithObject(checkDirection.transform);
		}
		boxCollider.enabled = true;
	}

	// Sentence: What you want to say, Textcolor: Color, SpeakDelay: Delay before speaking Endtime: How long does it display for, WriteDelay: Time per character being written
	public void CallPlayerSpeak(string sentence, Color textColor, int speakDelay, int endTime, float writeDelay){
		StartCoroutine(PlayerSpeak(sentence, textColor, speakDelay, endTime, writeDelay));
	}

	// Handles player speaking, not to be called directly
	private IEnumerator PlayerSpeak(string sentence, Color textColor, int speakDelay, int endTime, float writeDelay){
		yield return new WaitForSeconds(speakDelay);
		isPlayerSpeaking = true;
		playerSpeakingText.color = textColor;
		playerSpeakingText.gameObject.SetActive(true);
		StopAllCoroutines();
		IEnumerator sent = TypeSentence(sentence, writeDelay);
		IEnumerator end = EndPlayerSpeak(sentence, endTime, writeDelay);
		StartCoroutine(sent);
		StartCoroutine(end);
	}

	// Closes our speaking functions
	private IEnumerator EndPlayerSpeak(string sentence, int endTime, float writeDelay){
		yield return new WaitForSeconds(endTime);

		int textLength = playerSpeakingText.text.Length;
		for(int i = 0; i < textLength; i++){
			playerSpeakingText.text = playerSpeakingText.text.Remove(playerSpeakingText.text.Length-1);
			yield return new WaitForSeconds(writeDelay);
		}

		playerSpeakingText.gameObject.SetActive(false);
		isPlayerSpeaking = false;
	}
	
	// Handles writing our sentence
	private IEnumerator TypeSentence(string sentence, float writeDelay){
		playerSpeakingText.text = "";
		foreach(char letter in sentence.ToCharArray()){
			playerSpeakingText.text += letter;
			yield return new WaitForSeconds(writeDelay);
		}
	}
}