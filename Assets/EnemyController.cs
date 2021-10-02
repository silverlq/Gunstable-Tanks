using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Tank
{
    public float lockOnDistance = 10f;
    public int gridSlot = -1;
    public float spawnOffset = 15f;

    private Transform player;
    private Vector3 targetRelativePosition;

    // Start is called before the first frame update
    public override void Start()
    {
        player = LevelManager.player.transform;
        base.Start();
    }

    // Update is called once per frame
    internal override void Update()
    {
        UpdateGunTarget();
        targetDirection = Vector3.Normalize(LevelManager.GetSlot3dPosition(gridSlot) - transform.position);

        base.Update();
    }


    private void ChooseTargetRelativePosition ()
    {

    }

    public void Spawn(int gridSlotId)
    {
        gameObject.SetActive(true);
        gridSlot = gridSlotId;
        transform.position = LevelManager.GetSlot3dPosition(gridSlot) + new Vector3(0f,0f,spawnOffset);
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
