using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Components")]
	public CharacterController2D controller;
	public Rigidbody2D rigidbody;
	public VariableJoystick joystick;

	[Header("Motion Settings")]
	float xSpeed;
	public float speedFactor;
	public bool jumping = false;
	public bool falling = false;
	public bool justLanded = false;
	public float yBound;

	[Serializable]
	public class Parts
	{
		public GameObject weapon;
		public GameObject spell;
		public GameObject hair;
		public GameObject body;
		public GameObject arms;
		public GameObject helmet;
		public GameObject shirt;
		public GameObject sleeves;
		public GameObject pants;
	}

	private void Start()
	{
		// hides joystick if not android
		if(Application.platform != RuntimePlatform.Android)
		{
			joystick.gameObject.SetActive(false);
		}
	}

	void Update()
	{
		// only does these things while not paused
		if (Time.timeScale > 0)
		{
			// run

			// sets horizontal speed
			xSpeed = 0;

			if(Mathf.Abs(joystick.Direction.x) > 0.01f)
			{
				xSpeed = joystick.Direction.x * speedFactor;
			}
			else
			{
				xSpeed = Input.GetAxisRaw("Horizontal") * speedFactor;
			}

			// jump

			// checks if jump button pressed
			if ((Input.GetButton("Jump") || Input.GetAxis("Jump") > 0.5f || joystick.Direction.y > 0.5f) && !justLanded && !falling)
			{
				jumping = true;
			}
			else
			{
				justLanded = false;
			}
			// falling

			// falling if not on ground and accelerating downards
			falling = !controller.grounded; //&& rb.velocity.y < -0.1;
		}

		// boundary check
		if(transform.position.y < yBound)
		{
			FindObjectOfType<GameManager>().LevelFailed("Don't fall off the piano.");
		}
	}

	void FixedUpdate()
	{
		// moves player using parameters from before
		controller.Move(xSpeed * Time.deltaTime, false, jumping);

		// resets jumping variable
		jumping = false;
	}

	// turns player around without motion
	public void StillTurn()
	{
		if (Mathf.Abs(xSpeed) > 0.01f) return;

		if(transform.localScale.x > 0)
		{
			controller.Move(-0.000000001f, false, jumping);
		} 
		else
		{
			controller.Move(0.000000001f, false, jumping);
		}
	}
}
