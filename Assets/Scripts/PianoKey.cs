using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoKey : MonoBehaviour
{
	public Rigidbody2D rigidbody;
	public AudioSource audioSource;

	public bool menu;

	public float pressLimit;
	public float releaseSpeed = 2f;

	public int pitch;
	const float middleCFrequency = 264.072f;

	public Vector2 startPosition;
	public bool press = false;
	bool release = false;
	public bool sound = false;

	public NoteController noteController;

	void Start()
	{
		menu = FindObjectOfType<GameManager>().menu;
		ResetStartPosition();
	}

	public void ResetStartPosition()
	{
		startPosition = transform.position;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		// checks if collided with player
		if (collision.collider.CompareTag("Player") || (collision.collider.CompareTag("Note") && menu))
		{
			// checks if player is on top of key
			Vector2 pointOfContact = collision.contacts[0].normal;

			if (pointOfContact == new Vector2(0, -1))
			{
				// starts to press key
				release = false;
				press = true;
				rigidbody.isKinematic = false;
				rigidbody.mass = 0.005f;
				rigidbody.velocity = Vector2.zero;

				// play note
				sound = true;
				if(!collision.collider.CompareTag("Note")) StartCoroutine(Play());

				if(!!noteController) noteController.OnPlay();
			}
		}
	}

	void OnCollisionExit2D(Collision2D collision)
	{
		// checks if collided with player
		if (collision.collider.CompareTag("Player"))
		{
			ReleaseKey();
		}
	}

	void Update()
	{
		// checks if key is being pressed below limit
		if (press && (startPosition.y - rigidbody.position.y) > pressLimit)
		{
			// sets position to be perfectly at limit
			rigidbody.position = new Vector2(transform.position.x, startPosition.y - pressLimit);

			// stops key from falling anymore
			rigidbody.isKinematic = true;
			rigidbody.velocity = Vector2.zero;

			// stops presing key
			press = false;
		}

		if (release)
		{
			if (transform.position.y < startPosition.y)
			{
				transform.position = new Vector2(rigidbody.position.x, rigidbody.position.y + releaseSpeed * Time.deltaTime);
			}
			else
			{
				transform.position = startPosition;
				release = false;
			}			
		}
	}

	public IEnumerator Play()
	{
		if(!!noteController) pitch = noteController.note.pitch;

		// calculates frequency
		float frequency = 440;
		frequency *= Mathf.Pow(2, (pitch - 69) / 12f);
		float changeFromMiddleC = frequency / middleCFrequency;
		audioSource.pitch = changeFromMiddleC;

		// plays clip
		audioSource.Play();

		// waits until told to stop and minimum play time reached
		while (sound)
		{
			yield return null;
		}

		// stops playing note
		audioSource.Stop();

		yield break;

		/*AudioClip middleC = FindObjectOfType<GameManager>().middleC;

		// loads middle c data
		float[] middleCData = new float[middleC.samples * middleC.channels];
		middleC.GetData(middleCData, 0);

		// calculates frequency
		int sampleRate = 44100;
		float frequency = 440;
		frequency *= Mathf.Pow(2, (pitch - 69) / 12f);
		float changeFromMiddleC = frequency / middleCPitch;

		// creates clip
		AudioClip clip = AudioClip.Create(name, middleCData.Length, 1, sampleRate, false);

		float[] data = new float[middleCData.Length];

		// loops through data count
		int position = 0;
		while (position < data.Length)
		{
			// copies frequency from middle c sample
			float x = position * changeFromMiddleC;
			float y = middleCData[Mathf.FloorToInt(x - (middleCData.Length * Mathf.Floor(x / middleCData.Length)))];

			data[position] = y;
			position++;
		}

		// sets clip data
		clip.SetData(data, 0);

		// plays clip
		audioSource.clip = clip;
		audioSource.loop = true;
		audioSource.Play();

		// waits until told to stop
		while (sound)
		{
			yield return null;
		}

		// stops playing note
		audioSource.Stop();*/
	}

	public void StartSound()
	{
		// stops playing note
		sound = true;
		StartCoroutine(Play());
	}

	public void Stop()
	{
		// stops playing note
		sound = false;
	}

	public void ReleaseKey()
	{
		release = true;
		press = false;
		Stop();

		// stops key from falling anymore
		if (!!rigidbody)
		{
			rigidbody.isKinematic = true;
			rigidbody.velocity = Vector2.zero;
		}
	}
}