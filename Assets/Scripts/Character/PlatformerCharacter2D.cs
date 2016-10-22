using UnityEngine;
using System.Collections;

public class PlatformerCharacter2D : MonoBehaviour 
{
	// facing
	bool facingRight = true;							// For determining which way the player is currently facing.

	// speed and jump
	[SerializeField] float maxSpeed = 10f;				// The fastest the player can travel in the x axis.
	public float maxSpeedY; // to avoid bug: dash at the edge of ground will fly to the sky
	[SerializeField] float jumpForce = 400f;			// Amount of force added when the player jumps.	
	bool doubleJump = false; // has double jumped or not
	
	// dash
	[SerializeField] float dashSpeed = 70f;
	[SerializeField] float dashTime = 0.1f;
	[SerializeField] float dashCooldown = 0.5f;
	bool dashing = false; // is dashing or not
	bool airDashed = false; // can olny dash once in the air. reset on ground

	// crouch
	[Range(0, 1)]
	[SerializeField] float crouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%

	// air control
	[SerializeField] bool airControl = false;			// Whether or not a player can steer while jumping;

	// ground and ceiling check
	[SerializeField] LayerMask whatIsGround;			// A mask determining what is ground to the character
	//Transform groundCheck;								// A position marking where to check if the player is grounded.
	//float groundedRadius = .1f;							// Radius of the overlap circle to determine if grounded
	bool grounded = false;								// Whether or not the player is grounded.
	//Transform ceilingCheck;								// A position marking where to check for ceilings
	//float ceilingRadius = .01f;							// Radius of the overlap circle to determine if the player can stand up
	Transform LeftChecker_Start, LeftChecker_End; // vertices of left line checker
	Transform MiddleChecker_Start, MiddleChecker_End; // vertices of middle line checker
	Transform RightChecker_Start, RightChecker_End; // vertices of right line checker
	RaycastHit2D[] groundCheckHits = new RaycastHit2D[1]; // hit object through the line (only read in one)
	int leftCheck, middleCheck, rightCheck; // the amount of hits from the three lines

	// SFX
	public AudioSource audioExplosion; // audio for explode
	public AudioSource audioDash; // audio for dash

	// animator and other effect
	Animator anim;										// Reference to the player's animator component.
	ParticleSystem dashFlame; // the effect of dash
	ParticleSystem deadExplode; // the effect of gameover
	ParticleSystem jumpWave; // the effect of jump
	JumpHalo jumpHalo; // the effect of jump indicator

	// for other script to inform that it's gameover
	public void gameover()
	{
		// make it invisible
		gameObject.GetComponent<SpriteRenderer>().enabled = false;

		// cancel the control script
		GameObject.Find("2D Character").GetComponent<Platformer2DUserControl>().enabled = false;

		// Explosion SFX
		audioExplosion.Play();

		// dead explode effect
		deadExplode.transform.parent = null; // unparent the particle system so it won't follow the character anymore
		deadExplode.Play();
	}

    void Start()
	{
		// for ground and ceiling check
		//groundCheck = transform.Find("GroundCheck");
		//ceilingCheck = transform.Find("CeilingCheck");
		LeftChecker_Start = GameObject.Find("LeftChecker_Start").transform;
		LeftChecker_End = GameObject.Find("LeftChecker_End").transform;
		MiddleChecker_Start = GameObject.Find("MiddleChecker_Start").transform;
		MiddleChecker_End = GameObject.Find("MiddleChecker_End").transform;
		RightChecker_Start = GameObject.Find("RightChecker_Start").transform;
		RightChecker_End = GameObject.Find("RightChecker_End").transform;

		// animator
		anim = GetComponent<Animator>();

		// particle systems
		dashFlame = GameObject.Find("Dash Flame").GetComponent<ParticleSystem>();
		deadExplode = GameObject.Find("Dead Explode").GetComponent<ParticleSystem>();
		jumpWave = GameObject.Find("JumpWave").GetComponent<ParticleSystem>();

		// Scripts
		jumpHalo = GameObject.Find("JumpHalo").GetComponent<JumpHalo>();
	}

