using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

	bool clicked; // indicates if there is any button clicked or not
	bool isGameover; // is gameover or not

	// menu items
	string[] menuItems = new string[2];
	
	// current selected menu item
	int selectedItemIndex;

	// SFX
	public AudioSource audioSelect;
	public AudioSource audioPress;
	public AudioSource audioStreak;

	// Score
	public float scoreFromPos = 0.2f;
	float playerScore = 0;
	float lastCameraX;

	// tutorial on GUI
	public Font GUIfont;
	public float fadeTime = 2f; // seconds after game start to fade out
	public float fadeSmooth = 0.005f; // the amount of color.alpha reducing per frame
	Color tutorColor; // color for tutorial fade out
	float tutorColorA; // transparency of tutorial GUI

	// show streak on GUI
	int streaks; // streaks now
	int streakNext; // next streak goal
	int streakInterval = 50; // with which interval to show streak
	Color streakColor;
	float streakColorA;

	// highscores
	const int SCORE_NOT_LOADED = -1;
	const string DEFAULT_PLAYERNAME = "Anonymous Robot";
	string playerName; // the string to store the player's name
	int scoreMin; // the last score from the top 100
	bool isHighscore; // indicates it's a new record, wait for add score to server
	bool isSubmitting; // submitting the new record, please wait
	bool isSubmitted; // indicates a new record hs been sent, then show gameover

	// Display FPS
	public bool FPSDisplay = false;

	// for Destroyer to inform the camera when it's gameover
	public void gameover()
	{
		isGameover = true;
	}

	// for coins to increase the score
	public void increaseScore(int amount)
	{
		playerScore += amount;
	}

	public int getScore()
	{
		return (int)playerScore;
	}

	void Start()
	{
		// hide cursor
		Screen.showCursor = false;

		// menu items
		menuItems[0] = "Try Again";
		menuItems[1] = "Back to Menu";
		
		// current selected menu item
		selectedItemIndex = 0; // set to the first item

		// Score
		lastCameraX = gameObject.transform.position.x; // store the original position

		// Streak
		streaks = 0;
		streakNext = streakInterval; // first streak goal
		streakColorA = 0f; // invisible

		// Tutorial
		tutorColorA = 1f; // visible

		// reset button clicked
		clicked = false;

		// highscores (get the last score from the top 100)
		playerName = ""; // default player name
		scoreMin = SCORE_NOT_LOADED; // indicates not loaded
		isGameover = false;// reset the gameover flag
		isHighscore = false; // default to not a new record
		isSubmitting = false; // not submitting anything
		isSubmitted = false; // default to not submitted
		StartCoroutine(HighscoreUtil.getScoreMin(value => scoreMin = value)); // lambda expression

		// display tutorial GUI, then fade away
		StartCoroutine(GUIfade(fadeTime, fadeSmooth));
	}


	void Update() 
	{
		// Score
		playerScore += (gameObject.transform.position.x - lastCameraX)*scoreFromPos;
		lastCameraX = gameObject.transform.position.x;

		// when gameover
		if(isGameover)
		{
			// result window
			if(isSubmitted || !isHighscore)
			{
				// when input "Jump" or "Enter", trigger the item
				if((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Return)) && !clicked)
				{
					clicked = true;
					handleSelect();
				}
				
				// when input left or right, move the index
				if(Input.GetKeyDown(KeyCode.LeftArrow))
					menuSelect("negtive");
				if(Input.GetKeyDown(KeyCode.RightArrow))
					menuSelect("positive");
			}
		}
	}

	void OnGUI()
	{
		// if there is any button clicked, let the scene fade away
		if(clicked)
			GUI.color = new Color(1, 1, 1, 0); // invisible

		// FPS
		if(FPSDisplay)
			GUI.Label(new Rect(Screen.width-100, Screen.height-30, 100, 30), "FPS: " + (int) 1/Time.deltaTime);

		// Score
		showScore();

		// Streak
		showStreak();

		// Tutorial
		showTutorial();

		// when gameover
		if(isGameover)
		{
			if((playerScore > scoreMin) && (scoreMin != SCORE_NOT_LOADED) && !isSubmitted) // if reach the highscore
			{
				isHighscore = true; // it's about to add a new record to server
				showHighscore(); // handle the new highscore
			}
			else // result
			{
				showGameover(); // show the result and buttons
				GUI.FocusControl(menuItems[selectedItemIndex]); // focus the selected item
			}
		}
	}

	// show score on left-top corner
	void showScore()
	{
		// GUIstyle for label of score
		GUIStyle styleScore = new GUIStyle(GUI.skin.label);
		styleScore.fontSize = 20;
		styleScore.font = GUIfont;

		// display
		GUI.Label(new Rect(20, 10, 200, 30), (int)(playerScore>0 ? playerScore : 0) + " m", styleScore);
	}

	// show the streak on the middle of screen
	void showStreak()
	{
		// when reach the streak
		if(playerScore >= streakNext)
		{
			audioStreak.Play(); // SFX

			streakColorA = 1f; // make the streak visible
			streaks = streakNext; // set streaks now
			streakNext += streakInterval; // set next streak
		}

		// reduce the alpha of the color gradually until invisible
		if(streakColorA > 0)
			streakColorA -= fadeSmooth;

		// setting the global color to change the streak after
		streakColor = new Color(1, 1, 1, streakColorA);
		GUI.color = streakColor;

		// GUIstyle for label of streak
		GUIStyle styleStreak = new GUIStyle(GUI.skin.label);
		styleStreak.fontSize = 40;
		styleStreak.font = GUIfont;
		styleStreak.alignment = TextAnchor.MiddleCenter;

		// display
		GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 - 100, 400, 100), streaks + " m", styleStreak);

		// setting back the global color
		GUI.color = Color.white; 
	}

	// show the tutorial and fade out
	void showTutorial()
	{
		// setting the global color to change the tutorial after
		tutorColor = new Color(1, 1, 1, tutorColorA);
		GUI.color = tutorColor;

		// GUIStyle for label in tutorial
		GUIStyle styleTutorLabel = new GUIStyle(GUI.skin.label);
		styleTutorLabel.fontSize = 11; // fontsize
		styleTutorLabel.alignment = TextAnchor.MiddleCenter; // center
		styleTutorLabel.font = GUIfont;

		// GUIStyle for box in tutorial
		GUIStyle styleTutorBox = new GUIStyle(GUI.skin.box);
		styleTutorBox.fontSize = 30; // fontsize
		styleTutorBox.alignment = TextAnchor.MiddleCenter; // center
		styleTutorBox.font = GUIfont;

		// display
		GUI.Label(new Rect(36, Screen.height-80, 40, 20), "JUMP", styleTutorLabel);
		GUI.Label(new Rect(96, Screen.height-80, 40, 20), "DASH", styleTutorLabel);
		GUI.Box(new Rect(30, Screen.height-60 , 50, 50), "Z", styleTutorBox);
		GUI.Box(new Rect(90, Screen.height-60 , 50, 50), "X", styleTutorBox);

		// setting back the global color
		GUI.color = Color.white; 
	}

	// to fade out the tutorial
	IEnumerator GUIfade(float fadeTime, float fadeSmooth)
	{
		yield return new WaitForSeconds(fadeTime);

		// reduce the alpha of the color gradually
		while(tutorColorA > 0)
		{
			tutorColorA -= fadeSmooth;
			yield return null; // go to next frame
		}
	}

	void showHighscore()
	{
		// get current event
		Event e = Event.current;

		// GUIStyle for title of highscore
		GUIStyle styleHStitle = new GUIStyle(GUI.skin.label);
		styleHStitle.alignment = TextAnchor.MiddleCenter;
		styleHStitle.font = GUIfont;
		styleHStitle.fontSize = 40;

		// GUISytle for label of highscore
		GUIStyle styleHSlabel = new GUIStyle(GUI.skin.label);
		styleHSlabel.alignment = TextAnchor.MiddleCenter;
		styleHSlabel.font = GUIfont;
		styleHSlabel.fontSize = 24;

		// GUIStyle for textField of highscore
		GUIStyle styleHStxt = new GUIStyle(GUI.skin.textField);
		styleHStxt.alignment = TextAnchor.MiddleCenter;
		styleHStxt.font = GUIfont;
		styleHStxt.fontSize = 18;
		styleHStxt.focused = styleHStxt.hover; // set focused effect as hover effect
		styleHStxt.focused.textColor = Color.red;
		styleHStxt.hover = styleHStxt.normal; // no hover effect

		// GUIStyle for button of highscore
		GUIStyle styleHSButton = new GUIStyle(GUI.skin.button);
		styleHSButton.alignment = TextAnchor.MiddleCenter;
		styleHSButton.font = GUIfont;
		styleHSButton.fontSize = 18;
		styleHSButton.focused = styleHSButton.hover; // set focused effect as hover effect
		styleHSButton.focused.textColor = Color.red;
		styleHSButton.hover = styleHSButton.normal; // no hover effect

		// display
		GUILayout.BeginArea(new Rect(Screen.width/2-250, Screen.height/2-150, 500, 300), GUI.skin.box);
		GUILayout.Label("- New Record -", styleHStitle);
		GUILayout.FlexibleSpace();
		GUILayout.Label((int)playerScore + " m", styleHStitle); // final score
		GUILayout.FlexibleSpace();
		GUILayout.Label("Please left your name for eternity.", styleHSlabel);
		if(isSubmitting) // end of highscore phase, wait for submitting
		{
			GUILayout.Label("[ Submitting... Please wait. ]", styleHSlabel);
			isHighscore = false;
		}
		else // highscore phase
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if((e.keyCode == KeyCode.Return)) // have to get key before textField
			{
				// when input "Enter" at textfield
				if((e.keyCode == KeyCode.Return) && !isSubmitting)
				{
					submitHighscore();
					isSubmitting = true;
				}
			}
			else // textField
			{
				GUI.SetNextControlName("PlayerName");
				playerName = GUILayout.TextField(playerName, 20, styleHStxt, GUILayout.Width(300), GUILayout.ExpandHeight(false)) // text field
												.Replace("\n", ""); // prevent line break
				GUI.FocusControl("PlayerName");
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
	}

	void showGameover()
	{
		// GUIStyle for result label
		GUIStyle styleResult = new GUIStyle(GUI.skin.label);
		styleResult.alignment = TextAnchor.MiddleCenter;
		styleResult.font = GUIfont;
		styleResult.fontSize = 40;

		// GUIStyle for buttons
		GUIStyle styleButton = new GUIStyle(GUI.skin.button);
		styleButton.alignment = TextAnchor.MiddleCenter;
		styleButton.font = GUIfont;
		styleButton.fontSize = 18;
		styleButton.focused = styleButton.hover; // set focused effect as hovered
		styleButton.focused.textColor = Color.red;
		styleButton.hover = styleButton.normal; // mouse hover won't trigger effect anymore

		// GUIStyle for box of disconnected
		GUIStyle styleDisconBox = new GUIStyle(GUI.skin.box);
		styleDisconBox.alignment = TextAnchor.MiddleCenter;
		styleDisconBox.font = GUIfont;
		styleDisconBox.fontSize = 20;

		// GUIStyle for label of disconnected
		GUIStyle styleDisconLabel = new GUIStyle(GUI.skin.label);
		styleDisconLabel.alignment = TextAnchor.MiddleCenter;
		styleDisconLabel.font = GUIfont;
		styleDisconLabel.fontSize = 12;

		// display
		GUILayout.BeginArea(new Rect(Screen.width/2-300, Screen.height/2-150, 600, 300), GUI.skin.box);
		GUILayout.Label("- Escape Failed -", styleResult);
		GUILayout.FlexibleSpace();
		GUILayout.Label((int)playerScore + " m", styleResult); // final score
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.SetNextControlName(menuItems[0]); // Try Again
			GUILayout.Button(menuItems[0], styleButton, GUILayout.Width(160));
			GUILayout.FlexibleSpace();
			GUI.SetNextControlName(menuItems[1]); // Back to Menu
			GUILayout.Button(menuItems[1], styleButton, GUILayout.Width(160));
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		if(scoreMin == SCORE_NOT_LOADED) // not connected to the server
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Box("!", styleDisconBox, GUILayout.Width(25), GUILayout.Height(25));
			GUILayout.Label("There was a problem connecting to the server. Couldn't check for highscores.");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		
		GUI.color = new Color(1, 1, 1, 0); // set GUI below invisible
	}

	// read in the keyboard input and move the index
	void menuSelect(string arrowkey)
	{
		// SFX
		audioSelect.Play();

		if(arrowkey == "positive")
		{
			// if it has already selected the first item, loop to the last one
			if(selectedItemIndex == 0)
				selectedItemIndex = menuItems.Length-1;
			
			// else, move upward
			else
				selectedItemIndex -= 1;
		}
		
		else if(arrowkey == "negtive")
		{
			// if it has already selected the last item, loop to the first one
			if(selectedItemIndex == menuItems.Length-1)
				selectedItemIndex = 0;
			
			// else, move downward
			else
				selectedItemIndex += 1;
		}
	}

	// as press the buttom, handle the corresponding selected item
	void handleSelect()
	{
		// SFX
		audioPress.Play();

		switch(selectedItemIndex)
		{
			// Try Again
			case 0:
				SceneFade.FadeAndLoad(Application.loadedLevelName, Color.black, 1f);
				break;
				
			// Back to Menu
			case 1:
				SceneFade.FadeAndLoad("MenuScene", Color.black, 1f);
				break;
				
			// invalid selected item
			default:
				Debug.LogError("Invalid selected item.");
				break;
		}
	}

	void submitHighscore()
	{
		// SFX
		audioPress.Play();
		
		// if the player name is left blank
		if(playerName == "") 
			playerName = DEFAULT_PLAYERNAME;
		
		// submit the score
		StartCoroutine(HighscoreUtil.addScore(playerName, (int)playerScore, value => isSubmitted = value)); 
	}
}
