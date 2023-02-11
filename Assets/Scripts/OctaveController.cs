using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctaveController : MonoBehaviour
{
	public Transform notesOrigin;
	public Transform playPoint;

	public List<PianoKey> keys;

	public void ResetStartPosition()
	{
		foreach(PianoKey key in keys)
		{
			key.ResetStartPosition();
		}
	}
}
