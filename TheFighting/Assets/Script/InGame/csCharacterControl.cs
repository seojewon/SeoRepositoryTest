using UnityEngine;
using System.Collections;
using SmoothMoves;

public class csCharacterControl : MonoBehaviour {

    public BoneAnimation mBoneAnimation = null;
    public CharacterType mCharacter = CharacterType.IPPO;

    [HideInInspector] public State mState = State.STAND;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        /*
        if (Input.GetButton("Fire2"))
        {
            if (mCharacter == CharacterType.IPPO) csInGameManager.m_Instance.PlayBoneAnimation(this.gameObject, State.ATTACK);
            else csInGameManager.m_Instance.PlayBoneAnimation(this.gameObject, State.GUARD);
        }

        if (mState != State.STAND) csInGameManager.m_Instance.CheckState(this.gameObject);
        */
	}
}
