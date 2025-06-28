using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
{
    [SerializeField] Transform itemContainer;

    readonly Dictionary<string, ItemView> itemViews = new Dictionary<string, ItemView>();

    // Start is called before the first frame update
    void Start()
    {
        var items = itemContainer.GetComponentsInChildren<ItemView>(true);
        foreach (var item in items)
        {
            itemViews.Add(item.ItemName, item);
        }

        if (Inventory.Instance != null)
        {
            OnItemChange(Inventory.Instance.GetAllItems());
            Inventory.Instance.onItemChanged += OnItemChange;
        }
    }

    private void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.onItemChanged -= OnItemChange;
    }

    void OnItemChange(List<Item> items)
    {
        foreach (var item in itemViews.Values)
        {
            item.SetInBag(false);
        }
        foreach (var i in items)
        {
            if (itemViews.ContainsKey(i.name))
            {
                itemViews[i.name].SetInBag(true);
            }
            else
            {

            }
        }
    }

    //public void AddItem(PickableItem item)
    //{
    //    if (itemViews.ContainsKey(item.itemName))
    //    {

    //    }
    //}

}
