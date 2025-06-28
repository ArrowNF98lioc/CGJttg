using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 背包槽位组件
/// 管理单个背包槽位的显示和交互
/// </summary>
public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI组件")]
    [SerializeField] private Image itemImage;           // 物品图片
    [SerializeField] private Image backgroundImage;     // 背景图片
    [SerializeField] private Text itemNameText;         // 物品名称文本
    [SerializeField] private Text itemCountText;        // 物品数量文本
    [SerializeField] private GameObject highlightObject; // 高亮对象
    
    [Header("槽位设置")]
    [SerializeField] private Color normalColor = Color.white;      // 正常颜色
    [SerializeField] private Color highlightColor = Color.yellow;  // 高亮颜色
    [SerializeField] private Color emptyColor = Color.gray;        // 空槽位颜色
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    // 槽位数据
    private int slotIndex = -1;
    private Item currentItem = null;
    private Sprite defaultSprite;
    private Sprite emptySprite;
    private bool isHighlighted = false;
    
    // 事件
    public System.Action<int, Item> OnSlotClicked;      // 槽位点击事件
    public System.Action<int, Item> OnSlotHovered;      // 槽位悬停事件
    public System.Action<int> OnSlotExited;             // 槽位退出事件
    
    /// <summary>
    /// 初始化槽位
    /// </summary>
    /// <param name="index">槽位索引</param>
    /// <param name="defaultSpr">默认图片</param>
    /// <param name="emptySpr">空槽位图片</param>
    public void InitializeSlot(int index, Sprite defaultSpr, Sprite emptySpr)
    {
        slotIndex = index;
        defaultSprite = defaultSpr;
        emptySprite = emptySpr;
        
        // 初始化UI组件
        if (itemImage == null)
        {
            itemImage = GetComponent<Image>();
        }
        
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
        
        // 设置默认状态
        ClearSlot();
        
        if (showDebugInfo)
        {
            Debug.Log($"[InventorySlot] 初始化槽位 {index}");
        }
    }
    
    /// <summary>
    /// 设置物品
    /// </summary>
    /// <param name="item">物品对象</param>
    public void SetItem(Item item)
    {
        currentItem = item;
        
        if (item != null)
        {
            // 设置物品图片
            if (itemImage != null)
            {
                // 尝试从ItemManager获取物品图片
                Sprite itemSprite = GetItemSprite(item.name);
                itemImage.sprite = itemSprite != null ? itemSprite : defaultSprite;
                itemImage.color = normalColor;
            }
            
            // 设置物品名称
            if (itemNameText != null)
            {
                itemNameText.text = item.name;
                itemNameText.gameObject.SetActive(true);
            }
            
            // 设置物品数量（如果有的话）
            if (itemCountText != null)
            {
                itemCountText.text = "1"; // 目前每个槽位只放一个物品
                itemCountText.gameObject.SetActive(true);
            }
            
            // 设置背景颜色
            if (backgroundImage != null)
            {
                backgroundImage.color = normalColor;
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[InventorySlot] 槽位 {slotIndex} 设置物品: {item.name}");
            }
        }
        else
        {
            ClearSlot();
        }
    }
    
    /// <summary>
    /// 清空槽位
    /// </summary>
    public void ClearSlot()
    {
        currentItem = null;
        
        // 清空物品图片
        if (itemImage != null)
        {
            itemImage.sprite = emptySprite != null ? emptySprite : defaultSprite;
            itemImage.color = emptyColor;
        }
        
        // 清空物品名称
        if (itemNameText != null)
        {
            itemNameText.text = "";
            itemNameText.gameObject.SetActive(false);
        }
        
        // 清空物品数量
        if (itemCountText != null)
        {
            itemCountText.text = "";
            itemCountText.gameObject.SetActive(false);
        }
        
        // 设置背景颜色
        if (backgroundImage != null)
        {
            backgroundImage.color = emptyColor;
        }
        
        // 关闭高亮
        SetHighlight(false);
        
        if (showDebugInfo)
        {
            Debug.Log($"[InventorySlot] 槽位 {slotIndex} 已清空");
        }
    }
    
    /// <summary>
    /// 设置高亮状态
    /// </summary>
    /// <param name="highlight">是否高亮</param>
    public void SetHighlight(bool highlight)
    {
        isHighlighted = highlight;
        
        if (highlightObject != null)
        {
            highlightObject.SetActive(highlight);
        }
        
        if (backgroundImage != null)
        {
            backgroundImage.color = highlight ? highlightColor : 
                (currentItem != null ? normalColor : emptyColor);
        }
    }
    
    /// <summary>
    /// 获取物品图片
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>物品图片</returns>
    private Sprite GetItemSprite(string itemName)
    {
        // 这里可以根据物品名称返回对应的图片
        // 你可以创建一个物品图片管理器，或者使用Resources.Load
        
        // 示例：从Resources文件夹加载图片
        string spritePath = $"Items/{itemName}";
        Sprite sprite = Resources.Load<Sprite>(spritePath);
        
        if (sprite == null)
        {
            // 如果找不到图片，尝试从Art/Items文件夹加载
            spritePath = $"Art/Items/{itemName}";
            sprite = Resources.Load<Sprite>(spritePath);
        }
        
        return sprite;
    }
    
    /// <summary>
    /// 获取当前物品
    /// </summary>
    /// <returns>当前物品</returns>
    public Item GetCurrentItem()
    {
        return currentItem;
    }
    
    /// <summary>
    /// 获取槽位索引
    /// </summary>
    /// <returns>槽位索引</returns>
    public int GetSlotIndex()
    {
        return slotIndex;
    }
    
    /// <summary>
    /// 检查槽位是否为空
    /// </summary>
    /// <returns>是否为空</returns>
    public bool IsEmpty()
    {
        return currentItem == null;
    }
    
    /// <summary>
    /// 获取槽位信息
    /// </summary>
    /// <returns>槽位信息字符串</returns>
    public string GetSlotInfo()
    {
        string info = $"槽位 {slotIndex}: ";
        
        if (currentItem != null)
        {
            info += $"物品={currentItem.name}, 健康值={currentItem.health}, 有生命={currentItem.hasLife}";
        }
        else
        {
            info += "空槽位";
        }
        
        info += $", 高亮={isHighlighted}";
        
        return info;
    }
    
    // UI事件处理
    
    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotClicked?.Invoke(slotIndex, currentItem);
        
        if (showDebugInfo)
        {
            Debug.Log($"[InventorySlot] 槽位 {slotIndex} 被点击，物品: {(currentItem != null ? currentItem.name : "无")}");
        }
    }
    
    /// <summary>
    /// 悬停事件
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHighlight(true);
        OnSlotHovered?.Invoke(slotIndex, currentItem);
        
        if (showDebugInfo)
        {
            Debug.Log($"[InventorySlot] 槽位 {slotIndex} 悬停，物品: {(currentItem != null ? currentItem.name : "无")}");
        }
    }
    
    /// <summary>
    /// 退出事件
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        SetHighlight(false);
        OnSlotExited?.Invoke(slotIndex);
        
        if (showDebugInfo)
        {
            Debug.Log($"[InventorySlot] 槽位 {slotIndex} 退出");
        }
    }
    
    /// <summary>
    /// 设置UI组件引用
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
        
        Debug.Log($"[InventorySlot] 设置UI组件完成，槽位 {slotIndex}");
    }
} 