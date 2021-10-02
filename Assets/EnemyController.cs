using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Tank
{
    public float lockOnDistance = 65f;
    public int[] gridSlotXY;
    public float spawnOffset = 15f;
    public float positionRadius = 2.5f;
    public float positionTurnSpeed = 3f;
    public float targetRadius = 4f;
    public float targetTurnSpeed = 1.5f;
    public float targetSmoothing = 0.1f;
    public float moveTime = 4f;

    public bool Alive = false;

    private bool driveAway = false;
    private float lastMove;
    private int turnDir;
    private float startAngle;
    private float startTargetAngle;
    private Transform player;
    private Vector3 targetRelativePosition;

    // Start is called before the first frame update
    public override void Start()
    {
        player = LevelManager.player.transform;
        turnDir = Random.value > 0.5f?1:-1;
        startAngle = Random.value * 360f;
        startTargetAngle = Random.value * 360f;
        base.Start();
    }

    // Update is called once per frame
    internal override void Update()
    {
        UpdateGunTarget();

        UpdateTargetPosition();

        base.Update();

        MoveInGrid();

        FireGun();

        //Die if off-screen
        if (transform.position.z < player.position.z - LevelManager.player.enemyGridHeight/2)
            Death();
    }


    private void UpdateTargetPosition()
    {
        var timePos = startAngle + Time.time * positionTurnSpeed * turnDir;
        Vector3 posDisplace = new Vector3(Mathf.Cos(timePos), 0f, -Mathf.Sin(timePos)) * positionRadius;
        targetDirection = Vector3.Normalize(LevelManager.GetSlot3dPosition(new int[2] { gridSlotXY[0], gridSlotXY[1] + (driveAway?10:0) }) + posDisplace - transform.position);
    }

    public void Spawn(int[] gridSlotId)
    {
        MoveSpeed = LevelManager.player.MoveSpeed;
        lastMove = Time.time + Random.value*moveTime;
        gridSlotXY = gridSlotId;
        Alive = true;
        gameObject.SetActive(true);
        transform.position = LevelManager.GetSlot3dPosition(gridSlotXY) + new Vector3(0f,0f,spawnOffset);
    }

    public void Death()
    {
        Alive = false;
        gameObject.SetActive(false);
    }

    private void MoveInGrid()
    {
        if(Time.time > lastMove + moveTime)
        {
            int[] nextSlot = new int[2] { gridSlotXY[0], gridSlotXY[1] + 1 };
            if (gridSlotXY[1] == LevelManager.GRIDH - 1) //At last row
            {
                MoveSpeed = LevelManager.player.MoveSpeed * 1.5f;
                driveAway = true;
            }
            else
            {
                //If downwards slot is too close to player move sideways
                if(LevelManager.EnemyGrid[nextSlot[0], nextSlot[1]] == -1)
                {
                    nextSlot[1] = gridSlotXY[1];
                    if(gridSlotXY[0] == 0)
                        nextSlot[0] = gridSlotXY[0] + 1;
                    else if (gridSlotXY[0] == LevelManager.GRIDW -1)
                        nextSlot[0] = gridSlotXY[0] - 1;
                    else
                        nextSlot[0] = gridSlotXY[0] +(Random.value>0.5f?1:-1);
                }

                if (LevelManager.IsSlotFree(nextSlot))
                {
                    gridSlotXY = nextSlot;
                    lastMove = Time.time;
                }
            }
        }
    }

    private void UpdateGunTarget()
    {
        Vector3 newTarget;
        if (driveAway || Mathf.Abs(player.position.z - transform.position.z) > lockOnDistance)
            newTarget = transform.position + new Vector3(0f, 0f, -1f);
        else
        {
            var timePos = startTargetAngle + Time.time * targetTurnSpeed * turnDir * -1;
            Vector3 posDisplace = new Vector3(Mathf.Cos(timePos), 0f, -Mathf.Sin(timePos)) * targetRadius;
            newTarget = player.position + posDisplace + LevelManager.player.GetMoveVector();
        }

        gunTargetPosition = Vector3.Lerp(gunTargetPosition, newTarget, targetSmoothing);
    }
}
