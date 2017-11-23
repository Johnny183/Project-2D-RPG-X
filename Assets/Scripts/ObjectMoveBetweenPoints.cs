using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMoveBetweenPoints : MonoBehaviour {

	public Vector3 pointA;
	public Vector3 pointB;
	public float moveTime = 3.0f;
    public float delayTime = 0f;

	IEnumerator Start()
    {
        while (true) {
            yield return StartCoroutine(MoveObject(transform, pointA, pointB, moveTime));
            yield return StartCoroutine(MoveObject(transform, pointB, pointA, moveTime));
        }
    }
  
    IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
    {
        yield return new WaitForSeconds(delayTime);
        var i= 0.0f;
        var rate= 1.0f/time;
        while (i < 1.0f) {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            yield return new WaitForFixedUpdate();
        }
    }

    /*
	void OnTriggerEnter2D(Collider2D other){
		if (other.transform.tag == "Player") {
			other.transform.SetParent(gameObject.transform);
 		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.transform.tag == "Player") {
			other.transform.SetParent(null);
 		}
	}*/
}
