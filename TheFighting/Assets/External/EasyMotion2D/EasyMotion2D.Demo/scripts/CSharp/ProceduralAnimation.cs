//This script show you show to use program to control the animation in runtime 

using UnityEngine;
using System.Collections;
using EasyMotion2D;

public class ProceduralAnimation : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private SpriteAnimation spriteAnimation;

	private SpriteAnimation.SpriteAnimatonDelegate updateHandler;

	public Transform target;

	// Use this for initialization
	void Start () {
		spriteAnimation = GetComponent<SpriteAnimation>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		updateHandler = PreUpdateHandler;

		//register a function to handler the animation evaluate
		spriteAnimation.AddComponentUpdateHandler( SpriteAnimationComponentUpdateType.PreUpdateComponent, updateHandler);
	}
	

	float t = 0f;

	// PreUpdateHandler is called bone transformed in bone local space by SpriteAnimation
	void PreUpdateHandler( SpriteAnimation ani )
	{
		//move the root in circle
		SpriteTransform root = spriteRenderer.GetSpriteTransform("/root");
		if ( root != null )
		{
			root.position = Quaternion.Euler( 0f, 0f, t) * (Vector3.right * 100f) ;
			t += 60f * Time.deltaTime;
		}

		//make the bone lookat target
		SpriteTransform bone = spriteRenderer.GetSpriteTransform("/root/node");
		if ( bone != null )
			bone.LookAt( transform.InverseTransformPoint(target.position));
	}
}
