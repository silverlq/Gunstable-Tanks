using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelManager
{
    public static PlayerController player;
    public static EnemySpawner spawner;
    public const int GRIDW = 5;
    public const int GRIDH = 4;

    private const int NBULLETS = 40;
    private const int NPICKUPS = 20;
    private const int NCRATES = 20;
    private static int maxEnemies;

    public static int[,] EnemyGrid { get; private set; }

    private static Transform BulletHolder;
    private static Transform PickUpHolder;
    private static Transform EnemyHolder;
    private static Transform CrateHolder;
    private static List<BulletController> bullets;
    private static List<GunPickup> pickups;
    private static List<CratePickup> crates;
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

    public static void PreparePickups(GameObject pickupPrefab)
    {
        if (pickups != null && pickups.Count > 0)
            return;

        pickups = new List<GunPickup>();

        PickUpHolder = new GameObject("PICKUP_HOLDER").transform;

        for (int i = 0; i < NPICKUPS; i++)
        {
            GameObject pickup = GameObject.Instantiate(pickupPrefab, PickUpHolder);
            GunPickup gunPickup = pickup.GetComponent<GunPickup>();
            pickups.Add(gunPickup);
            pickup.SetActive(false);
        }
    }

    public static void PrepareCrates(GameObject cratePrefab)
    {
        if (crates != null && crates.Count > 0)
            return;

        crates = new List<CratePickup>();

        CrateHolder = new GameObject("CRATE_HOLDER").transform;

        for (int i = 0; i < NCRATES; i++)
        {
            GameObject crate = GameObject.Instantiate(cratePrefab, CrateHolder);
            CratePickup cratePickup = crate.GetComponent<CratePickup>();
            crates.Add(cratePickup);
            crate.SetActive(false);
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



    public static void SpawnBullet(Vector3 position, Vector3 direction, bool friendly = false, float moveSpeed = 10f, float life = 7f, int damage = 3, bool critical = false)
    {
        for (int i = 0; i < NBULLETS; i++)
        {
            if (!bullets[i].gameObject.activeSelf)
            {
                bullets[i].Spawn(position, direction, friendly, moveSpeed, life, damage, critical);
                break;
            }
        }
    }

    public static void SpawnPickup(Vector3 position, float percentGunHealth)
    {
        for (int i = 0; i < NPICKUPS; i++)
        {
            if (!pickups[i].gameObject.activeSelf)
            {
                pickups[i].Spawn(position, percentGunHealth);
                break;
            }
        }
    }

    public static void SpawnCrate()
    {
        for (int i = 0; i < NCRATES; i++)
        {
            if (!crates[i].gameObject.activeSelf)
            {
                crates[i].Spawn(new Vector3(player.transform.position.x+Random.Range(-1,1)*spawner.enemyGridWidth/3f,0f, player.transform.position.z + spawner.enemyGridHeight * 1.25f)) ;
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
        EnemyGrid = new int[GRIDW, GRIDH];
        maxEnemies = EnemyGrid.Length;
        // Enemy position grid (Example 5 x 4)
        // X  X  X  X  X  - 5 positions
        // X  X  X  X  X  - 5 positions
        // X  X     X  X  - 4 positions
        // X           X  - 2 positions
        for (int x = 0; x < GRIDW; x++)
        {
            for (int y = 0; y < GRIDH; y++)
            {
                EnemyGrid[x, y] = x + y;
                //If one away from player
                if (Mathf.Abs(x - (GRIDW - 1) / 2) + Mathf.Abs(y - (GRIDH - 1)) <= 1)
                    EnemyGrid[x, y] = -1;
            }
        }

    }

    public static bool IsSlotFree (int[] xy)
    {
        if (EnemyGrid[xy[0], xy[1]] == -1)
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
            player.transform.position.x + (xy[0] - (GRIDW - 1) / 2) * spawner.enemyGridWidth / (GRIDW + 1),
            0f,
            player.transform.position.z - (xy[1] - (GRIDH - 1)) * spawner.enemyGridHeight / (GRIDH + 1) - spawner.enemyGridOffset
            );
    }

    public static void RestartLevel()
    {
        player.CoreHealth = (int)player.maxCore;
        player.GunHealth = (int)player.maxGun;
        player.CoreModel.gameObject.SetActive(true);
        player.transform.position = Vector3.zero;
        player.ResetLocalVars();
        spawner.ResetSpawnTime();

        bullets.ForEach(o => { o.gameObject.SetActive(false); });
        crates.ForEach(o => { o.gameObject.SetActive(false); });
        enemies.ForEach(o => { o.gameObject.SetActive(false); });
        pickups.ForEach(o => { o.gameObject.SetActive(false); });
    }

}
