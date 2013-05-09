using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Vector3 direction = new Vector3(1,0,0);
	public float speed = 100;

	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		if ( Physics.Raycast(transform.position, direction, out hit, speed * Time.deltaTime ))
		{
			Destroy(gameObject);
		}

		transform.position = transform.position + direction * speed * Time.deltaTime;
	}
}
