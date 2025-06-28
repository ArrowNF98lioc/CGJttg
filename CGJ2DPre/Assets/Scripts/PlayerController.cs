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
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    private Vector2 input;
    private bool isMoving;
    private Coroutine currentMoveCoroutine;
    private Animator animator;
    private Player playerHealth;  // 引用Player健康系统
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
        
        // 如果有输入且当前没有在移动
        if (input != Vector2.zero && !isMoving)
        {
            animator.SetFloat("moveX", input.x);
            animator.SetFloat("moveY", input.y);

            Vector3 targetPos = transform.position;
            targetPos.x += input.x;
            targetPos.y += input.y;
            
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
            transform.position = Vector3.MoveTowards(transform.position, targetPos, currentMoveSpeed * Time.deltaTime);
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
}
