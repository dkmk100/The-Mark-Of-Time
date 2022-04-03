using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    [SerializeField]
    int minAchievements;
    [SerializeField]
    Text displayText;
    [SerializeField]
    Image rend;
    [SerializeField]
    [TextArea]
    string hint;
    public void UpdateHint(int achievements, UIManager manager)
    {
        if (achievements >= minAchievements)
        {
            displayText.text = hint;
            rend.sprite = manager.hintSprite;
        }
        else
        {
            displayText.text = "Complete " + (minAchievements - achievements) + " more achievements to unlock this hint!";
            rend.sprite = manager.secretAchievementSprite;
        }
    }
}
