//this file use to control the warrior in the demo3
//it's just to show you how to use EasyMotion2D's features in Javascript
//it is not a 2D platform game tutorial, you should not use this code in your solution directly



@script RequireComponent(CharacterController)
@script RequireComponent(BehaviourControllerHolderJS)


//component reference
private var controller : CharacterController;
private var spriteAnimation : EasyMotion2D.SpriteAnimation;
private var spriteRenderer : EasyMotion2D.SpriteRenderer;
private var behaviourController : BehaviourControllerHolderJS;

//camera in the scene, will move camera to track warrior's position
public var gameCamera : Camera;

//move speed
public var speed : float = 100;

//
public var jumpSpeed : float = 50;

//x movement direction
public var xDirection : float = 1f;

//y movement direection
public var yDirection : float = 0f;

//is character grounded
private var isGrounded : boolean = false;

//use to get animation component's transform inforation, use to adjust warrior's position
private var warriorRoot : EasyMotion2D.SpriteTransform = null;

//save warrior's position in the last update 
private var lastPosition : Vector3 = Vector3.zero;

//what's this the warrior grounded
private var activePlatform : Transform;

//warrior position in grounded object's local space
private var activeLocalPlatformPoint : Vector3 = Vector3.zero;	

//warrior position in world space
private var activeGlobalPlatformPoint : Vector3 = Vector3.zero;	

//is warrior attacing
private var isAttacking : boolean = false;

//is need to do attack collision detect
private var isNeedAttackcollisionDetect : boolean = false;

//attack combo id
private var nextAttackID : int = 1;

//last x in the warriorRoot's position
private var lastRootX : float = 0;

//warrior attack hit inforamation
private var atkHit : AttackHitInfoJS = new AttackHitInfoJS();




// Use this for initialization
function Start () {
	//get components references
	controller = GetComponent(CharacterController);
	spriteRenderer = GetComponent(EasyMotion2D.SpriteRenderer);
	spriteAnimation = GetComponent(EasyMotion2D.SpriteAnimation);
	behaviourController = GetComponent(BehaviourControllerHolderJS);

	//init data member
	lastPosition = transform.position;
	isGrounded = controller.isGrounded;
}








function SetFaceToDirection()
{
	//if hold a movement key, and we are not in attack, we can set the face direction
	if ( Input.GetKey(KeyCode.LeftArrow) && !isAttacking )
		transform.localScale = new Vector3(-1f,1f,1f);
	
	if ( Input.GetKey(KeyCode.RightArrow) && !isAttacking )
		transform.localScale = new Vector3(1f,1f,1f);		
}




function Update()
{
	//is any move key hold
	var isMoveKey : boolean = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);

	//if press a key use for move, set move direction
	if ( isMoveKey )
	{
		if ( Input.GetKey(KeyCode.LeftArrow)  )
			xDirection = -1f;
		
		if ( Input.GetKey(KeyCode.RightArrow) )
			xDirection = 1f;

		//if warrior is grounded, we can dash
		if ( isGrounded )
			Dash();
	}
	else
		xDirection = 0f;

	//if we are grounded, then press up can jump
	if ( Input.GetKeyDown(KeyCode.UpArrow) && isGrounded )
		Jump();

	//if we are grounded and we are falling down, can go to idle
	if ( isGrounded && !isMoveKey && yDirection < 0f &&  behaviourController.currentState != "Damage" )
		Idle();

	//press space then attack
	if ( Input.GetKeyDown(KeyCode.Space) )
		Attack();

	//if need do a attack collision detection
	if ( isNeedAttackcollisionDetect )
		OnAttackcollisionDetect();

	//set the warrior face to correct direction
	SetFaceToDirection();

	//move warrior
	Move();

	//keep camera tracking the warrior
	MoveCamera();
}




function MoveCamera()
{
	//move camera's x to character's x using lerp
	var pos : Vector3 = transform.position;
	pos.x = Mathf.Lerp( gameCamera.transform.position.x, pos.x, 0.1f);
	gameCamera.transform.position = new Vector3(
		pos.x, gameCamera.transform.position.y, gameCamera.transform.position.z);
}





