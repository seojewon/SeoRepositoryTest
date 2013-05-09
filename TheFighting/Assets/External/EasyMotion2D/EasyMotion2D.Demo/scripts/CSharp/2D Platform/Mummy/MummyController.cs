//this file use to control the mummy in the demo3
//it's just to show you how to use EasyMotion2D's features in C#
//it is not a 2D platform game tutorial, you should not use this code in your solution directly




using UnityEngine;
using System.Collections;
using EasyMotion2D;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(BehaviourControllerHolder))]
public class MummyController : MonoBehaviour {

	//component's reference
	private BehaviourControllerHolder behaviourController;
	private SpriteRenderer spriteRenderer;
	private SpriteAnimation spriteAnimation;
	private CharacterController	controller;


	//mummy's hp, 1 hit 1 poion
	public int hp = 5;

	//is face to left in animation clip
	public bool faceToLeftInAnimation = false;

	//move direction, 1 to right, -1 to left
	public float moveDirection = 1;

	//mummy's move speed
	public float moveSpeed = 30f;

	//player character's transform
	public Transform target;

	//it mummy distance to player less than viewRange, mummy will move to player
	public float viewRange = 500f;

	//it mummy distance to player less than viewRange, mummy will attack player
	public float attackRanage = 50f;

	//sprite use to replace in animation
	public EasyMotion2D.Sprite brokenhead;

	//mummy state flag
	private bool moveToTarget = false;

	//last attack hit information by other character
	private AttackHitInfo lastAttackHit;

	//attack hit infomatrion of mine
	private AttackHitInfo atkHit = new AttackHitInfo();





	// Use this for initialization
	void Awake () {
		//get component reference
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteAnimation = GetComponent<SpriteAnimation>();
		controller = GetComponent<CharacterController>();
		behaviourController = GetComponent<BehaviourControllerHolder>();

		//random init move direction
		moveDirection = Random.Range(0f, 1f) > 0.5f ? -1 : 1;

		//face to move direction
		UpdateFaceTo();
	}


	void UpdateFaceTo()
	{
		//if character face direction in animation clip is left, then use a negative x axis scale
		float xs = faceToLeftInAnimation ? -1f : 1f;

		//apply x axis scale to the transform
		Vector3 ls = gameObject.transform.localScale;
		ls.x = Mathf.Abs( ls.x ) * moveDirection * xs;
		gameObject.transform.localScale = ls;
	}
	

	void SetTarget( Transform mummyTarget )
	{
		target = mummyTarget;
	}


