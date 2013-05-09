
public var direction : Vector3 = new Vector3(1,0,0);
public var speed : float = 100;

// Update is called once per frame
function Update () {

	//if bullet will hit some collider, destroy it self.
	var hit : RaycastHit ;
	if ( Physics.Raycast(transform.position, direction, hit, speed * Time.deltaTime ))
	{
		Destroy(gameObject);
	}

	//move the bullet
	transform.position = transform.position + direction * speed * Time.deltaTime;
}
