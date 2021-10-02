using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunPickup : MonoBehaviour
{
    public float rotateSpeed = 7f;
    public float GunPercentHealth = 1f;

    private Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider = gameObject.GetComponentInChildren<Slider>();
    }

    public void Spawn(Vector3 position, float gunPercentHealth)
    {
        GunPercentHealth = gunPercentHealth;
        transform.position = position;
        if (slider == null)
        {
            slider = gameObject.GetComponentInChildren<Slider>();
        }

        SetSliderPosition();
        slider.value = GunPercentHealth;
        
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        SetSliderPosition();
        transform.rotation = transform.rotation * Quaternion.Euler(0f, rotateSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        HitBox hitBox = other.GetComponent<HitBox>();
        if (hitBox != null && hitBox.tank is PlayerController)
        {
            hitBox.tank.GetGun(GunPercentHealth);
            gameObject.SetActive(false);
        }
    }

    private void SetSliderPosition()
    {
        var sliderPos = Camera.main.WorldToScreenPoint(transform.position);
        slider.transform.position = sliderPos;
    }
}
