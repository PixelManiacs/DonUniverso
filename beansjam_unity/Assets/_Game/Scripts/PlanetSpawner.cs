using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Planet spawner. This component spawns planets and asteroids "kind of" random
/// based.
/// </summary>
public class PlanetSpawner : MonoBehaviour {

	// Empty transform objects used to group the spawned planets and asteroids
	[Header("Containers")]
	public Transform planetContainer;
	public Transform asteroidContainer;

	// Prefabs of the objects to spawn (i.e. planets and asteroids);
	[Header("Prefabs")]
	public GameObject[] planetPrefabsDualColor;
	public GameObject[] planetPrefabsSingleColor;
	public GameObject planetPrefabSchmaturn;
	public GameObject planetPrefabSchmenus;
	public GameObject[] asteroidPrefabs;

	// The color palette for the planets
	public Color[] PlanetColorArray;

	// The Range of the universe
	// i.e. the area in which objects spawn is between -range and range
	// for both, the x- and y-axis
	public float range = 10000f;

	// the min and max level for the planets
	public int minLevel = 1;
	public int maxLevel = 100;

	// The min and max sizes for the planets
	public float sizeMin = 0.3f;
	public float sizeMax = 100;

    // The min and max size for asteroids
    public float sizeMinAsteroids = 0.5f;
    public float sizeMaxAsteroids = 3f;

	// The name generator, which is used to generate names for planets
	// Thanks, Captain Obvious.
	private NameGenerator nameGenerator;

	/// <summary>
	/// Initialize the PlanetSpawner i.e. create the name generator.
	/// </summary>
	private void Awake() {
		nameGenerator = new NameGenerator();
	}

	/// <summary>
	/// Spawns a planet.
	/// </summary>
	public void SpawnPlanet() 
    {
		float x, y, size;
		Vector3 position;
		int level;

		// roll out a position, which is at least 10 times the size away from 
		// every other planet. Thus smaller planets with a lower level appear 
		// closer together and more in the center of the universe, bigger 
		// planets spawn in the outer rim.
		do {
			x = NextGaussian(0, 100, -range, range);
			y = NextGaussian(0, 100, -range, range);
			position = new Vector3(x, 0, y);

			// roll out the level of the planet depending on the position
			// i.e. higher level planets are more away from the center/home planet
			level = Mathf.RoundToInt(
						Mathf.Ceil(Vector3.Distance(Vector3.zero, position) /
								   range * maxLevel * Random.Range(0.8f, 1.2f)));

			// "transform" the level value into the size interval
			size = MapToInterval(level, minLevel, maxLevel, sizeMin, sizeMax);

			// generate a new position if there is already a planet on this spot
		} while (Physics.OverlapSphere(position, size*10).Length > 0);

		// generate a name for the planet to spawn.
		var name = nameGenerator.GenerateName();

		// Create/instantiate the planet game object.
		// if it is a special planet i.e. its name is SCHMATURN or SCHMENUS
		// pick a special prefab.
		// Otherwise randomly choose between single or double colored planets
		GameObject go = null;
		if (name.Contains("SCHMATURN")) {
			go = Instantiate(planetPrefabSchmaturn) as GameObject;
			go.GetComponent<MeshRenderer>().material.color = PlanetColorArray[Random.Range(0, PlanetColorArray.Length)];
		} else if (name.Contains("SCHMENUS")) {
			go = Instantiate(planetPrefabSchmenus) as GameObject;
			go.GetComponent<MeshRenderer>().material.color = PlanetColorArray[Random.Range(0, PlanetColorArray.Length)];
		} else {
			if (Random.Range(0, 100) < 50) {
				go = InstantiateSingleColor();
			} else {
				go = InstantiateDualColor();
			}
		}

		// pack the spawned planet into the planet container
		// and initialize attributes like the layer, the name, position, etc.
		go.transform.SetParent(planetContainer);
		go.layer = LayerMask.NameToLayer("Planet");
		go.tag = "Planet";
		go.name = name;
		go.transform.position = position;
		go.transform.localScale = new Vector3(size, size, size);

		// add the planet component script to the planet game object
		var planet = go.AddComponent<Planet>();
		planet.level = level;
		planet.name = name;
	}

