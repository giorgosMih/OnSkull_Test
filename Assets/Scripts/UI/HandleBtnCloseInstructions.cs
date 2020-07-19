using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleBtnCloseInstructions : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject InstructionsPanel;

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(DoAction);
        }
    }

    void DoAction()
    {
        Time.timeScale = 1f;

        if (MainPanel != null)
        {
            MainPanel.SetActive(true);
        }

        if (InstructionsPanel != null)
        {
            InstructionsPanel.SetActive(false);
        }
    }
}
