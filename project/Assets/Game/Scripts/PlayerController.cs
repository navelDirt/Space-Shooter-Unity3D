using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private float acceleration;
    private float deceleration;
    private float roll;
    private float pitch;
    private float yaw;
    private float throttle;
    private float speed;
    private float deltaDistance;
    private bool gamePaused;
    private GameObject newProjectile;
    private WorldController worldController;

    public float maxSpeed;
    public float turnSpeed;
    public float defaultAcceleration;
    public float defaultBrakesDeceleration;
    public float angularDrag;
    public float agility = 1;
    public float throttleSensitivity = 1.0f;
    public int fireMode = 1;
    public GameObject laserShot;
    public GameObject gunShot;
    public GameObject godObject;

    public ParticleSystem engine1;
    public ParticleSystem engine2;
    public ParticleSystem engine3;

    public Text rollText;
    public Text rollTextBackground;
    public Text pitchText;
    public Text pitchTextBackground;
    public Text throttleText;
    public Text throttleTextBackground;
    public Text DeltaDistanceText;
    public Text DeltaDistanceTextBackground;
    public Text AccelerationText;
    public Text AccelerationTextBackground;
    public Text DeltaTimeText;
    public Text DeltaTimeTextBackground;
    public Text SpeedText;
    public Text SpeedTextBackground;


    public float fireRateLaser = 0.5F;
    public float fireRateMiniGun = 0.1f;
    public float throttleDelta = 0.5F;

    private float nextFire = 0.5F;
    private float nextFire2 = 0.5f;
    private float nextThrottle = 0.5f;

    private float myTime = 0.0F;
    private float myTime2 = 0.0F;

    // Use this for initialization
    void Start () {
        worldController = godObject.GetComponent<WorldController>();
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (Time.timeScale == 1.0f)
            {
                Time.timeScale = 0.0f;
                gamePaused = true;
            }
            else
            {
                Time.timeScale = 1.0f;
                gamePaused = false;
            }
        }
        
        if (!gamePaused)
        {
            FireWeapons();
        }
    }

    private void FireWeapons()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            FireLasers();
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            FireMiniGuns();
        }
    }

    private void FireMiniGuns()
    {
        myTime2 = myTime2 + Time.deltaTime;

        if (myTime2 > nextFire2)
        {
            var weaponEnds = GameObject.FindGameObjectsWithTag("WeaponEndMiniGun");

            foreach (var item in weaponEnds)
            {
                var trans = item.transform;

                nextFire2 = myTime2 + fireRateMiniGun;

                Instantiate(gunShot, trans.position, trans.rotation);

                nextFire2 = nextFire2 - myTime2;
                myTime2 = 0.0F;
            }
        }
    }

    private void FireLasers()
    {
        myTime = myTime + Time.deltaTime;

        if (myTime > nextFire)
        {
            var weaponEnds = GameObject.FindGameObjectsWithTag("WeaponEndLaser");

            foreach (var item in weaponEnds)
            {
                var trans = item.transform;

                nextFire = myTime + fireRateLaser;

                Instantiate(laserShot, trans.position, trans.rotation);

                nextFire = nextFire - myTime;
                myTime = 0.0F;
            }
        }            
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (!gamePaused)
        {
            ReadInput();

            CalculateAcceleration();

            Turn();

            CalculateDeltaDistance();

            Move();

            CalculateFinalSpeed();

            SetEngineTrails();

            //setDebugTexts(); 
        }
    }

    private void SetEngineTrails()
    {
        SetTrailSize(engine1);
        SetTrailSize(engine2);
        SetTrailSize(engine3);
    }

    private void SetTrailSize(ParticleSystem engine)
    {
        var sz = engine.sizeOverLifetime;
        sz.enabled = true;

        AnimationCurve curve = new AnimationCurve();

        float maxLifetime = throttle;

        var main = engine.main;

        main.startLifetime = maxLifetime;
    }

    private void CalculateTurnDrag()
    {
        // speed reduction factor (between 0 and 1)
        float srFactor = 0;

        if (pitch != 0)
        {
            srFactor += Math.Abs(pitch) * angularDrag;
        }   
        if (yaw != 0)
        {
            srFactor += Math.Abs(yaw) * angularDrag;
        }
        // pitch and yaw are values between 0 and 1 so srFactor will never be >1

        acceleration = acceleration - speed * srFactor;
    }

    private void ReadInput()
    {
        // Read input for the pitch, yaw, roll and throttle
        roll = Input.GetAxis("Horizontal");
        roll = roll > 1 ? 1 : (roll < -1 ? -1 : roll);

        pitch = Input.GetAxis("Vertical");
        pitch = pitch > 1 ? 1 : (pitch < -1 ? -1 : pitch);

        yaw = Input.GetKey(KeyCode.Q) ? -1 : (Input.GetKey(KeyCode.E) ? 1 : 0);
        yaw = yaw * Time.deltaTime * 5;


        myTime2 = myTime2 + Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftShift) && myTime2 > nextThrottle)
        {
            nextThrottle = myTime2 + throttleDelta;

            throttle += throttleSensitivity / 100.0f;

            // create code here that animates the newProjectile

            nextThrottle = nextThrottle - myTime2;
            myTime2 = 0.0F;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && myTime2 > nextThrottle)
        {
            nextThrottle = myTime2 + throttleDelta;

            throttle -= throttleSensitivity / 100.0f;

            // create code here that animates the newProjectile

            nextThrottle = nextThrottle - myTime2;
            myTime2 = 0.0F;
        }

        throttle = throttle < 0 ? 0 : (throttle > 1 ? 1 : throttle);
    }

    private void CalculateAcceleration()
    {
        acceleration = throttle * defaultAcceleration;

        // if maxSpeed is reached & is not braking pedal -> kill acceleration
        if (speed >= maxSpeed && throttle >= 0)
        {
            acceleration = 0;
        }
        else if (throttle == 0 && speed > 0)
        {
            // if throttle = 0 auto brakes!
            acceleration = -defaultBrakesDeceleration;
        }
    }

    private void CalculateDeltaDistance()
    {
        deltaDistance = speed * Time.deltaTime + (1 / 2) * acceleration * Time.deltaTime * Time.deltaTime;

        // ship cant go backwads for now!
        if (deltaDistance < 0)
        {
            deltaDistance = 0;
            acceleration = 0;
        }
    }

    private void CalculateFinalSpeed()
    {
        // finalSpeed = acceleration * time + initialSpeed
        speed = acceleration * Time.deltaTime + speed;

        speed = speed < 0 ? 0 : speed;
    }

    private void Move()
    {
        transform.position += transform.forward * deltaDistance;
    }

    private void Turn()
    {
        CalculateAgility();

        transform.Rotate(pitch, yaw, -roll);
    }

    private void CalculateAgility()
    {
        pitch = pitch * agility;
        yaw = yaw * agility;
        roll = roll * agility;
    }

    private void setDebugTexts()
    {
        rollText.text = "Roll: " + roll.ToString();
        rollTextBackground.text = "Roll: " + roll.ToString();
        pitchText.text = "Pitch: " + pitch.ToString();
        pitchTextBackground.text = "Pitch: " + pitch.ToString();
        throttleText.text = "throttle: " + throttle.ToString();
        throttleTextBackground.text = "throttle: " + throttle.ToString();
        DeltaDistanceText.text = "D-Distance: " + deltaDistance.ToString();
        DeltaDistanceTextBackground.text = "D-Distance: " + deltaDistance.ToString();
        AccelerationText.text = "Acceleration: " + acceleration.ToString();
        AccelerationTextBackground.text = "Acceleration: " + acceleration.ToString();
        DeltaTimeText.text = "Delta-Time: " + Time.deltaTime.ToString();
        DeltaTimeTextBackground.text = "Delta-Time: " + Time.deltaTime.ToString();
        SpeedText.text = "Speed: " + speed.ToString();
        SpeedTextBackground.text = "Speed: " + speed.ToString();
    }
}

public static class FireModes
{
    public static int Laser = 1;

    public static int MiniGun = 2;
}
