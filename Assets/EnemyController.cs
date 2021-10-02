using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Tank
{
    public float lockOnDistance = 10f;
    public int[] gridSlotXY;
    public float spawnOffset = 15f;
    public float positionRadius = 2.5f;
    public float positionTurnSpeed = 3f;

    public bool Alive = false;

    private int turnDir;
    private float startAngle;
    private Transform player;
    private Vector3 targetRelativePosition;

    // Start is called before the first frame update
    public override void Start()
    {
        player = LevelManager.player.transform;
        turnDir = Random.value > 0.5f?1:-1;
        startAngle = Random.value * 360f;
        base.Start();
    }

    // Update is called once per frame
    internal override void Update()
    {
        UpdateGunTarget();

        UpdateTargetPosition();

        base.Update();
    }


    private void UpdateTargetPosition()
    {
        var timePos = startAngle + Time.time * positionTurnSpeed * turnDir;
        Vector3 posDisplace = new Vector3(Mathf.Cos(timePos), 0f, -Mathf.Sin(timePos)) * positionRadius;
        targetDirection = Vector3.Normalize(LevelManager.GetSlot3dPosition(gridSlotXY) + posDisplace - transform.position);
    }

    public void Spawn(int[] gridSlotId)
    {
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

    private void UpdateGunTarget()
    {
        if (Vector3.Distance(player.position, transform.position) > lockOnDistance)
            gunTargetPosition = transform.position + new Vector3(0f, 0f, -1f);
        else
        {
            gunTargetPosition = player.position;
        }
    }
}
