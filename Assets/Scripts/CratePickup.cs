using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CratePickup : MonoBehaviour
{

    public float GunPercentHealth = 0.4f;
    public float CorePercentHealth = 0.4f;
    public float ChanceCore = 0.7f;
    public GameObject CoreUI;
    public GameObject GunUI;
    public float GuiHeight = 2f;

    private bool isCore = false;
    // Start is called before the first frame update
    void Update()
    {
        SetTextPosition();
    }

    // Update is called once per frame
    public void Spawn(Vector3 position)
    {
        transform.position = position;
        isCore = Random.value > ChanceCore;
        CoreUI.SetActive(isCore);
        GunUI.SetActive(!isCore);
        SetTextPosition();
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        HitBox hitBox = other.GetComponent<HitBox>();
        if (hitBox != null && hitBox.tank is PlayerController)
        {
            if(isCore)
                hitBox.tank.GetCore(CorePercentHealth);
            else
                hitBox.tank.GetGun(GunPercentHealth);
            gameObject.SetActive(false);
        }
    }

    private void SetTextPosition()
    {
        var textPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, GuiHeight, 0f));
        GunUI.transform.position = textPos;
        CoreUI.transform.position = textPos;
    }
}
