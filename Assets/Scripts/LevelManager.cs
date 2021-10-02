using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelManager
{
    public static PlayerController player;
    public const int GRIDW = 5;
    public const int GRIDH = 4;

    private const int NBULLETS = 40;
    private static int maxEnemies;

    private static int[,] enemyGrid;

    private static Transform BulletHolder;
    private static Transform EnemyHolder;
    private static List<BulletController> bullets;
    private static List<EnemyController> enemies;

    public static void PrepareBullets(GameObject bulletPrefab)
    {
        if (bullets != null && bullets.Count > 0)
            return;

        bullets = new List<BulletController>();

        BulletHolder = new GameObject("BULLET_HOLDER").transform;

        for (int i = 0; i < NBULLETS; i++)
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, BulletHolder);
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bullets.Add(bulletController);
            bullet.SetActive(false);
        }
    }

    public static void PrepareEnemies(GameObject enemyPrefab)
    {
        if (enemies != null && enemies.Count > 0)
            return;

        enemies = new List<EnemyController>();
        CreateGrid();

        EnemyHolder = new GameObject("ENEMY_HOLDER").transform;

        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject enemy = GameObject.Instantiate(enemyPrefab, EnemyHolder);
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemies.Add(enemyController);
            enemy.SetActive(false);
        }
    }



    public static void SpawnBullet(Vector3 position, Vector3 direction, bool friendly = false, float moveSpeed = 10f, float life = 7f)
    {
        for (int i = 0; i < NBULLETS; i++)
        {
            if(!bullets[i].gameObject.activeSelf)
            {
                bullets[i].Spawn(position, direction, friendly, moveSpeed, life);
                break;
            }
        }
    }

    public static void SpawnEnemy()
    {
        List<int[]> availableSlots = AvailableSlotsForNewEnemy();
        if (availableSlots.Count > 0)
        {
            int[] slot = availableSlots[Random.Range(0, availableSlots.Count)];
            for (int i = 0; i < maxEnemies; i++)
            {
                if (!enemies[i].gameObject.activeSelf)
                {
                    enemies[i].Spawn(slot);
                    break;
                }
            }
        }
    }

    private static List<int[]> AvailableSlotsForNewEnemy()
    {
        List<int[]> slots = new List<int[]>();
        for (int i = 0; i < GRIDW; i++)
        {
            if (EnemyAtSlot(new int[2] { i, 0 }) == null)
                slots.Add(new int[2] {i, 0 });
        }
        return slots;
    }

    private static void CreateGrid()
    {
        enemyGrid = new int[GRIDW, GRIDH];
        maxEnemies = enemyGrid.Length;
        // Enemy position grid (Example 5 x 4)
        // X  X  X  X  X  - 5 positions
        // X  X  X  X  X  - 5 positions
        // X  X     X  X  - 4 positions
        // X           X  - 2 positions
        for (int x = 0; x < GRIDW; x++)
        {
            for (int y = 0; y < GRIDH; y++)
            {
                enemyGrid[x, y] = x + y;
                //If one away from player
                if (Mathf.Abs(x - (GRIDW - 1) / 2) + Mathf.Abs(y - (GRIDH - 1)) <= 1)
                    enemyGrid[x, y] = -1;
            }
        }

    }

    public static bool IsSlotFree (int[] xy)
    {
        if (enemyGrid[xy[0], xy[1]] == -1)
            return false;
        else if (EnemyAtSlot(xy) != null)
            return false;
        else
            return true;
    }

    public static EnemyController EnemyAtSlot (int[] xy)
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].Alive && enemies[i].gridSlotXY[0] == xy[0] && enemies[i].gridSlotXY[1] == xy[1])
                return enemies[i];
        }
        return null;
    }

    public static Vector3 GetSlot3dPosition(int[] xy)
    {
        return new Vector3(
            player.transform.position.x + (xy[0] - (GRIDW - 1) / 2) * player.enemyGridWidth / (GRIDW + 1),
            0f,
            player.transform.position.z - (xy[1] - (GRIDH - 1)) * player.enemyGridHeight / (GRIDH + 1) - player.enemyGridOffset
            );
    }

}
