using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text quantityText;


    private void Awake()
    {
        FindOwnUIReferences();
        ClearSlot();
    }


    private void FindOwnUIReferences()
    {
        Transform iconTransform =
            transform.Find("ItemIcon");

        if (iconTransform != null)
        {
            itemIcon =
                iconTransform.GetComponent<Image>();
        }

        Transform quantityTransform =
            transform.Find("QuantityTest");

        if (quantityTransform == null)
        {
            quantityTransform =
                transform.Find("QuantityText");
        }

        if (quantityTransform != null)
        {
            quantityText =
                quantityTransform.GetComponent<TMP_Text>();
        }
    }


    public void ClearSlot()
    {
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        if (quantityText != null)
        {
            quantityText.text = "";
            quantityText.enabled = false;
        }
    }


    public void DisplayItem(InventoryItem item)
    {
        if (item == null ||
            item.ItemDefinition == null)
        {
            ClearSlot();
            return;
        }

        ItemDefinition definition =
            item.ItemDefinition;


        if (itemIcon != null)
        {
            itemIcon.sprite =
                definition.InventoryIcon;

            itemIcon.enabled =
                definition.InventoryIcon != null;
        }


        if (quantityText != null)
        {
            if (definition.Stackable)
            {
                quantityText.text =
                    "x" + item.quantity;

                quantityText.enabled = true;

                quantityText.transform
                    .SetAsLastSibling();

                quantityText.ForceMeshUpdate();
            }
            else
            {
                quantityText.text = "";
                quantityText.enabled = false;
            }
        }
    }
}