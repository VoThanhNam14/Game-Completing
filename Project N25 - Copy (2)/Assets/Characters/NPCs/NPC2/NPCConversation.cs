using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCConversation : MonoBehaviour
{
    public NPCDialogue dialogueData;
    private DialogueManager dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    public GameObject SpaceButton;
    public bool hasChoice;
    //Quest
    private enum QuestState {NotStarted, InProgress, Completed}
    private QuestState questState = QuestState.NotStarted;
    private QuestState stateAtConversationStart;
    void Update()
    {
        if(isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            if (hasChoice)
            {
                return;
            }
            NextLine();
        }
    }

    private void Start()
    {
        dialogueUI = DialogueManager.Instance;
    }
    public bool CanInterAct()
    {
        return !isDialogueActive;
    }
    public void Interact()
    {
        if (dialogueData == null || !isDialogueActive)
        {
            return;
        }
        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }
    public void StartDialogue()
    {
        SyncQuestState();
        stateAtConversationStart = questState;

        if(questState == QuestState.NotStarted)
        {
            dialogueIndex = 0;
        }else if (questState == QuestState.InProgress)
        {
            dialogueIndex = dialogueData.questInProgressIndex;
        }else if (questState == QuestState.Completed)
        {
            dialogueIndex = dialogueData.questCompletedIndex;
        }

        PauseController.SetPause(true);
        dialogueUI.ClearChoices();
        isDialogueActive = true;
        //dialogueIndex = 0;

        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);
        DisplayCurrentLine();

        hasChoice = false;
    }
    private void SyncQuestState()
    {
        if(dialogueData.quest == null) return;
        string questID = dialogueData.quest.questID;
        if(QuestController.Instance.IsQuestCompleted(questID) || QuestController.Instance.IsQuestHandedIn(questID))
        {
            questState = QuestState.Completed;
        }
        else if (QuestController.Instance.IsQuestActive(questID))
        {
            questState = QuestState.InProgress;
        }
        else
        {
            questState = QuestState.NotStarted;
        }
    }
    public void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
            return;            
        } 
        dialogueUI.ClearChoices();
        if(dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }
        foreach(DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if(dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }
        
        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }
    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");
        foreach(char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }
        isTyping = false;
        if(dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }
    void DisplayChoices(DialogueChoice choice)
    {
        for(int i=0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            bool givesQuest = choice.givesQuest[i];
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex, givesQuest));
            hasChoice = true;
        }
    }
    void ChooseOption(int nextIndex, bool givesQuest)
    {
        if (givesQuest)
        {
            QuestController.Instance.AcceptQuest(dialogueData.quest);
            questState = QuestState.InProgress;
            SyncQuestState();
        }
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        hasChoice = false;
        DisplayCurrentLine();
    }
    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }
    public void EndDialogue()
    {
        if(stateAtConversationStart == QuestState.Completed && !QuestController.Instance.IsQuestHandedIn(dialogueData.quest.questID))
        {
            HandleQuestCompletion(dialogueData.quest);
        }
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);
    }
    void HandleQuestCompletion(Quest quest)
    {
        QuestController.Instance.HandInQuest(quest.questID);

        if (RewardController.Instance != null)
        {
            RewardController.Instance.GiveRewards(quest.questRewards);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy RewardController trong Scene! Hãy chắc chắn bạn đã tạo object và gắn script này.");
        }
    }
}
