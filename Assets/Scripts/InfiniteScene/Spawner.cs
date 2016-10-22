using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	// spawn options
	public GameObject[] objs;
	public int spawnDistanceMin = 10;
	public int spawnDistanceMax = 20;
	int spawnNext; // next goal for cemera to reach

	// camera transform
	Transform mainCamera;

	void Awake()
	{
		// get the camera transform
		mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>().transform;
	}

	void Start() 
	{
		// set the first spawn goal
		spawnNext = spawnDistanceMin;

		// pre spawn
		spawn(); 
	}

	void Update()
	{
		if(mainCamera.position.x >= spawnNext)
		{
			spawn();
			spawnNext += Random.Range(spawnDistanceMin, spawnDistanceMax); // set next goal
		}
	}

	// spawn a new ground
	void spawn()
	{
		// spawn a random object at quad's position with no rotation
		Instantiate(objs[Random.Range(0, objs.GetLength(0))], transform.position, Quaternion.identity);

//		// if the GameObject has been disabled, stop invoking
//		if(gameObject.activeSelf)
//		{
//			// invoke itself at random time
//			Invoke("spawn", Random.Range(spawnTimeMin, spawnTimeMax));
//		}
	}
}
