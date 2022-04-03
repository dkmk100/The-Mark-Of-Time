using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public List<Achievement> achievements = new List<Achievement>();
    public List<Achievement> runAchievements = new List<Achievement>();
    public List<AchievementRequirement> triggered = new List<AchievementRequirement>();
    public GameManager manager;
    public int achievementsGotten;

    public void Start()
    {
        manager = GameManager.gameManager;
        manager.achievementsManager = this;
        for (int i = 0; i < achievements.Count; i++)
        {
            for (int i2 = 0; i2 < achievements[i].requirements.Length; i2++)
            {
                if(achievements[i].requirements[i2].type== RequirementType.triggered)
                {
                    triggered.Add(achievements[i].requirements[i2]);
                }
            }
        }
    }
    public void ActivateTrigger(string key)
    {
        for (int i = 0; i < triggered.Count; i++)
        {
            for(int i2 = 0; i2 <triggered[i].parameters.Length; i2++)
            {
                if (triggered[i].parameters[i] == key)
                {
                    triggered[i].completed = true;
                    break;
                }
            }
        }
        UpdateAchievements(false);
    }
    public void UpdateAchievements(bool endOfGame)
    {
        for(int i = 0; i < achievements.Count; i++)
        {
            if(achievements[i].tryComplete(this, endOfGame))
            {
                achievementsGotten += 1;//for the hints system
                runAchievements.Add(achievements[i]);//unused, might use later if there's time       update: probably won't have time for this kindo fancy UI stuff
                manager.uiManager.NotifyAchievement(achievements[i]);//so you see the banner
            }
        }
    }

}
[System.Serializable]
public class Achievement
{
    public string name;
    public bool completed;
    public bool requireGameEnd;
    public bool hidden;
    public bool hideName;
    public bool hideDesc;

    [TextArea]
    public string description;
    public Sprite sprite;
    public AchievementRequirement[] requirements;
    public bool tryComplete(AchievementsManager manager, bool endOfGame)
    {
        if (completed)
        {
            return false;//already completed, we don't wanna re-trigger stuff from on completion
        }
        else if (requireGameEnd && !endOfGame)
        {
            return false;
        }
        bool missing = false;
        foreach(AchievementRequirement req in requirements)
        {
            if (!req.tryComplete(manager.manager))
            {
                missing = true;
            }
        }
        if (!missing)
        {
            completed = true;
            for(int i=0;i<requirements.Length;i++)
            {
                requirements[i].completed = true;
            }
        }
        return completed;
    }
}
[System.Serializable]
public class AchievementRequirement
{
    public RequirementType type;
    public ComparisonType comparison;
    public string[] parameters;
    public bool singleRun;
    public bool completed;
    public float minValue;
    public bool tryComplete(GameManager manager)
    {
        //don't do logic if permanently done
        if (completed)
        {
            return true;
        }
        float value = 0;
        bool tinyComplete = false;
        if (type == RequirementType.wave)
        {
            value = manager.waveHandler.wave;
        }
        else if (type == RequirementType.score)
        {
            if (singleRun)
            {
                value = manager.score;
            }
            else
            {
                value = manager.bestScore;
            }
        }
        else if (type == RequirementType.time)
        {
            if (singleRun)
            {
                value = manager.timeSurvived;
            }
            else
            {
                value = manager.bestTime;
            }
        }
        else if (type == RequirementType.killEnemies && parameters.Length > 0)//check  to avoud crash on empty parameters
        {
            if (singleRun)
            {
                //temprorary stats
                int i = 0;
                while (i < manager.tempStats.Count)
                {
                    if (manager.tempStats[i].enemyName == parameters[0])
                    {
                        value = manager.tempStats[i].amountKilled;
                    }
                    i += 1;
                }
            }
            else
            {
                //permanent stats
                int i = 0;
                while (i < manager.killStats.Count)
                {
                    if (manager.killStats[i].enemyName == parameters[0])
                    {
                        value = manager.killStats[i].amountKilled;
                    }
                    i += 1;
                }
            }
        }
        else if (type == RequirementType.shots)
        {
            if (singleRun)
            {
                value = manager.shots;
            }
            else
            {
                value = manager.totalShots;
            }
        }
        else if (type == RequirementType.misses)
        {
            if (singleRun)
            {
                value = manager.misses;
            }
            else
            {
                value = manager.totalMisses;
            }
        }
        else if (type == RequirementType.movement)
        {
            if (singleRun)
            {
                value = manager.movedAmount;
            }
            else
            {
                value = manager.totalMovement;
            }
        }
        else if (type == RequirementType.damageTaken)
        {
                value = manager.damgeTaken;
        }
        bool valid = false;
        if(comparison == ComparisonType.greater)
        {
            valid = value >= minValue;
        }
        else if (comparison == ComparisonType.lesser)
        {
            valid = value <= minValue;
        }
        else if (comparison == ComparisonType.equal)
        {
            valid =  Mathf.Round(value) == Mathf.Round(minValue);//don't want them to need to be *too* exact, this works
        }
        else if (comparison == ComparisonType.notEqual)
        {
            valid = Mathf.Round(value) != Mathf.Round(minValue);//don't want them to need to be *too* exact, this works
        }
        else if (type == RequirementType.triggered)
        {
            valid = false;
        }
        if (valid)
        {
            tinyComplete = true;
        }
        if(tinyComplete && !singleRun)
        {
            completed = true;
        }
        return tinyComplete;
    }
}
[System.Serializable]
public enum RequirementType { score, time, wave, killEnemies, shots, misses, movement, damageTaken, triggered};
[System.Serializable]
public enum ComparisonType { greater, equal, lesser, notEqual }
