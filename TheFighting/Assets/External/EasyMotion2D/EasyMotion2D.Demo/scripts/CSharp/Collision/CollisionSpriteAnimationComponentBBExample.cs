using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyMotion2D;

public class CollisionSpriteAnimationComponentBBExample : MonoBehaviour
{

	private SpriteRenderer spriteRenderer;
	private SpriteAnimation spriteAnimation;

	//last mouse hover component, a component that means a bone in the animation.
	private SpriteTransform lastHoverSprite = null;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteAnimation = GetComponent<SpriteAnimation>();
	}
	
	
	
	void OnGUI()
	{
		//reset the character animation and collider
		if ( GUI.Button( buttonPosition, "Reset" ))
		{
			//make sure all sub sprite visible
			for( int i = 0; i < spriteRenderer.GetAttachSpriteCount(); i++)
				spriteRenderer.GetSpriteTransform(i).visible = true;
			
			//enable all sub colliders
			Transform[] children = transform.GetComponentsInChildren<Transform>();
			foreach( Transform tran in children)
			{
				if ( tran.collider != null)
					tran.collider.enabled = true;
			}
			
			//destory created gameobjects
			foreach( GameObject obj in objects )
			{
				Destroy( obj);
			}
			
			//clear gameobject list
			objects.Clear();
		}
	}
	
	//reset button position
	public Rect buttonPosition;
	
	//collider material
	public PhysicMaterial phyMat;	
	
	//gameobject list, store the created gameobject
	public List<GameObject> objects = new List<GameObject>();
	
	
	//if mouse over on any sub collider
    void OnComponentMouseUpAsButton(CollisionNodeInfo collisionInfo)
	{
		SpriteTransform tmpSprTransform = spriteRenderer.GetSpriteTransform( collisionInfo.hitNode.componentPath);

		//save the last hover component
		if ( tmpSprTransform != null )
		{
			//make the collisioned sub part unvisibile
			tmpSprTransform.visible = false;
			//make the collisioned collider disable
			collisionInfo.hitNode.collider.enabled = false;
			
			//create a new gameobject in scene
			GameObject tmp = new GameObject();
			
			//add a rigidbody to gameobject
			tmp.AddComponent<Rigidbody>();
			//set the position and rotation constraint
			tmp.rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
			//give a random velocity
			tmp.rigidbody.velocity = Random.insideUnitSphere * 300f;
			
			//add a boxcollider
			BoxCollider col = tmp.AddComponent<BoxCollider>();
			
			//set the collider size like the collision sub collider
			Vector3 size = (collisionInfo.hitNode.collider as BoxCollider).size;
			col.size = new Vector3( size.x, size.y, 200f);
			//set the collider ceneter like the collision sub collider
			col.center = (collisionInfo.hitNode.collider as BoxCollider).center;
			//set the collider material
			col.material = phyMat;
			
			//add a spriterenderer
			SpriteRenderer tmpRenderer = tmp.AddComponent<SpriteRenderer>();
			tmpRenderer.renderMode = spriteRenderer.renderMode;
			tmpRenderer.updateMode = SpriteRendererUpdateMode.LateUpdate;
			tmpRenderer.localRotation = spriteRenderer.localRotation;
			tmpRenderer.localScale = spriteRenderer.localScale;
			tmpRenderer.plane = spriteRenderer.plane;
			
			//add the collisioned sprite to the spriterenderer
			int i = tmpRenderer.AttachSprite( tmpSprTransform.sprite );
			SpriteTransform tran = tmpRenderer.GetSpriteTransform(i);
			//set the rotation and scale
			tran.rotation = tmpSprTransform.rotation;
			tran.scale = tmpSprTransform.scale;
			
			//set the new gameobject position as the collsioned sprite's position in world space
			Matrix4x4 l2wMat = spriteRenderer.GetLocalToWorldMatrix();
			tmp.transform.position = l2wMat.MultiplyPoint(tmpSprTransform.position);
			
			
			//add it to list
			objects.Add( tmp);
		}
	}	
}