// Update is called once per frame
function Move() 
{
	//moving platform support
	MovingPlatformSupport();

	var delta : float =  Time.deltaTime;
	
	//update y movement
	yDirection -= delta;
	yDirection = Mathf.Clamp( yDirection, -1f, 1f);

	//calculate x movement
	var x : float = 0f;
		
	//if not in attacking, then we can move warrior
	if ( !isAttacking &&  behaviourController.currentState != "Damage" )
	{
		x = speed * xDirection * delta;
	}

	//apply movement to character
	controller.Move( new Vector3( x, 
		yDirection * jumpSpeed * delta,
		0) );

	//lock the transform's z
	transform.position = new Vector3( transform.position.x,
		transform.position.y, 118);

	//if we are in falling
	if ( lastPosition.y > transform.position.y )
	{
		var down : Vector3 = new Vector3(0,-1,0);

		//if something below character and distance less to 20, try to landing
		if (Physics.Raycast(transform.position, down, 20))
			Idle();
	}

	//last update not grounded, and now grounded
	//so at this moment, our warrir is landing to floor or something else
	if (!isGrounded && controller.isGrounded )
		Idle();

	//save the changes
	isGrounded = controller.isGrounded;
	lastPosition = transform.position;
}






function MovingPlatformSupport()
{
	// Moving platform support
	if (activePlatform != null && isGrounded ) 
	{
		var newGlobalPlatformPoint : Vector3 = activePlatform.TransformPoint(activeLocalPlatformPoint);			
		var moveDistance : Vector3 = (newGlobalPlatformPoint - activeGlobalPlatformPoint);
		transform.position = transform.position + moveDistance;

		activeGlobalPlatformPoint = transform.position;
		activeLocalPlatformPoint = activePlatform.InverseTransformPoint (transform.position);
	}
}







function OnControllerColliderHit( hit : ControllerColliderHit) 
{
	if (hit.moveDirection.y > 0.01) 
		return;
//		
//		// Make sure we are really standing on a straight platform
//		// Not on the underside of one and not falling down from it either!
	if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.9)
		activePlatform = hit.collider.transform;
		activeGlobalPlatformPoint = transform.position;
		activeLocalPlatformPoint = activePlatform.InverseTransformPoint (transform.position);
}












//character action functions, just translate the character's state
function Idle()
{
	behaviourController.ChangeState("Idle");
}



function Dash()
{
	behaviourController.ChangeState("Dash");
}


function Damage()
{
	behaviourController.ChangeState("Damage");
}



function Attack()
{
	behaviourController.ChangeState("Attack" + nextAttackID);

	//increase attack state id
	nextAttackID++;
}



function Jump()
{
	behaviourController.ChangeState("Jumping");
}



function Landing()
{
	behaviourController.ChangeState("JumpEnd");
}










function On_Idle_Start()
{
	//reset next attack state id
	nextAttackID = 1;

	spriteAnimation.StopAll();
	spriteAnimation.Play("Warrior_idle");
}




function On_Dash_Start()
{
	//reset next attack state id
	nextAttackID = 1;

	spriteAnimation.StopAll();
	spriteAnimation.Play("Warrior_dash");
}





function On_Jumping_Start()
{
	controller.height = 20;
	controller.center = new Vector3(0, 10, 0);

	yDirection = 0.5f;


	spriteAnimation.StopAll();
	spriteAnimation.Play("Warrior_jumping");
}




function On_Jumping_End()
{
	controller.height = 140;
	controller.center = new Vector3(0, 70, 0);
	xDirection = 0f;
}



function InitAttack( aniName : String )
{
	isAttacking = true;
	xDirection = 0f;

	if ( Input.GetKey(KeyCode.LeftArrow) )
		transform.localScale = new Vector3(-1f,1f,1f);
	
	if ( Input.GetKey(KeyCode.RightArrow) )
		transform.localScale = new Vector3(1f,1f,1f);	
	
	spriteAnimation.StopAll();
	spriteAnimation.Play(aniName);

	RegisterMoveHandler();
}




