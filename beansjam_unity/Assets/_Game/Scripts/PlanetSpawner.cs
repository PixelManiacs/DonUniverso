using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour {
	[Header("Containers")]
	public Transform planetContainer;
	public Transform asteroidContainer;

	[Header("Prefabs")]
	public GameObject[] planetPrefabsDualColor;
	public GameObject[] planetPrefabsSingleColor;
	public GameObject planetPrefabSchmaturn;
	public GameObject planetPrefabSchmenus;
	public GameObject[] asteroidPrefabs;

	public float range = 10000f;
	public int maxLevel = 100;
	public float sizeMin = 4;
	public float sizeMax = 100;
	private Game game;
	public Color[] PlanetColorArray;

	private void Awake() {
		game = GetComponent<Game>();
	}

	public void SpawnPlanet() {
		float x, y, size;
		Vector3 position;
		int level;
		do {
			x = NextGaussian(0, 100, -range, range);
			y = NextGaussian(0, 100, -range, range);
			position = new Vector3(x, 0, y);
			level = Mathf.RoundToInt(Mathf.Ceil(Vector3.Distance(Vector3.zero, position)/range*maxLevel*Random.Range(0.8f, 1.2f)));
			size = Map(level, 1, 100, sizeMin, sizeMax);
		} while (Physics.OverlapSphere(position, size*10).Length > 0);

		var name = game.nameGenerator.GenerateName();

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

		go.transform.SetParent(planetContainer);
		go.layer = LayerMask.NameToLayer("Planet");
		go.tag = "Planet";
		go.name = name;
		go.transform.position = position;
		go.transform.localScale = new Vector3(size, size, size);

		var planet = go.AddComponent<Planet>();
		planet.level = level;
		planet.name = name;

		go.GetComponent<MeshRenderer>().material.color = PlanetColorArray[Random.Range(0, PlanetColorArray.Length)];
	}

	private GameObject InstantiateSingleColor() {
		var prefab = planetPrefabsSingleColor[Random.Range(0, planetPrefabsSingleColor.Length)];
		var go = Instantiate(prefab) as GameObject;
		go.GetComponent<MeshRenderer>().material.color = PlanetColorArray[Random.Range(0, PlanetColorArray.Length)];
		return go;
	}

	private GameObject InstantiateDualColor() {
		var prefab = planetPrefabsDualColor[Random.Range(0, planetPrefabsDualColor.Length)];
		var go = Instantiate(prefab) as GameObject;
		go.GetComponent<MeshRenderer>().materials[0].color = PlanetColorArray[Random.Range(0, PlanetColorArray.Length)];
		go.GetComponent<MeshRenderer>().materials[1].color = PlanetColorArray[Random.Range(0, PlanetColorArray.Length)];
		return go;
	}

	public void SpawnAsteroid() {
		var prefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
		var go = Instantiate(prefab) as GameObject;
		go.transform.SetParent(asteroidContainer);

		var x = NextGaussian(0, range/4, -range, range);
		var y = NextGaussian(0, range/4, -range, range);
		go.transform.position = new Vector3(x, 0, y);

		var size = Random.Range(0.5f, 3f);
		go.transform.localScale = new Vector3(size, size, size);
	}

	private static float Map(float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	private static float NextGaussian() {
		float v1, v2, s;
		do {
			v1 = 2.0f * Random.Range(0f,1f) - 1.0f;
			v2 = 2.0f * Random.Range(0f,1f) - 1.0f;
			s = v1 * v1 + v2 * v2;
		} while (s >= 1.0f || s == 0f);

		s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

		return v1 * s;
	}

	private static float NextGaussian(float mean, float standard_deviation) {
		return mean + NextGaussian() * standard_deviation;
	}

	private static float NextGaussian (float mean, float standard_deviation, float min, float max) {
		float x;
		do {
			x = NextGaussian(mean, standard_deviation);
		} while (x < min || x > max);
		return x;
	}
}
