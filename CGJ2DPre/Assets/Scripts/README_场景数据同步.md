# 场景数据同步系统使用说明

## 概述

这个场景数据同步系统确保在每次进入新场景时重新读取所有游戏数据，包括：
- 老奶状态（Player健康值、健康阶段）
- 血槽状态（健康值显示）
- 物品状态（当前携带的物品）
- 背包栏状态（已收集的物品）

## 核心组件

### 1. GameDataManager.cs - 全局数据管理器
- 存储所有需要在场景间保持的数据
- 提供数据同步方法
- 自动监听场景加载事件

### 2. SceneDataManager.cs - 场景数据管理器
- 管理场景切换过程
- 确保数据在场景切换时正确同步
- 提供场景切换的API

### 3. Player.cs - 玩家数据管理
- 管理玩家健康值和状态
- 提供数据同步方法
- 更新健康阶段和UI映射

### 4. Inventory.cs - 背包数据管理
- 管理背包物品数据
- 与GameDataManager同步
- 更新背包UI显示

## 数据同步流程

### 场景切换时的数据同步

```
场景切换开始
    ↓
保存当前场景数据到GameDataManager
    ↓
加载新场景
    ↓
场景加载完成
    ↓
从GameDataManager读取数据到各个组件
    ↓
更新所有UI显示
    ↓
数据同步完成
```

### 具体同步内容

1. **老奶状态同步**：
   - 当前生命值
   - 最大生命值
   - 健康阶段（影响移动速度）

2. **血槽状态同步**：
   - 健康值百分比
   - 健康阶段显示

3. **物品状态同步**：
   - 当前携带的物品
   - 物品UI映射更新

4. **背包栏状态同步**：
   - 已收集的物品列表
   - 背包槽位显示
   - 物品图片更新

## 使用方法

### 1. 设置场景数据管理器

在场景中创建一个GameObject，添加以下组件：
- `GameDataManager` - 全局数据管理
- `SceneDataManager` - 场景数据管理

### 2. 场景切换方式

#### 使用SceneDataManager（推荐）
```csharp
// 通过场景名称切换
SceneDataManager.Instance.LoadScene("Home");

// 通过场景索引切换
SceneDataManager.Instance.LoadScene(1);

// 重新加载当前场景
SceneDataManager.Instance.ReloadCurrentScene();
```

#### 使用Door组件
```csharp
// Door会自动使用SceneDataManager进行场景切换
// 只需设置targetSceneName即可
door.SetTargetScene("Shop");
```

#### 使用MainMenu组件
```csharp
// MainMenu中的按钮会自动使用SceneDataManager
// 无需额外配置
```

### 3. 手动数据同步

```csharp
// 强制刷新所有数据
SceneDataManager.Instance.ForceRefreshAllData();

// 同步特定组件数据
GameDataManager.Instance.SyncAllGameData();

// 更新Player数据
Player.Instance.ForceRefreshUI();
```

## 自动同步功能

### 1. 场景加载自动同步

系统会在以下时机自动同步数据：
- 场景加载完成时
- 组件初始化时
- 数据发生变化时

### 2. 数据变化监听

以下情况会触发数据同步：
- 玩家生命值变化
- 物品收集/移除
- 背包内容变化
- 健康阶段变化

### 3. UI自动更新

以下UI会在数据同步时自动更新：
- 背包显示
- 物品UI映射
- 健康值显示
- 物品提示

## 调试功能

### 1. 调试信息

启用调试信息后，会在Console中显示：
- 场景切换过程
- 数据同步状态
- 组件连接状态
- 错误信息

### 2. 调试方法

```csharp
// 显示当前游戏状态
MainMenu.Instance.ShowGameStatus();

// 显示场景状态
SceneDataManager.Instance.ShowCurrentSceneStatus();

// 显示门状态
door.ShowDoorStatus();

// 显示背包信息
inventory.ShowInventoryInfo();
```

### 3. 强制刷新

```csharp
// 强制刷新所有数据
SceneDataManager.Instance.ForceRefreshAllData();

// 强制刷新Player UI
Player.Instance.ForceRefreshUI();

// 强制刷新背包显示
inventory.UpdateInventoryDisplay();
```

## 配置说明

### 1. GameDataManager配置

在Inspector中设置：
- **调试**: `Show Debug Info` - 是否显示调试信息

### 2. SceneDataManager配置

在Inspector中设置：
- **调试**: `Show Debug Info` - 是否显示调试信息

### 3. Player配置

在Inspector中设置：
- **调试**: `Show Debug Info` - 是否显示调试信息
- **背包检测**: `Enable Inventory Check` - 是否启用背包检测

### 4. Inventory配置

在Inspector中设置：
- **调试**: `Show Debug Info` - 是否显示调试信息
- **背包设置**: `Max Slots` - 最大槽位数

## 注意事项

### 1. 组件依赖

确保以下组件按正确顺序初始化：
1. GameDataManager
2. SceneDataManager
3. Player
4. Inventory
5. UI组件

### 2. 单例模式

所有管理器都使用单例模式：
- 确保场景中只有一个实例
- 使用DontDestroyOnLoad保持数据
- 避免重复实例

### 3. 场景设置

确保场景正确配置：
- 场景已添加到Build Settings
- 场景名称与代码中的名称一致
- 场景索引正确

### 4. 数据一致性

系统确保数据一致性：
- 场景切换时自动保存数据
- 场景加载时自动恢复数据
- 数据变化时自动同步

## 扩展功能

### 1. 自定义数据同步

可以扩展GameDataManager添加更多数据：
```csharp
// 添加自定义数据
public class CustomGameData
{
    public int customValue;
    public string customString;
}

// 在GameDataManager中添加
public CustomGameData customData = new CustomGameData();
```

### 2. 自定义场景切换

可以创建自定义场景切换逻辑：
```csharp
public class CustomSceneLoader : MonoBehaviour
{
    public void LoadSceneWithTransition(string sceneName)
    {
        // 自定义切换逻辑
        StartCoroutine(LoadSceneWithFade(sceneName));
    }
}
```

### 3. 数据持久化

可以扩展为实际的文件保存：
```csharp
// 在GameDataManager中添加
public void SaveToFile()
{
    // 保存到PlayerPrefs或文件
    PlayerPrefs.SetString("GameData", JsonUtility.ToJson(this));
}

public void LoadFromFile()
{
    // 从PlayerPrefs或文件加载
    string data = PlayerPrefs.GetString("GameData");
    JsonUtility.FromJsonOverwrite(data, this);
}
```

## 故障排除

### 1. 数据不同步

检查：
- 组件是否正确初始化
- 单例模式是否正常工作
- 场景事件是否正确订阅

### 2. UI不更新

检查：
- UI组件是否正确引用
- 更新方法是否正确调用
- 调试信息是否显示错误

### 3. 场景切换失败

检查：
- 场景名称是否正确
- 场景是否在Build Settings中
- SceneDataManager是否正常工作

### 4. 性能问题

优化建议：
- 减少不必要的数据同步
- 使用对象池优化UI创建
- 延迟加载非关键数据 