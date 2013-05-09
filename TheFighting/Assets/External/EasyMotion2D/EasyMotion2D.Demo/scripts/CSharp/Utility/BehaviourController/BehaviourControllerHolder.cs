using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(BehaviourController))]
public class BehaviourControllerHolder : MonoBehaviour {

    private BehaviourController gameStateController = null;

    public BehaviourController behaviourController
    {
        get
        {
            if ( gameStateController == null )
                gameStateController = GetComponent<BehaviourController>();

            return gameStateController;
        }
    }

    [HideInInspector]
    public string currentState = "";
    [HideInInspector]
    public float currentStateTime = 0.0f;

    public bool debug = false;

    private bool controllerEnabled = true;

    void Awake()
    {
        gameStateController = GetComponent<BehaviourController>();
        gameStateController.onBehaviourChange += new BehaviourController.BehaviourChangeHandler(OnChangeState);
    }



    void Update()
    {
        gameStateController.debug = debug;

        if (controllerEnabled)
        {
            float delta = Time.smoothDeltaTime ;
            currentStateTime += delta;
            BroadcastMessage("On_" + currentState + "_Update", delta, SendMessageOptions.DontRequireReceiver);
        }
    }

    //hehaviour handler
    public bool ChangeState(string stepName)
    {
        return gameStateController.DoBehaviour(stepName);
    }

    void OnChangeState(string prev, string next)
    {
        if ( debug )
            Debug.Log("OnChangeState from " + prev + " to " + next);

        BroadcastMessage("On_" + prev + "_End", null, SendMessageOptions.DontRequireReceiver);
        currentState = next;
        currentStateTime = 0.0f;
        BroadcastMessage("On_" + next + "_Start", this, SendMessageOptions.DontRequireReceiver);
    }
}