function On_Attack1_Start()
{
	InitAttack("Warrior_attack_2");
}



function On_Attack2_Start()
{
	InitAttack("Warrior_attack_1");
}



function On_Attack3_Start()
{
	InitAttack("Warrior_attack_3");
}



function On_Attack4_Start()
{
	InitAttack("Warrior_attack_5");
}



function On_AttackEnd_End()
{
	isAttacking = false;
	UnregisterMoveHandler();
}




function RestoreColor()
{
	spriteRenderer.color = Color.white;
}





function On_Damage_Start()
{
	spriteRenderer.color = Color.red;
	Invoke("RestoreColor", 0.2f);


	spriteAnimation.StopAll();
	spriteAnimation.Play("Warrior_damage");

	RegisterMoveHandler();
}

function On_Damage_End()
{
	spriteAnimation.ClearComponentUpdateHandler( EasyMotion2D.SpriteAnimationComponentUpdateType.PreUpdateComponent);
}



var isRegisterMoveHandler : boolean = false;

//register a delegate
//it will use a animation component's movement information to move the CharacterController
function RegisterMoveHandler()
{
	//get transform of a compoent named "Lee"
	warriorRoot = spriteRenderer.GetSpriteTransform("Lee");

	//save the position of x
	lastRootX = warriorRoot.position.x;


	if ( !isRegisterMoveHandler  )
	{
		isRegisterMoveHandler = true;

		//set a delegate function that will callback before a component will transform 
		spriteAnimation.AddComponentUpdateHandler(  EasyMotion2D.SpriteAnimationComponentUpdateType.PreUpdateComponent, moveHandler);
	}
}


function UnregisterMoveHandler()
{
	spriteAnimation.ClearComponentUpdateHandler( EasyMotion2D.SpriteAnimationComponentUpdateType.PreUpdateComponent);
	isRegisterMoveHandler = false;
}



//the comoponent transform callback handler
function moveHandler( ani : EasyMotion2D.SpriteAnimation)
{
	if ( warriorRoot != null )
	{			
		//get offset at X-axis from last animation update
		var moveOffset : float =  warriorRoot.position.x - lastRootX;
		lastRootX = warriorRoot.position.x;
		
		//set the x to 0, then this component's animation will do not have a movement in x-axis
		warriorRoot.position.x = 0f;

		//use offset to move character
		controller.Move( new Vector3( moveOffset * transform.localScale.x, 0f, 0f) );

		//save current position
		lastPosition = transform.position;
	}
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
		var hits: RaycastHit[];
		hits = Physics.RaycastAll( atkHit.startPos, dir, dis );
		Debug.DrawLine( atkHit.startPos, currPos);


		for( var hit : RaycastHit in hits)
		{
			// a object with a rigidbody
			if ( hit.rigidbody != null )	
				AttackToCube( hit.transform.gameObject, fwd );

			// not me
			else if ( hit.transform != transform )
				AttackToEnemy( hit.transform.gameObject );
		}
	}
}






function AttackToCube( obj : GameObject,  fwd : Vector3)
{
	obj.rigidbody.AddForce( fwd * 3000);
}






function AttackToEnemy( obj : GameObject)
{
	obj.SendMessage("OnAttackHit", atkHit , SendMessageOptions.DontRequireReceiver );
}





//SpriteAnimation's event handler
//Define two event in clip
//When you play this clip, that will call function StartActtack and EndAttack
function StartAttack( evt : EasyMotion2D.SpriteAnimationEvent)
{
	isNeedAttackcollisionDetect = true;
	
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
	atkHit.direction.x = xDirection;
	atkHit.attacker = gameObject;
	atkHit.attackNo++;
}




function EndAttack( evt : EasyMotion2D.SpriteAnimationEvent)
{
	isNeedAttackcollisionDetect = false;
}




//function called by another gameobject use a SendMessage
//means someone doing an attack hit you
function OnAttackHit( atkHit : AttackHitInfoJS )
{
	Damage();
}

