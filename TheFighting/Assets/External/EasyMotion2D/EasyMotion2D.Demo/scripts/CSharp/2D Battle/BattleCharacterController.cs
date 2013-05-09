using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyMotion2D;


//define the gamelayer usage
enum BattleLayerUsage
{
    GUI = 0,
    Effect = 16,
    ForeBackground = 17,
    Character = 18,
    Background  = 19
}



//character state/animationclip pair
[System.Serializable]
public class GameCharacterAniamtionInfo
{
    public string stateName;
    public SpriteAnimationClip clip;
}

//sprite override to clip, but not used in this example
[System.Serializable]
public class GameCharacterAniamtionPart
{
    public string partName;
    public string componentFullPath;
    public Sprite sprite;
}

//a character appearance class, here just include the state/clip pairs.
[System.Serializable]
public class GameCharacterAppearance
{
    public GameCharacterAniamtionInfo[] stateAnimations;
}










//a base character data class
[System.Serializable]
public class GameCharacterData
{
	//is character a player character?
    public bool isPlayer = true;
	
    public float enemySearchRange = 50f;
    public float moveSpeed = 150f;
    public bool defaultFaceToRight = true;
	
	//attack range, it might be decide by another data to evaluate, such as weapon/skill/etc
	//here just use a value
    public float attackRange = 150f;
	
	//the appearance data
    public GameCharacterAppearance appearance;
	
	//add appearance data
    public void ApplyCharacterData(SpriteAnimation animation)
    {
		//add clip to SpriteAnimation
        animation.Clear();
        foreach (GameCharacterAniamtionInfo gcai in appearance.stateAnimations)
        {
            animation.AddClip(gcai.clip, gcai.stateName);
        }
    }
}



//a basic character controller controlled by a simple state machine
[RequireComponent(typeof( BehaviourControllerHolder))]
public class BattleCharacterController : MonoBehaviour {
	
	//character data
    public GameCharacterData characterData;
	
	//state machine 
    private BehaviourControllerHolder behaviourController;
	
	//sprite renderer
    private SpriteRenderer spriteRenderer;
	
	//sprite animation
    private SpriteAnimation spriteAnimation;

	//is this instance add to the static list
    private bool isAddToList = false;
	
	//a static list use the get the all BattleCharacterController instance in scene
    public static List<BattleCharacterController> characters = new List<BattleCharacterController>();


    void OnEnable()
    {
		//get components references
        behaviourController = GetComponent<BehaviourControllerHolder>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteAnimation = GetComponent<SpriteAnimation>();
		
		//add instance to list
        if (!isAddToList)
        {
            characters.Add(this);
            isAddToList = true;
        }
		
		//add state machine state change handler
        behaviourController.behaviourController.onBehaviourChange += OnChangeState;
    }


    void OnDisable()
    {
		//remove state machine state change handler
        behaviourController.behaviourController.onBehaviourChange -= OnChangeState;
		
		//remove instance from static list
        if (isAddToList)
        {
            characters.Remove(this);
            isAddToList = false;
        }
    }


    void Start()
    {
		//set the instance layer, this will decide the rendering depth in sccene
        gameObject.layer = (int)BattleLayerUsage.Character;
		
		//apply character data
        characterData.ApplyCharacterData(spriteAnimation);
		
		//entry idle state
        Idle();
    }


    // Update is called once per frame
    void Update()
    {
		//calculate the depth in layer by transform's y
        int depth = (int)((transform.position.y + 300) / 600 * 512f);
		
		//bring the player's depth more front
        if (characterData.isPlayer)
            depth -= 1;
		
		//apply the depth
        spriteRenderer.depth = depth;
    }




