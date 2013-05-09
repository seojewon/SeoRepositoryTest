
private var characterController : CharacterController;
private var spriteRenderer : EasyMotion2D.SpriteRenderer;
private var spriteAnimation : EasyMotion2D.SpriteAnimation;

// Use this for initialization
function Start () {

	spriteRenderer = GetComponent(EasyMotion2D.SpriteRenderer);
	spriteAnimation = GetComponent(EasyMotion2D.SpriteAnimation);
	characterController = GetComponent(CharacterController);

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
function LateUpdate () {

	// if we want crouch
	if ( Input.GetKey(KeyCode.DownArrow) )
	{
		if ( !spriteAnimation.IsPlaying("crouch") )
			spriteAnimation.CrossFade("crouch");
	}
	else  // else we want move or do nothing
	{
		//we want to move
		if ( Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) )
		{
			//change the character face to
			var tmpScale : Vector3 = transform.localScale;
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
		spriteAnimation.Blend("aimTo", 0f, 0.3f, true);
		spriteAnimation.Blend("shot", 0f, 0.3f, true);
	}


	//if hold shot button, then we calculate the aim to angle
	if ( Input.GetMouseButton(0) )
	{
		//get a bone in animation as a reference origin
		var armTransform : EasyMotion2D.SpriteTransform = spriteRenderer.GetSpriteTransform("/root/body/upperBody/armL");
		
		//transform origin into world space
		var armPosition : Vector3 = transform.TransformPoint( armTransform.position );

		//get the mouse position in world space
		var targetPosition : Vector3 = Camera.main.ScreenToWorldPoint( Input.mousePosition );

		//setting these point in same plane
		armPosition.z = 0f;
		targetPosition.z = 0f;

		Debug.DrawLine( armPosition, targetPosition, Color.red );

		//get direction from origin to target
		var targetDir : Vector3 = targetPosition - armPosition;

		//get the angle between the current foward to the target direction
		var aimAngle : float = Vector3.Angle(targetDir,  Mathf.Sign(transform.localScale.x) * Vector3.right);

		//half the aim value range
		//We create the aimTo clip in 61 frames, the first frame use for reference value
		//So the real aim to data in clip is 1-61 frames
		//and make the value normailzed
		var half : float = (1f - (1f / 61f)) * 0.5f;


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
		var na : float = aimAngle * Mathf.Sign( targetPosition.y - armPosition.y ) / 60f;
		
		//clamp the value in range
		var nt : float = Mathf.Clamp( half + na * half, 1f / 61f, 1f );
		spriteAnimation["aimTo"].normalizedTime = nt;
	}

	//apply gravity
	characterController.SimpleMove( Vector3.zero );
}



//the bullet prefab for Instantiate
public var bullecPrototype : GameObject;

//called by event in pistol_shot clip
function Fire( evt : EasyMotion2D.SpriteAnimationEvent )
{
	//if not hold shot button, then just leave
	if ( !Input.GetMouseButton(0) )
		return;

	//get the position of barrel in local space, event will fired when the all the compoents in clip transformed.
	var barrelTransform : EasyMotion2D.SpriteTransform  = spriteRenderer.GetSpriteTransform("/root/barrel");

	//the bullet spawn position in local space
	var bulletSpawnTransform : EasyMotion2D.SpriteTransform  = spriteRenderer.GetSpriteTransform("/root/barrel/bulletSpawnPosition");
	
	if ( barrelTransform != null && bulletSpawnTransform != null )
	{
		//transform two positions into world space
		var barrelPosition : Vector3 = transform.TransformPoint( barrelTransform.position );
		var bulletSpawnPosition : Vector3 = transform.TransformPoint( bulletSpawnTransform.position );
		
		//set they in same plane
		barrelPosition.z = 0f;
		bulletSpawnPosition.z = 0f;

		//angle of bullet, then it fired
		var bulletAngle : float = Mathf.Sign( bulletSpawnPosition.y - barrelPosition.y ) * Vector3.Angle( (bulletSpawnPosition - barrelPosition), Vector3.right);
		
		//Instantiate the bullet with the position and the rotation
		var bullet : GameObject = Instantiate( bullecPrototype, bulletSpawnPosition, Quaternion.Euler(0f,0f,bulletAngle) );

		//set the bullet movement direction
		bullet.GetComponent(Bullet_JS).direction = (bulletSpawnPosition - barrelPosition).normalized;
	}
}

