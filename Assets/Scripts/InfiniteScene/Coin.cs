using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	public int coinScore = 100;

	HUD hud;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			hud = GameObject.Find("Main Camera").GetComponent<HUD>();
			hud.increaseScore(coinScore);
			Destroy(this.gameObject);
		}
	}
}
