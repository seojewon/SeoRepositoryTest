
public var spriteAnimation : EasyMotion2D.SpriteAnimation;

// Use this for initialization
function Start () {
	//Get SpriteAnimation for GameObject
	spriteAnimation = GetComponent(EasyMotion2D.SpriteAnimation);

	spriteAnimation["Mummy_Attack"].layer = 1;
	spriteAnimation["Mummy_Attack"].AddMixingComponent( "/root/hip/body", true);		

	spriteAnimation["Mummy_Attack"].enabled = true;
	spriteAnimation["Mummy_Walk"].enabled = true;
}




public var guiPosiition : Rect;



function OnGUI()
{
	var state : EasyMotion2D.SpriteAnimationState = spriteAnimation["Mummy_Attack"];


	GUILayout.BeginArea (guiPosiition);		


	GUILayout.Label( "Attack Weight:" + state.weight.ToString() );
	var weight : float = GUILayout.HorizontalSlider( state.weight, 0.01f, 1f);
	if ( GUI.changed )
	{
		state.weight = weight;
	}


	GUILayout.EndArea();
}
