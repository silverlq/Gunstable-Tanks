using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float Life = 7f;
    public int damage = 3;
    public Material friendlyMat;
    public Material enemyMat;
    public MeshRenderer colorRenderer;
    public MeshRenderer opaqueRenderer;

    private float spawnTime;
    private Vector3 moveDirection;
    private bool friendly = false;
    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.Normalize(moveDirection) * MoveSpeed * Time.deltaTime;

        if(Time.time > spawnTime + Life)
        {
            gameObject.SetActive(false);
        }


    }

    public void Spawn(Vector3 position, Vector3 direction, bool friendly = false, float moveSpeed = 10f, float life = 7f, int damage = 3, bool critical = false)
    {
        if (critical)
        {
            opaqueRenderer.enabled = false;
            transform.localScale = Vector3.one*1.5f;
        }
        else
        {
            opaqueRenderer.enabled = true;
            transform.localScale = Vector3.one;
        }
        spawnTime = Time.time;
        moveDirection = direction;
        this.transform.position = position;
        this.friendly = friendly;
        this.MoveSpeed = moveSpeed;
        this.Life = life;
        this.damage = damage;
        colorRenderer.material = friendly ? friendlyMat : enemyMat;
        transform.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        HitBox hitBox = other.GetComponent<HitBox>();
        if (hitBox!=null)
        {
            if ((hitBox.tank is PlayerController && !friendly) || ((hitBox.tank is EnemyController || hitBox.tank is BossGun) && friendly))
            {
                hitBox.tank.GetHit(damage);
                transform.gameObject.SetActive(false);
            }
        }
    }
}
