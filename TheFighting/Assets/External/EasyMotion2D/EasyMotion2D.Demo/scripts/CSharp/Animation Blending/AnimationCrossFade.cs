using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyMotion2D;

public class AnimationCrossFade : MonoBehaviour {

	public SpriteAnimation spriteAnimation;
	public string[] animationNames = null;

	public SpriteAnimationClip clip;

	// Use this for initialization
	void Start () {

		//Get SpriteAnimation for GameObject
		spriteAnimation = GetComponent<SpriteAnimation>();

		spriteAnimation.AddClip( clip, "test1", 0, 10);
		spriteAnimation.AddClip( clip, "test2", 11, 20);
		spriteAnimation.AddClip( clip, "test3", 21, 30);


		List<string> tmp = new List<string>();
		//Get all clips assigned in SpriteAnimation
		SpriteAnimationClip[] clips = SpriteAnimationUtility.GetAnimationClips( spriteAnimation );
		foreach( SpriteAnimationClip _clip in clips)
		{
			tmp.Add( _clip.name);
		}

		tmp.Add("test1");
		tmp.Add("test2");
		tmp.Add("test3");
		animationNames = tmp.ToArray();
	}





	float fadeLength = 0.3f;

	void OnGUI()
	{
		GUILayout.BeginArea (new Rect (25, 45, 150, 500));
		


		GUILayout.BeginVertical();
		
		foreach( string clipName in animationNames )
		{
			//If click button, then CrossFade this clip.
			GUI.color = Color.white;

			if ( spriteAnimation.IsPlaying(clipName) )
				GUI.color = Color.yellow;

			if ( GUILayout.Button( clipName ) )
			{
				//CrossFade to other clips.
				spriteAnimation.CrossFade( clipName, fadeLength);
			}

			GUI.color = Color.white;
		}		

		GUILayout.EndVertical();


		//show the fade length
		GUILayout.Label( "CrossFade Length:" + fadeLength.ToString() );
		//get fade length from GUI
		fadeLength = GUILayout.HorizontalSlider( fadeLength, 0.01f, 1f);

		GUILayout.EndArea();
	}
}
