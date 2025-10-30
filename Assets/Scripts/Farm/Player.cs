// 30/10/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int XP;
    public int Coins;

    public void AddXP(int amount)
    {
        XP += amount;
        Debug.Log($"XP hiện tại: {XP}");
    }
}
