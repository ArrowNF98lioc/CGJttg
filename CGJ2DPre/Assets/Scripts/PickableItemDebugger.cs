using UnityEngine;

/// <summary>
/// PickableItem调试器
/// 用于测试物品选择限制功能
/// </summary>
public class PickableItemDebugger : MonoBehaviour
{
    [Header("调试设置")]
    [SerializeField] private bool showDebugInfo = true;
    
    void Update()
    {
        if (showDebugInfo && Input.GetKeyDown(KeyCode.F1))
        {
            ShowAllItemStates();
        }
        
        if (showDebugInfo && Input.GetKeyDown(KeyCode.F2))
        {
            ShowSelectedItemInfo();
        }
        
        if (showDebugInfo && Input.GetKeyDown(KeyCode.F3))
        {
            ResetAllItems();
        }
    }
    
    /// <summary>
    /// 显示所有物品状态
    /// </summary>
    void ShowAllItemStates()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning("[PickableItemDebugger] GameDataManager实例未找到");
            return;
        }
        
        Debug.Log("=== 所有物品状态 ===");
        foreach (var itemState in GameDataManager.Instance.itemStates)
        {
            Debug.Log($"物品: {itemState.Key}, 状态: {itemState.Value}");
        }
        Debug.Log("==================");
    }
    
    /// <summary>
    /// 显示当前选中物品信息
    /// </summary>
    void ShowSelectedItemInfo()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning("[PickableItemDebugger] GameDataManager实例未找到");
            return;
        }
        
        string selectedItem = GameDataManager.Instance.GetSelectedItemName();
        if (!string.IsNullOrEmpty(selectedItem))
        {
            Debug.Log($"[PickableItemDebugger] 当前选中的物品: {selectedItem}");
        }
        else
        {
            Debug.Log("[PickableItemDebugger] 当前没有选中的物品");
        }
        
        bool hasOtherSelected = GameDataManager.Instance.HasOtherItemSelected();
        Debug.Log($"[PickableItemDebugger] 是否有其他物品被选中: {hasOtherSelected}");
    }
    
    /// <summary>
    /// 重置所有物品状态
    /// </summary>
    void ResetAllItems()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning("[PickableItemDebugger] GameDataManager实例未找到");
            return;
        }
        
        Debug.Log("[PickableItemDebugger] 重置所有物品状态");
        foreach (var itemState in GameDataManager.Instance.itemStates)
        {
            GameDataManager.Instance.UpdateItemState(itemState.Key, PickableItem.ItemStateType.AtHome);
        }
    }
} 