	/// <summary>
	/// Instaniates a single-colored planet.
	/// </summary>
	/// <returns>The instaniated planet game object.</returns>
	private GameObject InstantiateSingleColor()
    {
		// Pick a random planet prefab
		var prefab = planetPrefabsSingleColor[Random.Range(0, planetPrefabsSingleColor.Length)];
        

        // Instantiate it and pick & set a color for the planet randomly out of the palette
		var go = Instantiate(prefab) as GameObject;
		go.GetComponent<MeshRenderer>().material.color = PlanetColorArray[Random.Range(0, PlanetColorArray.Length)];

		return go;
	}

	/// <summary>
	/// Instaniates a dual-colored planet.
	/// </summary>
	/// <returns>The instaniated planet game object.</returns>
	private GameObject InstantiateDualColor() 
    {
		// Pick a random planet prefab
		var prefab = planetPrefabsDualColor[Random.Range(0, planetPrefabsDualColor.Length)];

		// Instantiate it and pick & set two colors for the planet randomly out of the palette
		var go = Instantiate(prefab) as GameObject;
		go.GetComponent<MeshRenderer>().materials[0].color = PlanetColorArray[Random.Range(0, PlanetColorArray.Length)];
		go.GetComponent<MeshRenderer>().materials[1].color = PlanetColorArray[Random.Range(0, PlanetColorArray.Length)];

		return go;
	}

	/// <summary>
	/// Spawns the asteroid gameobjects.
	/// </summary>
	public void SpawnAsteroid() 
    {
		// pick a random asteroid prefab and instaniate it
		var prefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
		var go = Instantiate(prefab) as GameObject;
		go.transform.SetParent(asteroidContainer);

		// Calculate normal distributed values for the x and y position
		// and "move" the spawned asteroid to this randomly choosen position.
		var x = NextGaussian(0, range/4, -range, range);
		var y = NextGaussian(0, range/4, -range, range);
		go.transform.position = new Vector3(x, 0, y);
		
        // Randomly scale the asteroids.
		var size = Random.Range(sizeMinAsteroids, sizeMaxAsteroids);
		go.transform.localScale = new Vector3(size, size, size);
	}

	/// <summary>
	/// Map the given value from the interval (from1, to1) to value in the 
	/// interval (from2, to2). This is can be used to transform the level of a 
	/// planet to its size.
	/// Calculates:
	/// (value - from1) / (to1 - from1) * (to2-from2) + from2
	/// </summary>
	/// <returns>The mapped single value.</returns>
	/// <param name="value">The main contributing value e.g. the level of the
	///     planet</param>
	/// <param name="from1">The minimum of the value range e.g. the minimum
	///     planet levels</param>
	/// <param name="to1">The maximum of the value range e.g. the maximum planet
	///     level</param>
	/// <param name="from2">The minimum of the range for the returned value e.g.
	///     the minimum planet size</param>
	/// <param name="to2">The maximum of the range for the returned value e.g.
	///     the maximum planet size</param>
	private static float MapToInterval(float value, float from1, float to1, float from2, float to2) 
    {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	/// <summary>
	/// Picks a normal distributed random number, NOT limited to an interval.
	/// </summary>
	/// <returns>The gaussian normal distributed random number.</returns>
	private static float NextGaussian() 
    {
		float v1, v2, s;
        do
        {
            v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || Mathf.Abs(s - 0f) < 0.000000001f);

		s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

		return v1 * s;
	}

	/// <summary>
	/// Picks a normal distributed random number, NOT limited to an interval.
	/// </summary>
	/// <returns>The gaussian normal distributed random number.</returns>
	/// <param name="mean">The mean of the normal distribution, i.e. the values 
	///     are scattered around this mean value.</param>
	/// <param name="standard_deviation">The possible deviation factor of the 
	///     randomly choosen value.</param>
	private static float NextGaussian(float mean, float standard_deviation) 
    {
		return mean + NextGaussian() * standard_deviation;
	}

	/// <summary>
	/// Picks a normal distributed random number, limited to an interval.
	/// </summary>
	/// <returns>The gaussian normal distributed random number.</returns>
	/// <param name="mean">The mean of the normal distribution, i.e. the values 
	///     are scattered around this mean value.</param>
	/// <param name="standard_deviation">The possible deviation factor of the 
	///     randomly choosen value.</param>
	/// <param name="min">The minimum of the range. I.e. no random value will be
	///     smaller than the minimum.</param>
	/// <param name="max">The maximum of the range. I.e. no random value will be
	///     greater than the maximum.</param>
	private static float NextGaussian (float mean, float standard_deviation, float min, float max) 
    {
		float x;
		do {
			x = NextGaussian(mean, standard_deviation);
		} while (x < min || x > max);
		return x;
	}
}
