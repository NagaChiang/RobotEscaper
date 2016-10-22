using UnityEngine;
using System.Collections;

public class MenuRobot : MonoBehaviour {

	Animator anim;

	void Awake()
	{
		anim = GetComponent<Animator>();
		anim.SetBool("Ground", true); // He's always on the ground
	}
}
