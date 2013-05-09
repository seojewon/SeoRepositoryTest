using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyMotion2D;

public class AnimationPlayQueued : MonoBehaviour {

	public SpriteAnimation spriteAnimation;
	public string[] animationNames = null;


	// Use this for initialization
	void Start () {

		//Get SpriteAnimation for GameObject
		spriteAnimation = GetComponent<SpriteAnimation>();


		List<string> tmp = new List<string>();

		//Get all clips assigned in SpriteAnimation
		SpriteAnimationClip[] clips = SpriteAnimationUtility.GetAnimationClips( spriteAnimation );
		foreach( SpriteAnimationClip clip in clips)
		{
			tmp.Add( clip.name);
			//change the wrapmode of clip to once, because if play a looping clip, you can not waiting it complete.
			spriteAnimation[clip.name].wrapMode = WrapMode.Once;
		}


		animationNames = tmp.ToArray();
	}




	void OnGUI()
	{
		GUILayout.BeginArea (new Rect (25, 45, 150, 500));
		
		GUILayout.BeginVertical();
		
		foreach( string clipName in animationNames )
		{
			//If click button, then play this clip.
			if ( GUILayout.Button( clipName ) )
			{
				//Add selected clip to playing queue
				spriteAnimation.PlayQueued( clipName );
			}
		}


		GUILayout.EndVertical();

		
		GUILayout.EndArea();
	}
}
