using UnityEngine;
using System.Collections;
using System.Collections.Generic;



// A fixed interval mummy spawner.
public class MummySpawner : MonoBehaviour {

	//mummy spawn's position
	public Transform spawnTransform;

	public float spawnRange = 500f;

	//spawn object prototype, in this case, a mummy
	public GameObject spawnObject;

	//mummy's target: warrior
	//we use this assgin this to mummys after we instance them
	public GameObject mummyTarget;

	//if mummys in scene greater than is, will not spawn new mummy
	public int maxMummyCount = 5;

	//spawn time interval
	public float interval = 4f;


	IEnumerator Start()
	{
		int cnt = 0;

		while( true )
		{
			Transform[] mummys = gameObject.GetComponentsInChildren<Transform>();

			if ( mummys == null || mummys.Length < maxMummyCount)
			{
				//get a randomly spawn position
				Vector3 pos = spawnTransform.position;
				pos.x += Random.Range( -spawnRange, spawnRange);

				//instance a spawn prototype object
				GameObject obj = Instantiate(spawnObject, pos, Quaternion.identity) as GameObject;
				obj.transform.parent = transform;

				//set the mummy's target
				obj.SendMessage("SetTarget", mummyTarget.transform, SendMessageOptions.DontRequireReceiver );
				
				//give a unique name to object
				obj.name = obj.name + "_" + cnt;

				cnt++;
			}


			yield return new WaitForSeconds(interval);
		}
	}
}
