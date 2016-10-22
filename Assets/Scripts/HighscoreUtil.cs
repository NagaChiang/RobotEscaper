using UnityEngine;
using System.Collections;

public class HighscoreUtil {
	
	const string ADDSCORE_URL = "http://voicecrystal.lionfree.net/Games/RobotEscaper/addScore.php";
	const string GETSCORE_URL = "http://voicecrystal.lionfree.net/Games/RobotEscaper/getScore.php";
	const string GETSCOREMIN_URL = "http://voicecrystal.lionfree.net/Games/RobotEscaper/getScoreMin.php";
	const string HASHKEY = "sKeyOfRobotEscaperByNaga1"; // for MD5 hash check

	public const string ERROR_STATE = "ERROR!";

	// must be called by StartCoroutine()
	public static IEnumerator addScore(string name, int score, System.Action<bool> isComplete)
	{
		string hash = MD5Sum(name + HASHKEY + score); // generate the MD5 checksum
		string postURL = ADDSCORE_URL + "?name=" + name + "&score=" + score + "&hash=" + hash; // the string for posting

		WWW postWWW = new WWW(postURL); // connect
		yield return postWWW; // wait until the download is done to get the result

		// log the error
		if(postWWW.error != null)
			Debug.LogWarning("There was an error posting the high score: " + postWWW.error);

		// complete loading
		Debug.Log("Complete Loading: " + postURL);
		isComplete(true); // inform the game the submitting is complete
	}

	// must be called by StartCoroutine()
	// must pass a Lambda expression. (ex: value => highscores = value)
	public static IEnumerator getScore(System.Action<string> highscores)
	{
		WWW getWWW = new WWW(GETSCORE_URL); // connect
		yield return getWWW; // wait until the download is done to get the result

		// log the error
		if(getWWW.error != null)
		{
			Debug.Log("There was an error getting the high score: " + getWWW.error);
			highscores(ERROR_STATE);
		}
		else
		{
			// assign the score for output
			highscores(getWWW.text);
		}

		// complete loading
		Debug.Log("Complete Loading: " + GETSCORE_URL);
	}

	// get the last score of 100 highscores
	// must be called by StartCoroutine()
	// must pass a Lambda expression. (ex: value => highscores = value)
	public static IEnumerator getScoreMin(System.Action<int> scoreMin)
	{
		WWW getWWW = new WWW(GETSCOREMIN_URL); // connect
		yield return getWWW; // wait until the download is done to get the result
		
		// log the error
		if(getWWW.error != null)
			Debug.Log("There was an error getting the min score: " + getWWW.error);
		
		// assign the score for output
		scoreMin(int.Parse(getWWW.text));
		
		// complete loading
		Debug.Log("Min Score: " + getWWW.text);
		Debug.Log("Complete Loading: " + GETSCOREMIN_URL);
	}

	// MD5 hash
	private static string MD5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}
}
