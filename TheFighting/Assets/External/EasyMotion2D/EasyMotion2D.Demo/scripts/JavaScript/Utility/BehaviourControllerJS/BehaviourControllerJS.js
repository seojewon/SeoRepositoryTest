




@System.Serializable
public class ObjectBehaviourJS
{
	public var name : String = "";
	public var time : float = 0f;
	public var acceptList : String[] = new String[0];
	public var nextBehaviour : String = "";

	public function IsAccept( nextBehaviour : String )
	{
		if ( acceptList.Length == 0 )
			return true;		

		for ( var s:String in acceptList)
			if ( nextBehaviour == s)
				return true;			
		return false;
	}
}







public var stateTransitionTable : StateTransitionTableAssetJS;
public var startBehaviour : String;

public var debug : boolean = false;

public	var onBehaviourChange : function( String, String );

private	var __behaviourTable : Hashtable = new Hashtable();


private var	currentBehaviourTime : float;
public	var	currentBehaviourName : String = "";



public function	GetBehaviour(  name : String )
{
	return __behaviourTable[name];
}


public function AddBehaviour( beh : ObjectBehaviourJS )
{
	try
	{
		__behaviourTable.Add( beh.name, beh );
	}
	catch ( Exception )
	{
	}
}



public function RemoveBehaviour( beh : ObjectBehaviourJS )
{
	__behaviourTable.Remove( beh.name );
}



public function ClearBehaviour()
{
	__behaviourTable.Clear();
}




// Use this for initialization
function Start () {	

	if ( stateTransitionTable == null  )
		return;

	for( var item : ObjectBehaviourJS in stateTransitionTable.behaviourList )
		AddBehaviour( item );

	DoBehaviour( startBehaviour );
}




// Update is called once per frame
function Update () {
	if ( stateTransitionTable == null  )
		return;

	var currentBehaviour : ObjectBehaviourJS = GetBehaviour( currentBehaviourName );
	currentBehaviourTime += Time.smoothDeltaTime ;

	if ( currentBehaviour != null )
	{
		if ( currentBehaviour.time > 0.0f && currentBehaviourTime >= currentBehaviour.time )
		{
			if ( currentBehaviour.nextBehaviour.Length > 0 )
				DoBehaviour( currentBehaviour.nextBehaviour );
		}
	}
}



public function DoBehaviour( name : String )
{
	if ( currentBehaviourName.Length > 0 )
	{
		var currentBehaviour : ObjectBehaviourJS = GetBehaviour( currentBehaviourName );
		if ( currentBehaviour != null && currentBehaviour.IsAccept( name ) == false )
		{
			if (debug)
				Debug.Log( "Behaviour:"+name+" not exist or current behaviour do not accept " + name );
			return false;			
		}
	}
	var behaviour : ObjectBehaviourJS = GetBehaviour( name );
	if ( behaviour != null )
	{
		var currName : String = currentBehaviourName;
		currentBehaviourName = name;
		currentBehaviourTime = 0.0f;

		if (onBehaviourChange != null )
			onBehaviourChange( currName, name );
		return true;
	}

	if (debug)
		Debug.Log( "Behaviour:"+name+" not exist or current behaviour do not accept " + name );
	return false;
}

