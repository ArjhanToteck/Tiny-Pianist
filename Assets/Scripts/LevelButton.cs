using TMPro;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public TMP_Text text;
	public object song;
	public string path;
	public string name;
	public bool builtInLevel;
	public int levelIndex;

	public void LoadSong()
	{
		if (!builtInLevel) {
			song = new Song(path);
		}

		LevelState.song = (Song)song;
		LevelState.name = name;
		LevelState.builtInLevel = builtInLevel;
		LevelState.levelIndex = levelIndex;
		FindObjectOfType<GameManager>().FadeOut("Game");
	}
}
