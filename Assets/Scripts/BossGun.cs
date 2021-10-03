using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGun : Tank
{
    public float targetRadius = 4f;
    public float targetTurnSpeed = 1.5f;
    public float targetSmoothing = 0.1f;

    private float startTargetAngle;

    // Start is called before the first frame update
    public override void Start()
    {
        startTargetAngle = Random.value * 360f;
        base.Start();
    }

    // Update is called once per frame
    internal override void Update()
    {
        UpdateGunTarget();
        base.Update();
    }

    private void UpdateGunTarget()
    {
        var timePos = startTargetAngle + Time.time * targetTurnSpeed;
        Vector3 posDisplace = new Vector3(Mathf.Cos(timePos), 0f, -Mathf.Sin(timePos)) * targetRadius;
        var newTarget = LevelManager.player.transform.position + posDisplace + LevelManager.player.GetMoveVector();
        
        gunTargetPosition = Vector3.Lerp(gunTargetPosition, newTarget, targetSmoothing);
    }

    public void Reset()
    {
        if (maxCore != null && maxGun != null)
        {
            CoreHealth = (int)maxCore;
            GunHealth = (int)maxGun;
            explosion.Stop();
            sparks.Stop();
        }
    }
}
