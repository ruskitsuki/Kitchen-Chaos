using UnityEngine;

public class TrashCounter : MonoBehaviour
{
    public void Interact(Player player)
    {
        // 1. เช็คว่าผู้เล่นมีของถืออยู่ไหม (HasKitchenObject ตอนนี้เป็น bool แล้ว)
        if (player.HasKitchenObject())
        {
            Debug.Log("Destroy Item!");

            // 2. ดึงของที่ผู้เล่นถืออยู่ แล้วสั่งให้มันทำลายตัวเองทิ้งซะ
            // (ฟังก์ชัน DestroySelf() จะไปเรียก ClearKitchenObject() ใน Player ให้อัตโนมัติด้วย ทำให้มือผู้เล่นว่างจริงๆ)
            player.GetKitchenObject().DestroySelf();
        }
    }
}