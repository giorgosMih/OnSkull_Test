using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleBtnInstructions : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject InstructionsPanel;

    void Start()
    {
        Button btn = GetComponent<Button>();
        if(btn != null)
        {
            btn.onClick.AddListener(DoAction);
        }
    }

    void DoAction()
    {
        Time.timeScale = 0f;
        
        if(MainPanel != null)
        {
            MainPanel.SetActive(false);
        }

        if (InstructionsPanel != null)
        {
            InstructionsPanel.SetActive(true);
        }
    }
}
