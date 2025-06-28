# 背包系统使用说明

## 概述

这个背包系统包含以下核心组件：
- `Inventory.cs` - 背包数据管理
- `InventorySlot.cs` - 单个背包槽位管理
- `InventoryUIController.cs` - 背包UI控制器
- `InventorySlotPrefab.cs` - 背包槽位预制体示例

## 功能特性

### 1. 背包数据管理
- 自动创建指定数量的背包槽位
- 物品添加、移除、清空功能
- 与GameDataManager数据同步
- 背包状态查询（是否满、是否空等）

### 2. 背包UI显示
- 自动更新背包显示
- 物品图片显示
- 物品名称和数量显示
- 槽位高亮效果
- 背包开关动画

### 3. 交互功能
- 鼠标悬停显示物品提示
- 点击槽位选择物品
- 键盘快捷键（I键或Tab键）开关背包

## 设置步骤

### 1. 创建背包UI结构

在Unity中创建以下UI层级结构：
```
Canvas
├── InventoryPanel (背包面板)
│   ├── Background (背景)
│   ├── SlotsParent (槽位父对象)
│   │   ├── Slot0 (槽位0)
│   │   ├── Slot1 (槽位1)
│   │   └── ... (更多槽位)
│   ├── ItemCountText (物品数量文本)
│   ├── MaxSlotsText (最大槽位文本)
│   ├── SelectedItemText (选中物品文本)
│   ├── OpenButton (打开按钮)
│   └── CloseButton (关闭按钮)
└── TooltipPanel (提示面板)
    └── TooltipText (提示文本)
```

### 2. 设置背包槽位预制体

每个槽位预制体应包含：
- `Image` 组件（物品图片）
- `Text` 组件（物品名称）
- `Text` 组件（物品数量）
- `GameObject`（高亮效果）
- `InventorySlot` 脚本组件

### 3. 配置Inventory脚本

在Inspector中设置：
- **背包设置**
  - `Max Slots`: 背包最大槽位数（默认10）
  - `Show Debug Info`: 是否显示调试信息

- **背包UI设置**
  - `Inventory Panel`: 背包面板GameObject
  - `Slots Parent`: 槽位父对象Transform
  - `Slot Prefab`: 槽位预制体
  - `Auto Create Slots`: 是否自动创建槽位

- **物品图片设置**
  - `Default Slot Sprite`: 默认槽位图片
  - `Empty Slot Sprite`: 空槽位图片

### 4. 配置InventoryUIController脚本

在Inspector中设置：
- **背包UI组件**
  - `Inventory Panel`: 背包面板
  - `Slots Parent`: 槽位父对象
  - `Slot Prefab`: 槽位预制体
  - `Open Button`: 打开背包按钮
  - `Close Button`: 关闭背包按钮

- **背包信息显示**
  - `Item Count Text`: 物品数量文本
  - `Max Slots Text`: 最大槽位文本
  - `Selected Item Text`: 选中物品文本

- **背包设置**
  - `Show Item Tooltip`: 是否显示物品提示
  - `Tooltip Panel`: 提示面板
  - `Tooltip Text`: 提示文本

- **动画设置**
  - `Use Animation`: 是否使用动画
  - `Animation Duration`: 动画持续时间
  - `Animation Curve`: 动画曲线

## 使用方法

### 1. 基本操作

```csharp
// 获取背包实例
Inventory inventory = Inventory.Instance;

// 添加物品
inventory.AddItem("茶壶");

// 移除物品
inventory.RemoveItem("茶壶");

// 检查是否有物品
bool hasItem = inventory.HasItem("茶壶");

// 清空背包
inventory.ClearInventory();

// 获取背包信息
string info = inventory.GetInventoryInfo();
```

### 2. UI操作

```csharp
// 获取UI控制器
InventoryUIController uiController = FindObjectOfType<InventoryUIController>();

// 打开背包
uiController.OpenInventory();

// 关闭背包
uiController.CloseInventory();

// 切换背包显示
uiController.ToggleInventory();

// 刷新背包显示
uiController.RefreshInventoryDisplay();
```

### 3. 槽位操作

```csharp
// 获取指定槽位的物品
Item item = inventory.GetItemAtSlot(0);

// 获取槽位组件
InventorySlot slot = slots[0];

// 设置物品
slot.SetItem(item);

// 清空槽位
slot.ClearSlot();

// 设置高亮
slot.SetHighlight(true);
```

## 事件系统

### 1. 槽位事件

```csharp
// 订阅槽位点击事件
slot.OnSlotClicked += (slotIndex, item) => {
    Debug.Log($"槽位 {slotIndex} 被点击，物品: {item?.name}");
};

// 订阅槽位悬停事件
slot.OnSlotHovered += (slotIndex, item) => {
    Debug.Log($"槽位 {slotIndex} 悬停，物品: {item?.name}");
};

// 订阅槽位退出事件
slot.OnSlotExited += (slotIndex) => {
    Debug.Log($"槽位 {slotIndex} 退出");
};
```

### 2. UI控制器事件

```csharp
// 处理槽位点击
uiController.OnSlotClicked(slotIndex, item);

// 处理槽位悬停
uiController.OnSlotHovered(slotIndex, item);

// 处理槽位退出
uiController.OnSlotExited(slotIndex);
```

## 物品图片管理

### 1. 自动加载图片

系统会自动从以下路径加载物品图片：
1. `Resources/Items/{物品名称}`
2. `Resources/Art/Items/{物品名称}`

### 2. 手动设置图片

```csharp
// 在InventorySlot中手动设置物品图片
private Sprite GetItemSprite(string itemName)
{
    // 自定义图片加载逻辑
    switch (itemName)
    {
        case "茶壶":
            return Resources.Load<Sprite>("Art/Items/茶壶");
        case "项链":
            return Resources.Load<Sprite>("Art/Items/Necklace");
        default:
            return defaultSprite;
    }
}
```

## 数据持久化

背包系统会自动与GameDataManager同步：
- 场景切换时保持背包数据
- 游戏退出时保存背包数据
- 游戏启动时恢复背包数据

## 调试功能

### 1. 调试信息

启用调试信息后，会在Console中显示：
- 背包操作日志
- 物品添加/移除信息
- UI状态变化
- 错误信息

### 2. 调试方法

```csharp
// 显示背包信息
inventory.ShowInventoryInfo();

// 显示槽位信息
slot.GetSlotInfo();

// 显示预制体信息
prefab.ShowPrefabInfo();
```

## 注意事项

1. **单例模式**: Inventory使用单例模式，确保场景中只有一个实例
2. **UI层级**: 背包面板应该在Canvas下，确保正确的渲染顺序
3. **图片资源**: 物品图片需要放在Resources文件夹中才能自动加载
4. **事件清理**: 在销毁对象时记得清理事件订阅
5. **性能优化**: 大量物品时考虑使用对象池优化槽位创建

## 扩展功能

### 1. 物品分类
可以扩展Item类添加分类属性，实现分类显示

### 2. 物品堆叠
修改InventorySlot支持显示物品数量

### 3. 拖拽功能
添加拖拽接口实现物品拖拽

### 4. 快捷键
扩展快捷键系统支持更多操作

### 5. 动画效果
添加更多动画效果提升用户体验 