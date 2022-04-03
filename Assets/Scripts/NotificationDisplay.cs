using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationDisplay : MonoBehaviour
{
    public Text titleText;
    public Text nameText;
    public Text descriptionText;
    public Image image;
    public float remainingDuration;
    public RectTransform rect;

    public void Initialize(string title, string name, string description, Sprite sprite, float duration)
    {
        titleText.text = title;
        nameText.text = name;
        descriptionText.text = description;
        image.sprite = sprite;
        remainingDuration = duration;
    }
}
