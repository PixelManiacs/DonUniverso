﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Game script initializes the game by spawning the planets and asteroids.
/// </summary>
public class Game : MonoBehaviour {

    // The planet spawner component, used to spawn the planets and asteroids.
	[HideInInspector] 
    public PlanetSpawner planetSpawner;

	private void Awake() {
        // Initialize the planet spawner
		Random.InitState("hdmksdfkasdfkjh".GetHashCode());
		planetSpawner = GetComponent<PlanetSpawner>();
	}

	private void Start() {
        // spawn 100 planets
		for (int i = 0; i < 100; i++)
			planetSpawner.SpawnPlanet();

        // spawn 50 asteroids
		for (int i = 0; i < 50; i++)
			planetSpawner.SpawnAsteroid();
	}
}
