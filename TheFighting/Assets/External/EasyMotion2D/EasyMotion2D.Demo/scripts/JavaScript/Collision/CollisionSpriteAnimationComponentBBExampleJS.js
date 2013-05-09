


private var spriteRenderer : EasyMotion2D.SpriteRenderer;
private var spriteAnimation : EasyMotion2D.SpriteAnimation;

//last mouse hover component, a component that means a bone in the animation.
private var lastHoverSprite : EasyMotion2D.SpriteTransform = null;

function Start()
{
	spriteRenderer = GetComponent(EasyMotion2D.SpriteRenderer);
	spriteAnimation = GetComponent(EasyMotion2D.SpriteAnimation);
}



function OnGUI()
{
	//reset the character animation and collider
	if ( GUI.Button( buttonPosition, "Reset" ))
	{
		//make sure all sub sprite visible
		for( var i : int = 0; i < spriteRenderer.GetAttachSpriteCount(); i++)
			spriteRenderer.GetSpriteTransform(i).visible = true;
		
		//enable all sub colliders
		for( var tran : Transform in GetComponentsInChildren(Transform))
		{
			if ( tran.collider != null)
				tran.collider.enabled = true;
		}
		
		//destory created gameobjects
		for( var obj : GameObject in objects )
		{
			Destroy( obj);
		}
		
		//clear gameobject list
		objects.Clear();
	}
}

//reset button position
public var buttonPosition : Rect;

//collider material
public var phyMat : PhysicMaterial;

//gameobject list, store the created gameobject
public var objects : ArrayList = new ArrayList();


//if mouse over on any sub collider
function OnComponentMouseUpAsButton( collisionInfo : EasyMotion2D.CollisionNodeInfo )
{
	var tmpSprTransform : EasyMotion2D.SpriteTransform = spriteRenderer.GetSpriteTransform( collisionInfo.hitNode.componentPath);

	//save the last hover component
	if ( tmpSprTransform != null )
	{
		//make the collisioned sub part unvisibile
		tmpSprTransform.visible = false;
		//make the collisioned collider disable
		collisionInfo.hitNode.collider.enabled = false;
		
		//create a new gameobject in scene
		var tmp : GameObject = new GameObject();
		
		//add a rigidbody to gameobject
		tmp.AddComponent(Rigidbody);
		//set the position and rotation constraint
		tmp.rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
		//give a random velocity
		tmp.rigidbody.velocity = Random.insideUnitSphere * 300f;
		
		//add a boxcollider
		var col : BoxCollider = tmp.AddComponent(BoxCollider);
		
		//set the collider size like the collision sub collider
		var size : Vector3 = (collisionInfo.hitNode.collider as BoxCollider).size;
		col.size = new Vector3( size.x, size.y, 200f);
		//set the collider ceneter like the collision sub collider
		col.center = (collisionInfo.hitNode.collider as BoxCollider).center;
		//set the collider material
		col.material = phyMat;
		
		//add a spriterenderer
		var tmpRenderer : EasyMotion2D.SpriteRenderer = tmp.AddComponent(EasyMotion2D.SpriteRenderer);
		tmpRenderer.renderMode = spriteRenderer.renderMode;
		tmpRenderer.updateMode = EasyMotion2D.SpriteRendererUpdateMode.LateUpdate;
		tmpRenderer.localRotation = spriteRenderer.localRotation;
		tmpRenderer.localScale = spriteRenderer.localScale;
		tmpRenderer.plane = spriteRenderer.plane;
					
		//add the collisioned sprite to the spriterenderer
		var i : int = tmpRenderer.AttachSprite( tmpSprTransform.sprite );
		var tran : EasyMotion2D.SpriteTransform = tmpRenderer.GetSpriteTransform(i);
		//set the rotation and scale
		tran.rotation = tmpSprTransform.rotation;
		tran.scale = tmpSprTransform.scale;
		
		//set the new gameobject position as the collsioned sprite's position in world space
		var l2wMat : Matrix4x4 = spriteRenderer.GetLocalToWorldMatrix();
		tmp.transform.position = l2wMat.MultiplyPoint(tmpSprTransform.position);		
		
		//add it to list
		objects.Add( tmp);
	}
}	

