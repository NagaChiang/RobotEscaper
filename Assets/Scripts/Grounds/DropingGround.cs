using UnityEngine;
using System.Collections;

public class DropingGround : MonoBehaviour {

	// droping time
	public float dropTime;

	// is triggered or not; can only be triggered once
	bool isTrigger;

	void Start()
	{
		isTrigger = false; // wait to be triggered
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if((other.gameObject.tag == "Player") && !isTrigger)
		{
			isTrigger = true;
			StartCoroutine(drop());
		}
	}

	// countdown and drop
	IEnumerator drop()
	{
		yield return new WaitForSeconds(dropTime); // It's the final countdown!
		rigidbody2D.isKinematic = false; // start to be affected by gravity
	}
}
