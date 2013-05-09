//this file use to control the warrior in the demo3
//it's just to show you how to use EasyMotion2D's features in C#
//it is not a 2D platform game tutorial, you should not use this code in your solution directly



using UnityEngine;
using System.Collections;
using EasyMotion2D;




[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(BehaviourControllerHolder))]
public class WarriorController : MonoBehaviour {
	
	//component reference
	private CharacterController	controller;
	private SpriteAnimation spriteAnimation;
	private SpriteRenderer spriteRenderer;
	private BehaviourControllerHolder behaviourController;

	//camera in the scene, will move camera to track warrior's position
	public Camera gameCamera;

	//move speed
	public float speed = 100;

	//
	public float jumpSpeed = 50;

	//x movement direction
	public float xDirection = 1f;

	//y movement direection
	public float yDirection = 0f;

	//is character grounded
	private bool isGrounded = false;

	//use to get animation component's transform inforation, use to adjust warrior's position
	private SpriteTransform warriorRoot = null;

	//save warrior's position in the last update 
	private Vector3 lastPosition = Vector3.zero;
	
	//what's this the warrior grounded
	private Transform activePlatform;

	//warrior position in grounded object's local space
	private Vector3 activeLocalPlatformPoint = Vector3.zero;	

	//warrior position in world space
	private Vector3 activeGlobalPlatformPoint = Vector3.zero;	

	//is warrior attacing
	private bool isAttacking = false;

	//is need to do attack collision detect
	private bool isNeedAttackcollisionDetect = false;

	//attack combo id
	private int nextAttackID = 1;

	//last x in the warriorRoot's position
	float lastRootX = 0;

	//warrior attack hit inforamation
	private AttackHitInfo atkHit = new AttackHitInfo();




	// Use this for initialization
	void Start () {
		//get components references
		controller = GetComponent<CharacterController>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteAnimation = GetComponent<SpriteAnimation>();
		behaviourController = GetComponent<BehaviourControllerHolder>();

		//init data member
		lastPosition = transform.position;
		isGrounded = controller.isGrounded;
	}








	void SetFaceToDirection()
	{
		//if hold a movement key, and we are not in attack, we can set the face direction
		if ( Input.GetKey(KeyCode.LeftArrow) && !isAttacking )
			transform.localScale = new Vector3(-1f,1f,1f);
		
		if ( Input.GetKey(KeyCode.RightArrow) && !isAttacking )
			transform.localScale = new Vector3(1f,1f,1f);		
	}




	void Update()
	{
		//is any move key hold
		bool isMoveKey = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);

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




	void MoveCamera()
	{
		//move camera's x to character's x using lerp
		Vector3 pos = transform.position;
		pos.x = Mathf.Lerp( gameCamera.transform.position.x, pos.x, 0.1f);
		gameCamera.transform.position = new Vector3(
			pos.x, gameCamera.transform.position.y, gameCamera.transform.position.z);
	}





	// Update is called once per frame
	void Move() 
	{
		//moving platform support
		MovingPlatformSupport();

		float delta =  Time.deltaTime;
		
		//update y movement
		yDirection -= delta;
		yDirection = Mathf.Clamp( yDirection, -1f, 1f);

		//calculate x movement
		float x = 0f;

		//if not in a un-movable state, then we can move warrior
		if ( !isAttacking && behaviourController.currentState != "Damage" )
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
			Vector3 down = new Vector3(0,-1,0);

			//if something below character and distance less to 20, try to landing
			if (Physics.Raycast(transform.position, down, 20))
				Idle();
		}

		//last update not grounded, and now grounded
		//so at this moment, our warrir is landing to floor or something else
	    if (!this.isGrounded && controller.isGrounded )
			Idle();

