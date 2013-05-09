//This script show you how to unload unused resource in runtime.
//If there is no gameobject use a clip or a sprite, use Resources.UnloadUnusedAssets can be unload them. 
//Else you should clear the component that who refernece the clips and sprites.

using UnityEngine;
using System.Collections;
using EasyMotion2D;

public class UnloadResource : MonoBehaviour {

	public GameObject obj = null;

	void OnGUI()
	{
		if ( GUI.Button( new Rect(100,100,150,50), "load Resource") )
		{
			if ( obj != null )
			{
				Object.DestroyImmediate(obj);
				obj = null;
			}

			//load gameobject prototype prefab from resource
			GameObject pre = Resources.Load("UnloadResource/test") as GameObject;

			//Instantiate prototype prefab
			obj = Instantiate( pre, Vector3.zero, Quaternion.identity) as GameObject;
		}

		if ( GUI.Button( new Rect(100,200,150,50), "Unload Resource") )
		{
			//destroy the instance
			Object.DestroyImmediate(obj);
			obj = null;

			//resource all unused assets
			Resources.UnloadUnusedAssets();
		}

		if ( GUI.Button( new Rect(100,300,150,50), "Unload AnimationClip") )
		{
			if ( obj != null )
			{
				SpriteRenderer sprRenderer= obj.GetComponent<SpriteRenderer>();
				SpriteAnimation sprAnimation = obj.GetComponent<SpriteAnimation>();

				//clear all referenced clips in SpriteAnimation
				sprAnimation.Clear();
				//clear all referenced sprites in SpriteRenderer
				sprRenderer.Clear();
				//release the SpriteRenderModeSetting object, it reference a shader
				sprRenderer.renderMode = null;
			}

			//resource all unused assets
			Resources.UnloadUnusedAssets();
		}
	}
}
