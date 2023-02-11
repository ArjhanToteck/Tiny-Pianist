using Melanchall.DryWetMidi.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
	public GameManager gameManager;
	public GameObject noteContainer;
	public GameObject notePrefab;

	public Transform notesOrigin;
	public Transform playPoint;

	public GameObject octavePrefab;

	public bool menu = false;

	void Start()
	{
		menu = FindObjectOfType<GameManager>().menu;
	}

	public IEnumerator Menu()
	{
		// gets menu song files
		TextAsset[] songFiles = Resources.LoadAll<TextAsset>("Menu");

		// shuffles files
		for (int i = 0; i < songFiles.Length - 1; i++)
		{
			int random = Random.Range(i, songFiles.Length);
			TextAsset temp = songFiles[random];
			songFiles[random] = songFiles[i];
			songFiles[i] = temp;
		}

		// gets songs from files

		List<Song> songs = new List<Song>();

		foreach(TextAsset songFile in songFiles)
		{
			Stream stream = new MemoryStream(songFile.bytes);

			songs.Add(new Song(stream));

			stream.Close();
			stream.Dispose();
		}

		while (true)
		{
			foreach (Song song in songs)
			{
				yield return PlaySong(song);
			}
		}

		/*// gets menu songs
		string[] songPaths = FileManager.GetFilePathsInFolder(@"Songs\Menu");

		// shuffles song names
		for (int i = 0; i < songPaths.Length - 1; i++)
		{
			int rnd = Random.Range(i, songPaths.Length);
			string tempString = songPaths[rnd];
			songPaths[rnd] = songPaths[i];
			songPaths[i] = tempString;
		}

		while (true)
		{
			foreach (string songPath in songPaths)
			{
				Song song = new Song(songPath);

				yield return PlaySong(song);
			}
		}*/
	}

	public IEnumerator PlaySong(Song song)
	{
		List<PianoKey> keys = gameManager.keys;

		float time = 0;

		GameObject lastNoteObject = null;
		int lastNotePitch = -1;

		foreach (Song.Note note in song.rawNotes)
		{
			// waits until time for note to appear
			while (time < note.metricStartTime.TotalSeconds)
			{
				yield return null;
				time += Time.deltaTime;
			}

			// creates note object
			GameObject noteObject = Instantiate(notePrefab, noteContainer.transform);

			// identifies corresponding key to note
			PianoKey key = keys[Mathf.FloorToInt(note.pitch - (12 * Mathf.Floor(note.pitch / 12f)))];
			noteObject.name = key.name;

			// positions note prefab
			noteObject.transform.position = new Vector2(key.transform.position.x, notesOrigin.position.y);

			// sets variables in note
			noteObject.GetComponent<NoteController>().notesManager = this;
			noteObject.GetComponent<NoteController>().key = key;
			noteObject.GetComponent<NoteController>().note = note;
			noteObject.GetComponent<NoteController>().playPoint = playPoint.position;

			// enables collider only if in menu
			if (menu) noteObject.GetComponent<NoteController>().collider.enabled = true;

			// sets size and speed
			noteObject.transform.localScale = new Vector2(noteObject.transform.localScale.x, noteObject.transform.localScale.y * ((float)(note.musicalDuration.Numerator / (float)note.musicalDuration.Denominator) / (float)(1f / 4f) ));
			noteObject.GetComponent<NoteController>().speed /= (float)note.metricDuration.TotalSeconds / ((float)(note.musicalDuration.Numerator / (float)note.musicalDuration.Denominator) / (float)(1f / 4f));

			// makes skinnier if black note
			if(key.name.IndexOf('#') != -1) noteObject.transform.localScale = new Vector2(noteObject.transform.localScale.x / 1.5f, noteObject.transform.localScale.y);

			lastNoteObject = noteObject;
			lastNotePitch = note.pitch;
		}

		// waits for last note to finish playing before breaking
		while (!!lastNoteObject)
		{
			yield return null;
		}

		// wins level
		if(!menu) FindObjectOfType<GameManager>().LevelComplete();
	}

	void PrepareOctave(Song.Note note)
	{
		/*// creates octave
		GameObject newOctave = Instantiate(octavePrefab);

		if (!!octave)
		{
			newOctave.transform.position = octave.transform.position + new Vector3(0, -3);
		}
		else
		{
			newOctave.transform.position = new Vector3(0, -4);
		}

		if (lastNotePitch == -1)
		{ 
		}
		else if(lastNotePitch > note.pitch)
		{
			newOctave.transform.position += new Vector3(-4, 0);
		}
		else if (lastNotePitch < note.pitch)
		{
			newOctave.transform.position += new Vector3(4, 0);
		}

		octave = newOctave.GetComponent<OctaveController>();
		octave.ResetStartPosition();

		// sets new markers
		notesOrigin = octave.notesOrigin;
		playPoint = octave.playPoint;
		*/

		// gets the pitch of the C from the octave that the note is in
		int baseC = Mathf.FloorToInt(note.pitch / 12f) * 12;

		// sets pitch for every key in octave
		for (int i = 0; i < gameManager.keys.Count; i++)
		{
			gameManager.keys[i].pitch = baseC - 12 + i;
		}
	}
}
