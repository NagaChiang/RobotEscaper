using UnityEngine;
using System.Collections;

public class DifficultyManager : MonoBehaviour {

	// prefabs path
	const string GROUND_PREFAB_PATH = "Prefabs/Grounds/";

	// Main Camera HUD
	HUD mainCameraHUD;

	// Spawners' scripts
	Spawner spawnerTop, spawnerMiddle, spawnerBottom;

	// particle system of Stars effect
	ParticleSystem particleStars;

	// next goal
	int goal;
	int goalInterval = 50;

	void Awake()
	{
		// Main Camera HUD
		mainCameraHUD = GameObject.Find("Main Camera").GetComponent<HUD>();

		// Spawners' scripts
		spawnerTop = GameObject.Find("GroundSpawner_top").GetComponent<Spawner>();
		spawnerMiddle = GameObject.Find("GroundSpawner_middle").GetComponent<Spawner>();
		spawnerBottom = GameObject.Find("GroundSpawner_bottom").GetComponent<Spawner>();

		// particle system of Stars effect
		particleStars = GameObject.Find("Stars").particleSystem;
	}

	void Start()
	{
		// the first goal
		goal = goalInterval;
	}

	void Update()
	{

		// when reach the next goal, raise the difficulty
		if(mainCameraHUD.getScore() >= goal)
		{
			// 50m
			if(goal == 50)
			{
				// top
				spawnerTop.objs = new GameObject[] {
					getGround("GroundSmall"),
					getGround("MovingGroundMedium (horizontal)"),
					getGround("MovingGroundLarge (horizontal)")
				};

				// middle
				spawnerMiddle.objs = new GameObject[] {
					getGround("GroundSmall"),
					getGround("GroundMedium"),
					getGround("GroundLarge")
				};

				// bottom
				spawnerBottom.objs = new GameObject[] {
					getGround("GroundLarge")
				};
			}

			// 100m
			else if(goal == 100)
			{
				// top
				spawnerTop.objs = new GameObject[] {
					getGround("MovingGroundSmall (horizontal)"),
					getGround("MovingGroundMedium (horizontal)"),
					getGround("MovingGroundLarge (horizontal)")
				};

				// middle
				spawnerMiddle.objs = new GameObject[] {
					getGround("GroundSmall"),
					getGround("MovingGroundMedium (vertical)"),
					getGround("MovingGroundLarge (vertical)")
				};
				spawnerMiddle.spawnDistanceMin = 10;
				spawnerMiddle.spawnDistanceMax = 20;

				// bottom
				spawnerBottom.objs = new GameObject[] {
					getGround("GroundMedium"),
					getGround("MovingGroundLarge (horizontal)"),
					getGround("DropingGroundLarge")
				};

				// stars
				particleStars.emissionRate = 50;
				particleStars.startSpeed = 7;
			}

			// 150m
			else if(goal == 150)
			{
				// top
				spawnerTop.objs = new GameObject[] {
					getGround("MovingGroundSmall (horizontal)"),
					getGround("MovingGroundMedium (horizontal)"),
					getGround("DropingGroundLarge")
				};
				spawnerTop.spawnDistanceMin = 10;
				spawnerTop.spawnDistanceMax = 20;

				// middle
				spawnerMiddle.objs = new GameObject[] {
					getGround("GroundSmall"),
					getGround("MovingGroundMedium (vertical)"),
					getGround("MovingGroundLarge (vertical)"),
					getGround("DropingGroundLarge")
				};

				// bottom
				spawnerBottom.objs = new GameObject[] {
					getGround("MovingGroundMedium (horizontal)"),
					getGround("MovingGroundLarge (horizontal)"),
					getGround("DropingGroundMedium")
				};
				spawnerBottom.spawnDistanceMin = 10;
				spawnerBottom.spawnDistanceMax = 15;
			}

			// 200m
			else if(goal == 200)
			{
				// top
				spawnerTop.objs = new GameObject[] {
					getGround("MovingGroundSmall (vertical)"),
					getGround("MovingGroundMedium (vertical)"),
					getGround("DropingGroundMedium")
				};

				// middle
				spawnerMiddle.objs = new GameObject[] {
					getGround("MovingGroundMedium (horizontal)"),
					getGround("MovingGroundMedium (vertical)"),
					getGround("DropingGroundLarge")
				};

				// bottom
				spawnerBottom.objs = new GameObject[] {
					getGround("MovingGroundMedium (horizontal)"),
					getGround("DropingGroundMedium")
				};
			}

			// 250m
			else if(goal == 250)
			{
				// top
				spawnerTop.objs = new GameObject[] {
					getGround("MovingGroundSmall (horizontal)"),
					getGround("MovingGroundSmall (vertical)"),
					getGround("DropingGroundSmall"),
					getGround("DropingGroundMedium")
				};

				// middle
				spawnerMiddle.objs = new GameObject[] {
					getGround("MovingGroundMedium (horizontal)"),
					getGround("MovingGroundMedium (vertical)"),
					getGround("DropingGroundSmall"),
					getGround("DropingGroundMedium")
				};

				// bottom
				spawnerBottom.objs = new GameObject[] {
					getGround("MovingGroundSmall (horizontal)"),
					getGround("MovingGroundMedium (horizontal)"),
					getGround("MovingGroundMedium (vertical)"),
					getGround("DropingGroundMedium")
				};
			}

			// 300m
			else if(goal == 300)
			{
				// top
				spawnerTop.objs = new GameObject[] {
					getGround("MovingGroundSmall (vertical)"),
					getGround("DropingGroundSmall")
				};

				// middle
				spawnerMiddle.objs = new GameObject[] {
					getGround("MovingGroundSmall (horizontal)"),
					getGround("MovingGroundMedium (horizontal)"),
					getGround("MovingGroundMedium (vertical)"),
					getGround("DropingGroundSmall")
				};

				// bottom
				spawnerBottom.objs = new GameObject[] {
					getGround("MovingGroundSmall (vertical)"),
					getGround("MovingGroundSmall (horizontal)"),
					getGround("MovingGroundMedium (horizontal)"),
					getGround("DropingGroundMedium")
				};
			}

			// set the next goal
			goal += goalInterval;
		}
	}

	// get ground prefabs from resources
	GameObject getGround(string name)
	{
		GameObject obj;
		obj = (GameObject)Resources.Load<GameObject>(GROUND_PREFAB_PATH + name);

		if(obj == null) // if doesn't exist
		{
			Debug.LogWarning("The assigned prefabs name doesn't exist.");
			return null;
		}
		else // if exist
			return obj;
	}
}
