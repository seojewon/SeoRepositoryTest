using UnityEngine;
using System.Collections;
using EasyMotion2D;

public class Ball : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		//move gameobject as a circle
		transform.position = Quaternion.Euler( 0, 0, Time.time * 60f) * Vector3.right * 250;
	}
}
