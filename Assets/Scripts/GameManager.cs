using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public WaveHandler waveHandler;
    public UIManager uiManager;
    public AchievementsManager achievementsManager;
    public MusicHandler musicHandler;
    public Player player;
    public int score = 0;
    public int bestScore;
    public int misses = 0;
    public int shots = 0;
    public float movedAmount;
    public float damgeTaken;
    public int totalMisses;
    public int totalShots;
    public float totalMovement;
    public float bestTime;
    public bool ui = false;
    public bool sfxDisabled = false;
    public bool musicDisabled = false;
    public bool gameRunning = false;
    public bool killAll;
    public float timeSurvived = 0;
    public float worldSizeX = 9.5f;
    public float worldSizeY = 4.5f;
    bool paused;
    public List<EnemyStats> killStats = new List<EnemyStats>();
    public List<EnemyStats> tempStats = new List<EnemyStats>();

    public int calcScore()
    {
        if (gameRunning)
        {
            return score + Mathf.RoundToInt(timeSurvived * 0.2f + waveHandler.wave * 1.2f);
        }
        else
        {
            return score;
        }
    }
    public void addScore(int amount)
    {
        score += amount;
    }
    private void Awake()
    {
        GameManager.gameManager = this;
        ui = true;
        gameRunning = false;
    }
    public void TogglePause()
    {
        if (paused)
        {
            UnPause();
        }
        else
        {
            Pause();
        }
    }
    public void Pause()
    {
        Time.timeScale = 0.0f;
        if (gameRunning)
        {
            gameRunning = false;
            paused = true;
            uiManager.ShowPauseScreen();
        }
    }
    public void UnPause()
    {
        Time.timeScale = 1.0f;
        uiManager.HidePauseScreen();
        if (paused)
        {
            gameRunning = true;
            paused = false;
        }
    }
    public void GiveUp()
    {
        //so you can't cheese stuff
        movedAmount += 2;
        misses += 2;
        if (calcScore() == 42)
        {
            score -=3;
        }
        UnPause();
        player.onDeath();
    }
    private void Start()
    {
        player.gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        if (gameRunning)
        {
            timeSurvived += Time.fixedDeltaTime;
        }
    }
    public void UpdateAchievements(bool gameEnded)
    {
        achievementsManager.UpdateAchievements(gameEnded);
    }
    public void OnEnemyDestroyed(Enemy enemy)
    {
        waveHandler.onEnemyDeath(enemy.gameObject);

        //update permanent kill stats:
        EnemyStats stats = null;
        foreach(EnemyStats kills in killStats)
        {
            if(kills.enemyName == enemy.enemyName)
            {
                stats = kills;
            }
        }
        if (stats == null) //new stats needed, so add enemy type to the list
        {
            stats = new EnemyStats();
            stats.enemyName = enemy.enemyName;
            killStats.Add(stats);
        }
        stats.amountKilled += 1;

        //do it again but with temp stats:
        EnemyStats stats2 = null;
        foreach (EnemyStats kills in tempStats)
        {
            if (kills.enemyName == enemy.enemyName)
            {
                stats2 = kills;
            }
        }
        if (stats2 == null) //new temp stats needed, so add enemy type to the list
        {
            stats2 = new EnemyStats();
            stats2.enemyName = enemy.enemyName;
            tempStats.Add(stats2);
        }
        stats2.amountKilled += 1;

        UpdateAchievements(false);
    }
    public class EnemyStats
    {
        public string enemyName;
        public int amountKilled;
        public int killedBy;//don't know if I'll have time to implement this as it involves messing with bullets
    }
    public void StartGame()
    {
        ui = false;
        score = 0;
        timeSurvived = 0;
        shots = 0;
        movedAmount = 0;
        misses = 0;
        damgeTaken = 0;
        player.gameObject.SetActive(true);
        tempStats = new List<EnemyStats>();//flush the temporary stats to refresh them. I love GC!
        achievementsManager.runAchievements = new List<Achievement>();//reset achivements this run
        waveHandler.Flush();
        musicHandler.SetTrackInstant(waveHandler.gameMusic[0]);
        uiManager.DisableStartScreen();
        StartCoroutine(DoStartGame());
    }
    IEnumerator DoStartGame()
    {
        yield return new WaitForSeconds(0.3f);
        gameRunning = true;
        waveHandler.BeginGame();//redo waves and stuff
        player.Initialize();//sets initial cooldown so that start button doesn't make you shoot
    }
    public void EndGame()
    {
        gameRunning = false;
        ui = true;
        waveHandler.spawningWave = true;//to make waves not spawn when dead, just in case
        totalShots += shots;
        totalMisses += misses;
        totalMovement += movedAmount;
        CalculateScoreBonus();
        UpdateAchievements(true);
        StartCoroutine(ShowGameOverScreen());
    }
    IEnumerator ShowGameOverScreen()
    {
        yield return new WaitForSeconds(0.5f);
        player.gameObject.SetActive(false);
        uiManager.ShowGameOverScreen();
        waveHandler.Flush();
    }
    void CalculateScoreBonus()
    {
        score += Mathf.RoundToInt(timeSurvived*0.2f + waveHandler.wave*1.2f);
        if (score > bestScore)
        {
            bestScore = score;
        }
        if(timeSurvived > bestTime)
        {
            bestTime = timeSurvived;
        }
    }

}
