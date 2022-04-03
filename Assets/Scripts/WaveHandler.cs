using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : MonoBehaviour
{
    public int wave = 0;
    [SerializeField]
    GameObject[] easyEnemies;
    [SerializeField]
    GameObject[] mediumEnemies;
    [SerializeField]
    GameObject[] hardEnemies;
    [SerializeField]
    GameObject lootObject;
    [SerializeField]
    GameObject indicatorObject;
    public AudioClip[] gameMusic;
    public int musicMaxWave = 25;

    int lastMusicId;

    GameManager gameManager;

    public GameObject spawnedLoot = null;
    List<GameObject> enemies = new List<GameObject>();
    public bool spawningWave;
    public int worldSizeX;
    public int worldSizeY;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.gameManager;
        gameManager.waveHandler = this;
        spawningWave = true;//should stop waves until we start
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!spawningWave && enemies.Count == 0)//should be no enemies
        {
            StartCoroutine(nextWave());
        }
    }

    public void BeginGame()
    {
        Flush();
        spawningWave = false;

    }

    public void Flush()
    {
        if (spawnedLoot != null)
        {
            Destroy(spawnedLoot);
        }
        wave = 0;
        foreach (GameObject obj in enemies)
        {
            Destroy(obj);
        }
        enemies = new List<GameObject>();
    }
    

    IEnumerator nextWave()
    {
        spawningWave = true;
        if (wave > 0)
        {
            spawnedLoot = Instantiate(lootObject, gameManager.player.rb.position * 0.5f, Quaternion.identity);
        }
        else
        {
            yield return new WaitForSeconds(0.4f);
        }
        while (spawnedLoot != null)
        {
            yield return new WaitForFixedUpdate();
        }

        wave += 1;
        Debug.Log(Mathf.Clamp((gameMusic.Length - 1) * (wave / ((float)musicMaxWave)), 0, gameMusic.Length - 1));
        int musicId = Mathf.RoundToInt(Mathf.Clamp((gameMusic.Length-1) * (wave/((float)musicMaxWave)),0,gameMusic.Length-1));
        if (musicId != lastMusicId)
        {
            lastMusicId = musicId;
            gameManager.musicHandler.SetTrack(gameMusic[musicId]);
        }
        gameManager.UpdateAchievements(false);
        EnemySpawn[] spawns = prepareWave(wave);

        displayWave(spawns,0.6f);

        yield return new WaitForSeconds(0.6f);
        spawnWave(spawns);
        spawningWave = false;
    }
    public void onEnemyDeath(GameObject enemy)
    {
        enemies.Remove(enemy);
    }
    
    EnemySpawn[] prepareWave(int wave)
    {
        List<EnemySpawn> spawns = new List<EnemySpawn>();
        float enemyCount = 1 + (0.62f * wave + 0.05f * Mathf.Pow(wave, 1.05f));//new enemies are very slow
        float enemyDifficulty = 0.8f + 0.6f * wave + 0.21f * Mathf.Pow(wave, 1.15f);//new difficulty is not, so we get more hard enemies not only in count, but in ratio as the game progresses

        //old values from day 1, as a backup:
        /*
        float enemyCount = 1 + (0.6f * wave + 0.1f * Mathf.Pow(wave, 1.1f));
        float enemyDifficulty = 0.8f + 0.4f * wave + 0.3f * Mathf.Pow(wave, 1.25f);
        hard enemy threshold was 2 above normal, with these stats:
        enemyDifficulty -= 2.4f;
        enemyCount -= 1.2f;
        normal enemy threshhold was normal, with these stats:
        enemyDifficulty -= 1.05f;
        enemyCount -= 0.4f;
        */
        int i = 0;
        while (i < enemyCount)
        {
            EnemySpawn spawn;
            GameObject enemyPrefab = null;
            if (enemyDifficulty >= enemyCount + 1.85f && hardEnemies.Length > 0)
            {
                enemyDifficulty -= 3.3f;
                enemyCount -= 2f;

                int id = Random.Range(0, hardEnemies.Length);
                if (id >= hardEnemies.Length)
                {
                    id = 0;
                }
                enemyPrefab = hardEnemies[id];

            }
            else if (enemyDifficulty >= enemyCount && mediumEnemies.Length > 0)
            {
                enemyDifficulty -= 1.25f;
                enemyCount -= 0.6f;

                int id = Random.Range(0, mediumEnemies.Length);
                if (id >= mediumEnemies.Length)
                {
                    id = 0;
                }
                enemyPrefab = mediumEnemies[id];
            }
            else
            {
                int id = Random.Range(0, easyEnemies.Length);
                if (id >= easyEnemies.Length)
                {
                    id = 0;
                }
                enemyPrefab = easyEnemies[id];
            }

            spawn = new EnemySpawn(Random.Range(-1 * worldSizeX, worldSizeX), Random.Range(-1 * worldSizeY, worldSizeY), enemyPrefab);
            spawns.Add(spawn);

            i += 1;
        }
        return spawns.ToArray();
    }

    void displayWave(EnemySpawn[] spawns, float duration)
    {
        for (int i = 0; i < spawns.Length; i++)
        {
            GameObject indicator = Instantiate(indicatorObject, new Vector3(spawns[i].x, spawns[i].y, 0), Quaternion.identity);
            DieTimer timer = indicator.GetComponent<DieTimer>();
            if (timer != null)
            {
                timer.timeUntilDeath = duration;
            }
        }
    }

    void spawnWave(EnemySpawn[] enemies)
    {
        for(int i = 0; i < enemies.Length; i++)
        {
            GameObject enemy = Instantiate(enemies[i].enemy, new Vector3(enemies[i].x, enemies[i].y, 0), Quaternion.identity);
            enemy.GetComponent<Enemy>().Initialize(enemies[i].enemy);
            this.enemies.Add(enemy);
        }
    }
    private class EnemySpawn
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public GameObject enemy { get; private set; }
        public EnemySpawn(int x, int y, GameObject enemy)
        {
            this.x = x;
            this.y = y;
            this.enemy = enemy;
        }
    }
}
