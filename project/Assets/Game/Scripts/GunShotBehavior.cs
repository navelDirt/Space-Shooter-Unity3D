using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShotBehavior : ShotBehavior {

    private Rigidbody rigBody;
    public float speed = 400f;
    public float damage = 10f;
    public ParticleSystem hitAnimation;

    public override float GetDamage()
    {
        return damage;
    }

    public override ParticleSystem GetHitAnimation()
    {
        if (hitAnimation == null)
        {
            throw new Exception("Hit Animation is not defined in object " + this.name);
        }
        return hitAnimation;
    }

    public override float GetSpeed()
    {
        return speed;
    }

    public override Rigidbody GetRigidBody()
    {
        return rigBody;
    }

    // Use this for initialization
    void Start()
    {
        rigBody = GetComponent<Rigidbody>();

        rigBody.velocity = transform.forward * speed;

        Destroy(gameObject, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
