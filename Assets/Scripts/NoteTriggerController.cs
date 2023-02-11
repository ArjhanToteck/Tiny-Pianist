using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTriggerController : MonoBehaviour
{
	public NoteController noteController;

	void OnTriggerEnter2D(Collider2D collision)
	{
		noteController.OnNoteTriggerEnter(collision);
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		noteController.OnNoteTriggerExit(collision);
	}
}