	void FixedUpdate()
	{
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		//grounded = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);
		//anim.SetBool("Ground", grounded);

		// to avoid bug: dash at the edge of ground will fly to the sky
		if(rigidbody2D.velocity.y > maxSpeedY)
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);

		// ground check
		groundCheck();

		// Set the vertical animation
		anim.SetFloat("vSpeed", rigidbody2D.velocity.y);

		// If it's on the ground
		if(grounded)
		{
			// reset the doubleJump and airDashed
			doubleJump = false;
			airDashed = false;

			// reset the amount of jumphalos
			jumpHalo.setParticleAmount(2);
		}
		else // not on the ground
		{
			if(!doubleJump) // hasn't double jumped; still can double jump
				jumpHalo.setParticleAmount(1);
			else // double jumped; can't double jump
				jumpHalo.setParticleAmount(0);
		}
	}


	public void Move(float move, bool crouch, bool jump, bool dash)
	{
		// If crouching, check to see if the character can stand up
//		if(!crouch && anim.GetBool("Crouch"))
//		{
//			// If the character has a ceiling preventing them from standing up, keep them crouching
//			if( Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround))
//				crouch = true;
//		}

//		// Set whether or not the character is crouching in the animator
//		anim.SetBool("Crouch", crouch);

		// dash
		if(dash && !dashing && (grounded || !airDashed))
		{
			if(!grounded) // if not on the ground, player can only dash once in the air
				airDashed = true;

			StartCoroutine(toDash()); // start the coroutine
		}

		// only control the player if grounded or airControl is turned on
		if((!dashing) && (grounded || airControl))
		{
			// Reduce the speed if crouching by the crouchSpeed multiplier
			move = (crouch ? move * crouchSpeed : move);

			// The Speed animator parameter is set to the absolute value of the horizontal input.
			anim.SetFloat("Speed", Mathf.Abs(move));

			// Move the character
			rigidbody2D.velocity = new Vector2(move * maxSpeed, rigidbody2D.velocity.y);
			
			// If the input is moving the player right and the player is facing left...
			if(move > 0 && !facingRight)
				// ... flip the player.
				Flip();
			// Otherwise if the input is moving the player left and the player is facing right...
			else if(move < 0 && facingRight)
				// ... flip the player.
				Flip();
		}

        // If the player should jump...
        if ((grounded || !doubleJump) && jump)
			toJump(); // perform jump
	}

	void toJump()
	{
		// Add a vertical force to the player.
		anim.SetBool("Ground", false);

		// jump effect
		jumpWave.Play();
		
		// zero out the y velocity
		rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
		
		rigidbody2D.AddForce(new Vector2(0f, jumpForce));
		
		if(!grounded)
			doubleJump = true;
	}
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	// coroutine for dash
	IEnumerator toDash()
	{
		dashing = true; // set dashing to true so that we can't keep dashing

		audioDash.Play(); // SFX

		anim.SetBool("Dash", true); // update the animator
		dashFlame.Play(); // play the dash flame effect

		rigidbody2D.velocity = new Vector2(dashSpeed, 0f); // set the speed of dash
		yield return new WaitForSeconds(dashTime); // the dash duration

		rigidbody2D.velocity = new Vector2(maxSpeed, rigidbody2D.velocity.y); // remove the dash and recover the run speed
		anim.SetBool("Dash", false); // update the animator

		yield return new WaitForSeconds(dashCooldown); // cooldown
		dashing = false; // set back to true to enable another dash
	}

	// ground check in FixedUpdate
	void groundCheck()
	{
		// check the intersections
		leftCheck = Physics2D.LinecastNonAlloc(LeftChecker_Start.position, LeftChecker_End.position, groundCheckHits, whatIsGround);
		middleCheck = Physics2D.LinecastNonAlloc(MiddleChecker_Start.position, MiddleChecker_End.position, groundCheckHits, whatIsGround);
		rightCheck = Physics2D.LinecastNonAlloc(RightChecker_Start.position, RightChecker_End.position, groundCheckHits, whatIsGround);

		// if there is any checker return more than 1 object
		if(leftCheck > 0 || middleCheck > 0 || rightCheck > 0)
			grounded = true;
		else
			grounded = false;

		anim.SetBool ("Ground", grounded);
	}
}
