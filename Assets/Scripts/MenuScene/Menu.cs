using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	const string VERSION = "v0.65 beta";

	// GUI
	public Font font;
	public int fontSize = 18;
	public float fadeDuration = 1f;
	bool clicked; // indicates there is any button clicked or not

	// menu items
	string[] menuItems = new string[4];

	// current selected menu item
	int selectedItemIndex;

	// SFX
	public AudioSource audioSelect;
	public AudioSource audioPress;

	void Start()
	{
		// menu items
		menuItems[0] = "Infinite Mode";
		menuItems[1] = "Story Mode";
		menuItems[2] = "Highscores";
		menuItems[3] = "Credits";

		// current selected menu item
		selectedItemIndex = 0; // set to the first item

		// hide cursor
		Screen.showCursor = false;
	}

	void Update()
	{
		// when input "Jump" or "Enter", trigger the item
		if((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Return)) && !clicked)
		{
			clicked = true;
			handleSelect();
		}

		// when input up or down, move the index
		if(Input.GetKeyDown(KeyCode.UpArrow))
			menuSelect("up");
		if(Input.GetKeyDown(KeyCode.DownArrow))
			menuSelect("down");
	}

	void OnGUI()
	{
		// when there is any button clicked to let the scene fade away
		if(clicked)
			GUI.color = new Color(1, 1, 1, 0); // invisible

		// GUI Style of buttons
		GUIStyle GUIstyle = new GUIStyle(GUI.skin.button);
		GUIstyle.font = font;
		GUIstyle.fontSize = fontSize;
		GUIstyle.focused = GUIstyle.hover; // set focused effect as hovered
		GUIstyle.focused.textColor = Color.red;
		GUIstyle.hover = GUIstyle.normal; // mouse hover won't trigger effect anymore
		
		// Buttons
		GUILayout.BeginArea(new Rect(60, 100, 180, 200));
		for(int i=0; i<menuItems.Length; i++)
		{
			GUI.SetNextControlName(menuItems[i]);
			GUILayout.Button(menuItems[i], GUIstyle);
		} 
		GUILayout.EndArea();

		// focus the selected item
		GUI.FocusControl(menuItems[selectedItemIndex]);
		
		// Version
		GUI.Label(new Rect(Screen.width-75, Screen.height-30, 100, 20), VERSION);
	}

	// read in the keyboard input and move the index
	void menuSelect(string arrowkey)
	{
		// SFX
		audioSelect.Play();

		if(arrowkey == "up")
		{
			// if it has already selected the first item, loop to the last one
			if(selectedItemIndex == 0)
				selectedItemIndex = menuItems.Length-1;

			// else, move upward
			else
				selectedItemIndex -= 1;
		}

		else if(arrowkey == "down")
		{
			// if it has already selected the last item, loop to the first one
			if(selectedItemIndex == menuItems.Length-1)
				selectedItemIndex = 0;

			// else, move downward
			else
				selectedItemIndex += 1;
		}
	}

	void handleSelect()
	{
		// SFX
		audioPress.Play();

		switch(selectedItemIndex)
		{
			// Infinite Mode
			case 0:
				SceneFade.FadeAndLoad("InfiniteScene", Color.black, fadeDuration);
				break;

			// Story Mode
			case 1:
				SceneFade.FadeAndLoad("StoryScene", Color.black, fadeDuration);
				break;

			// Highscores
			case 2:
				SceneFade.FadeAndLoad("HighscoresScene", Color.black, fadeDuration);
				break;

			// Credits
			case 3:
				SceneFade.FadeAndLoad("CreditsScene", Color.black, fadeDuration);
				break;

			// invalid selected item
			default:
				Debug.LogError("Invalid selected item.");
				break;
		}
	}
}

