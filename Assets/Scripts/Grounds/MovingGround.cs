using UnityEngine;
using System.Collections;

public class MovingGround : MonoBehaviour {

	// direction
	public enum Axis {Horizontal, Vertical};
	public Axis axis;

	// speed and time
	public float halfLength;
	public float speed;

	// direction
	enum Direction {Positive=1, Negative=-1}; // 1 as rightward/upward; -1 as leftward/downward
	Direction direction;

	// the new speed is set or not
	bool isSet;

	// positions
	float negLimit, posLimit; // negtive limit and positive limit

	void Start()
	{
		// random direction
		if(Random.value > 0.5)
			direction = Direction.Positive;
		else
			direction = Direction.Negative;

		// Horizontal
		if(axis == Axis.Horizontal)
		{
			// positions
			negLimit = transform.position.x - halfLength; // negtive limit
			posLimit = transform.position.x + halfLength; // positive limit

			// initial state
			transform.position += new Vector3((-1)*((int)direction)*halfLength, 0f, 0f); // set position to the limit (inversion of direction)
			isSet = false; // wait to be set
		}

		// Vertical
		else if(axis == Axis.Vertical)
		{
			// positions
			negLimit = transform.position.y - halfLength; // negtive limit
			posLimit = transform.position.y + halfLength; // positive limit

			// initial state
			transform.position += new Vector3(0f, (-1)*((int)direction)*halfLength, 0f); // set position to the limit (inversion of direction)
			isSet = false; // wait to be set
		}
	}

	void FixedUpdate()
	{
		// Horizontal
		if(axis == Axis.Horizontal)
		{
			// as reach left limit, set direction to right
			if((transform.position.x <= negLimit) && (direction == Direction.Negative))
			{
				direction = Direction.Positive; // inverse direction
				isSet = false; // wait to be set
			}

			// as reach right limit, set direction to left
			else if((transform.position.x >= posLimit) && (direction == Direction.Positive))
			{
				direction = Direction.Negative; // inverse direction
				isSet = false; // wait to be set
			}


			// set the new speed
			if(!isSet)
			{
				rigidbody2D.velocity = new Vector2(((int)direction)*speed, 0f);
				isSet = true; // avoid to set more than once
			}
		}

		// Vertical
		else if(axis == Axis.Vertical)
		{
			// as reach bottom limit, set direction to right
			if((transform.position.y <= negLimit) && (direction == Direction.Negative))
			{
				direction = Direction.Positive; // inverse direction
				isSet = false; // wait to be set
			}
			
			// as reach right limit, set direction to left
			else if((transform.position.y >= posLimit) && (direction == Direction.Positive))
			{
				direction = Direction.Negative; // inverse direction
				isSet = false; // wait to be set
			}
			
			
			// set the new speed
			if(!isSet)
			{
				rigidbody2D.velocity = new Vector2(0f, ((int)direction)*speed);
				isSet = true; // avoid to set more than once
			}
		}
	}
}
