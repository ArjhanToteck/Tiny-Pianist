using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
	public GameObject levelsContainer;
	public GameObject builtInLevelsHeading;
	public GameObject customLevelsHeading;
	public GameObject customLevelsMessage;

	public GameObject levelButtonPrefab;

	void Start()
	{
		// built-in levels only need to be loaded once at start because they should not change during game
		LoadBuiltInLevels();

		// makes sure directory for custom songs exists
		Directory.CreateDirectory(Application.persistentDataPath + "/CustomSongs/");
	}

	void OnApplicationFocus(bool hasFocus)
	{
		// loads custom levels whenever game is focused
		if(hasFocus) LoadCustomLevels();
	}

	public void LoadBuiltInLevels()
    {
		// clears old levels

		// loops through every sibling index between the built-in level heading and custom level heading
		for (int i = builtInLevelsHeading.transform.GetSiblingIndex() + 1; i < customLevelsHeading.transform.GetSiblingIndex(); i++)
		{
			Destroy(builtInLevelsHeading.transform.parent.GetChild(i).gameObject);
		}

		// gets menu song files
		TextAsset[] songFiles = Resources.LoadAll<TextAsset>("Levels");

		LevelState.levelCount = songFiles.Length;

		// gets songs from files
		List<Song> songs = new List<Song>();

		foreach (TextAsset songFile in songFiles)
		{
			Stream stream = new MemoryStream(songFile.bytes);

			songs.Add(new Song(stream));

			stream.Close();
			stream.Dispose();
		}

		// loops through loaded files
		for (int i = 0; i < songs.Count; i++)
        {
			Song song = songs[i];

			// creates new button
			GameObject button = Instantiate(levelButtonPrefab);

            // moves button into levels container
            button.transform.SetParent(levelsContainer.transform, false);

			// sets position as sibling
            button.transform.SetSiblingIndex(builtInLevelsHeading.transform.GetSiblingIndex() + 1 + i);

			// sets name
			button.GetComponent<LevelButton>().text.text = songFiles[i].name;

			// sets controller variables
			button.GetComponent<LevelButton>().song = song;
			button.GetComponent<LevelButton>().name = songFiles[i].name;
			button.GetComponent<LevelButton>().builtInLevel = true;
			button.GetComponent<LevelButton>().levelIndex = i;
		}
	}

	// loops through files
	public void LoadCustomLevels()
	{
		// clears old levels

		// loops through every sibling index between custom level heading and the message saying there are no custom levels
		for (int i = customLevelsMessage.transform.GetSiblingIndex() + 1; i < customLevelsMessage.transform.parent.childCount; i++)
		{
			Destroy(customLevelsMessage.transform.parent.GetChild(i).gameObject);
		}

		// loads built in levels
		FileInfo[] files = FileManager.GetFileInfoInFolder(Application.persistentDataPath + "/CustomSongs/");

		for (int i = 0; i < files.Length; i++)
		{
			FileInfo file = files[i];

			// creates new button
			GameObject button = Instantiate(levelButtonPrefab);

			// moves button into levels container
			button.transform.SetParent(levelsContainer.transform, false);

			// sets position as sibling
			//button.transform.SetSiblingIndex(customLevelsHeading.transform.GetSiblingIndex() + 1 + i);

			// sets name
			button.GetComponent<LevelButton>().text.text = Path.GetFileNameWithoutExtension(file.Name);

			// sets controller variables
			button.GetComponent<LevelButton>().path = file.FullName;
			button.GetComponent<LevelButton>().name = Path.GetFileNameWithoutExtension(file.Name);
			button.GetComponent<LevelButton>().builtInLevel = false;
			button.GetComponent<LevelButton>().levelIndex = i;
		}
	}

	public void OpenCustomLevelsFolder()
	{
		Application.OpenURL(Application.persistentDataPath + "/CustomSongs/");
	}
}
