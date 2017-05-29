using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the behaviour of the compass, which shows the way to the home planet.
/// </summary>
public class Compass : MonoBehaviour 
{
	void Update () {
        transform.LookAt(Vector3.zero);
	}
}
