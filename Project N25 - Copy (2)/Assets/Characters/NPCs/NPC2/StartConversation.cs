using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartConversation : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject InteractButton;
    public string tagTarget1 = "Player";
    public string tagTarget2 = "NPC";

    public Collider2D col;
    public List<Collider2D> detectedObjs = new List<Collider2D>();
    public bool hasPlayer, hasNPC = false;
    public bool isTalking = false;
    public NPCConversation conversation;

    [Header("Quest Settings")]
    public Quest npcQuest;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(tagTarget1))
        {
            detectedObjs.Add(col);
            InteractButton.SetActive(true);
            hasPlayer = true;
        }
        if (col.CompareTag(tagTarget2))
        {
            detectedObjs.Add(col);
            hasNPC = true;
        }
        
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag(tagTarget1))
        {
            detectedObjs.Remove(col);
            InteractButton.SetActive(false);
            hasPlayer = false;
            isTalking = false;
        }
    }

    void Start()
    {
        col.GetComponent<Collider2D>();
    }

    void Update()
    {
        if (hasPlayer && hasNPC && Input.GetKeyDown(KeyCode.E))
        {
            Talking();
        }
    }
    public void Talking()
    {
        isTalking = true;
        conversation.StartDialogue();
    }
    public void EndTalking()
    {
        isTalking = false;
    }
}
