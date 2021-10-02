using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Tank
{
    public GameObject bulletPrefab;
    public GameObject enemyPrefab;
    public float enemyGridWidth = 60f;
    public float enemyGridHeight = 50f;
    public float enemyGridOffset = 6f;

    // Start is called before the first frame update
    public override void Start()
    {
        LevelManager.player = this;
        LevelManager.PrepareBullets(bulletPrefab);
        LevelManager.PrepareEnemies(enemyPrefab);
        LevelManager.SpawnEnemy(); LevelManager.SpawnEnemy(); LevelManager.SpawnEnemy(); LevelManager.SpawnEnemy(); LevelManager.SpawnEnemy(); LevelManager.SpawnEnemy();
        base.Start();
    }

    // Update is called once per frame
    internal override void Update()
    {
        UpdateCursorPosition();
        targetDirection = Vector3.Normalize(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")));

        base.Update();

        if(Input.GetButton("Fire1"))
        {
            FireGun();
        }

        DebugEnemyGrid();
    }

    private void UpdateCursorPosition()
    {
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        // create a ray from the mousePosition
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // plane.Raycast returns the distance from the ray start to the hit point
        float distance;
        if (ground.Raycast(ray, out distance))
        {
            // some point of the plane was hit - get its coordinates
            var hitPoint = ray.GetPoint(distance);
            // use the hitPoint to aim your cannon
            if (hitPoint != null)
            {
                gunTargetPosition = hitPoint;
                Debug.DrawRay(hitPoint, Vector3.up);
            }
        }
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
