using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance {get; private set;}
    public List<QuestProgress> activateQuests = new();
    private QuestUI questUI;
    public List<string> handinQuestIDs = new();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        questUI = FindObjectOfType<QuestUI>();
        InventoryController.Instance.OnInventoryChanged += CheckInventoryForQuests;
    }

    public void AcceptQuest(Quest quest)
    {
        if(IsQuestActive(quest.questID)) return;
        activateQuests.Add(new QuestProgress(quest));
        CheckInventoryForQuests();
        questUI.UpdateQuestUI();
    }
    public void CheckInventoryForQuests()
    {
        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();
        //Debug.Log($"--- BẮT ĐẦU CHECK INVENTORY ---");
        //Debug.Log($"Tổng số loại item có trong túi đồ hiện tại: {itemCounts.Count}");
        foreach(QuestProgress quest in activateQuests)
        {
            Debug.Log($"Đang xét Quest ID: {quest.QuestID} | Tổng số Objective: {quest.objectives.Count}");
            foreach(QuestObjective questObjective in quest.objectives)
            {
                if(questObjective.type != ObjectiveType.CollectItem) continue;
                //Debug.Log("-> Bỏ qua: Objective không phải loại CollectItem.");
                if(!int.TryParse(questObjective.objectiveID, out int itemID)) continue;
                //Debug.Log($"-> LỖI ÉP KIỂU: objectiveID '{questObjective.objectiveID}' không phải là số (int) hợp lệ! Có dư khoảng trắng không?");
                bool hasItem = itemCounts.TryGetValue(itemID, out int count);
                //int newAmount = itemCounts.TryGetValue(itemID, out int count)? Mathf.Min(count, questObjective.requiredAmount) : 0;
                int newAmount = hasItem ? Mathf.Min(count, questObjective.requiredAmount) : 0;
                //Debug.Log($"-> Tình trạng túi đồ: Có item ID {itemID} không? {hasItem} | Số lượng trong túi: {count} | requiredAmount: {questObjective.requiredAmount}");
                //Debug.Log($"-> So sánh: newAmount tính được = {newAmount} | currentAmount hiện tại = {questObjective.currentAmount}");
                if(questObjective.currentAmount != newAmount)
                {
                    questObjective.currentAmount = newAmount;
                    //Debug.Log("Counting");
                    //Debug.Log("-> COUNTING: Cập nhật thành công!");
                }
                else
                {
                    Debug.Log("-> KHÔNG COUNTING: Vì số lượng mới (newAmount) giống hệt số lượng cũ (currentAmount).");
                }
            }
        }
        questUI.UpdateQuestUI();
        //Debug.Log($"--- KẾT THÚC CHECK INVENTORY ---");
    }
    public bool IsQuestActive(string questID) => activateQuests.Exists(q => q.QuestID == questID);
    public bool IsQuestCompleted(string questID)
    {
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        return quest != null && quest.objectives.TrueForAll(o => o.IsCompleted);
    }
    public void HandInQuest(string questID)
    {
        if (!RemoveRequiredItemsFromInventory(questID))
        {
            return;
        }
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        if(quest != null)
        {
            handinQuestIDs.Add(questID);
            activateQuests.Remove(quest);
            questUI.UpdateQuestUI();
        }
    }
    public bool IsQuestHandedIn(string questID)
    {
        return handinQuestIDs.Contains(questID);
    }
    public bool RemoveRequiredItemsFromInventory(string questID)
    {
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        if (quest == null) return false;
        Dictionary<int, int> requiredItems = new();
        foreach(QuestObjective objective in quest.objectives)
        {
            if(objective.type == ObjectiveType.CollectItem && int.TryParse(objective.objectiveID, out int itemID))
            {
                requiredItems[itemID] = objective.requiredAmount;
            }
        }
        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();
        foreach(var item in requiredItems)
        {
            if(itemCounts.GetValueOrDefault(item.Key) < item.Value)
            {
                return false;
            }
        }
        foreach(var itemRequirement in requiredItems)
        {
            InventoryController.Instance.RemoveItemsFromInventory(itemRequirement.Key, itemRequirement.Value);
        }
        return true;
    }
}
