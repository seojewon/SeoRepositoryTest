using UnityEngine;
using System.Collections;
using EasyMotion2D;

public class fps : MonoBehaviour {

	BitmapFontTextRenderer text = null;
	float time = 0f;
	int c = 0;
	// Use this for initialization
	void Start () {
		text = GetComponent<BitmapFontTextRenderer>();
		time  = Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update () {
		c++;
		if ( Time.realtimeSinceStartup - time > 1f)
		{		
			text.text = "FPS:" + c.ToString();
			
			time  = Time.realtimeSinceStartup;
			c = 0;
		}
	}
}
