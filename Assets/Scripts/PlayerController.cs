using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Tank
{
    public GameObject bulletPrefab;
    public GameObject pickupPrefab;

    public float dashInputDelay = 0.08f;
    public float dashCooldown = 1.5f;
    public float dashDuration = 0.3f;
    public float dashSpeed = 4.5f;
    public float maxTrailWidth = 10f;
    public float TargetTravelDistance = 2000f;
    public Slider CoreHealthSlider;
    public Slider GunHealthSlider;
    public Slider ProgressSlider;

    private Vector3 dashVector = Vector3.zero;
    private Vector3 lastInputVector;
    private float lastInput = Mathf.NegativeInfinity;
    private float lastInputDown = Mathf.NegativeInfinity;
    private bool noInput = false;
    private float lastDash = Mathf.NegativeInfinity;
    private TrailRenderer trailRenderer;

    // Start is called before the first frame update
    public override void Start()
    {
        LevelManager.player = this;
        LevelManager.PrepareBullets(bulletPrefab);
        LevelManager.PreparePickups(pickupPrefab);
        trailRenderer = gameObject.GetComponentInChildren<TrailRenderer>();
        trailRenderer.enabled = false;
        base.Start();
    }

    // Update is called once per frame
    internal override void Update()
    {
        UpdateCursorPosition();
        Vector3 inputVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (inputVector.magnitude < 0.1f)
        { 
            targetDirection = new Vector3(0f, 0f, 0.5f); //Slowly go up if no input
            noInput = true;
        }
        else
        {
            //Dash
            if (noInput && Time.time - lastInput < dashInputDelay && //Has quickly pressed input after a short delay
                lastInput-lastInputDown < dashDuration && //Last input was a tap and not a long press
                Time.time > lastDash + dashCooldown && //Dash cooldown is over
                Vector3.Dot(Vector3.Normalize(lastInputVector), Vector3.Normalize(inputVector)) > 0.25f //Direction is similar to last input
                )
            {
                lastDash = Time.time;
                dashVector = Vector3.Normalize(inputVector) * dashSpeed;
                trailRenderer.enabled = true;
                trailRenderer.Clear();
                trailRenderer.AddPosition(trailRenderer.transform.position);
            }
            //Target Direction based on input
            targetDirection = Vector3.Normalize(inputVector);
            lastInput = Time.time;
            if(noInput)
            {
                lastInputDown = Time.time;
            }
            noInput = false;
            lastInputVector = inputVector;
        }

        //Add dash speed
        targetDirection += dashVector;
        //Stop dash
        if (Time.time > lastDash + dashDuration)
        {
            dashVector = Vector3.zero;
            trailRenderer.enabled = false;
        }
        //else
            //trailRenderer.widthMultiplier = (dashDuration - (Time.time - lastDash) / dashDuration) * maxTrailWidth;

        base.Update();

        if(Input.GetButton("Fire1"))
        {
            FireGun();
        }

        CoreHealthSlider.value = ((float)CoreHealth / (float)maxCore);
        GunHealthSlider.value = ((float)GunHealth / (float)maxGun);
        ProgressSlider.value = Mathf.Clamp01(transform.position.z/ TargetTravelDistance) ;
    }

    private void UpdateCursorPosition()
    {
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        // create a ray from the mousePosition
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // plane.Raycast returns the distance from the ray start to the hit point
        float distance;
        if (ground.Raycast(ray, out distance))
        {
            // some point of the plane was hit - get its coordinates
            var hitPoint = ray.GetPoint(distance);
            // use the hitPoint to aim your cannon
            if (hitPoint != null)
            {
                gunTargetPosition = hitPoint;
                Debug.DrawRay(hitPoint, Vector3.up);
            }
        }
    }

}
