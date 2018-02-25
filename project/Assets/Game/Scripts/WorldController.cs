using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldController : MonoBehaviour {

    public int maxAsteroids = 100;

    public GameObject playerGameObject;

    public GameObject asteroidObject;

    public float minimumDistanceAsteroidToAsteroid = 10f;

    public float minimumDistanceAsteroidToPlayer = 10f;

    public float maxAsteroidScale = 10f;

    public float minimumAsteroidScale = 1f;

    public int maxStartingAsteroids = 100;

    public float asteroidDrawDistance = 500f;

    public Int32 randomSeed;

    public GameObject testSphere;

    private PlayerController playerController;

    private int asteroidCount = 0;

	// Use this for initialization
	void Start () {
        playerController = playerGameObject.GetComponent<PlayerController>();

        FirstRandomGenerateObjects();
	}

    private void FirstRandomGenerateObjects()
    {
        FirstRandomGenerateAsteroids();
    }

    /// <summary>
    /// Generate a set of asteroids in a 100*100*100 virtual cube around the player.
    /// Asteroids must follow two rules:
    ///     1- The distance between the asteroid and the player must be greater or equal to minimumDistanceAsteroidToPlayer
    ///     2- The distance between two asteroids must be greater or equal to minimumDistanceAsteroidToAsteroid
    /// </summary>
    private void FirstRandomGenerateAsteroids()
    {
        // let's first generate a "spanning cube": a set points so that the points are <<minimumDistanceAsteroidToAsteroid>> apart
        // and they cover the entire cube. Later we will randombly pick those points.
        var possibleLocations = SpanningCube(asteroidDrawDistance, asteroidDrawDistance, asteroidDrawDistance);

        for (int i = 0; i < maxStartingAsteroids; i++)
        {
            // let's choose a random item from the possible asteroids locations
            int randomIndex = UnityEngine.Random.Range(0, possibleLocations.Count - 1);
            
            var position = possibleLocations.ElementAt(randomIndex);

            var asteroid = Instantiate(asteroidObject, position, transform.rotation) as GameObject;
            asteroidCount++;

            var randomScale = new Vector3(UnityEngine.Random.Range(minimumAsteroidScale, maxAsteroidScale),
                                            UnityEngine.Random.Range(minimumAsteroidScale, maxAsteroidScale),
                                                UnityEngine.Random.Range(minimumAsteroidScale, maxAsteroidScale));

            asteroid.transform.localScale = randomScale;

            // remove item from the list
            possibleLocations.Remove(position);
        }
    }

    private void LateUpdate()
    {
        // Let's randombly generate object around the player
        GenerateAsteroids();
    }

    private void GenerateAsteroids()
    {
        
    }

    private List<Vector3> SpanningCube(float x, float y, float z)
    {
        List<Vector3> possibleLocations = new List<Vector3>();

        var center = playerController.transform.position;

        var cubeDimensions = new Vector3(x, y, z);

        // let's go to the corner of the cube
        var corner = center - cubeDimensions / 2;

        var currentPosition = corner;

        // now let's generate a set of positions that are <<minimumDistanceAsteroidToAsteroid>> apart
        for (float i = corner.x; i < center.x + x/2; i += minimumDistanceAsteroidToAsteroid)
        {
            for (float j = corner.y; j < center.y + y/2; j += minimumDistanceAsteroidToAsteroid)
            {
                for (float k = corner.z; k < center.z + z/2; k += minimumDistanceAsteroidToAsteroid)
                {
                    var pos = new Vector3(i, j, k);

                    if (!pos.Equals(center))
                    {
                        possibleLocations.Add(pos);
                        //Instantiate(testSphere, pos, playerController.transform.rotation);
                    }
                }
            }
        }

        return possibleLocations;
    }
}
