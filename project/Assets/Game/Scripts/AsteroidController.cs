using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    private float HitPoints = 100f;

    public float HitpointsToSizeRatio = 100f;

    public bool IsInmortal;

    public ParticleSystem DieAnimation;

    public GameObject godObject;

    public float animationSizeMultiplier = 5f;

    private WorldController worldController;

    private void Start()
    {
        worldController = godObject.GetComponent<WorldController>();

        float x = gameObject.transform.lossyScale.x;
        float y = gameObject.transform.lossyScale.y;
        float z = gameObject.transform.lossyScale.z;

        float meanScale = (x + y + z) / 3;

        HitPoints = meanScale * HitpointsToSizeRatio;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(this.gameObject.name + " has collided with " + other.gameObject.name);

        // if is a shot -> substract shot damage to current hitpoints
        var shotController = other.GetComponent<ShotBehavior>();

        // if is not a shot then shotController will be null
        if (shotController != null)
        {
            var damage = shotController.GetDamage();

            HitPoints = HitPoints - damage;
        }

        if (HitPoints <= 0f && !IsInmortal)
        {
            DestroyAsteroid();
        }
    }

    private void DestroyAsteroid()
    {
        var dieAnimation = Instantiate(DieAnimation, transform.position, transform.rotation) as ParticleSystem;

        var scale = transform.localScale;

        var main = dieAnimation.main;

        main.startSize = (scale.x + scale.y + scale.z) * animationSizeMultiplier;

        dieAnimation.Play();

        Destroy(dieAnimation, main.duration);

        Destroy(this.gameObject);
    }
}