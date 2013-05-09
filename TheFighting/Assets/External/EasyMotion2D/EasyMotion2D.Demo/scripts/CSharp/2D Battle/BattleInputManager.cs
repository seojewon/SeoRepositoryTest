using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyMotion2D;

//This class use to pick up select a selectable character in scene
public class BattleInputManager : MonoBehaviour
{

    public BattleCharacterController  player;
    public Camera2D gameCamera;


    // Update is called once per frame
    void LateUpdate()
    {
		//if mosue down on screen, the select a character
        if (Input.GetMouseButtonDown(0))
        {
			//start a raycast to get all gameobject with collider or charactercontroller
            Ray ray = gameCamera.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 10000f);
            if ( hits.Length > 0 )
            {
                List<BattleCharacterController> hitChars = new List<BattleCharacterController>();
				
				//if pick up a player character, then add to list
                foreach (RaycastHit hit in hits)
                {
                    BattleCharacterController character = hit.transform.GetComponent<BattleCharacterController>();
                    if (character != null && character.characterData.isPlayer && character != null)
                    {
                        hitChars.Add(character);
                    }
                }

				//select the nearest character on scene
                if (hitChars.Count > 0)
                {
                    BattleCharacterController tmp = hitChars[0];

                    foreach (BattleCharacterController bcc in hitChars)
                    {
                        if (bcc.transform.position.y < tmp.transform.position.y)
                            tmp = bcc;
                    }

                    if (player != null)
                        player.Unselect();

                    player = tmp;
                    player.Select();
                }
            }
        }


		//if mouse up, then set the character move target
        if (Input.GetMouseButtonUp(0) && player != null)
        {

            Rect scrRc = new Rect(0, 0, Screen.width, Screen.height);
            if (!scrRc.Contains(Input.mousePosition))
                return;


            Ray ray = gameCamera.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
			
			//if raycast any collider or character, then move to the target object
			if (Physics.Raycast(ray, out hit, 10000f))
            {
                BattleCharacterController target = hit.transform.GetComponent<BattleCharacterController>();
                if (target != null && target.characterData.isPlayer != player.characterData.isPlayer)
                {
                    BattleCharacterController.MoveTarget moveTarget = new BattleCharacterController.MoveTarget(target);
                    player.Move(moveTarget);
                }
            }
			//move to the target position
            else
            {
                Vector3 tmp = gameCamera.camera.ScreenToWorldPoint(Input.mousePosition);
                BattleCharacterController.MoveTarget moveTarget = new BattleCharacterController.MoveTarget(tmp);
                player.Move(moveTarget);
            }
        }

    }

}
