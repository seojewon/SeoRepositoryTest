using UnityEngine;
using System.Collections;
using EasyMotion2D;

public class Easy2DDemo_CharacterSpawn : MonoBehaviour {

	public GameObject prototypeObject;
	public int count = 50;

	// Use this for initialization
	void Start () {
		StartCoroutine(CreateObject());	
	}
	
	IEnumerator CreateObject()
	{
		//Instance one character per frame
		for( int i = 0; i < count; i ++)
		{
			Instantiate(prototypeObject, new Vector3(400,300,0), Quaternion.identity);
			yield return new WaitForEndOfFrame();
		}
	}


}
