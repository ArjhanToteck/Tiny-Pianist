using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
	public Action OnFadeOutEvent;
	public Action OnFadeInEvent;

	public void OnFadeOut()
	{
		OnFadeOutEvent();
	}

	public void OnFadeIn()
	{
		OnFadeInEvent();
	}

}
