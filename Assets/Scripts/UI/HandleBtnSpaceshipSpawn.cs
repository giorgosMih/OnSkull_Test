using UnityEngine;
using UnityEngine.UI;

public class HandleBtnSpaceshipSpawn : MonoBehaviour
{
    public GameObject spawnObject;//GameObject to Spawn
	public GameObject spawnAtTarget;//Target GameObject, where spawned object will rotate around it

	void Start()
	{
		Button btn = GetComponent<Button>();//get UI Button
		btn.onClick.AddListener(SpawnObject);//add click listener for spawning
	}

	void SpawnObject()
	{
		GameObject o = Instantiate(spawnObject);//instantiate object
		BotLogic bl = o.GetComponent(typeof(BotLogic)) as BotLogic;//get logic component

		if(bl != null)
        {
			bl.SetPlanet(spawnAtTarget);//add spawn target to logic component
		}
	}
}
