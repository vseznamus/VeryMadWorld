using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private List<Image> icons = new List<Image>();
    [SerializeField] private List<TMP_Text> amounts = new List<TMP_Text>();
    public void UpdateUI(Inventory inventory)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].color = i < inventory.GetSize() ? Color.white : new Color(1, 1, 1, 0);
            icons[i].sprite = i < inventory.GetSize() ? inventory.GetItem(i).icon : null;
            amounts[i].text = i < inventory.GetSize() && inventory.GetAmount(i) > 1 ? inventory.GetAmount(i).ToString() : "";
        }


    }
}
