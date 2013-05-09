
private var	spriteAnimation : EasyMotion2D.SpriteAnimation ;
private var spriteRenderer : EasyMotion2D.SpriteRenderer ;

private var	stepX : float = 0;
private var stepY : float  = 0;

private var _transform : Transform;


	// Use this for initialization
function Start () {
	//Get component reference
	spriteAnimation = GetComponent( EasyMotion2D.SpriteAnimation);
	spriteRenderer = GetComponent( EasyMotion2D.SpriteRenderer);

	//set gameobject position randomly.
	transform.position = Vector3( 
		Random.Range(0, Screen.width),
		Random.Range(0, Screen.height * 0.7f ),
		0);

	//set gameobject depth randomly.
	spriteRenderer.depth = Random.Range(0, 512);



	//Get all clips in SpriteAnimation Component, and random select one to play.
	var clips = EasyMotion2D.SpriteAnimationUtility.GetAnimationClips( spriteAnimation );

	var idx : int = Random.Range(0, clips.Length);
	var name : String = clips[ idx ].name;
	spriteAnimation.Play( name );

	var state : EasyMotion2D.SpriteAnimationState = spriteAnimation[ name ];
	state.wrapMode = WrapMode.Loop;


	//set move step randomly.
	stepX = Random.Range(50, 250);
	stepY = Random.Range(50, 250);

	//Hold a Transform reference
	_transform = transform;
}	



function Update()
{
	var pos : Vector3 = _transform.position;
	var scale : Vector3 = _transform.localScale;

	var delta : float = Time.deltaTime;


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
	_transform.localScale = scale;
}

