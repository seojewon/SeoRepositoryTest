//this file use to control the mummy in the demo3
//it's just to show you how to use EasyMotion2D's features in Javascript
//it is not a 2D platform game tutorial, you should not use this code in your solution directly




@script RequireComponent(CharacterController)
@script RequireComponent(BehaviourControllerHolderJS)

//component's reference
private var behaviourController : BehaviourControllerHolderJS;
private var spriteRenderer : EasyMotion2D.SpriteRenderer;
private var spriteAnimation : EasyMotion2D.SpriteAnimation;
private var controller : CharacterController;


//mummy's hp, 1 hit 1 poion
public var hp : int = 5;

//is face to left in animation clip
public var faceToLeftInAnimation : boolean = false;

//move direction, 1 to right, -1 to left
public var moveDirection : float= 1;

//mummy's move speed
public var moveSpeed : float = 30f;

//player character's transform
public var target : Transform;

//it mummy distance to player less than viewRange, mummy will move to player
public var viewRange : float = 500f;

//it mummy distance to player less than viewRange, mummy will attack player
public var attackRanage : float = 50f;

//sprite use to replace in animation
public var brokenhead : EasyMotion2D.Sprite;

//mummy state flag
private var moveToTarget : boolean = false;

//last attack hit information by other character
private var  lastAttackHit : AttackHitInfoJS;

//attack hit infomatrion of mine
private var atkHit : AttackHitInfoJS = new AttackHitInfoJS();





// Use this for initialization
function Awake () {
	//get component reference
	spriteRenderer = GetComponent(EasyMotion2D.SpriteRenderer);
	spriteAnimation = GetComponent(EasyMotion2D.SpriteAnimation);
	controller = GetComponent(CharacterController);
	behaviourController = GetComponent(BehaviourControllerHolderJS);

	//random init move direction
	moveDirection = Random.Range(0f, 1f) > 0.5f ? -1 : 1;

	//face to move direction
	UpdateFaceTo();
}



function SetTarget( mummyTarget : Transform )
{
	target = mummyTarget;
}


function UpdateFaceTo()
{
	//if character face direction in animation clip is left, then use a negative x axis scale
	var xs : float = faceToLeftInAnimation ? -1f : 1f;

	//apply x axis scale to the transform
	var ls : Vector3 = gameObject.transform.localScale;
	ls.x = Mathf.Abs( ls.x ) * moveDirection * xs;
	gameObject.transform.localScale = ls;
}
	




// Update is called once per frame
function Update () {
	
	//if no target, then do not doing anything
	if ( target == null)
		return;
	
	
	//get distance to target
	var dis : float = Mathf.Abs( target.transform.position.x - transform.position.x);

	//if distance to target less attackRanage, then do attack
	if ( dis < attackRanage )
	{
		Attack();
	}

	//if distance to target less viewRange, then move to target
	else if ( dis < viewRange )
	{
		moveDirection = Mathf.Sign( target.transform.position.x - transform.position.x);
		moveToTarget = true;
	}
	else
	{
		moveToTarget = false;
	}
}





//if CharacterController detect collision with something
function OnControllerColliderHit( hit : ControllerColliderHit) {

	//if collision a cube in scene and we not move to the target, then turn around
	if ( hit.transform.name != "Floor" &&  !moveToTarget  )
	{
		moveDirection = -moveDirection;
	} 
}







//character action functions, just translate the character's state
function Idle()
{
	behaviourController.ChangeState("Idle");
}

function Walk()
{
	behaviourController.ChangeState("Walk");
}

function Attack()
{
	behaviourController.ChangeState("Attack");
}


function Damage()
{
	behaviourController.ChangeState("Damage");
}

function Dead()
{
	behaviourController.ChangeState("Dead");
}








//send a message to call the function OnAttackHit in enemy
function AttackToEnemy( obj : GameObject)
{
	obj.SendMessage("OnAttackHit", atkHit , SendMessageOptions.DontRequireReceiver );
}







//restore mummy color to white
function RestoreColor()
{
	spriteRenderer.color = Color.white;
}






//function called by another gameobject use a SendMessage
//means someone doing an attack hit you
function OnAttackHit( atkHit : AttackHitInfoJS)
{
	lastAttackHit = atkHit;
	Damage();
}

	




//use a animation componet's motion track to detect attack collision
function OnAttackcollisionDetect()
{
	//get the component's transform in SpriteAnimation
	//the transform is in the local space
	var tran : EasyMotion2D.SpriteTransform = spriteRenderer.GetSpriteTransform("hitbox");
	
	if ( tran != null )
	{
		//create a transform matrix with animation component's motion information
		var trs : Matrix4x4 = Matrix4x4.TRS(
			new Vector3(tran.position.x, tran.position.y), 
			Quaternion.Euler(0, 0, tran.rotation), 
			tran.scale);

		//transform hitbox position from local space to world space
		trs = transform.localToWorldMatrix * trs;
		var currPos : Vector3 = trs.MultiplyPoint( Vector3.zero );

		//attack direction
		var fwd : Vector3 = new Vector3( Mathf.Sign(transform.localScale.x), 0, 0);

		//fill attack hit info
		atkHit.currPos = currPos;
		atkHit.direction = fwd;
		atkHit.attacker = gameObject;


		//raycast length
		var dis : float = Mathf.Abs( Vector3.Distance( atkHit.startPos, currPos) );
		if ( dis <= 0f)
			return;

		//raycast direction
		var dir : Vector3 = currPos - atkHit.startPos;

		//do a raycast to find does any object was hit?
		var hits : RaycastHit[];
		hits = Physics.RaycastAll( atkHit.startPos, dir, dis );
		Debug.DrawLine( atkHit.startPos, currPos);

		for ( var hit : RaycastHit in hits)
		{
			// if hit the target, call AttackToEnenmy
			if ( hit.transform == target )
				AttackToEnemy( hit.transform.gameObject );
		}
	}
}




