using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public bool menu = false;

	public Animator canvasAnimator;

	public TMP_Text levelName;
	public Animator levelFailedMenu;
	public TMP_Text levelFailedMessage;
	public Animator levelCompleteMenu;
	public Animator pauseMenu;
	public GameObject tutorial;
	public TMP_Text movementTutorial;

	public AudioClip middleC;

	public List<PianoKey> keys;
	public NotesManager notesManager;

	public Song song;

	public bool paused;
	public bool gameOver;

	void Start()
	{
		Time.timeScale = 1;

		if (menu)
		{
			StartCoroutine(notesManager.Menu());
		}
		else
		{
			if (LevelState.name == "1. Tutorial")
			{
				tutorial.SetActive(true);

				if (Application.platform == RuntimePlatform.Android)
				{
					movementTutorial.text = "Use the joystick on the bottom left to move around.";
				}
			}

			song = LevelState.song;
			levelName.text = LevelState.name;
			StartCoroutine(notesManager.PlaySong(song));
		}
	}

	public void FadeOut(string scene)
	{
		canvasAnimator.GetComponent<FadeEffect>().OnFadeOutEvent = () =>
		{
			SceneManager.LoadScene(scene);
		};

		canvasAnimator.SetTrigger("fadeOut");
	}

	public void LevelComplete()
	{
		gameOver = true;
		StopAllAudio();
		Time.timeScale = 0;
		levelCompleteMenu.SetTrigger("enter");
	}

	public void LevelFailed(string message)
	{
		gameOver = true;
		StopAllAudio();
		Time.timeScale = 0;
		levelFailedMenu.SetTrigger("enter");
		levelFailedMessage.text = message;
	}

	public void StopAllAudio()
	{
		foreach(AudioSource audioSource in FindObjectsOfType<AudioSource>())
		{
			audioSource.Stop();
		}
	}

	public void LoadNextLevel()
	{
		TextAsset file = Resources.LoadAll<TextAsset>("Levels")[LevelState.levelIndex + 1];
		LevelState.song = new Song(new MemoryStream(file.bytes));
		LevelState.name = file.name;
		LevelState.levelIndex++;
		LevelState.builtInLevel = true;
		FadeOut("Game");
	}

	void Update()
	{
		if (Input.GetButtonUp("Pause") && !gameOver)
		{
			Pause();
		}
	}

	public void Pause()
	{
		if (paused)
		{
			pauseMenu.SetTrigger("exit");
			Time.timeScale = 1;
			paused = false;
		}
		else
		{
			pauseMenu.SetTrigger("enter");
			StopAllAudio();
			Time.timeScale = 0;
			paused = true;
		}
	}

	public void OpenLink(string url)
	{
		Application.OpenURL(url);
	}

	public void Quit()
	{
		Application.Quit();
	}
}