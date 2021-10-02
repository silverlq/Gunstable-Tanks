using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelManager
{
    public static PlayerController player;
    private const int NBULLETS = 40;
    private const int NENEMIES = 16;

    private static EnemyController[] gridOccupancy;

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
        gridOccupancy = new EnemyController[16];

        EnemyHolder = new GameObject("ENEMY_HOLDER").transform;

        for (int i = 0; i < NENEMIES; i++)
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
        List<int> availableSlots = AvailableSlotsForNewEnemy();
        if (availableSlots.Count > 0)
        {
            int slot = availableSlots[Random.Range(0, availableSlots.Count)];
            for (int i = 0; i < NENEMIES; i++)
            {
                if (!enemies[i].gameObject.activeSelf)
                {
                    enemies[i].Spawn(slot);
                    break;
                }
            }
        }
    }

    private static List<int> AvailableSlotsForNewEnemy()
    {
        List<int> slots = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            if (gridOccupancy[i] == null)
                slots.Add(i);
        }
        return slots;
    }

    public static void ArrangeEnemies()
    {
        // Enemy position grid (Max 16 enemies at a time)
        // X  X  X  X  X  - 5 positions
        // X  X  X  X  X  - 5 positions
        // X  X     X  X  - 4 positions
        // X           X  - 2 positions
        for(int i = 0; i < enemies.Count; i++)
        {
            

        }

    }

    public static Vector3 GetSlot3dPosition(int[] xy)
    {
        return new Vector3(
            player.transform.position.x + (xy[0] - 2) * player.enemyGridWidth / 6,
            0f,
            player.transform.position.z - (xy[1] - 3) * player.enemyGridHeight / 5 - player.enemyGridOffset
            );
    }

    public static Vector3 GetSlot3dPosition(int slotId)
    {
        return GetSlot3dPosition(GetSlot2dPosition(slotId));
    }

    public static int[] GetSlot2dPosition(int slotId)
    {
        int x, y;
        if (slotId < 5)
        {
            x = slotId;
            y = 0;
        }
        else if (slotId < 10)
        {
            x = slotId - 5;
            y = 1;
        }
        else if (slotId < 12)
        {
            x = slotId - 10;
            y = 2;
        }
        else if (slotId < 14)
        {
            x = slotId - 10 + 1;
            y = 2;
        }
        else if (slotId < 15)
        {
            x = 0;
            y = 3;
        }
        else
        {
            x = 4;
            y = 3;
        }
        return new int[] { x, y };
    }

}