    //draw character search range
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, characterData.enemySearchRange);
    }






	//state machine state change handler, here just play new state's animation 
    void OnChangeState(string prev, string next)
    {
        spriteAnimation.Play(next);
    }








	//select character, set to mark appearance
    public void Select()
    {
        spriteRenderer.color = Color.white;
    }
	
	//unselect, reset to normal appearance
    public void Unselect()
    {
        Color color = Color.white * 0.5f;
        color.a = 1f;
        spriteRenderer.color = color;
    }


	//Movetarget class
    public class MoveTarget
    {
        public enum MoveTargetType
        {
            None = 0,
            Position,
            Character,
        }

        public MoveTargetType type = MoveTargetType.None;
        public Vector3 targetPosition = Vector3.zero;
        public BattleCharacterController targetObject = null;

        public MoveTarget( Vector3 target )
        {
            type = MoveTargetType.Position;
            targetPosition = target;
        }

        public MoveTarget(BattleCharacterController target)
        {
            type = MoveTargetType.Character;
            targetObject = target;
        }
    }




	//change character state to idle
    public void Idle()
    {
        behaviourController.ChangeState("Idle");
    }



    private MoveTarget moveTarget = null;
	
	//change character state to move, and set the move target
    public void Move( MoveTarget target )
    {
        moveTarget = target;
        behaviourController.ChangeState("Move");
    }


	//change character state to damage
    public void Damage()
    {
        behaviourController.ChangeState("Damage");
    }

	//change character state to attack
    public void Attack()
    {
        behaviourController.ChangeState("Attack");
    }




	//find a enemy as a target
    public MoveTarget SearchEnemy()
    {
		//if we have a target object already, just return it
        if (moveTarget != null && moveTarget.type == MoveTarget.MoveTargetType.Character )
            return moveTarget;


        MoveTarget tmpTarget = null;
		//find the nearest enemy in search range
        foreach (BattleCharacterController bcc in characters)
        {
			//not same category type, and not myself
            if (bcc.characterData.isPlayer != characterData.isPlayer && bcc != this)
            {
                float dis = GetDistanceToCharacter(bcc, Vector3.zero);
                if (dis < characterData.enemySearchRange)
                {
                    if (tmpTarget == null)
                        tmpTarget = new MoveTarget(bcc);
                    else
                    {
                        float lastTargetDis = GetDistanceToCharacter(tmpTarget.targetObject, Vector3.zero);

                        if (dis < lastTargetDis)
                            tmpTarget.targetObject = bcc;
                    }
                }
            }
        }

        return tmpTarget;
    }



    public Vector3 GetDirectionToCharacter(BattleCharacterController target, Vector3 offset)
    {
        return (target.transform.position + offset) - transform.position;
    }


    float GetDistanceToCharacter(BattleCharacterController target, Vector3 offset)
    {
        Vector3 src = transform.position;
        Vector3 dst = target.transform.position + offset;
        src.z = dst.z = 0f;

        return Vector3.Distance(src, dst);
    }


	//is a target in my attack range?
    public bool IsTargetInAttackRange()
    {
        if (moveTarget != null && moveTarget.type == MoveTarget.MoveTargetType.Character )
        {
            float dis = GetDistanceToCharacter( moveTarget.targetObject, Vector3.zero);
            return dis < characterData.enemySearchRange;
        }
        return false;
    }

	//if a target out side the battle scene?
    public bool IsTargetOutOfBattleField()
    {
        return false;
    }

	//is target down?
    public bool IsTargetDown()
    {
        if (moveTarget != null && moveTarget.type == MoveTarget.MoveTargetType.Character)
        {
            return moveTarget.targetObject.behaviourController.currentState == "Dead";
        }
        return false;
    }

























	//these On_xxx_xxx function called by BehaviourController with SendMessage
	
	//on state idle update
    public void On_Idle_Update(float dt)
    {
		//we are idle, try to find a enemy
        MoveTarget tmp =  SearchEnemy();  
		
		//if got one
        if ( tmp != null )
        {
            float dis = GetDistanceToCharacter(tmp.targetObject, Vector2.zero);
            float yDis = Mathf.Abs(tmp.targetObject.transform.position.y - transform.position.y);
			
			//if we can attack enemy immediatally? if yes, attack it, if no move to it
			if ( dis < characterData.attackRange && yDis < characterData.attackRange * 0.2f )
            {
                moveTarget = tmp;
                Attack();
            }
            else
                Move(tmp);
        }
    }


	//on state move update
    public void On_Move_Update( float t )
    {
		//if target lost by some reason, stop, change to idle
        if (moveTarget == null)
        {
            Idle();
            return;
        }

		//get target position
        Vector3 targetPos = Vector3.zero;
		
		//if target is a position, just use the position as target
		if (moveTarget.type == MoveTarget.MoveTargetType.Position)
        {
            targetPos = moveTarget.targetPosition;
        }		
		//if target is a character, adjust a offset to the target, might evaluate by weapon/skill/etc
        else if (moveTarget.type == MoveTarget.MoveTargetType.Character)
        {
            Vector3 _dir = GetDirectionToCharacter(moveTarget.targetObject, Vector3.zero);
            Vector3 offset = new Vector3( characterData.attackRange * (_dir.x > 0f ? -1 : 1), 0, 0);
            targetPos = moveTarget.targetObject.transform.position + offset;
        }


        //get the distance to target
        Vector3 dir = targetPos - transform.position;
        float dis = dir.magnitude;
		
		//if readlly near to target
        if (dis <= 3f)
        {
			//fix the position
            targetPos.z = 0f;
            transform.position = targetPos;
			
			//if reach target position, go to idle
            if (moveTarget.type == MoveTarget.MoveTargetType.Position)
            {
                moveTarget = null;
                Idle();
            }
			//if reach target character, then attack it
            else if (moveTarget.type == MoveTarget.MoveTargetType.Character)
            {
                Attack();
            }

            return;
        }
		
		//move the character position by speed * time
        transform.position = transform.position + dir.normalized * characterData.moveSpeed * Time.deltaTime;
		
		//adjust character face to direct
        {
            float s = dir.x > 0f ? 1 : -1;
            if (!characterData.defaultFaceToRight)
                s = -s;

            Vector3 scale = spriteRenderer.localScale;
            scale.x = Mathf.Abs(scale.x) * s;
            spriteRenderer.localScale = scale;
        }

    }


	//on state attack update
    public void On_Attack_Update(float dt)
    {
		//if target lost, go to idle
        if (moveTarget == null || moveTarget.type != MoveTarget.MoveTargetType.Character ||
            moveTarget.targetObject == null)
        {
            Idle();
            return;
        }

        BattleCharacterController character = moveTarget.targetObject;

        //if target down or lost in battle, go to idle
        if ( character.IsTargetDown() ||
            character.IsTargetOutOfBattleField() )
        {
            Idle();
        }

        //if target out of my attack range, then move to it
        if (!character.IsTargetInAttackRange() && 
            GetDistanceToCharacter(moveTarget.targetObject, Vector2.zero) > characterData.attackRange * 1.1f )
        {
            Move(moveTarget);
        }
    }


	//on state attack start
    public void On_Attack_Start()
    {
		//if target lost, go to idle
        if (moveTarget == null || moveTarget.type != MoveTarget.MoveTargetType.Character ||
            moveTarget.targetObject == null)
        {
            Idle();
            return;
        }
		
		//adjust the character face to direction
        Vector3 dir = GetDirectionToCharacter(moveTarget.targetObject, Vector3.zero);
        float s = dir.x > 0f ? 1 : -1;
        if (!characterData.defaultFaceToRight)
            s = -s;

        Vector3 scale = spriteRenderer.localScale;
        scale.x = Mathf.Abs(scale.x) * s;
        spriteRenderer.localScale = scale;
    }




    public GameObject damageMessage;
    Color tmpColor;
	
	//on state damage start
    public bool On_Damage_Start()
    {
		//create a damage effect
        GameObject tmp = Instantiate(damageMessage, transform.position, Quaternion.identity) as GameObject;
        tmp.layer = (int)BattleLayerUsage.Effect;
        tmp.GetComponent<DamageMessage>().SetText("damage");


		//store old state data
        tmpColor = spriteRenderer.color;
        
		//set damage state data
        Color color = Color.red * 0.5f;
        color.a = 1f;
        spriteRenderer.color = color;

        return false;
    }
	
	
	//on state damage end
    public void On_Damage_End()
    {
		//restore old state data
        spriteRenderer.color = tmpColor;
    }







    private AttackHitInfo atkHit = new AttackHitInfo();
    bool doAttackCollisionDetect = false;

	//Attack handler, here just called by event in animation clip.
	//Usually, you should do collision detection here, or call this function after objects have collision.
    public void StartAttack(SpriteAnimationEvent evt)
    {
        doAttackCollisionDetect = true;

        if (moveTarget.targetObject.moveTarget != null)
            moveTarget.targetObject.moveTarget = new MoveTarget(this);

		//target get damage
        moveTarget.targetObject.Damage();
    }
	
	//
    public void EndAttack(SpriteAnimationEvent evt)
    {
        doAttackCollisionDetect = false;
    }
	

    //public void AttackHit(SpriteAnimationEvent evt)
    //{
    //    if (moveTarget.type == MoveTarget.MoveTargetType.Character &&
    //        moveTarget.targetObject != null)
    //    {
    //        moveTarget.targetObject.Damage();
    //    }
    //}

}
