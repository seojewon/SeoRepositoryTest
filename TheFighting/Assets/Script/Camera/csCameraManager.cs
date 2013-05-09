using UnityEngine;
using System.Collections;

public class csCameraManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Camera mCamera = GetComponent<Camera>();
        mCamera.aspect = 1280.0f / 720.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
