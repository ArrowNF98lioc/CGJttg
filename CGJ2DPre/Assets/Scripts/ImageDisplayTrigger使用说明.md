# ImageDisplayTrigger 使用说明

## 概述
`ImageDisplayTrigger` 是一个Unity脚本，用于检测玩家距离并在屏幕左下角按顺序显示图像。当玩家进入指定距离范围内时，会自动开始显示预设的图像序列。

## 功能特性
- ✅ 距离检测：自动检测玩家与触发器的距离
- ✅ 图像序列：支持按顺序显示多张图像
- ✅ 原始大小：图像按照原始尺寸显示，保持比例
- ✅ 淡入淡出：每张图像都有平滑的淡入淡出效果
- ✅ 自定义时间：每张图像可以设置不同的显示时间
- ✅ 循环显示：可选择是否循环显示图像序列
- ✅ 自动销毁：可选择完成后自动销毁触发器
- ✅ 调试功能：提供丰富的调试信息和可视化

## 安装步骤

### 1. 添加脚本到GameObject
1. 在场景中创建一个空的GameObject
2. 将 `ImageDisplayTrigger.cs` 脚本添加到该GameObject上
3. 在Inspector中配置脚本参数

### 2. 配置图像序列
1. 在Inspector中找到 "图像显示设置" 部分
2. 设置 "Image Sequence" 的Size为你需要的图像数量
3. 为每个Element配置：
   - **Image Sprite**: 拖拽要显示的图像Sprite
   - **Display Duration**: 设置显示持续时间（秒）
   - **Fade In Time**: 设置淡入时间（秒）
   - **Fade Out Time**: 设置淡出时间（秒）

## 参数说明

### 距离检测设置
- **Trigger Distance**: 触发距离，玩家进入此范围内时开始显示图像
- **Player Transform**: 玩家Transform（可选，脚本会自动查找Player.Instance）

### 图像显示设置
- **Image Sequence**: 图像序列列表，包含要显示的图像和显示参数
- **Image Size**: 图像显示大小（像素，当Use Original Size为false时使用）
- **Image Position**: 图像位置（相对于屏幕左下角的偏移）
- **Use Original Size**: 是否使用图像原始大小（推荐开启）

### 显示控制
- **Auto Start On Trigger**: 是否在触发时自动开始显示
- **Loop Sequence**: 是否循环显示图像序列
- **Destroy On Complete**: 是否在完成后销毁触发器

### 调试
- **Show Debug Info**: 是否显示调试信息
- **Show Distance Gizmo**: 是否在Scene视图中显示距离检测范围

## 使用示例

### 基本使用
1. 创建一个GameObject并添加ImageDisplayTrigger脚本
2. 设置触发距离（例如：3）
3. 配置图像显示设置：
   - **Use Original Size**: 勾选（推荐，让图像按原始大小显示）
   - **Image Position**: 设置为(100, 100)（距离左下角100像素）
4. 添加图像到序列：
   - 设置Size为3
   - Element 0: 拖拽第一张图像，设置显示时间为2秒
   - Element 1: 拖拽第二张图像，设置显示时间为3秒
   - Element 2: 拖拽第三张图像，设置显示时间为2秒
5. 运行游戏，当玩家靠近时自动在左下角显示图像

### 高级配置
```csharp
// 通过代码添加图像
ImageDisplayTrigger trigger = GetComponent<ImageDisplayTrigger>();
trigger.AddImageToSequence(mySprite, 2f, 0.5f, 0.5f);

// 手动触发显示
trigger.ManualTrigger();

// 设置触发距离
trigger.SetTriggerDistance(5f);

// 获取状态信息
string status = trigger.GetStatusInfo();
```

## 脚本方法

### 公共方法
- `StartImageSequence()`: 开始显示图像序列
- `StopImageSequence()`: 停止显示图像序列
- `ManualTrigger()`: 手动触发图像显示
- `AddImageToSequence(Sprite, float, float, float)`: 添加图像到序列
- `ClearImageSequence()`: 清空图像序列
- `SetTriggerDistance(float)`: 设置触发距离
- `GetStatusInfo()`: 获取状态信息
- `ShowStatusInfo()`: 显示状态信息

### 私有方法
- `CheckPlayerDistance()`: 检测玩家距离
- `OnPlayerEnterRange()`: 玩家进入范围处理
- `OnPlayerExitRange()`: 玩家离开范围处理
- `CreateDisplayCanvas()`: 创建显示Canvas
- `DisplayImageSequence()`: 显示图像序列协程
- `DisplayImage(ImageDisplayData)`: 显示单张图像
- `FadeImage(float, float, float)`: 图像淡入淡出

## 注意事项

### 1. 图像资源
- 确保图像Sprite已正确导入到Unity项目中
- 图像格式建议使用PNG或JPG
- 当Use Original Size开启时，图像会按照原始尺寸显示
- 当Use Original Size关闭时，图像会按照Image Size设置的大小显示

### 2. Canvas设置
- 脚本会自动查找现有的Canvas
- 如果没有找到Canvas，会自动创建一个
- 创建的Canvas使用Screen Space Overlay模式

### 3. 性能优化
- 避免在Update中频繁调用距离检测
- 合理设置触发距离，避免不必要的计算
- 图像序列不宜过长，建议控制在10张以内

### 4. 调试功能
- 开启Show Debug Info可以查看详细日志
- 开启Show Distance Gizmo可以在Scene视图中看到检测范围
- 使用ContextMenu可以手动触发和查看状态

## 故障排除

### 常见问题

**Q: 图像不显示**
A: 检查以下几点：
- 图像Sprite是否正确设置
- Canvas是否正确创建
- 玩家Transform是否正确获取
- 触发距离是否合适

**Q: 图像位置不正确**
A: 调整Image Position参数：
- 正值向右/向上偏移
- 负值向左/向下偏移
- 相对于屏幕左下角计算
- 默认位置为(100, 100)，表示距离左下角100像素

**Q: 淡入淡出效果不明显**
A: 调整Fade In Time和Fade Out Time参数：
- 增加时间值使效果更明显
- 减少时间值使切换更快

**Q: 性能问题**
A: 优化建议：
- 减少图像序列长度
- 增加触发距离检测间隔
- 关闭不必要的调试信息

## 扩展功能

### 自定义触发条件
你可以继承ImageDisplayTrigger类并重写CheckPlayerDistance方法来实现自定义的触发条件：

```csharp
public class CustomImageTrigger : ImageDisplayTrigger
{
    protected override void CheckPlayerDistance()
    {
        // 自定义触发逻辑
        if (Player.Instance.HasItem("特殊物品"))
        {
            // 特殊触发条件
        }
    }
}
```

### 多触发器协调
可以创建多个ImageDisplayTrigger实例，通过代码协调它们的显示：

```csharp
public class ImageTriggerManager : MonoBehaviour
{
    public List<ImageDisplayTrigger> triggers;
    
    public void StartAllSequences()
    {
        foreach (var trigger in triggers)
        {
            trigger.StartImageSequence();
        }
    }
}
```

## 版本信息
- 版本：1.0
- 兼容Unity版本：2019.4及以上
- 最后更新：2024年

## 技术支持
如果在使用过程中遇到问题，请检查：
1. Unity控制台的错误信息
2. 脚本的调试输出
3. Scene视图中的Gizmo显示
4. 确保所有依赖组件正确设置 