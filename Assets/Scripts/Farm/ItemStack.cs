// ItemStack.cs
[System.Serializable]
public struct ItemStack
{
    public ItemData item;
    public int quantity;

    public bool IsEmpty => item == null || quantity <= 0;
    public ItemStack(ItemData i, int q) { item = i; quantity = q; }
    public void Clear() { item = null; quantity = 0; }
}
