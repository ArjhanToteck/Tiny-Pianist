using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelButton : MonoBehaviour
{
    void OnEnable()
    {
        if(!LevelState.builtInLevel || LevelState.levelIndex >= LevelState.levelCount - 1) Destroy(gameObject);
    }
}
