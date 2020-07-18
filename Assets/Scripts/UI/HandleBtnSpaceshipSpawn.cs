using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleBtnSpaceshipSpawn : MonoBehaviour
{
    public GameObject obj;
	public Button btn;
	public GameObject target;

	void Start()
	{
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick()
	{
		GameObject o = Instantiate(obj);
		BotLogic bl = o.GetComponent(typeof(BotLogic)) as BotLogic;

		bl.setPlanet(target);
	}
}
