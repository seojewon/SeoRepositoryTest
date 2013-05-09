using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyMotion2D;

public class Easy2DDemo_CharacterActor : MonoBehaviour {

	SpriteAnimation spriteAnimation;
	SpriteRenderer spriteRenderer;

	float stepX = 0;
	float stepY = 0;

	Transform _transform;


	// Use this for initialization
	void Start () {
		//Get component reference
		spriteAnimation = GetComponent<SpriteAnimation>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		//set gameobject position randomly.
		transform.position = new Vector3( 
			Random.Range(0, Screen.width),
			Random.Range(0, Screen.height * 0.7f ),
			0);

		//set gameobject depth randomly.
		spriteRenderer.depth = Random.Range(0, 512);



		//Get all clips in SpriteAnimation Component, and random select one to play.
		List<string> tmp = new List<string>();

		SpriteAnimationClip[] clips = SpriteAnimationUtility.GetAnimationClips( spriteAnimation );

		foreach( SpriteAnimationClip clip in clips)
		{
			tmp.Add( clip.name);
		}

		int idx = Random.Range(0, tmp.Count);
		string name = tmp[ idx ];
		spriteAnimation.Play( name );
		SpriteAnimationState state = spriteAnimation[ name ];
		state.wrapMode = WrapMode.Loop;


		//set move step randomly.
		stepX = Random.Range(50, 250);
		stepY = Random.Range(50, 250);

		//Hold a Transform reference
		_transform = transform;
	}	



	void Update()
	{
		Vector3 pos = _transform.position;
  		Vector3 scale = spriteRenderer.localScale;

		float delta = Time.deltaTime;


		//Move the sprite
		pos.x += stepX * delta;
		pos.y += stepY * delta;

		if ( pos.x < 50)
		{
			pos.x = 50;
			stepX = -stepX;
		}

		if ( pos.x > Screen.width - 50)
		{
			pos.x = Screen.width - 50;
			stepX = -stepX;
		}

		if ( pos.y < 0)
		{
			pos.y = 0;
			stepY = -stepY;
		}

		if ( pos.y > Screen.height - 100)
		{
			pos.y = Screen.height - 100;
			stepY = -stepY;
		}

		//Set localScale to set sprite's direction
		scale.x = Mathf.Sign( -stepX ) * Mathf.Abs( scale.x);

		//set position and scale to transform
		_transform.position = pos;

		//spriteRenderer.localScale = scale;
		_transform.localScale = scale;
	}
}
