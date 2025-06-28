using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 生命值系统
/// 管理玩家的生命值，支持治疗和伤害功能
/// </summary>
public class Health : MonoBehaviour
{
    [Header("生命值设置")]
    [SerializeField] private int maxHealth = 100;        // 最大生命值
    [SerializeField] private int currentHealth = 100;    // 当前生命值
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    [Header("事件")]
    public UnityEvent<int> OnHealthChanged;      // 生命值变化事件
    public UnityEvent<int> OnHealed;             // 治疗事件
    public UnityEvent<int> OnDamaged;            // 受伤事件
    public UnityEvent OnDeath;                   // 死亡事件
    public UnityEvent OnRevived;                 // 复活事件
    
    // 属性
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public float HealthPercentage => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
    public bool IsDead => currentHealth <= 0;
    public bool IsFullHealth => currentHealth >= maxHealth;
    
    private void Start()
    {
        // 确保当前生命值不超过最大生命值
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log($"[Health] 生命值系统初始化完成 - 当前生命值: {currentHealth}/{maxHealth}");
    }
    
    /// <summary>
    /// 治疗
    /// </summary>
    /// <param name="healAmount">治疗量</param>
    /// <returns>实际治疗量</returns>
    public int Heal(int healAmount)
    {
        if (IsDead)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("[Health] 角色已死亡，无法治疗");
            }
            return 0;
        }
        
        if (IsFullHealth)
        {
            if (showDebugInfo)
            {
                Debug.Log("[Health] 生命值已满，无需治疗");
            }
            return 0;
        }
        
        if (healAmount <= 0)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("[Health] 治疗量必须大于0");
            }
            return 0;
        }
        
        int oldHealth = currentHealth;
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        int actualHealAmount = currentHealth - oldHealth;
        
        if (showDebugInfo)
        {
            Debug.Log($"[Health] 治疗: {actualHealAmount} 点生命值 ({oldHealth} -> {currentHealth})");
        }
        
        OnHealthChanged?.Invoke(currentHealth);
        OnHealed?.Invoke(actualHealAmount);
        
        return actualHealAmount;
    }
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damageAmount">伤害量</param>
    /// <returns>实际伤害量</returns>
    public int TakeDamage(int damageAmount)
    {
        if (IsDead)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("[Health] 角色已死亡，无法受到伤害");
            }
            return 0;
        }
        
        if (damageAmount <= 0)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("[Health] 伤害量必须大于0");
            }
            return 0;
        }
        
        int oldHealth = currentHealth;
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);
        int actualDamageAmount = oldHealth - currentHealth;
        
        if (showDebugInfo)
        {
            Debug.Log($"[Health] 受到伤害: {actualDamageAmount} 点生命值 ({oldHealth} -> {currentHealth})");
        }
        
        OnHealthChanged?.Invoke(currentHealth);
        OnDamaged?.Invoke(actualDamageAmount);
        
        // 检查是否死亡
        if (IsDead && oldHealth > 0)
        {
            if (showDebugInfo)
            {
                Debug.Log("[Health] 角色死亡");
            }
            OnDeath?.Invoke();
        }
        
        return actualDamageAmount;
    }
    
    /// <summary>
    /// 设置生命值
    /// </summary>
    /// <param name="newHealth">新的生命值</param>
    public void SetHealth(int newHealth)
    {
        int oldHealth = currentHealth;
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        
        if (showDebugInfo)
        {
            Debug.Log($"[Health] 设置生命值: {oldHealth} -> {currentHealth}");
        }
        
        OnHealthChanged?.Invoke(currentHealth);
        
        // 检查状态变化
        if (IsDead && oldHealth > 0)
        {
            OnDeath?.Invoke();
        }
        else if (!IsDead && oldHealth <= 0)
        {
            OnRevived?.Invoke();
        }
    }
    
    /// <summary>
    /// 设置最大生命值
    /// </summary>
    /// <param name="newMaxHealth">新的最大生命值</param>
    public void SetMaxHealth(int newMaxHealth)
    {
        if (newMaxHealth <= 0)
        {
            Debug.LogError("[Health] 最大生命值必须大于0");
            return;
        }
        
        int oldMaxHealth = maxHealth;
        maxHealth = newMaxHealth;
        
        // 调整当前生命值
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[Health] 设置最大生命值: {oldMaxHealth} -> {maxHealth}");
        }
        
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    /// <summary>
    /// 复活
    /// </summary>
    /// <param name="reviveHealth">复活时的生命值（默认满血）</param>
    public void Revive(int reviveHealth = -1)
    {
        if (!IsDead)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("[Health] 角色未死亡，无法复活");
            }
            return;
        }
        
        int newHealth = reviveHealth >= 0 ? reviveHealth : maxHealth;
        SetHealth(newHealth);
        
        if (showDebugInfo)
        {
            Debug.Log($"[Health] 角色复活，生命值: {currentHealth}");
        }
        
        OnRevived?.Invoke();
    }
    
    /// <summary>
    /// 完全恢复生命值
    /// </summary>
    public void FullHeal()
    {
        if (IsFullHealth)
        {
            if (showDebugInfo)
            {
                Debug.Log("[Health] 生命值已满，无需恢复");
            }
            return;
        }
        
        int healAmount = maxHealth - currentHealth;
        Heal(healAmount);
    }
    
    /// <summary>
    /// 获取生命值信息字符串
    /// </summary>
    /// <returns>生命值信息</returns>
    public string GetHealthInfo()
    {
        string info = $"生命值: {currentHealth}/{maxHealth}\n";
        info += $"百分比: {HealthPercentage:P1}\n";
        info += $"状态: {(IsDead ? "死亡" : IsFullHealth ? "满血" : "正常")}";
        
        return info;
    }
    
    /// <summary>
    /// 显示生命值信息（调试用）
    /// </summary>
    public void ShowHealthInfo()
    {
        Debug.Log($"[Health] {GetHealthInfo()}");
    }
    
    /// <summary>
    /// 检查是否可以治疗
    /// </summary>
    /// <param name="healAmount">治疗量</param>
    /// <returns>是否可以治疗</returns>
    public bool CanHeal(int healAmount = 1)
    {
        return !IsDead && !IsFullHealth && healAmount > 0;
    }
    
    /// <summary>
    /// 检查是否可以受到伤害
    /// </summary>
    /// <param name="damageAmount">伤害量</param>
    /// <returns>是否可以受到伤害</returns>
    public bool CanTakeDamage(int damageAmount = 1)
    {
        return !IsDead && damageAmount > 0;
    }
} 