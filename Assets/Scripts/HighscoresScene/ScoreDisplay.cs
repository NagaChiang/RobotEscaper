using UnityEngine;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {

	public Font font; // font of highscores
	public int fontsize; // fontsize of highscores

	public float MAX_LIST_SPEED = 20f; // the max speed of moving list
	Vector2 currentListPos; // current position of list
	float arrowAxis; // the value from vertical axis

	string _orgScores; // scores from the server
	string[,] _highscores; // scores after processing
	bool isLoaded; // indicates the highscores are loaded or not
	bool isError; // indicates there was error getting the score

	const int MAX_ENTRIES = 100; // the max highscore entries
	const int MAX_COLS = 4; // the columns of each entry

	bool clicked; // there is any button clicked or not

	// SFX
	public AudioSource audioPress;

	void Start()
	{
		// hide cursor
		Screen.showCursor = false;

		// not loaded yet
		isLoaded = false;
		isError = false;
		_orgScores = null;
		_highscores = null;

		// list position starts at 0
		currentListPos = Vector2.zero;

		// there is any button clicked or not
		clicked = false;

		// get scores
		StartCoroutine(HighscoreUtil.getScore(value => _orgScores = value)); // Lambda expression
	}

	void Update()
	{
		// as the orgScore is got from the server
		if((_orgScores == HighscoreUtil.ERROR_STATE) && !isLoaded)
			isError = true;
		else if((_orgScores != null) && !isLoaded)
			handleScores(); // process the original scores string from server to array

		// as the highscores is loaded
		if((_highscores != null) && !isLoaded)
			isLoaded = true;

		// use the arrow key to move the list
		handleArrowkeys();

		// button for back to menu
		if(Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Return) && !clicked)
		{
			// SFX
			audioPress.Play();
			
			clicked = true;
			SceneFade.FadeAndLoad("MenuScene", Color.black, 1f);
		}
	}

	void OnGUI()
	{
		// show the highscores with scrollbar
		showHighscores();

		// if not loaded, show loading 
		if(!isLoaded && !isError)
			showLoading();
	}

	// if not loaded, show loading
	void showLoading()
	{
		// GUIStyle for loading
		GUIStyle styleLoading = new GUIStyle(GUI.skin.label);
		styleLoading.alignment = TextAnchor.MiddleCenter;
		styleLoading.font = font;
		styleLoading.fontSize = fontsize+10;

		// display
		GUI.Label(new Rect(Screen.width/2-400, Screen.height/2-80, 800, 220), "Loading...", styleLoading);
	}

	// show the highscores with scrollbar
	void showHighscores()
	{
		// GUIStyle for the text of highscores
		GUIStyle styleScore = new GUIStyle(GUI.skin.label);
		styleScore.alignment = TextAnchor.MiddleLeft;
		styleScore.font = font;
		styleScore.fontSize = fontsize;

		// GUIStyle for button of highscores
		GUIStyle styleButton = new GUIStyle(GUI.skin.button);
		styleButton.alignment = TextAnchor.MiddleCenter;
		styleButton.font = font;
		styleButton.fontSize = 18;
		styleButton.hover.textColor = Color.red;

		// GUIStyle for error
		GUIStyle styleError = new GUIStyle(GUI.skin.label);
		styleError.alignment = TextAnchor.MiddleCenter;
		styleError.font = font;
		styleError.fontSize = 24;

		// display
		int place = 0; // the score place of this entry
		string lastScore = null; // the last score
		int toNextPlace = 1; // the amount to add on to be the next place
		GUILayout.BeginArea(new Rect(Screen.width/2-400, Screen.height/2-80, 800, 220), GUI.skin.box);
		if(isLoaded) // display after loaded
		{
			currentListPos = GUILayout.BeginScrollView(currentListPos);
			for(int i=0; i<MAX_ENTRIES; i++)
			{
				// to arrange the place
				if(_highscores[i, 2] == lastScore) // this score = last score
				{
					toNextPlace++; // accumulate the amount to next place
				}
				else
				{
					place += toNextPlace; // to the next place
					toNextPlace = 1; // reset the amount to next place
				}

				lastScore = _highscores[i, 2]; // store this score for next loop

				// display
				GUILayout.BeginHorizontal();
				GUILayout.Space(20);
				GUILayout.Label(place.ToString(), styleScore, GUILayout.Height(60), GUILayout.Width(75)); // place
				GUILayout.Label(_highscores[i, 1], styleScore, GUILayout.Height(60), GUILayout.Width(300)); // name
				GUILayout.Label(_highscores[i, 2], styleScore, GUILayout.Height(60), GUILayout.Width(125)); // score
				GUILayout.Label(_highscores[i, 3], styleScore, GUILayout.Height(60), GUILayout.Width(200)); // time
				GUILayout.Space(20);
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}
		else if(isError) // can't get the scores
		{
			GUILayout.FlexibleSpace();
			GUILayout.Label("Oops! There was an error loading the highscores.", styleError);
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndArea();

		// button
		GUI.SetNextControlName("Back to Menu"); // set control name for focusing later
		GUI.Button(new Rect(Screen.width/2-80, Screen.height-35, 160, 30), "Back to Menu", styleButton);
		GUI.FocusControl("Back to Menu"); // always focus the button
	}

	// process the original scores string from server to array
	void handleScores()
	{
		char[] splitwords = {'\t', '\n'};
		string[] scores = _orgScores.Split(splitwords); // split by space
		string[,] arrayScores = new string[MAX_ENTRIES, MAX_COLS]; // 100 entries, 4 columns

		// log the scores
		Debug.Log("Scores:\n" + _orgScores);

		// divide into multidimentional array
		// 0: id, 1: name, 2: score, 3:timestamp
		for(int i=0; i<MAX_ENTRIES; i++)
		{
			for(int j=0; j<MAX_COLS; j++)
				arrayScores[i, j] = scores[i*MAX_COLS + j];
		}

		// assign to _highscores to complete loading
		_highscores = arrayScores;
	}

	// handle the arrow key to move the list
	void handleArrowkeys()
	{
		arrowAxis = Input.GetAxis("Vertical"); // get input axis
		currentListPos.y -= arrowAxis * MAX_LIST_SPEED; // move the list
	}
}
