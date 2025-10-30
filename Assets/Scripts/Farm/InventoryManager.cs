// 30/10/2025
// Quản lý kho đồ của người chơi (Singleton)

using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    // Giúp các script khác có thể gọi InventoryManager.Instance.HàmGìĐó()
    public static InventoryManager Instance { get; private set; }

    void Awake()
    {
        // Chỉ cho phép 1 InventoryManager tồn tại
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // (Tùy chọn) Giữ kho đồ khi chuyển scene
        }
    }
    // --- Hết Singleton ---

    [Header("Settings")]
    public int maxSlots = 20; // Số ô kho tối đa

    [Header("Inventory Data")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    // --- CÁC HÀM CỐT LÕI ---

    /**
     * HÀM QUAN TRỌNG (UC-1.3)
     * Kiểm tra xem có đủ chỗ để thêm vật phẩm không.
     */
    public bool CheckForSpace(ItemData item, int amount)
    {
        int amountLeftToAdd = amount;

        // 1. Kiểm tra các ô ĐÃ CÓ item này và còn chỗ
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item && !slot.IsStackFull())
            {
                int spaceInStack = item.maxStackSize - slot.quantity;
                amountLeftToAdd -= spaceInStack;

                if (amountLeftToAdd <= 0)
                {
                    return true; // Có đủ chỗ trong các stack cũ
                }
            }
        }

        // 2. Nếu vẫn còn, kiểm tra xem còn Ô TRỐNG không
        if (amountLeftToAdd > 0)
        {
            int emptySlotsNeeded = Mathf.CeilToInt((float)amountLeftToAdd / item.maxStackSize);
            int emptySlotsAvailable = maxSlots - slots.Count;

            return emptySlotsAvailable >= emptySlotsNeeded; // True nếu đủ ô trống
        }

        return true; // Đã đủ chỗ ở bước 1
    }

    /**
     * Thêm vật phẩm vào kho. 
     * (Giả định bạn đã dùng CheckForSpace() trước đó)
     */
    public void AddItem(ItemData item, int amount)
    {
        int amountLeft = amount;

        // 1. Thêm vào các stack cũ
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item && !slot.IsStackFull())
            {
                int spaceInStack = item.maxStackSize - slot.quantity;
                int amountToAdd = Mathf.Min(amountLeft, spaceInStack);

                slot.quantity += amountToAdd;
                amountLeft -= amountToAdd;

                if (amountLeft <= 0)
                {
                    Debug.Log($"Thêm {amount} {item.itemName} vào kho.");
                    return; // Thêm xong
                }
            }
        }

        // 2. Nếu còn, dùng các ô trống
        while (amountLeft > 0 && slots.Count < maxSlots)
        {
            int amountForNewStack = Mathf.Min(amountLeft, item.maxStackSize);
            slots.Add(new InventorySlot(item, amountForNewStack));
            amountLeft -= amountForNewStack;
        }

        Debug.Log($"Thêm {amount} {item.itemName} vào kho.");
    }

    // TODO: Thêm các hàm RemoveItem(), HasItem() v.v...
}