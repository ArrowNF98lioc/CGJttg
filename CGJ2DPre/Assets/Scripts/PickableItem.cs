using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public string itemName;           // 物品名称
    public GameObject glowEffect;     // 金光特效对象
    private bool canPickUp = false;

    void Start()
    {
        if (glowEffect != null)
            glowEffect.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = true;
            if (glowEffect != null)
                glowEffect.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = false;
            if (glowEffect != null)
                glowEffect.SetActive(false);
        }
    }

    void Update()
    {
        if (canPickUp && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit != null && hit.gameObject == this.gameObject)
            {
                PickUp();
            }
        }
    }

    void PickUp()
    {
        Inventory.Instance.AddItem(itemName);
        Destroy(gameObject);
    }
}