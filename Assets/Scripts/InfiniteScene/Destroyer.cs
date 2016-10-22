using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour {

	bool isGameover; // indicates it's gameover or not

	void Start()
	{
		isGameover = false;
	}

	void OnTriggerEnter2D(Collider2D other) 
	{
		// player touchs the destroyer below: gameover
		if(other.tag == "Player")
		{
			if(!isGameover)
			{
				isGameover = true;
				gameover();
			}
		}
		else
		{
			if(other.gameObject.transform.parent) // if the object has a parent
				Destroy(other.gameObject.transform.parent.gameObject);
			else // if the object doesn't have a parent
				Destroy(other.gameObject);
		}
	}

	void gameover()
	{
		// infrom the camera it's gameover
		GameObject.Find("Main Camera").GetComponent<HUD>().gameover();
		
		// stop the following camera
		GameObject.Find("Main Camera").GetComponent<RunnerCamera>().enabled = false;

		// inform the character it's gameover
		GameObject.Find("2D Character").GetComponent<PlatformerCharacter2D>().gameover();
	}
}
