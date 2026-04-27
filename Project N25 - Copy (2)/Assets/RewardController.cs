using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RewardController : MonoBehaviour
{
    public static RewardController Instance {get; private set;}
    public TMP_Text moneyStatText;
    public int currentMoney = 0;
    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        UpdateUI(); // Cập nhật UI ngay khi bắt đầu game
    }

    public void GiveRewards(List<QuestReward> rewards)
    {
        foreach (QuestReward reward in rewards)
        {
            if (reward.type == RewardType.Stat)
            {
                currentMoney += reward.amount;
                Debug.Log($"Đã nhận thưởng: {reward.amount} Tiền!");
            }
            else if (reward.type == RewardType.Item)
            {
                ItemDicitonary itemDict = FindObjectOfType<ItemDicitonary>();
                    
                if (itemDict != null)
                {
                    GameObject itemPrefab = itemDict.GetItemPrefab(reward.rewardID);
                    if (itemPrefab != null)
                    {
                        int itemsAdded = 0;
                        for (int i = 0; i < reward.amount; i++)
                        {
                            bool success = InventoryController.Instance.AddItem(itemPrefab);
                        
                            if (success)
                            {
                                itemsAdded++;
                            }
                            else
                            {
                                Debug.LogWarning("Túi đồ đã đầy, không thể nhận hết phần thưởng!");
                                break; 
                            }
                            }
                        Debug.Log($"Đã nhận thưởng vật phẩm ID {reward.rewardID} x{itemsAdded}");
                    }
                    else
                    {
                        Debug.LogWarning($"Không tìm thấy Prefab cho Item có ID {reward.rewardID} trong ItemDictionary!");
                    }
                }
            }
        }
        UpdateUI(); // Cập nhật lại Text sau khi nhận hết thưởng
    }

    private void UpdateUI()
    {
        if (moneyStatText != null)
        {
            moneyStatText.text = currentMoney.ToString();
        }
    }
}
