
@script RequireComponent(BehaviourControllerJS)


private var gameStateController : BehaviourControllerJS = null;

@HideInInspector
public var currentState : String = "";

@HideInInspector
public var currentStateTime : float = 0.0f;

public var debug : boolean = false;

private var controllerEnabled : boolean = true;





function Awake()
{
	gameStateController = GetComponent(BehaviourControllerJS);
	gameStateController.onBehaviourChange = OnChangeState;
}




function Update()
{
	gameStateController.debug = debug;

	if (controllerEnabled)
	{
		var delta : float = Time.smoothDeltaTime ;
		currentStateTime += delta;
		BroadcastMessage("On_" + currentState + "_Update", delta, SendMessageOptions.DontRequireReceiver);
	}
}




//hehaviour handler
function ChangeState( stepName : String)
{
	return gameStateController.DoBehaviour(stepName);
}




function OnChangeState( prev : String, next : String)
{
	if ( debug )
		Debug.Log("OnChangeState from " + prev + " to " + next);

	BroadcastMessage("On_" + prev + "_End", null, SendMessageOptions.DontRequireReceiver);
	currentState = next;
	currentStateTime = 0.0f;
	BroadcastMessage("On_" + next + "_Start", this, SendMessageOptions.DontRequireReceiver);
}