//SpriteAnimation's event handler
//Define two event in clip
//When you play this clip, that will call function StartActtack and EndAttack by SpriteAnimation component
function StartAttack( evt : EasyMotion2D.SpriteAnimationEvent)
{
	//get component's SpriteTransform named "hitbox"
	//we use the "hitbox"'s move track to do attacking raycast collision detection
	var tran : EasyMotion2D.SpriteTransform = spriteRenderer.GetSpriteTransform("hitbox");

	//transform hitbox position from local space to world space
	var trs : Matrix4x4 = Matrix4x4.TRS(
		new Vector3(tran.position.x, tran.position.y), 
		Quaternion.Euler(0, 0, tran.rotation), 
		tran.scale);

	trs = transform.localToWorldMatrix * trs;
	atkHit.startPos = trs.MultiplyPoint( Vector3.zero );

	//fill attack hit info
	atkHit.currPos = atkHit.startPos;
	atkHit.direction.x = moveDirection;
	atkHit.attacker = gameObject;
	atkHit.attackNo++;
}

function EndAttack( evt : EasyMotion2D.SpriteAnimationEvent)
{
}







//all functions are state handler below this line.
//there are call by GameObject.SendMessage in BehaviourControllerHolder.ChangeState.

function On_Spawn_Start()
{
	spriteAnimation.StopAll();
	spriteAnimation.Play("Mummy_Spawn");
}

function On_Idle_Start()
{
	spriteAnimation.StopAll();
	spriteAnimation.Play("Mummy_Idle");

	//after a random time, start walk
	var t : float = Random.Range(1f,2f);
	Invoke("Walk", t);
}



function On_Walk_Start()
{
	spriteAnimation.StopAll();
	spriteAnimation.Play("Mummy_Walk");
}



function On_Walk_Update( deltaTime : float)
{
	//move character
	controller.Move( new Vector3( moveSpeed * moveDirection * deltaTime, 0f, 0f) );
	UpdateFaceTo();
}



function On_Attack_Start()
{
	spriteAnimation.StopAll();
	spriteAnimation.Play("Mummy_Attack");

	UpdateFaceTo();
}

function On_Attack_Update( deltaTime : float )
{
	OnAttackcollisionDetect();
}

function On_Attack_End()
{
}


function On_Damage_Start()
{
	spriteRenderer.color = Color.red;
	Invoke("RestoreColor", 0.2f);


	spriteAnimation.StopAll();
	spriteAnimation.Play("Mummy_Damage");

	RegisterMoveHandler();
	Invoke("Idle", 0.333f);


	hp--;
	if ( hp == 2)
	{
		//at here, we set a sprite to overwrite a animation component information
		//so we replace a sprite to a part of the original clip, and the animation is go as nomarlly.
		//it can be use to implement a Avatar, or something like.
		var head : EasyMotion2D.SpriteTransform = spriteRenderer.GetSpriteTransform("/root/hip/body/head");
		head.overrideSprite = brokenhead;
	}

	if ( hp == 0)
	{
		Dead();
		return;
	}
}


function On_Damage_End()
{
	spriteAnimation.ClearComponentUpdateHandler( EasyMotion2D.SpriteAnimationComponentUpdateType.PreUpdateComponent);
}



function On_Dead_Start()
{
	controller.enabled = false;

	spriteAnimation.StopAll();
	spriteAnimation.Play("Mummy_dead");
}




function On_Destroy_Start()
{
	CancelInvoke();
	Destroy(gameObject);
}











private var mummyHip : EasyMotion2D.SpriteTransform = null;
private var lastHipX : float = 0f;

//register a delegate
//it will use a animation component's movement information to move the CharacterController
function RegisterMoveHandler()
{
	//get transform of a compoent named "hip"
	mummyHip = spriteRenderer.GetSpriteTransform("hip");

	//save the position of x
	lastHipX = mummyHip.position.x;

	//set a delegate function that will callback before a component will transform 
	spriteAnimation.AddComponentUpdateHandler(  EasyMotion2D.SpriteAnimationComponentUpdateType.PreUpdateComponent, moveHandler);
}




//the comoponent transform callback handler
function moveHandler( ani : EasyMotion2D.SpriteAnimation )
{
	if ( mummyHip != null )
	{			
		//get offset at X-axis from last animation update
		var moveOffset : float =  mummyHip.position.x - lastHipX;
		lastHipX = mummyHip.position.x;

		//set the x to 0, then this component's animation will do not have a movement in x-axis
		mummyHip.position.x = 0f;

		//use offset to move character
		controller.Move( new Vector3( lastAttackHit.direction.x * moveOffset, 0f, 0f) );			

		//set position to character, and lock the z positino with a const
		transform.position = new Vector3( transform.position.x,
			transform.position.y, 118);
	}
}