		//save the changes
		this.isGrounded = controller.isGrounded;
		lastPosition = transform.position;
	}






	void MovingPlatformSupport()
	{
		// Moving platform support
		if (activePlatform != null && isGrounded ) 
		{
			Vector3 newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPoint);			
			Vector3 moveDistance = (newGlobalPlatformPoint - activeGlobalPlatformPoint);
			transform.position = transform.position + moveDistance;

			activeGlobalPlatformPoint = transform.position;
			activeLocalPlatformPoint = activePlatform.InverseTransformPoint (transform.position);
		}
	}






	
	void OnControllerColliderHit(ControllerColliderHit hit) 
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
	void Idle()
	{
		behaviourController.ChangeState("Idle");
	}



	void Dash()
	{
		behaviourController.ChangeState("Dash");
	}


	void Damage()
	{
		behaviourController.ChangeState("Damage");
	}



	void Attack()
	{
		behaviourController.ChangeState("Attack" + nextAttackID);

		//increase attack state id
		nextAttackID++;
	}



	void Jump()
	{
		behaviourController.ChangeState("Jumping");
	}



	void Landing()
	{
		behaviourController.ChangeState("JumpEnd");
	}










	void On_Idle_Start()
	{
		//reset next attack state id
		nextAttackID = 1;

		spriteAnimation.StopAll();
		spriteAnimation.Play("Warrior_idle");
	}




	void On_Dash_Start()
	{
		//reset next attack state id
		nextAttackID = 1;

		spriteAnimation.StopAll();
		spriteAnimation.Play("Warrior_dash");
	}





	void On_Jumping_Start()
	{
		controller.height = 20;
		controller.center = new Vector3(0, 10, 0);

		yDirection = 0.5f;


		spriteAnimation.StopAll();
		spriteAnimation.Play("Warrior_jumping");
	}




	void On_Jumping_End()
	{
		controller.height = 140;
		controller.center = new Vector3(0, 70, 0);
		xDirection = 0f;
	}



	void InitAttack( string aniName )
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



	void On_Attack1_Start()
	{
		InitAttack("Warrior_attack_2");
	}



	void On_Attack2_Start()
	{
		InitAttack("Warrior_attack_1");
	}



	void On_Attack3_Start()
	{
		InitAttack("Warrior_attack_3");
	}



	void On_Attack4_Start()
	{
		InitAttack("Warrior_attack_5");
	}




	void On_AttackEnd_End()
	{
		isAttacking = false;
		UnregisterMoveHandler();
	}


	void RestoreColor()
	{
		spriteRenderer.color = Color.white;
	}





	void On_Damage_Start()
	{
		spriteRenderer.color = Color.red;
		Invoke("RestoreColor", 0.2f);


		spriteAnimation.StopAll();
		spriteAnimation.Play("Warrior_damage");

		RegisterMoveHandler();
	}

	void On_Damage_End()
	{
		UnregisterMoveHandler();
	}




	bool isRegisterMoveHandler = false;


	//register a delegate
	//it will use a animation component's movement information to move the CharacterController
	void RegisterMoveHandler()
	{
		//get transform of a compoent named "Lee"
		warriorRoot = spriteRenderer.GetSpriteTransform("Lee");

		//save the position of x
		lastRootX = warriorRoot.position.x;

		if ( !isRegisterMoveHandler  )
		{
			isRegisterMoveHandler = true;

			//set a delegate function that will callback before a component will transform 
			spriteAnimation.AddComponentUpdateHandler( SpriteAnimationComponentUpdateType.PreUpdateComponent, moveHandler);
		}
	}


	void UnregisterMoveHandler()
	{
		spriteAnimation.ClearComponentUpdateHandler( SpriteAnimationComponentUpdateType.PreUpdateComponent);
		isRegisterMoveHandler = false;
	}



	

	//the comoponent transform callback handler
	void moveHandler( SpriteAnimation ani )
	{
		if ( warriorRoot != null )
		{			
			//get offset at X-axis from last animation update
			float moveOffset =  warriorRoot.position.x - lastRootX;
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
	void OnAttackcollisionDetect()
	{
		//get the component's transform in SpriteAnimation
		//the transform is in the local space
		SpriteTransform tran = spriteRenderer.GetSpriteTransform("hitbox");

		if ( tran != null )
		{
			//create a transform matrix with animation component's motion information
			Matrix4x4 trs = Matrix4x4.TRS(
				new Vector3(tran.position.x, tran.position.y), 
				Quaternion.Euler(0, 0, tran.rotation), 
				tran.scale);

			//transform hitbox position from local space to world space
			trs = transform.localToWorldMatrix * trs;
			Vector3 currPos = trs.MultiplyPoint( Vector3.zero );

			//attack direction
			Vector3 fwd = new Vector3( Mathf.Sign(transform.localScale.x), 0, 0);

			//fill attack hit info
			atkHit.currPos = currPos;
			atkHit.direction = fwd;
			atkHit.attacker = gameObject;


			//raycast length
			float dis = Mathf.Abs( Vector3.Distance( atkHit.startPos, currPos) );
			if ( dis <= 0f)
				return;

			//raycast direction
			Vector3 dir = currPos - atkHit.startPos;

			//do a raycast to find does any object was hit?
			RaycastHit[] hits;
			hits = Physics.RaycastAll( atkHit.startPos, dir, dis );
			Debug.DrawLine( atkHit.startPos, currPos);


			foreach( RaycastHit hit in hits)
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

	




	void AttackToCube( GameObject obj, Vector3 fwd )
	{
		obj.rigidbody.AddForce( fwd * 3000);
	}






	void AttackToEnemy( GameObject obj)
	{
		obj.SendMessage("OnAttackHit", atkHit , SendMessageOptions.DontRequireReceiver );
	}





	//SpriteAnimation's event handler
	//Define two event in clip
	//When you play this clip, that will call function StartActtack and EndAttack
	void StartAttack( SpriteAnimationEvent evt)
	{
		isNeedAttackcollisionDetect = true;
		
		//get component's SpriteTransform named "hitbox"
		//we use the "hitbox"'s move track to do attacking raycast collision detection
		SpriteTransform tran = spriteRenderer.GetSpriteTransform("hitbox");

		//transform hitbox position from local space to world space
		Matrix4x4 trs = Matrix4x4.TRS(
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




	void EndAttack( SpriteAnimationEvent evt)
	{
		isNeedAttackcollisionDetect = false;
	}




	//function called by another gameobject use a SendMessage
	//means someone doing an attack hit you
	void OnAttackHit( AttackHitInfo atkHit )
	{
		Damage();
	}
}
