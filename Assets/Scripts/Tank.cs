using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public float MoveSpeed = 7f;
    public float SpeedSmoothing = 0.1f;
    public float RotationSmoothing = 0.1f;

    public int CoreHealth = 9;
    public int GunHealth = 50;

    public float CriticalChance = 0.1f;
    public int GunDamage = 3;
    public int CriticalDamage = 9;
    public int GunCost = 3;
    public int CriticalCost = 10;
    public float BulletSpeed = 25f;
    public float CriticalSpeed = 31f;
    public float BulletLife = 7f;
    public float BulletCooldown = 0.8f;

    public Transform CoreModel;
    public Transform GunModel;
    public GameObject ExplosionPrefab;
    public GameObject SparksPrefab;

    internal int? maxCore = null;
    internal int? maxGun = null;
    internal ParticleSystem explosion;
    internal ParticleSystem sparks;
    private Transform bulletSpawnPoint;
    private float lastShotTime = Mathf.NegativeInfinity;
    internal Vector3 moveSpeed = Vector3.zero;
    internal Vector3 gunTargetPosition;
    internal Vector3 targetSpeed;
    internal Vector3 targetDirection;

    // Start is called before the first frame update
    public virtual void Start()
    {
        maxCore = CoreHealth;
        maxGun = GunHealth;
        bulletSpawnPoint = GunModel.GetChild(0);
        var explosionObj = GameObject.Instantiate(ExplosionPrefab, CoreModel);
        explosion = explosionObj.GetComponent<ParticleSystem>();
        explosion.Stop();
        var sparksObj = GameObject.Instantiate(SparksPrefab, CoreModel);
        sparks = sparksObj.GetComponent<ParticleSystem>();
        sparks.Stop();
    }

    // Update is called once per frameww
    internal virtual void Update()
    {
        targetSpeed = targetDirection * MoveSpeed * Time.deltaTime;

        moveSpeed = Vector3.Lerp(moveSpeed, targetSpeed, SpeedSmoothing);

        Quaternion targetRotation = Quaternion.LookRotation(targetSpeed, Vector3.up);

        transform.position += moveSpeed;
        if (targetSpeed.sqrMagnitude > 0.000000000001)
            CoreModel.rotation = BlenderSafeRotate(CoreModel.rotation, targetRotation, RotationSmoothing);

        UpdateGunDirection();

        GunModel.gameObject.SetActive(GunHealth > 0);
    }

    private void UpdateGunDirection()
    {
        if (gunTargetPosition != null)
            GunModel.rotation = BlenderSafeRotate(GunModel.rotation, Quaternion.LookRotation(gunTargetPosition - transform.position, Vector3.up), RotationSmoothing);
    }

    internal void FireGun()
    {
        if (CanFire())
        {
            bool critical = Random.value < CriticalChance;
            if (critical)
            {
                sparks.Play();
                targetDirection = Vector3.zero;
            }
            GunHealth -= critical ? CriticalCost : GunCost;
            if (GunHealth <= 0)
            {
                GunHealth = 0;
                explosion.Play();
            }
            LevelManager.SpawnBullet(bulletSpawnPoint.position, Vector3.Normalize(gunTargetPosition - transform.position), this is PlayerController, critical ? CriticalSpeed : BulletSpeed, BulletLife, critical ? CriticalDamage : GunDamage, critical);
            lastShotTime = Time.time;
        }
    }

    public bool CanFire()
    {
        return (GunHealth > 0 && CoreHealth > 0 && Time.time > lastShotTime + BulletCooldown);
    }

    public void GetHit(int damage)
    {
        sparks.Play();

        if (CoreHealth > 0)
        {
            CoreHealth -= damage;
            if (CoreHealth <= 0)
            {
                CoreHealth = 0;
                explosion.Play();
                if (this is EnemyController && GunHealth > 0)
                    LevelManager.SpawnPickup(transform.position, (float)GunHealth/ (float)maxGun);
                CoreModel.gameObject.SetActive(false);
            }

            if (GunHealth > 0)
            {
                GunHealth -= damage;
                if (GunHealth <= 0)
                {
                    GunHealth = 0;
                    explosion.Play();
                }
            }
        }
    }

    public void GetGun(float GunPercentHealth)
    {
        GunHealth += Mathf.Clamp(Mathf.CeilToInt((float)maxGun * GunPercentHealth), 0, (int)maxGun);
    }

    internal Quaternion BlenderSafeRotate(Quaternion currentRotation, Quaternion targetRotation, float smoothing)
    {
        return Quaternion.Lerp(currentRotation, Quaternion.Euler(currentRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0f), smoothing);
    }

    public Vector3 GetMoveVector()
    { return targetDirection * MoveSpeed;  }
}
