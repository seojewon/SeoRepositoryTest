//This script show you how to use CollisionComponent component with AnimationComponentBB flag.
//When use SpriteRendererAABB flag, will Create a few of sub gameobjects with BoxCollider.
//You should use the extand handler funcion to respond the collusion. All the sub collider's event will send to the same gameobject the CollisionComponent attached.
//Usually, these handler function just named On + Component + EventType.
//For example: 
//If you want to respond MouseOver a part of animation. You just need write a function named OnComponentMouseOver. Then all the mouse over event on sub colliders will send to this function.
//And collusion information will store in a CollisionNodeInfo as a parameter.





private var spriteRenderer : EasyMotion2D.SpriteRenderer;
private var spriteAnimation : EasyMotion2D.SpriteAnimation;

//last mouse hover component, a component that means a bone in the animation.
private var lastHoverSprite : EasyMotion2D.SpriteTransform = null;

function Start()
{
	spriteRenderer = GetComponent(EasyMotion2D.SpriteRenderer);
	spriteAnimation = GetComponent(EasyMotion2D.SpriteAnimation);

	//register a animation update handler to change the color.
	//Because if do not change the color in handler, the color will overwrite by animation update in current implemention.
	spriteAnimation.AddComponentUpdateHandler( EasyMotion2D.SpriteAnimationComponentUpdateType.PreUpdateComponent, OnUpdateAnimation);
}


//if mouse over on any sub collider
function OnComponentMouseOver( collisionInfo : EasyMotion2D.CollisionNodeInfo )
{
	var tmpSprTransform : EasyMotion2D.SpriteTransform = spriteRenderer.GetSpriteTransform( collisionInfo.hitNode.componentPath );

	//save the last hover component
	if ( tmpSprTransform != null )
	{
		lastHoverSprite = tmpSprTransform;
	}
}

//if mouse exit on any sub collider
function OnComponentMouseExit( collisionInfo : EasyMotion2D.CollisionNodeInfo )
{
	//clear the last hover component
	lastHoverSprite = null;
}


//a list that store all the component that collusion another collider.
private var collisionComponents : ArrayList = new ArrayList();

//if collider is trigger and same other collider enter it
function OnComponentTriggerEnter(  collisionInfo : EasyMotion2D.CollisionNodeInfo)
{
	var tmpSprTransform : EasyMotion2D.SpriteTransform = spriteRenderer.GetSpriteTransform( collisionInfo.hitNode.componentPath);

	//if we can get the component's SpriteTransform, then add it to list.
	if ( tmpSprTransform != null )
	{
		collisionComponents.Add( tmpSprTransform );
	}
}


//if collider is trigger and same other collider exit it
function OnComponentTriggerExit(  collisionInfo : EasyMotion2D.CollisionNodeInfo)
{
	var tmpSprTransform : EasyMotion2D.SpriteTransform = spriteRenderer.GetSpriteTransform( collisionInfo.hitNode.componentPath);

	//if we can get the component's SpriteTransform, then remove it from list.
	if ( tmpSprTransform != null )
	{
		collisionComponents.Remove( tmpSprTransform );
	}
}



//if a component in collusion list, change its color.
function OnUpdateAnimation( ani : EasyMotion2D.SpriteAnimation)
{
	//if mouse hover on a component, then change its color.
	if (lastHoverSprite != null )
	{
		lastHoverSprite.color = Color.red;
	}

	//if mouse hover on a component, then change its color.
	for( var sprTran : EasyMotion2D.SpriteTransform in collisionComponents)
	{
		sprTran.color = Color.green;
	}
}