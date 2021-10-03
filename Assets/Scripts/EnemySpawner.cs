using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float enemyGridWidth = 60f;
    public float enemyGridHeight = 50f;
    public float enemyGridOffset = 6f;
    public float[] spawnTime = new float[2] { 4f, 12f };

    private float nextSpawn;

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.spawner = this;
        LevelManager.PrepareEnemies(enemyPrefab);
        ResetSpawnTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn && LevelManager.player.transform.position.z < LevelManager.player.TargetTravelDistance)
        {
            int difficulty = Mathf.RoundToInt(2 * (1 - (LevelManager.player.TargetTravelDistance - LevelManager.player.transform.position.z) / LevelManager.player.TargetTravelDistance));

            int nSpawn = Random.Range(1, difficulty + 1);
            for (int i = 0; i < nSpawn; i++)
            {
                LevelManager.SpawnEnemy();
            }
            ResetSpawnTime();
        }
        DebugEnemyGrid();
    }

    public void ResetSpawnTime()
    {
        nextSpawn = Time.time + Random.Range(spawnTime[0], spawnTime[1]);
    }


    private void DebugEnemyGrid()
    {
        for (int x = 0; x < LevelManager.GRIDW; x++)
        {
            for (int y = 0; y < LevelManager.GRIDH; y++)
            {
                if (LevelManager.IsSlotFree(new int[2] { x, y }))
                    Debug.DrawRay(LevelManager.GetSlot3dPosition(new int[2] { x, y }), Vector3.up);
            }
        }
    }
}
