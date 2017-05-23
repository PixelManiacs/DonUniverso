using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
	public NameGenerator nameGenerator { get; internal set; }
	[HideInInspector] public PlanetSpawner planetSpawner;

	private void Awake() {
		Random.InitState("hdmksdfkasdfkjh".GetHashCode());
		nameGenerator = new NameGenerator();
		planetSpawner = GetComponent<PlanetSpawner>();
	}

	private void Start() {
		for (int i = 0; i < 100; i++)
			planetSpawner.SpawnPlanet();

		for (int i = 0; i < 50; i++)
			planetSpawner.SpawnAsteroid();
	}
}
