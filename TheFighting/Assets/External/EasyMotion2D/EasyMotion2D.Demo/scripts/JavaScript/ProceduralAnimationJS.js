//This script show you show to use program to control the animation in runtime 

private var spriteRenderer : EasyMotion2D.SpriteRenderer;
private var spriteAnimation : EasyMotion2D.SpriteAnimation;

private var updateHandler : EasyMotion2D.SpriteAnimation.SpriteAnimatonDelegate;

public var target : Transform;

// Use this for initialization
function Start () {
	spriteAnimation = GetComponent( EasyMotion2D.SpriteAnimation);
	spriteRenderer = GetComponent( EasyMotion2D.SpriteRenderer );


	updateHandler = PreUpdateHandler;

	//register a function to handler the animation evaluate
	spriteAnimation.AddComponentUpdateHandler( EasyMotion2D.SpriteAnimationComponentUpdateType.PreUpdateComponent, updateHandler);
}


private var t : float = 0f;

// PreUpdateHandler is called bone transformed in bone local space by SpriteAnimation
function PreUpdateHandler( ani : EasyMotion2D.SpriteAnimation )
{
	//move the root in circle
	var root : EasyMotion2D.SpriteTransform = spriteRenderer.GetSpriteTransform("/root");
	if ( root != null )
	{
		root.position = Quaternion.Euler( 0f, 0f, t) * (Vector3.right * 100f) ;
		t += 60f * Time.deltaTime;
	}

	//make the bone lookat target
	var bone : EasyMotion2D.SpriteTransform = spriteRenderer.GetSpriteTransform("/root/node");
	if ( bone != null )
		bone.LookAt( transform.InverseTransformPoint(target.position) );
}