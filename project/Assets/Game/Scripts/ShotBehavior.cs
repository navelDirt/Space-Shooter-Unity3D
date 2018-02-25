using System;
using UnityEngine;

public abstract class ShotBehavior : MonoBehaviour
{
    protected void OnTriggerEnter(Collider other)
    {        
        //TODO: fix this suspicious looking check
        if (other.gameObject.tag != "Player" && other.gameObject.tag != "LaserShot"
            && other.gameObject.tag != "WeaponEndLaser")
        {
            Debug.Log(gameObject.name + " has collided with " + other.gameObject.name);

            var trans = gameObject.transform;

            var animation = Instantiate(GetHitAnimation(), trans.position, trans.rotation * Quaternion.Euler(0, 180f, 0)) as ParticleSystem;
            
            animation.Play();

            Destroy(animation.gameObject, 1.0f);

            Destroy(this.gameObject);
        }
    }

    public abstract float GetSpeed();

    public abstract float GetDamage();

    public abstract ParticleSystem GetHitAnimation();

    public abstract Rigidbody GetRigidBody();
}