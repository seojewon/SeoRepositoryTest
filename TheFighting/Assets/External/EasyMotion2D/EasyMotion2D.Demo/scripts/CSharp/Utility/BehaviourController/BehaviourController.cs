using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;





[System.Serializable]
public class ObjectBehaviour
{
	public string		name = "";
	public float		time = 0f;
	public string[]		acceptList = new string[]{};
	public string		nextBehaviour = "";

	public bool IsAccept( string nextBehaviour )
	{
		if ( acceptList.Length == 0 )
			return true;		

		foreach ( string s in acceptList)
			if ( nextBehaviour == s)
				return true;			
		return false;
	}
}






public class BehaviourController : MonoBehaviour {

	public StateTransitionTableAsset	stateTransitionTable;
	public string						startBehaviour;

    public bool debug = false;

	public delegate		void BehaviourChangeHandler( string prev, string next );
	public	BehaviourChangeHandler		onBehaviourChange = new BehaviourChangeHandler( delegate( string prev, string next){ } );


	private	Dictionary<string, ObjectBehaviour> __behaviourTable = new Dictionary<string, ObjectBehaviour>();


	private float	currentBehaviourTime;
	public	string	currentBehaviourName = "";



	public ObjectBehaviour		GetBehaviour(  string name )
	{
		ObjectBehaviour	item = null;
		if ( __behaviourTable.TryGetValue( name, out item) )
			return item;
		return null;
	}


	public void AddBehaviour( ObjectBehaviour beh )
	{
		try
		{
			__behaviourTable.Add( beh.name, beh );
		}
		catch ( Exception )
		{
		}
	}

	public void RemoveBehaviour( ObjectBehaviour beh )
	{
		__behaviourTable.Remove( beh.name );
	}

	public void ClearBehaviour()
	{
		__behaviourTable.Clear();
	}


	// Use this for initialization
	void Start () {	

		if ( stateTransitionTable == null  )
			return;

		foreach ( ObjectBehaviour item in stateTransitionTable.behaviourList )
			AddBehaviour( item );

		DoBehaviour( startBehaviour );
	}
	
	// Update is called once per frame
	void Update () {
		if ( stateTransitionTable == null  )
			return;

		ObjectBehaviour currentBehaviour = GetBehaviour( currentBehaviourName );
		currentBehaviourTime += Time.smoothDeltaTime ;

		if ( currentBehaviour != null )
		{
			if ( currentBehaviour.time > 0.0f && currentBehaviourTime >= currentBehaviour.time )
			{
				if ( currentBehaviour.nextBehaviour.Length > 0 )
					DoBehaviour( currentBehaviour.nextBehaviour );
			}
		}
	}

	public bool DoBehaviour( string name )
	{
		if ( currentBehaviourName.Length > 0 )
		{
			ObjectBehaviour currentBehaviour = GetBehaviour( currentBehaviourName );
			if ( currentBehaviour != null && currentBehaviour.IsAccept( name ) == false )
			{
                if (debug)
				    Debug.Log( "Behaviour:"+name+" not exist or current behaviour do not accept " + name );
				return false;			
			}
		}
		ObjectBehaviour behaviour = GetBehaviour( name );
		if ( behaviour != null )
		{
			string currName = currentBehaviourName;
			currentBehaviourName = name;
			currentBehaviourTime = 0.0f;

			onBehaviourChange( currName, name );
			return true;
		}

        if (debug)
		    Debug.Log( "Behaviour:"+name+" not exist or current behaviour do not accept " + name );
		return false;
	}
}


