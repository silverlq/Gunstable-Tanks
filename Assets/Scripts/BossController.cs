using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    public int numGuns = 10;
    public float bossWidth = 40f;

    public float fireWaitTime = 1f;
    public float fireGapTime = 0.2f;
    public float zPosition = 40f;
    public float positionSmoothing = 0.15f;

    public GameObject bossGunPrefab;

    public float GuiHeight = 5f;

    private List<BossGun> bossGuns;
    private float lastFireTime;
    private int lastFireGun = 0;
    private bool dead = false;
    private float lastDead;

    // Start is called before the first frame update
    void Start()
    {
        lastFireTime = Time.time;
        bossGuns = new List<BossGun>();
        for (int i = 0; i < numGuns; i++)
        {
            GameObject gun = GameObject.Instantiate(bossGunPrefab, transform);
            gun.transform.localPosition = new Vector3(-bossWidth / 2f + 0.5f * bossWidth / (float)numGuns +  i * bossWidth / (float)numGuns,0f,0f);
            BossGun bossGun = gun.GetComponent<BossGun>();
            bossGun.Start();
            bossGuns.Add(bossGun);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AliveGuns() > 0 && Time.time > lastFireTime + fireGapTime + (lastFireGun == 0? fireWaitTime:0))
        {
            lastFireTime = Time.time;
            bossGuns[lastFireGun].FireGun();
            lastFireGun += 1;
            if (lastFireGun >= numGuns)
            {
                lastFireGun = 0;
            }
            if(bossGuns[lastFireGun].CoreHealth <= 0)
            {
                lastFireTime -= fireGapTime;
            }
        }

        UpdateBossPosition();

        if (AliveGuns() <= 0 && Time.time > lastFireTime + fireGapTime)
        {
            lastFireTime = Time.time;
            bossGuns[lastFireGun].explosion.Play();
            lastFireGun += 1;
            if (lastFireGun >= numGuns)
            {
                lastFireGun = 0;
            }
        }

        if(AliveGuns() <=0)
        {
            if (!dead)
            {
                dead = true;
                lastDead = Time.time;
            }
            if (Time.time > lastDead + 6f)
            {
                LevelManager.RestartLevel();
                LevelManager.player.DisableOnWin.ForEach(o => { o.SetActive(false); });
                LevelManager.player.EnableOnWin.ForEach(o => { o.SetActive(true); });
            }
        }
    }

    private int AliveGuns()
    {
        int count = 0;
        for (int i = 0; i < numGuns; i++)
        {
            if (bossGuns[i].CoreHealth > 0)
                count++;
        }
        return count;
    }

    public void Reset()
    {
        if (bossGuns == null)
            return;

        for (int i = 0; i < numGuns; i++)
        {
            bossGuns[i]?.Reset();
        }
        dead = false;
    }

    public void UpdateBossPosition(bool snap = false)
    {
        transform.position = Vector3.Lerp(transform.position, LevelManager.player.transform.position + new Vector3(0f, 0f, snap? zPosition*2 : zPosition), snap?1f:positionSmoothing);
    }

}
