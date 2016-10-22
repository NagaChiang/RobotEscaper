using UnityEngine;
using System.Collections;

public class RunnerCamera : MonoBehaviour {

	public Transform player;
	public Transform backgroundCamera;

	[Range(0, 1)]
	public float cameraRotateSpeed;

	float originalCameraPosX;

	void Awake()
	{
		originalCameraPosX = transform.position.x; // store the original camera position
	}

	void Update () 
	{
		transform.position = new Vector3(player.position.x + 6, 0, -10); // follow the player

		// move the background
		backgroundCamera.Rotate(0f, (transform.position.x - originalCameraPosX)*cameraRotateSpeed, 0f);
	}
}
