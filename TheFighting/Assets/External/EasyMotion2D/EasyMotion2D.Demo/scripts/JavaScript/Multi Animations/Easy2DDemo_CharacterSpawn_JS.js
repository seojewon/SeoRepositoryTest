public var prototypeObject : GameObject;
public var count : int = 50;

// Use this for initialization
function Start () {
	StartCoroutine(CreateObject());	
}
	
function CreateObject()
{
	//Instance one character per frame
	for( var i : int = 0; i < count; i ++)
	{
		Instantiate( prototypeObject, Vector3(400,300,0), Quaternion.identity);
		yield WaitForEndOfFrame();
	}
}