using UnityEngine;
using System.Collections;

public class ShowCredits : MonoBehaviour {

	public float speed; // speed*Time.deltaTime
	public Font font;
	public int titleFontSize;
	public int contentFontSize;
	public float initTop;
	public float space;

	GUIStyle styleTitle; // GUIStyle for title
	GUIStyle styleCont; // GUIStyle for content
	float posTop; // top of the GUILayout area

	void Start()
	{
		// hide cursor
		Screen.showCursor = false;

		posTop = initTop; // top of the GUILayout area
	}

	void Update()
	{
		// scroll the credits upwards
		posTop -= speed * Time.deltaTime;
		transform.position -= new Vector3(0f, speed*Time.deltaTime*0.05f, 0f);

		// press any key to return to main menu
		if(Input.anyKeyDown)
			SceneFade.FadeAndLoad("MenuScene", Color.black, 1f);
	}

	void OnGUI()
	{
		// GUIStyle for title
		styleTitle = new GUIStyle(GUI.skin.label);
		styleTitle.font = font;
		styleTitle.fontSize = titleFontSize;
		styleTitle.alignment = TextAnchor.UpperCenter;

		// GUIStyle for content
		styleCont = new GUIStyle(GUI.skin.label);
		styleCont.font = font;
		styleCont.fontSize = contentFontSize;
		styleCont.alignment = TextAnchor.UpperCenter;

		// display
		GUILayout.BeginArea(new Rect(10, posTop, Screen.width-20, 8000));
		{
			// Game Designer
			GUILayout.Label(" - Game Designer - ", styleTitle);
			GUILayout.Label("Naga Chiang", styleCont);
			GUILayout.Space(space);

			// Programming
			GUILayout.Label(" - Programming - ", styleTitle);
			GUILayout.Label("Naga Chiang", styleCont);
			GUILayout.Space(space);

			// Graphics
			GUILayout.Label(" - Graphics - ", styleTitle);
			displayCredits("2D Character", "Sample Assets (Unity)");
			displayCredits("Sprites", "Sample Assets (Unity)");
			displayCredits("Skybox", "SkyBox Volume 2 (Hedgehog Team)");
			displayCredits("Particle Systems", "Elementals (G.E.TeamDev)");
			displayCredits("Particle Systems", "SimpleParticlePack (Unity)");
			displayCredits("Font", "Audiowide (Brian J. Bonislawsky)");
			GUILayout.Space(space);

			// Audios
			GUILayout.Label(" - Audios - ", styleTitle);
			displayCredits("Musics", "Naga Chiang");
			displayCredits("SFX", "小森　平");
			displayCredits("SFX", "blastwavefx (www.freeSFX.co.uk)");
			GUILayout.Space(space);

			// Special Thanks
			GUILayout.Label(" - Special Thanks - ", styleTitle);
			GUILayout.Label("Mike Geig", styleCont);
			GUILayout.Space(space);

			// thanks
			GUILayout.Space(space);
			GUILayout.Label("Thanks for playing:)", styleTitle);
		}
		GUILayout.EndArea();
	}

	void displayCredits(string item, string author)
	{
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label(item, styleCont);
			GUILayout.FlexibleSpace();
			GUILayout.Label(author, styleCont);
		}
		GUILayout.EndHorizontal();
	}
}
