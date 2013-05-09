//This script show you how to use CollisionComponent component with SpriteRendererAABB flag.
//When use SpriteRendererAABB flag, will Create a BoxCollider at same gameobject.
//You just need use the Unity3D build-in handler to respond the collusion.


using UnityEngine;
using System.Collections;
using EasyMotion2D;

public class CollisionSpriteRendererAABB : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}


    void OnMouseOver()
	{
		spriteRenderer.color = Color.red;
	}


	void OnMouseExit()
	{
		spriteRenderer.color = Color.white;
	}



	void OnTriggerEnter(Collider col)
	{
		spriteRenderer.color = Color.green;
	}



	void OnTriggerExit(Collider collision)
	{
		spriteRenderer.color = Color.white;
	}
}
