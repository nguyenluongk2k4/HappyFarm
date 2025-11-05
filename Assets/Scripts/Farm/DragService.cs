using UnityEngine;
using UnityEngine.UI;

public enum DragSource { None, Inventory, Hotbar }

public static class DragService
{
    public static DragSource Source = DragSource.None;
    public static int SourceIndex = -1;
    public static ItemData Item = null;
    public static int Amount = 0;

    static Image ghost;
    static RectTransform ghostRect;
    static Canvas canvas;

    public static void Begin(Canvas c, DragSource src, int index, ItemData item, int amount)
    {
        canvas = c; Source = src; SourceIndex = index; Item = item; Amount = amount;
        if (item == null) return;
        ghost = new GameObject("DragGhost").AddComponent<Image>();
        ghost.sprite = item.icon; ghost.raycastTarget = false;
        ghostRect = ghost.GetComponent<RectTransform>();
        ghostRect.SetParent(canvas.transform, false);
        ghostRect.sizeDelta = new Vector2(64, 64);
    }

    public static void Move(Vector2 screenPos)
    {
        if (ghostRect != null) ghostRect.position = screenPos;
    }

    public static void End()
    {
        if (ghost != null) Object.Destroy(ghost.gameObject);
        ghost = null; ghostRect = null;
        Source = DragSource.None; SourceIndex = -1; Item = null; Amount = 0;
    }
}
