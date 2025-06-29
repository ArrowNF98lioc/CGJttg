using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("移动设置")]
    public float baseMoveSpeed = 5f;  // 基础移动速度
    
    [Header("健康状态速度设置")]
    public float stage1Speed = 5f;    // 健康阶段1的移动速度
    public float stage2Speed = 3f;    // 健康阶段2的移动速度
    public float stage3Speed = 1.5f;  // 健康阶段3的移动速度
    
    [Header("移动范围限制")]
    [SerializeField] private bool enableMovementBounds = true;  // 是否启用移动范围限制
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -10f);  // 最小边界
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 10f);    // 最大边界
    [SerializeField] private bool showBoundsInEditor = true;  // 在编辑器中显示边界
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    private Vector2 input;
    private bool isMoving;
    private Coroutine currentMoveCoroutine;
    private Animator animator;
    public Player playerHealth;  // 引用Player健康系统
    private float currentMoveSpeed;  // 当前实际移动速度

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<Player>();
        
        if (playerHealth == null)
        {
            Debug.LogWarning("[PlayerController] 未找到Player组件，将使用基础移动速度");
        }
    }
    
    private void Start()
    {
        // 初始化移动速度
        UpdateMoveSpeedBasedOnHealth();
        
        // 加载当前场景的移动边界
        LoadSceneBounds();
        
        Debug.Log("[PlayerController] 玩家控制器初始化完成");
    }
    
    private void Update()
    {
        // 获取输入
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        
        if (showDebugInfo)
        {
            Debug.Log("input.x:" + input.x);
            Debug.Log("input.y:" + input.y);
        }

        animator.SetInteger("Status", playerHealth.GetHealthStageNumber());
        
        // 如果有输入且当前没有在移动
        if (input != Vector2.zero && !isMoving)
        {
            animator.SetFloat("moveX", input.x);
            animator.SetFloat("moveY", input.y);

            Vector3 targetPos = transform.position;
            targetPos.x += input.x;
            targetPos.y += input.y;
            
            // 检查移动边界
            if (enableMovementBounds)
            {
                targetPos = ClampPositionToBounds(targetPos);
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[PlayerController] 开始移动: {transform.position} -> {targetPos}");
            }
            
            // 停止之前的协程（如果有的话）
            if (currentMoveCoroutine != null)
            {
                StopCoroutine(currentMoveCoroutine);
            }
            
            // 启动新的移动协程
            currentMoveCoroutine = StartCoroutine(Move(targetPos));
        }

        if (input == Vector2.zero && isMoving)
        {
            StopMovement();
        }

        animator.SetBool("isMoving", isMoving);

        // 根据健康状态更新移动速度
        UpdateMoveSpeedBasedOnHealth();
    }
    
    /// <summary>
    /// 将位置限制在边界内
    /// </summary>
    /// <param name="position">目标位置</param>
    /// <returns>限制后的位置</returns>
    private Vector3 ClampPositionToBounds(Vector3 position)
    {
        Vector3 clampedPosition = position;
        clampedPosition.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        
        // 如果位置被限制，说明超出了边界
        if (clampedPosition != position && showDebugInfo)
        {
            Debug.Log($"[PlayerController] 移动被限制: {position} -> {clampedPosition}");
        }
        
        return clampedPosition;
    }
    
    /// <summary>
    /// 加载当前场景的移动边界
    /// </summary>
    private void LoadSceneBounds()
    {
        // 优先使用SceneBoundsManager
        if (SceneBoundsManager.Instance != null)
        {
            var (min, max) = SceneBoundsManager.Instance.GetCurrentSceneBounds();
            SetMovementBounds(min, max);
        }
        else
        {
            // 如果没有SceneBoundsManager，使用内置的默认配置
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            switch (currentScene)
            {
                case "Home":
                    SetMovementBounds(new Vector2(-8f, -6f), new Vector2(8f, 6f));
                    break;
                case "Gallery":
                    SetMovementBounds(new Vector2(-12f, -8f), new Vector2(12f, 8f));
                    break;
                case "Shop":
                    SetMovementBounds(new Vector2(-6f, -4f), new Vector2(6f, 4f));
                    break;
                case "MainMenu":
                    SetMovementBounds(new Vector2(-5f, -3f), new Vector2(5f, 3f));
                    break;
                default:
                    // 默认边界
                    SetMovementBounds(new Vector2(-10f, -10f), new Vector2(10f, 10f));
                    break;
            }
        }
        
        Debug.Log($"[PlayerController] 场景 '{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}' 移动边界已加载: {minBounds} -> {maxBounds}");
    }
    
    /// <summary>
    /// 设置移动边界
    /// </summary>
    /// <param name="min">最小边界</param>
    /// <param name="max">最大边界</param>
    public void SetMovementBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        
        // 确保最小边界小于最大边界
        if (minBounds.x > maxBounds.x)
        {
            float temp = minBounds.x;
            minBounds.x = maxBounds.x;
            maxBounds.x = temp;
        }
        
        if (minBounds.y > maxBounds.y)
        {
            float temp = minBounds.y;
            minBounds.y = maxBounds.y;
            maxBounds.y = temp;
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[PlayerController] 移动边界已设置: {minBounds} -> {maxBounds}");
        }
    }
    
    /// <summary>
    /// 获取当前移动边界
    /// </summary>
    /// <returns>边界信息</returns>
    public (Vector2 min, Vector2 max) GetMovementBounds()
    {
        return (minBounds, maxBounds);
    }
    
    /// <summary>
    /// 启用或禁用移动范围限制
    /// </summary>
    /// <param name="enable">是否启用</param>
    public void SetMovementBoundsEnabled(bool enable)
    {
        enableMovementBounds = enable;
        Debug.Log($"[PlayerController] 移动范围限制: {(enable ? "启用" : "禁用")}");
    }
    
    /// <summary>
    /// 检查位置是否在边界内
    /// </summary>
    /// <param name="position">要检查的位置</param>
    /// <returns>是否在边界内</returns>
    public bool IsPositionInBounds(Vector3 position)
    {
        return position.x >= minBounds.x && position.x <= maxBounds.x &&
               position.y >= minBounds.y && position.y <= maxBounds.y;
    }
    
    /// <summary>
    /// 强制将玩家位置限制在边界内
    /// </summary>
    public void ClampPlayerPositionToBounds()
    {
        if (enableMovementBounds)
        {
            Vector3 clampedPosition = ClampPositionToBounds(transform.position);
            if (clampedPosition != transform.position)
            {
                transform.position = clampedPosition;
                Debug.Log($"[PlayerController] 玩家位置已限制在边界内: {clampedPosition}");
            }
        }
    }
    
    /// <summary>
    /// 根据健康状态更新移动速度
    /// </summary>
    public void UpdateMoveSpeedBasedOnHealth()
    {
        if (playerHealth == null)
        {
            currentMoveSpeed = baseMoveSpeed;
            return;
        }
        
        Player.HealthStage currentStage = playerHealth.GetHealthStage();
        float oldSpeed = currentMoveSpeed;
        
        switch (currentStage)
        {
            case Player.HealthStage.Stage1:
                currentMoveSpeed = stage1Speed;
                break;
            case Player.HealthStage.Stage2:
                currentMoveSpeed = stage2Speed;
                break;
            case Player.HealthStage.Stage3:
                currentMoveSpeed = stage3Speed;
                break;
            default:
                currentMoveSpeed = baseMoveSpeed;
                break;
        }
        
        if (showDebugInfo && oldSpeed != currentMoveSpeed)
        {
            Debug.Log($"[PlayerController] 健康状态: {playerHealth.GetHealthStageName()}, 移动速度: {oldSpeed} -> {currentMoveSpeed}");
        }
    }
    
    private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        
        if (showDebugInfo)
        {
            Debug.Log($"[PlayerController] 移动协程开始: {targetPos}");
        }
        
        // 移动到目标位置
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPos, currentMoveSpeed * Time.deltaTime);
            
            // 在移动过程中也检查边界
            if (enableMovementBounds)
            {
                newPosition = ClampPositionToBounds(newPosition);
            }
            
            transform.position = newPosition;
            yield return null;
        }
        
        // 确保精确到达目标位置
        transform.position = targetPos;
        
        isMoving = false;
        currentMoveCoroutine = null;
        
        if (showDebugInfo)
        {
            Debug.Log($"[PlayerController] 移动完成: {transform.position}");
        }
    }
    
    /// <summary>
    /// 停止当前移动
    /// </summary>
    public void StopMovement()
    {
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
        }
        isMoving = false;
        
        if (showDebugInfo)
        {
            Debug.Log("[PlayerController] 移动已停止");
        }
    }
    
    /// <summary>
    /// 设置移动速度（手动设置，会覆盖健康状态的速度）
    /// </summary>
    public void SetMoveSpeed(float newSpeed)
    {
        currentMoveSpeed = newSpeed;
        
        if (showDebugInfo)
        {
            Debug.Log($"[PlayerController] 移动速度手动设置为: {currentMoveSpeed}");
        }
    }
    
    /// <summary>
    /// 获取当前移动速度
    /// </summary>
    public float GetCurrentMoveSpeed()
    {
        return currentMoveSpeed;
    }
    
    /// <summary>
    /// 获取当前是否在移动
    /// </summary>
    public bool IsMoving()
    {
        return isMoving;
    }
    
    /// <summary>
    /// 获取当前输入
    /// </summary>
    public Vector2 GetInput()
    {
        return input;
    }
    
    /// <summary>
    /// 强制更新移动速度（当健康状态改变时调用）
    /// </summary>
    public void ForceUpdateSpeed()
    {
        UpdateMoveSpeedBasedOnHealth();
    }
    
    /// <summary>
    /// 场景切换时重新加载边界
    /// </summary>
    public void OnSceneChanged()
    {
        LoadSceneBounds();
        ClampPlayerPositionToBounds();
    }
    
    /// <summary>
    /// 在编辑器中绘制边界（仅用于调试）
    /// </summary>
    private void OnDrawGizmos()
    {
        if (showBoundsInEditor && enableMovementBounds)
        {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2f, (minBounds.y + maxBounds.y) / 2f, 0f);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0.1f);
            Gizmos.DrawWireCube(center, size);
            
            // 绘制边界点
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(new Vector3(minBounds.x, minBounds.y, 0f), 0.2f);
            Gizmos.DrawWireSphere(new Vector3(maxBounds.x, maxBounds.y, 0f), 0.2f);
        }
    }
}
