using System.Collections.Generic;
using UnityEngine;

public class ShopItemStateController : MonoBehaviour
{
    [System.Serializable]
    public class ItemEntry
    {
        public string itemName;
        public GameObject itemGameObject;
    }

    [Header("商店物品列表")]
    public List<ItemEntry> shopItems;

    private void Update()
    {
        UpdateShopItemsActiveState();
    }

    /// <summary>
    /// 更新商店物品的Active状态
    /// </summary>      
    public void UpdateShopItemsActiveState()
    {
        // 如果GameDataManager实例不存在，则返回
        if (GameDataManager.Instance == null) return;
        foreach (var entry in shopItems)
        {
            // 如果物品对象不存在或物品名称不存在，则跳过
            if (entry.itemGameObject == null || string.IsNullOrEmpty(entry.itemName)) continue;
            var state = GameDataManager.Instance.GetItemState(entry.itemName);
            // 根据物品状态设置物品对象的Active状态
            entry.itemGameObject.SetActive(state == PickableItem.ItemStateType.Solved);
        }
    }
} 