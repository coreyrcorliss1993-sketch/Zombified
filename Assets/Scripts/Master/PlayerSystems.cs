using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerSystemsMaster : MonoBehaviour
{
    [Serializable]
    public class InventorySlot
    {
        public string itemId;
        public string displayName;
        public int quantity;
        public bool stackable = true;
    }

    [Header("Health")]
    [SerializeField] private float maximumHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private UnityEvent onDeath;

    [Header("Interaction")]
    [SerializeField] private Camera interactionCamera;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionMask = ~0;

    [Header("Inventory")]
    [SerializeField] private int inventoryCapacity = 12;
    [SerializeField] private List<InventorySlot> inventory = new List<InventorySlot>();

    public float CurrentHealth => currentHealth;
    public float MaximumHealth => maximumHealth;
    public IReadOnlyList<InventorySlot> Inventory => inventory;

    public event Action<float, float> HealthChanged;
    public event Action InventoryChanged;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maximumHealth);
        if (interactionCamera == null) interactionCamera = Camera.main;
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed) TryInteract();
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || currentHealth <= 0f) return;
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        HealthChanged?.Invoke(currentHealth, maximumHealth);
        if (currentHealth <= 0f) onDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || currentHealth <= 0f) return;
        currentHealth = Mathf.Min(maximumHealth, currentHealth + amount);
        HealthChanged?.Invoke(currentHealth, maximumHealth);
    }

    public bool AddItem(string itemId, string displayName, int quantity, bool stackable)
    {
        if (string.IsNullOrWhiteSpace(itemId) || quantity <= 0) return false;

        if (stackable)
        {
            InventorySlot existing = inventory.Find(slot => slot.stackable && slot.itemId == itemId);
            if (existing != null)
            {
                existing.quantity += quantity;
                InventoryChanged?.Invoke();
                return true;
            }
        }

        for (int i = 0; i < quantity; i++)
        {
            if (inventory.Count >= inventoryCapacity) return false;
            inventory.Add(new InventorySlot
            {
                itemId = itemId,
                displayName = displayName,
                quantity = stackable ? quantity : 1,
                stackable = stackable
            });
            if (stackable) break;
        }

        InventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(string itemId, int quantity)
    {
        if (quantity <= 0) return false;
        InventorySlot slot = inventory.Find(entry => entry.itemId == itemId);
        if (slot == null || slot.quantity < quantity) return false;

        slot.quantity -= quantity;
        if (slot.quantity <= 0) inventory.Remove(slot);
        InventoryChanged?.Invoke();
        return true;
    }

    public int CountItem(string itemId)
    {
        int total = 0;
        foreach (InventorySlot slot in inventory)
            if (slot.itemId == itemId) total += slot.quantity;
        return total;
    }

    private void TryInteract()
    {
        if (interactionCamera == null) return;
        Ray ray = new Ray(interactionCamera.transform.position, interactionCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactionMask, QueryTriggerInteraction.Collide))
            hit.collider.SendMessage("Interact", this, SendMessageOptions.DontRequireReceiver);
    }
}
