using UnityEngine;

public class ItemView : MonoBehaviour
{
    [SerializeField] string itemName;
    [SerializeField] RectTransform rect;

    public string ItemName => itemName;
    public RectTransform Rect => rect;

    public void SetInBag(bool inbag) => gameObject.SetActive(inbag);
}
