using UnityEngine;
using System.Collections;

public class ComingSoon : MonoBehaviour {

	void Start()
	{
		// hide cursor
		Screen.showCursor = false;
	}

	void Update()
	{
		// press any key to return to main menu
		if(Input.anyKeyDown)
			SceneFade.FadeAndLoad("MenuScene", Color.black, 1f);
	}
}
