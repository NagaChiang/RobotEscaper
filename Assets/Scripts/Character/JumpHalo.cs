using UnityEngine;
using System.Collections;

public class JumpHalo : MonoBehaviour {

	public float size; // the size of particle
	public float radius; // the radius of the position
	public float particlePeriod; // the period of a particle
	public float rotateDegree; // the rotation range
	public float heightLimit; // the height(y-axis) range
	public float degreeUnit; // every "degreeUnit" degrees, spawn one time
	public float rotateUnit; // rotate "rotateUnit" degrees at a time
	public float heightUnit; // move "heightUnit" at a time
	public Color color; // the color of particle

	float time; // for storing the time since the last particle spawned
	float i; // the degree of current position
	float deg; // the degree of current rotation
	float y; // the current height(y-axis)
	int rotateDirection; // 1 as rotate up; -1 as rotate down
	int heightDirection; // 1 as move up; -1 as move down

	Vector3 nextPos; // temporary storing the next position
	Quaternion nextQuate; // temporary storing the next Quatenion

	// show particle or not
	bool showParticle_1, showParticle_2;

	public void setParticleAmount(int n)
	{
		switch(n)
		{
		case 0:
			showParticle_1 = false;
			showParticle_2 = false;
			break;

		case 1:
			showParticle_1 = true;
			showParticle_2 = false;
			break;

		case 2:
			showParticle_1 = true;
			showParticle_2 = true;
			break;

		default:
			Debug.Log("Invalid amount of particles!");
			break;
		}
	}

	void Start()
	{
		// initialize
		time = 0;
		i = 0;
		deg = 0;
		y = 0;

		rotateDirection = 1; // rotate up
		heightDirection = 1; // move up

		// color
		color = Color.green;
		//color.a = 0.8f;
	}

	void Update()
	{
		time += Time.deltaTime; // timer

		// as timer reach the period
		if(time >= particlePeriod)
		{
			// rotate
			nextQuate.eulerAngles = new Vector3(0f, 0f, (float)deg);
			transform.rotation = nextQuate;

			// emit particle 1
			if(showParticle_1)
			{
				nextPos = new Vector3(radius*Mathf.Cos(Mathf.Deg2Rad * i), y, radius*Mathf.Sin(Mathf.Deg2Rad * i));
				particleSystem.Emit(nextPos, Vector3.zero, size, Time.deltaTime, color);
			}

			// emit particle 2 after 180 degree
			if(showParticle_2)
			{
				nextPos = new Vector3(radius*Mathf.Cos(Mathf.Deg2Rad * (i+180)), y, radius*Mathf.Sin(Mathf.Deg2Rad * (i+180)));
				particleSystem.Emit(nextPos, Vector3.zero, size, Time.deltaTime, color);
			}

			i = (i + degreeUnit)%360; // set next degree of position
			deg += rotateUnit*rotateDirection; // set next degree of rotation
			y += heightUnit*heightDirection;
			time = 0; // reset the timer

			// inverse rotate direction
			if((deg > rotateDegree) || (deg < rotateDegree*(-1)))
				rotateDirection = rotateDirection*(-1);

			// inverse height moving direction
			if((y > heightLimit) || (y < heightLimit*(-1)))
				heightDirection = heightDirection*(-1);
		}
	}
}
