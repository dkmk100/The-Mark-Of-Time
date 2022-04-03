using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsDisplay : MonoBehaviour
{
    UIManager manager;
    [SerializeField]
    Text nameDisplay;
    string desc;
    Image rend;
    public RectTransform rect;
    public void Awake()
    {
        manager = GameManager.gameManager.uiManager;
        rend = this.GetComponent<Image>();
        rect = this.GetComponent<RectTransform>();
    }

    public void DisplayTooltip()
    {
        manager.DisplayTooltip(nameDisplay.text, desc);
    }
    public void HideTooltip()
    {
        manager.HideTooltip();
    }

    public void UpdateDisplay(Achievement achievement)
    {
        string displayName = "???";
        string displayDesc = "???";

        if (!achievement.hideName || achievement.completed)
        {
            displayName = achievement.name;
        }
        if (!achievement.hideDesc || achievement.completed)
        {
            displayDesc = achievement.description;
        }

        if (achievement.hidden && !achievement.completed)
        {
            rend.sprite = manager.secretAchievementSprite;
        }
        else
        {
            rend.sprite = achievement.sprite;
        }

        if (achievement.completed)
        {
            rend.color = new Color(1, 1, 1, 1);
        }
        else
        {
            rend.color = new Color(0, 0, 0, 0.6f);
        }

        nameDisplay.text = displayName;
        desc = displayDesc;
    }

}