	// Update is called once per frame
	void Update () {
		
		//if no target, then do not doing anything
		if ( target == null)
			return;
		
		
		//get distance to target
		float dis = Mathf.Abs( target.transform.position.x - transform.position.x);

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
	void OnControllerColliderHit( ControllerColliderHit hit) {

		//if collision a cube in scene and we not move to the target, then turn around
		if ( hit.transform.name != "Floor" &&  !moveToTarget  )
		{
			moveDirection = -moveDirection;
		} 
	}







	//character action functions, just translate the character's state
	void Idle()
	{
		behaviourController.ChangeState("Idle");
	}

	void Walk()
	{
		behaviourController.ChangeState("Walk");
	}

	void Attack()
	{
		behaviourController.ChangeState("Attack");
	}


	void Damage()
	{
		behaviourController.ChangeState("Damage");
	}

	void Dead()
	{
		behaviourController.ChangeState("Dead");
	}








	//send a message to call the function OnAttackHit in enemy
	void AttackToEnemy( GameObject obj)
	{
		obj.SendMessage("OnAttackHit", atkHit , SendMessageOptions.DontRequireReceiver );
	}







	//restore mummy color to white
	void RestoreColor()
	{
		spriteRenderer.color = Color.white;
	}






	//function called by another gameobject use a SendMessage
	//means someone doing an attack hit you
	void OnAttackHit( AttackHitInfo atkHit )
	{
		lastAttackHit = atkHit;
		Damage();
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
				// if hit the target, call AttackToEnenmy
				if ( hit.transform == target )
					AttackToEnemy( hit.transform.gameObject );
			}
		}
	}




	//SpriteAnimation's event handler
	//Define two event in clip named Mummy_Attack
	//When you play this clip, that will call function StartActtack and EndAttack
	void StartAttack( SpriteAnimationEvent evt)
	{
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
		atkHit.direction.x = moveDirection;
		atkHit.attacker = gameObject;
		atkHit.attackNo++;
	}

	void EndAttack( SpriteAnimationEvent evt)
	{
	}







	//all functions are state handlers.
	//there are call by GameObject.SendMessage in BehaviourControllerHolder.ChangeState.

	void On_Spawn_Start()
	{
		//spriteAnimation.StopAll();
		spriteAnimation.Play("Mummy_Spawn");
	}

	void On_Idle_Start()
	{
		//spriteAnimation.StopAll();
		spriteAnimation.Play("Mummy_Idle");

		//after a random time, start walk
		float t = Random.Range(1f,2f);
		Invoke("Walk", t);
	}



	void On_Walk_Start()
	{
		//spriteAnimation.StopAll();
		spriteAnimation.Play("Mummy_Walk");
	}



	void On_Walk_Update(float deltaTime)
	{
		//move character
		controller.Move( new Vector3( moveSpeed * moveDirection * deltaTime, 0f, 0f) );
		UpdateFaceTo();
	}



	void On_Attack_Start()
	{
		//spriteAnimation.StopAll();
		spriteAnimation.Play("Mummy_Attack");

		UpdateFaceTo();
	}

	void On_Attack_Update( float deltaTime )
	{
		OnAttackcollisionDetect();
	}

	void On_Attack_End()
	{
	}


	void On_Damage_Start()
	{
		spriteRenderer.color = Color.red;
		Invoke("RestoreColor", 0.2f);


		//spriteAnimation.StopAll();
		spriteAnimation.Play("Mummy_Damage");

		RegisterMoveHandler();
		Invoke("Idle", 0.333f);


		hp--;
		if ( hp == 2)
		{
			//at here, we set a sprite to overwrite a animation component information
			//so we replace a sprite to a part of the original clip, and the animation is go as nomarlly.
			//it can be use to implement a Avatar, or something like.
			SpriteTransform head = spriteRenderer.GetSpriteTransform("/root/hip/body/head");
			if ( head != null)
				head.overrideSprite = brokenhead;
		}

		if ( hp == 0)
		{
			Dead();
			return;
		}
	}


	void On_Damage_End()
	{
		spriteAnimation.ClearComponentUpdateHandler( SpriteAnimationComponentUpdateType.PreUpdateComponent);
	}


	void On_Dead_Start()
	{
		controller.enabled = false;

		//spriteAnimation.StopAll();
		spriteAnimation.Play("Mummy_dead");
	}




	void On_Destroy_Start()
	{
		CancelInvoke();
		Destroy(gameObject);
	}











	private SpriteTransform mummyHip = null;
	float lastHipX = 0f;

	//register a delegate
	//it will use a animation component's movement information to move the CharacterController
	void RegisterMoveHandler()
	{
		//get transform of a compoent named "hip"
		mummyHip = spriteRenderer.GetSpriteTransform("hip");

		//save the position of x
		lastHipX = mummyHip.position.x;

		//set a delegate function that will callback before a component will transform 
		spriteAnimation.AddComponentUpdateHandler( SpriteAnimationComponentUpdateType.PreUpdateComponent, moveHandler);
	}




	//the comoponent transform callback handler
	void moveHandler( SpriteAnimation ani )
	{
		if ( mummyHip != null )
		{			
			//get offset at X-axis from last animation update
			float moveOffset =  mummyHip.position.x - lastHipX;
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

}
