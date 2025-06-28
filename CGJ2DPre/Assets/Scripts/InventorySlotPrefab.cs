using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包槽位预制体示例
/// 演示如何设置背包槽位的UI组件
/// </summary>
public class InventorySlotPrefab : MonoBehaviour
{
    [Header("UI组件引用")]
    [SerializeField] private Image itemImage;           // 物品图片
    [SerializeField] private Image backgroundImage;     // 背景图片
    [SerializeField] private Text itemNameText;         // 物品名称文本
    [SerializeField] private Text itemCountText;        // 物品数量文本
    [SerializeField] private GameObject highlightObject; // 高亮对象
    
    [Header("预制体设置")]
    [SerializeField] private bool autoSetupOnStart = true;  // 是否在Start时自动设置
    
    private InventorySlot slotComponent;
    
    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupSlotComponent();
        }
    }
    
    /// <summary>
    /// 设置槽位组件
    /// </summary>
    public void SetupSlotComponent()
    {
        // 获取或添加InventorySlot组件
        slotComponent = GetComponent<InventorySlot>();
        if (slotComponent == null)
        {
            slotComponent = gameObject.AddComponent<InventorySlot>();
        }
        
        // 设置UI组件引用
        slotComponent.SetUIComponents(itemImage, backgroundImage, itemNameText, itemCountText, highlightObject);
        
        Debug.Log("[InventorySlotPrefab] 槽位预制体设置完成");
    }
    
    /// <summary>
    /// 手动设置UI组件引用
    /// </summary>
    /// <param name="image">物品图片</param>
    /// <param name="background">背景图片</param>
    /// <param name="nameText">名称文本</param>
    /// <param name="countText">数量文本</param>
    /// <param name="highlight">高亮对象</param>
    public void SetUIComponents(Image image, Image background, Text nameText, Text countText, GameObject highlight)
    {
        itemImage = image;
        backgroundImage = background;
        itemNameText = nameText;
        itemCountText = countText;
        highlightObject = highlight;
        
        // 如果已经有InventorySlot组件，更新其引用
        if (slotComponent != null)
        {
            slotComponent.SetUIComponents(itemImage, backgroundImage, itemNameText, itemCountText, highlightObject);
        }
    }
    
    /// <summary>
    /// 获取槽位组件
    /// </summary>
    /// <returns>InventorySlot组件</returns>
    public InventorySlot GetSlotComponent()
    {
        return slotComponent;
    }
    
    /// <summary>
    /// 检查预制体是否正确设置
    /// </summary>
    /// <returns>是否正确设置</returns>
    public bool IsProperlySetup()
    {
        return slotComponent != null && itemImage != null;
    }
    
    /// <summary>
    /// 显示预制体信息
    /// </summary>
    public void ShowPrefabInfo()
    {
        string info = "背包槽位预制体信息:\n";
        info += $"物品图片: {(itemImage != null ? "已设置" : "未设置")}\n";
        info += $"背景图片: {(backgroundImage != null ? "已设置" : "未设置")}\n";
        info += $"名称文本: {(itemNameText != null ? "已设置" : "未设置")}\n";
        info += $"数量文本: {(itemCountText != null ? "已设置" : "未设置")}\n";
        info += $"高亮对象: {(highlightObject != null ? "已设置" : "未设置")}\n";
        info += $"槽位组件: {(slotComponent != null ? "已设置" : "未设置")}";
        
        Debug.Log(info);
    }
} 