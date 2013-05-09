//This script show you how to use CollisionComponent component with AnimationComponentBB flag.
//When use SpriteRendererAABB flag, will Create a few of sub gameobjects with BoxCollider.
//You should use the extand handler funcion to respond the collusion. All the sub collider's event will send to the same gameobject the CollisionComponent attached.
//Usually, these handler function just named On + Component + EventType.
//For example: 
//If you want to respond MouseOver a part of animation. You just need write a function named OnComponentMouseOver. Then all the mouse over event on sub colliders will send to this function.
//And collusion information will store in a CollisionNodeInfo as a parameter.


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyMotion2D;

public class CollisionSpriteAnimationCompoentBB : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private SpriteAnimation spriteAnimation;

	//last mouse hover component, a component that means a bone in the animation.
	private SpriteTransform lastHoverSprite = null;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteAnimation = GetComponent<SpriteAnimation>();

		//register a animation update handler to change the color.
		//Because if do not change the color in handler, the color will overwrite by animation update in current implemention.
		spriteAnimation.AddComponentUpdateHandler( SpriteAnimationComponentUpdateType.PreUpdateComponent, OnUpdateAnimation);

		//register a hanlder to the event delegate handler, use delegate to handle event will cost less the SendMessage on mobile
		GetComponent<CollisionComponent>().OnComponentMouseOver += OnComponentMouseOverHandle;
	}


	//if mouse over on any sub collider
    void OnComponentMouseOverHandle(CollisionNodeInfo collisionInfo)
	{
		SpriteTransform tmpSprTransform = spriteRenderer.GetSpriteTransform( collisionInfo.hitNode.componentPath);

		//save the last hover component
		if ( tmpSprTransform != null )
		{
			lastHoverSprite = tmpSprTransform;
		}
	}

	//if mouse exit on any sub collider
	void OnComponentMouseExit(CollisionNodeInfo collisionInfo)
	{
		//clear the last hover component
		lastHoverSprite = null;
	}


	//a list that store all the component that collusion another collider.
	private List<SpriteTransform> collisionComponents = new List<SpriteTransform>();

	//if collider is trigger and same other collider enter it
	void OnComponentTriggerEnter(CollisionNodeInfo collisionInfo)
	{
		SpriteTransform tmpSprTransform = spriteRenderer.GetSpriteTransform( collisionInfo.hitNode.componentPath);

		//if we can get the component's SpriteTransform, then add it to list.
		if ( tmpSprTransform != null )
		{
			collisionComponents.Add( tmpSprTransform );
		}
	}


	//if collider is trigger and same other collider exit it
	void OnComponentTriggerExit(CollisionNodeInfo collisionInfo)
	{
		SpriteTransform tmpSprTransform = spriteRenderer.GetSpriteTransform( collisionInfo.hitNode.componentPath);

		//if we can get the component's SpriteTransform, then remove it from list.
		if ( tmpSprTransform != null )
		{
			collisionComponents.Remove( tmpSprTransform );
		}
	}



	//if a component in collusion list, change its color.
	void OnUpdateAnimation(SpriteAnimation ani)
	{
		//if mouse hover on a component, then change its color.
		if (lastHoverSprite != null )
		{
			lastHoverSprite.color = Color.red;
		}

		//if mouse hover on a component, then change its color.
		foreach( SpriteTransform sprTran in collisionComponents)
		{
			sprTran.color = Color.green;
		}
	}
}
