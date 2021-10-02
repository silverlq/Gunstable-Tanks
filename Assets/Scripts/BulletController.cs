using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float Life = 7f;
    public Material friendlyMat;
    public Material enemyMat;
    public MeshRenderer colorRenderer;

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

    public void Spawn(Vector3 position, Vector3 direction, bool friendly = false, float moveSpeed = 10f, float life = 7f)
    {
        transform.gameObject.SetActive(true);
        spawnTime = Time.time;
        moveDirection = direction;
        this.transform.position = position;
        this.friendly = friendly;
        this.MoveSpeed = moveSpeed;
        this.Life = life;
        colorRenderer.material = friendly ? friendlyMat : enemyMat;
    }
}
