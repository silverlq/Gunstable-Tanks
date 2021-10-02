using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public float MoveSpeed = 7f;
    public float SpeedSmoothing = 0.1f;
    public float RotationSmoothing = 0.1f;
    public float BulletSpeed = 10f;
    public float BulletLife = 7f;
    public float BulletCooldown = 0.8f;

    public Transform CoreModel;
    public Transform GunModel;

    private Transform bulletSpawnPoint;
    private float lastShotTime = Mathf.NegativeInfinity;
    internal Vector3 moveSpeed = Vector3.zero;
    internal Vector3 gunTargetPosition;
    internal Vector3 targetSpeed;
    internal Vector3 targetDirection;

    // Start is called before the first frame update
    public virtual void Start()
    {
        bulletSpawnPoint = GunModel.GetChild(0);
    }

    // Update is called once per frame
    internal virtual void Update()
    {
        targetSpeed = targetDirection * MoveSpeed * Time.deltaTime;

        moveSpeed = Vector3.Lerp(moveSpeed, targetSpeed, SpeedSmoothing);

        Quaternion targetRotation = Quaternion.LookRotation(targetSpeed, Vector3.up);

        transform.position += moveSpeed;
        if (targetSpeed.sqrMagnitude > 0.000000000001)
            CoreModel.rotation = BlenderSafeRotate(CoreModel.rotation, targetRotation, RotationSmoothing);

        UpdateGunDirection();
    }

    private void UpdateGunDirection ()
    {
        if(gunTargetPosition != null)
            GunModel.rotation = BlenderSafeRotate(GunModel.rotation, Quaternion.LookRotation(gunTargetPosition - transform.position, Vector3.up), RotationSmoothing);
    }

    internal void FireGun()
    {
        if(CanFire())
        { 
            LevelManager.SpawnBullet(bulletSpawnPoint.position, Vector3.Normalize(gunTargetPosition - transform.position), this is PlayerController, BulletSpeed, BulletLife);
            lastShotTime = Time.time;
        }
    }

    public bool CanFire()
    {
        return (Time.time > lastShotTime + BulletCooldown) ;
    }

    internal Quaternion BlenderSafeRotate(Quaternion currentRotation, Quaternion targetRotation, float smoothing)
    {
        return Quaternion.Lerp(currentRotation, Quaternion.Euler(currentRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0f), smoothing);
    }
}
