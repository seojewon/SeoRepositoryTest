//This script show you how to use CollisionComponent component with SpriteRendererAABB flag.
//When use SpriteRendererAABB flag, will Create a BoxCollider at same gameobject.
//You just need use the Unity3D build-in handler to respond the collusion.




private var spriteRenderer : EasyMotion2D.SpriteRenderer;

function Start()
{
	spriteRenderer = GetComponent(EasyMotion2D.SpriteRenderer);
}



function OnMouseOver()
{
	spriteRenderer.color = Color.red;
}


function OnMouseExit()
{
	spriteRenderer.color = Color.white;
}



function OnTriggerEnter( col : Collider) 
{
	spriteRenderer.color = Color.green;
}



function OnTriggerExit( collision : Collider)
{
	spriteRenderer.color = Color.white;
}
