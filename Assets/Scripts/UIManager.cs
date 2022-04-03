using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Text text;
    GameManager gameManager;
    [SerializeField]
    GameObject gameOverScreen;
    [SerializeField]
    GameObject startScreen;
    [SerializeField]
    Text gameOverText;
    [SerializeField]
    GameObject achivementScreen;
    [SerializeField]
    GameObject howToPlayScreen;
    public Sprite secretAchievementSprite;
    public List<AchievementsDisplay> displays;
    public GameObject achievementPrefab;
    public RectTransform tooltip;
    bool onGameOverScreen;
    [SerializeField]
    Text tooltipName;
    [SerializeField]
    Text tooltipDesc;
    [SerializeField]
    RectTransform canvasTransform;
    List<NotificationDisplay> notifications = new List<NotificationDisplay>();
    [SerializeField]
    GameObject notificationPrefab;
    [SerializeField]
    GameObject notificationParent;
    [SerializeField]
    AudioClip notificationSound;
    [SerializeField]
    Text sfxText;
    [SerializeField]
    Text musicText;
    [SerializeField]
    GameObject achievementsParent;
    [SerializeField]
    AudioClip mainMenuMusic;
    private void Start()
    {
        gameManager = GameManager.gameManager;
        gameManager.uiManager = this;
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        CloseAllUI();
        startScreen.SetActive(true);
        gameManager.musicHandler.SetTrackInstant(mainMenuMusic);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.gameRunning)
        {
            text.text = "Score: " + gameManager.calcScore() + "\nLife Remaining: " + (Mathf.Round(gameManager.player.lifeRemaining * 10f) / 10) + "\nWave: " + gameManager.waveHandler.wave;
        }
        tooltip.anchoredPosition = new Vector2(Input.mousePosition.x/canvasTransform.localScale.x,Input.mousePosition.y/canvasTransform.localScale.y);

        const int spacing = 60;
        for(int i = 0; i < notifications.Count; i++)
        {
            notifications[i].remainingDuration -= Time.deltaTime;
            if (notifications[i].remainingDuration <= 0)
            {
                Destroy(notifications[i].gameObject);
                notifications.Remove(notifications[i]);
                i -= 1;
            }
            else
            {
                notifications[i].rect.anchoredPosition = new Vector2(0,i*spacing);
            }
        }
    }
    public void ToggleSFX()
    {
        gameManager.sfxDisabled = !gameManager.sfxDisabled;
        if (gameManager.sfxDisabled)
        {
            sfxText.text = "SFX off";
        }
        else
        {
            sfxText.text = "SFX on";
        }
    }
    public void ToggleMusic()
    {
        gameManager.musicDisabled = !gameManager.musicDisabled;
        gameManager.musicHandler.SetMusic(!gameManager.musicDisabled);//the not is because it needs to know if enabled
        if (gameManager.musicDisabled)
        {
            musicText.text = "Music off";
        }
        else
        {
            musicText.text = "Music on";
        }
    }
    public void AddNotification(string title, string name, string description, Sprite sprite, float duration)
    {
        NotificationDisplay notification = Instantiate(notificationPrefab, notificationParent.transform).GetComponent<NotificationDisplay>();
        notification.Initialize(title, name, description, sprite, duration);
        notifications.Add(notification);
        if (notificationSound != null && !gameManager.sfxDisabled)
        {
            AudioSource.PlayClipAtPoint(notificationSound, Vector3.forward * -10);
        }
    }
    public void NotifyAchievement(Achievement achievement)
    {
        AddNotification("Achievement Get!", achievement.name, achievement.description, achievement.sprite, 5f);
    }
    public void DisplayTooltip(string name, string desc)
    {
        tooltip.gameObject.SetActive(true);
        tooltipName.text = name;
        tooltipDesc.text = desc;
    }
    public void HideTooltip()
    {
        tooltip.gameObject.SetActive(false);
    }

    public void DisplayHowToPlayScreen()
    {
        CloseAllUI();
        howToPlayScreen.SetActive(true);
    }

    public void DisplayAchivements()
    {
        CloseAllUI();
        achivementScreen.SetActive(true);
        const int rowSize = 5;
        const int spacing = 90;
        const int spacingY = -90;
        const int startX = 45;
        const int startY = -40;
        List<Achievement> achievements = gameManager.achievementsManager.achievements;
        int i = 0;
        while (i < displays.Count || i<achievements.Count)
        {
            if (i >= displays.Count) {
                displays.Add(Instantiate(achievementPrefab,achievementsParent.transform).GetComponent<AchievementsDisplay>());
                displays[i].gameObject.SetActive(true);
                int row = i / rowSize;
                int col = i % rowSize;
                displays[i].UpdateDisplay(achievements[i]);
                displays[i].rect.anchoredPosition = new Vector2(col*spacing + startX, row * spacingY + startY);
            }
            else if(i>=achievements.Count)
            {
                displays[i].gameObject.SetActive(false);
            }
            else
            {
                displays[i].gameObject.SetActive(true);
                int row = i / rowSize;
                int col = i % rowSize;
                displays[i].UpdateDisplay(achievements[i]);
                displays[i].rect.anchoredPosition = new Vector2(col * spacing + startX, row * spacingY + startY);
            }
            i += 1;
        }
    }
    
    void CloseAllUI()
    {
        achivementScreen.SetActive(false);
        howToPlayScreen.SetActive(false);
        startScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        HideTooltip();
    }
    public void BackButton()
    {
        CloseAllUI();
        if (onGameOverScreen)
        {
            gameOverScreen.SetActive(true);
        }
        else
        {
            startScreen.SetActive(true);
        }
    }
    public void StartGame()
    {
        gameManager.StartGame();
        onGameOverScreen = false;
    }
    public void DisableStartScreen()
    {
        startScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }
    public void ShowGameOverScreen()
    {
        onGameOverScreen = true;
        string text = "Score: " + gameManager.score + ", Best Score: "+gameManager.bestScore;
        if(gameManager.bestScore == gameManager.score)
        {
            text += " New Best!!!";
        }
        text += "\nTime Survived: " + (Mathf.Round(gameManager.timeSurvived * 10f) / 10) + "s, Best Time: " + (Mathf.Round(gameManager.bestTime * 10f) / 10) + "s";
        if (gameManager.bestTime == gameManager.timeSurvived)
        {
            text += " New Best!!!";
        }

        text += "\nWave Reached: " + gameManager.waveHandler.wave;

        int statsId = Random.Range(0, gameManager.tempStats.Count);
        if (gameManager.tempStats.Count > 0)
        {
            GameManager.EnemyStats stats = gameManager.tempStats[statsId];
            int killed = stats.amountKilled;

            if (killed == 1)
            {
                text += "\nKilled " + killed + " enemy of type " + stats.enemyName + " this run";
            }
            else
            {
                text += "\nKilled " + killed + " enemies of type " + stats.enemyName + " this run";
            }
        }
        else
        {
            text += "\nYou... you didn't kill anything. Noble, I suppose. But foolish.";
        }

        gameOverText.text = text;
        gameOverScreen.SetActive(true);
    }
}
