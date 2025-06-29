# 游戏结束系统使用说明

## ⚠️ 重要设置提醒

**在使用游戏结束系统之前，必须手动将 `GameEndManager` 添加到场景中！**

### 快速设置步骤：

1. **在Unity编辑器中打开场景**（建议从 `Home` 场景开始）
2. **找到包含 `GameDataManager` 的GameObject**（通常在场景根级别）
3. **右键点击该GameObject**，选择 "Create Empty"
4. **将新创建的GameObject重命名为 "GameEndManager"**
5. **选中该GameObject**，在Inspector中点击 "Add Component"
6. **搜索并添加 `GameEndManager` 脚本**
7. **设置UI组件**（见下面的详细设置说明）

### 验证设置：
- 运行游戏后，在Console中应该看到 "[GameEndManager] 游戏结束管理器初始化完成"
- 如果没有看到这个日志，说明GameEndManager没有正确设置

---

## 概述

本系统实现了三种游戏结束条件：
1. **生命值归零** - 当玩家生命值降为0时自动触发
2. **所有物品被当掉** - 当所有物品状态变为Solved时自动触发
3. **玩家放弃** - 当玩家点击放弃按钮时手动触发

## 组件说明

### 1. GameEndManager (游戏结束管理器)

**功能：**
- 统一管理所有游戏结束条件
- 显示游戏结束UI
- 处理重新开始和退出游戏

**设置方法：**
1. 在场景中创建一个空的GameObject
2. 添加GameEndManager脚本
3. 在Inspector中设置UI组件：
   - `gameOverPanel`: 游戏结束面板
   - `giveUpButton`: 放弃按钮
   - `restartButton`: 重新开始按钮
   - `quitButton`: 退出按钮
   - `gameOverTitleText`: 游戏结束标题文本
   - `gameOverReasonText`: 游戏结束原因文本
   - `gameOverDescriptionText`: 游戏结束描述文本

**重要：** GameEndManager使用单例模式，确保在场景间保持一致。

### 2. GiveUpButton (放弃按钮)

**功能：**
- 为任何按钮添加放弃功能
- 支持确认对话框

**设置方法：**
1. 选择要作为放弃按钮的UI Button
2. 添加GiveUpButton脚本
3. 在Inspector中设置：
   - `giveUpButton`: 放弃按钮（如果脚本直接挂在按钮上，可以留空）
   - `showConfirmation`: 是否显示确认对话框
   - `confirmationPanel`: 确认面板（可选）
   - `confirmButton`: 确认按钮（可选）
   - `cancelButton`: 取消按钮（可选）

## 游戏结束条件详解

### 1. 生命值归零

**触发条件：**
- 玩家生命值降为0或以下

**自动检测：**
- 在Player.cs的HealthDecayCoroutine中检测
- 在Player.cs的SetHealth方法中检测

**触发逻辑：**
```csharp
if (currentHealth <= 0)
{
    GameEndManager.Instance.EndGame(GameEndManager.GameEndReason.HealthZero);
}
```

### 2. 所有物品被当掉

**触发条件：**
- 所有物品的状态都变为Solved

**自动检测：**
- 在GameDataManager.cs的UpdateItemState方法中检测
- 当物品状态变为Solved时自动检查

**触发逻辑：**
```csharp
if (totalItems > 0 && soldItems == totalItems)
{
    GameEndManager.Instance.EndGame(GameEndManager.GameEndReason.AllItemsSold);
}
```

### 3. 玩家放弃

**触发条件：**
- 玩家点击放弃按钮

**手动触发：**
- 通过GiveUpButton脚本实现
- 支持确认对话框

**触发逻辑：**
```csharp
GameEndManager.Instance.EndGame(GameEndManager.GameEndReason.PlayerGaveUp);
```

## UI设置建议

### 游戏结束面板布局

```
GameOverPanel (Canvas Group)
├── Background (Image - 半透明黑色)
├── TitleText (Text - "游戏结束")
├── ReasonText (Text - 结束原因)
├── DescriptionText (Text - 详细描述)
├── ButtonContainer (Horizontal Layout Group)
    ├── RestartButton (Button - "重新开始")
    └── QuitButton (Button - "退出游戏")
```

### 放弃按钮设置

1. **简单版本：**
   - 直接添加GiveUpButton脚本到按钮上
   - 设置`showConfirmation = false`

2. **带确认版本：**
   - 设置`showConfirmation = true`
   - 创建确认面板，包含确认和取消按钮
   - 在Inspector中关联确认面板和按钮

## 调试功能

### GameEndManager调试

- `[ContextMenu("强制结束游戏")]` - 强制触发游戏结束
- `[ContextMenu("重置游戏结束状态")]` - 重置游戏结束状态

### GiveUpButton调试

- `[ContextMenu("强制触发放弃")]` - 强制触发放弃

## 场景设置步骤

1. **创建GameEndManager：**
   ```
   1. 创建空GameObject，命名为"GameEndManager"
   2. 添加GameEndManager脚本
   3. 设置UI组件引用
   ```

2. **创建游戏结束UI：**
   ```
   1. 在Canvas中创建游戏结束面板
   2. 设置适当的文本和按钮
   3. 将UI组件拖拽到GameEndManager的Inspector中
   ```

3. **添加放弃按钮：**
   ```
   1. 在需要的地方创建放弃按钮
   2. 添加GiveUpButton脚本
   3. 根据需要设置确认对话框
   ```

## 注意事项

1. **单例模式：** GameEndManager使用单例模式，确保在场景间保持一致
2. **UI层级：** 游戏结束面板应该设置较高的Canvas Order，确保显示在最上层
3. **数据保存：** 游戏结束时会自动保存游戏数据
4. **场景切换：** 重新开始游戏会重置所有数据并返回主菜单
5. **调试模式：** 开启showDebugInfo可以看到详细的调试信息

## 扩展功能

### 自定义游戏结束原因

可以在GameEndManager.GameEndReason枚举中添加新的结束原因：

```csharp
public enum GameEndReason
{
    None,
    HealthZero,
    AllItemsSold,
    PlayerGaveUp,
    CustomReason  // 新增自定义原因
}
```

### 自定义结束文本

可以在GetReasonText和GetDescriptionText方法中添加对应的文本处理。

## 故障排除

1. **游戏结束不触发：**
   - 检查GameEndManager是否正确设置
   - 确认UI组件引用是否正确
   - 查看Console中的调试信息

2. **UI不显示：**
   - 检查Canvas设置
   - 确认UI组件的Active状态
   - 检查Canvas Order设置

3. **按钮不响应：**
   - 检查Button组件的Interactable状态
   - 确认EventSystem是否存在
   - 检查按钮的Raycast Target设置 