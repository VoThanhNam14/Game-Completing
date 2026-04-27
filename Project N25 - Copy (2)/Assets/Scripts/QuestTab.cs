using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTab : MonoBehaviour
{
    public GameObject QuestsTab;
    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && QuestsTab.activeSelf == false)
        {
            Open();
        }
        else if (Input.GetKeyDown(KeyCode.C) && QuestsTab.activeSelf == true)
        {
            Close();
        }
    }
    void Open()
    {
        QuestsTab.SetActive(true);
        //Debug.Log("QuestAvailable");
    }
    void Close()
    {
        QuestsTab.SetActive(false);
        //Debug.Log("QuestCleared");
    }
}
