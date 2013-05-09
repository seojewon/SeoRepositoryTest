using UnityEngine;
using System.Collections;
using EasyMotion2D;

public class Character_Controller : MonoBehaviour {

	private CharacterController characterController;
	private SpriteRenderer spriteRenderer;
	private SpriteAnimation spriteAnimation;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteAnimation = GetComponent<SpriteAnimation>();
		characterController = GetComponent<CharacterController>();

		//we want mixing the shot, so make it it a higher layer
		//and we just want use the animation in "/root/body/upperBody" and its children
		spriteAnimation["shot"].layer = 1;
		spriteAnimation["shot"].AddMixingComponent( "/root/body/upperBody", true);

		//we use the aimTo clip to adjust the rotation the arm
		//so use the Additive mode, so we get the different between current time and first frame in clip, and apply to the bones
		//and set the speed is 0, because we dont want the time of clip grow up automatecally
		//set the wrapmode to clampforever, make sure the clip will never stop automatecally
		spriteAnimation["aimTo"].layer = 2;
		spriteAnimation["aimTo"].wrapMode = WrapMode.ClampForever;
		spriteAnimation["aimTo"].blendMode = AnimationBlendMode.Additive;
		spriteAnimation["aimTo"].speed = 0f;
	}
	



	//we need get transformed sprite transform, so we use lateupdate
	//becase sprite animation evaluate in update
	void LateUpdate () {

		// if we want crouch
		if ( Input.GetKey(KeyCode.DownArrow) )
		{
			if ( !spriteAnimation.IsPlaying("crouch") )
				spriteAnimation.CrossFade("crouch");
		}
		else // else we want move or do nothing
		{
			//we want to move
			if ( Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) )
			{
				//change the character face to
				Vector3 tmpScale = transform.localScale;
				if ( Input.GetKey(KeyCode.LeftArrow) )
					tmpScale.x = Mathf.Abs(tmpScale.x) * -1;
				else
					tmpScale.x = Mathf.Abs(tmpScale.x);

				transform.localScale = tmpScale;

				//if not playing a move clip, then crossfade it
				if ( !spriteAnimation.IsPlaying("walk") )
					spriteAnimation.CrossFade("walk");
			}
			else  // we do nothing, then play idle clip
			{
				//if not playing a idle clip, then crossfade it
				if ( !spriteAnimation.IsPlaying("idle") )
					spriteAnimation.CrossFade("idle");
			}
		}


		//if hold the left mouse button, then we can play the shot clips
		if ( Input.GetMouseButtonDown(0) && !spriteAnimation.IsPlaying("shot") )
		{
			spriteAnimation.CrossFade( "aimTo", 0.2f);
			spriteAnimation.CrossFade( "shot", 0.2f);
		}
		
		//if we release the shot button, then fade out and stop the shot clips
		if (Input.GetMouseButtonUp(0) && spriteAnimation.IsPlaying("shot") )
		{
			//fade out the clip's weight to 0 in 0.3 second, after end fade out, stop the clips
			spriteAnimation.Blend("aimTo", 0f, 0.3f, true);
			spriteAnimation.Blend("shot", 0f, 0.3f, true);
		}

		//if hold shot button, then we calculate the aim to angle
		if ( Input.GetMouseButton(0) )
		{
			//get a bone in animation as a reference origin
			SpriteTransform armTransform = spriteRenderer.GetSpriteTransform("/root/body/upperBody/armL");

			//transform origin into world space
			Vector3 armPosition = transform.TransformPoint( armTransform.position );

			//get the mouse position in world space
			Vector3 targetPosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );

			//setting these point in same plane
			armPosition.z = 0f;
			targetPosition.z = 0f;

			Debug.DrawLine( armPosition, targetPosition, Color.red );

			//get direction from origin to target
			Vector3 targetDir = targetPosition - armPosition;

			//get the angle between the current foward to the target direction
			float aimAngle = Vector3.Angle(targetDir,  Mathf.Sign(transform.localScale.x) * Vector3.right);

			//half the aim value range
			//We create the aimTo clip in 61 frames, the first frame use for reference value
			//So the real aim to data in clip is 1-61 frames
			//and make the value normailzed
			float half = (1f - (1f / 61f)) * 0.5f;

			//In shot clip, we think the aim to angle is zero, and zero is the the half of aimTo clip (1-61)
			//we think the aimto range is 120 degree, then
            //             60 degree (  61 frame at aimTo)
			//           /
			//          /
			//         /
			//      armTo ------- 0 degree ( half at aimTo )
			//         \
			//          \
			//           \  
			//             -60 degree ( 1 frame at aimTo)
			// use the y the decide the aim angle's sign, the normailze it in -1 to 1
			float na = aimAngle * Mathf.Sign( targetPosition.y - armPosition.y ) / 60f;

			//clamp the value in range
			float nt = Mathf.Clamp( half + na * half, 1f / 61f, 1f );
			spriteAnimation["aimTo"].normalizedTime = nt;
		}

		//apply gravity
		characterController.SimpleMove( Vector3.zero );
	}


	//the bullet prefab for Instantiate
	public GameObject bullecPrototype;

	//called by event in pistol_shot clip
	void Fire(SpriteAnimationEvent evt)
	{
		//if not hold shot button, then just leave
		if ( !Input.GetMouseButton(0) )
			return;

		//get the position of barrel in local space, event will fired when the all the compoents in clip transformed.
		SpriteTransform barrelTransform = spriteRenderer.GetSpriteTransform("/root/barrel");
		
		//the bullet spawn position in local space
		SpriteTransform bulletSpawnTransform = spriteRenderer.GetSpriteTransform("/root/barrel/bulletSpawnPosition");

		if ( barrelTransform != null && bulletSpawnTransform != null )
		{
			//transform two positions into world space
			Vector3 barrelPosition = transform.TransformPoint( barrelTransform.position );
			Vector3 bulletSpawnPosition = transform.TransformPoint( bulletSpawnTransform.position );

			//set they in same plane
			barrelPosition.z = 0f;
			bulletSpawnPosition.z = 0f;

			//angle of bullet, then it fired
			float bulletAngle = Mathf.Sign( bulletSpawnPosition.y - barrelPosition.y ) * Vector3.Angle( (bulletSpawnPosition - barrelPosition), Vector3.right);
			
			//Instantiate the bullet with the position and the rotation
			GameObject bullet = Object.Instantiate( bullecPrototype, bulletSpawnPosition, Quaternion.Euler(0f,0f,bulletAngle) ) as GameObject;

			//set the bullet movement direction
			bullet.GetComponent<Bullet>().direction = (bulletSpawnPosition - barrelPosition).normalized;
		}
	}
}
