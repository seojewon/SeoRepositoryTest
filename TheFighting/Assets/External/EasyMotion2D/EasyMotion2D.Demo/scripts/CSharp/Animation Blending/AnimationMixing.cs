using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyMotion2D;

public class AnimationMixing : MonoBehaviour {

	public SpriteAnimation spriteAnimation;

	// Use this for initialization
	void Start () {
		//Get SpriteAnimation for GameObject
		spriteAnimation = GetComponent<SpriteAnimation>();

		spriteAnimation["Mummy_Attack"].layer = 1;
		spriteAnimation["Mummy_Attack"].AddMixingComponent( "/root/hip/body", true);		

		spriteAnimation["Mummy_Attack"].enabled = true;
		spriteAnimation["Mummy_Walk"].enabled = true;
	}

	public Rect guiPosiition;

	void OnGUI()
	{
		SpriteAnimationState state = spriteAnimation["Mummy_Attack"];


		GUILayout.BeginArea (guiPosiition);		


		GUILayout.Label( "Attack Weight:" + state.weight.ToString() );
		float weight = GUILayout.HorizontalSlider( state.weight, 0.01f, 1f);
		if ( GUI.changed )
		{
			state.weight = weight;
		}


		GUILayout.EndArea();
	}
}
