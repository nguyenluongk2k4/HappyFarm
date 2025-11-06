using UnityEngine;

public enum DragSource { None, Inventory, Hotbar }

public static class DragService
{
    public static DragSource Source = DragSource.None;
    public static int SourceIndex = -1;
    public static ItemData Item = null;
    public static int Amount = 0;

    private static GameObject dragIcon;

    public static void Begin(Canvas canvas, DragSource src, int index, ItemData item, int amount)
    {
        Source = src;
        SourceIndex = index;
        Item = item;
        Amount = amount;

        if (item != null && item.icon != null)
        {
            dragIcon = new GameObject("DragIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
            dragIcon.transform.SetParent(canvas.transform, false);
            var img = dragIcon.GetComponent<UnityEngine.UI.Image>();
            img.sprite = item.icon;
            img.raycastTarget = false;
        }
    }

    public static void Move(Vector2 position)
    {
        if (dragIcon != null)
            dragIcon.transform.position = position;
    }

    public static void End()
    {
        if (dragIcon != null) Object.Destroy(dragIcon);
        dragIcon = null;
        Source = DragSource.None;
        SourceIndex = -1;
        Item = null;
        Amount = 0;
    }
}
