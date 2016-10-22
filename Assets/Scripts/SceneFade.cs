using UnityEngine;
using System.Collections;

public class SceneFade : MonoBehaviour {
	
	private string scene;
	private Color fadeColor;
	private float fadeDuration;

	// fade out the current scene, fade in the next scene
	static public void FadeAndLoad(string scene, Color fadeColor, float fadeDuration)
	{	
		// set the texture for fade effect
		Texture2D fadeTexture = new Texture2D(1, 1);
		fadeTexture.SetPixel(0,0, fadeColor);
		fadeTexture.Apply();

		// create a GameObject including the texture
		GameObject fadeObj = new GameObject("FadeObj");
		fadeObj.AddComponent<GUITexture>();
		fadeObj.guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
		fadeObj.guiTexture.texture = fadeTexture;

		// add this script to this game object and start fading and loading
		fadeObj.AddComponent<SceneFade>();

		// pass the parameters to the instance
		fadeObj.GetComponent<SceneFade>().scene = scene;
		fadeObj.GetComponent<SceneFade>().fadeColor = fadeColor;
		fadeObj.GetComponent<SceneFade>().fadeDuration = fadeDuration;
	}

	void Start()
	{
		StartCoroutine(fade());
	}

	IEnumerator fade()
	{
		// don't destroy the GameObject on load
		DontDestroyOnLoad(gameObject);

		// fade in the texture (fade out the current scene)
		guiTexture.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f); // begin with invisible
		float time = 0f;
		while(time < fadeDuration)
		{
			time += Time.deltaTime;
			guiTexture.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b,
			                                     Mathf.InverseLerp(0f, fadeDuration, time)); // return the percentage of time/fadeDuration
			yield return null;
		}
		
		guiTexture.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f); // end with totally visible for sure
		yield return null;
		
		// complete fading out the current scene, start loading the next scene
		Application.LoadLevel(scene);
		
		// fade out the texture (fade in the next scene)	
		time = 0f;
		while(time < fadeDuration)
		{
			time += Time.deltaTime;
			guiTexture.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b,
			                                     Mathf.InverseLerp(fadeDuration, 0f, time)); // return the percentage of time/fadeDuration
			yield return null;
		}
		
		guiTexture.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f); // end with totally invisible for sure
		yield return null;
		
		// destroy this GameObject
		Destroy(gameObject);
	}

	// for OnGUI()
	// when there is any button clicked to let the scene fade away
	// if(clicked)
	// 	   GUI.color = new Color(1, 1, 1, 0); // invisible
}
