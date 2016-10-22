using UnityEngine;

[RequireComponent(typeof(PlatformerCharacter2D))]
public class Platformer2DUserControl : MonoBehaviour 
{
	public bool alwaysRun; // if always run, set speed to 1
	float speed; // affected by "alwaysRun"

	private PlatformerCharacter2D character;
    private bool jump;
	private bool dash;

	void Awake()
	{
		character = GetComponent<PlatformerCharacter2D>();

		// Always run or not
		if(alwaysRun)
			speed = 1;
	}

    void Update ()
    {
        // Read the jump input in Update so button presses aren't missed.
		#if CROSS_PLATFORM_INPUT
        if (CrossPlatformInput.GetButtonDown("Jump")) jump = true;
		#else
		if(Input.GetButtonDown("Jump")) 
			jump = true;
		if(Input.GetButtonDown("Dash"))
			dash = true;
		#endif

    }

	void FixedUpdate()
	{
		// Read the inputs.
		//bool crouch = Input.GetKey(KeyCode.LeftControl);
		#if CROSS_PLATFORM_INPUT
		//float h = CrossPlatformInput.GetAxis("Horizontal");
		#else
		//float h = Input.GetAxis("Horizontal");
		#endif

		// Pass all parameters to the character control script.
		// edit: always move right, never crouch
		character.Move(speed, false, jump, dash);

        // Reset the input once it has been used.
	    jump = false;
		dash = false;
	}
}
