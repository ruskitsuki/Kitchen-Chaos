using UnityEngine;

// 1. นำ IKitchenObjectParent มาใช้งาน
public class PlateCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform counterTopPoint;

    // 2. เปลี่ยนจากเก็บ Transform เป็นเก็บ KitchenObject แทน
    private KitchenObject plateKitchenObject;

    private void Start()
    {
        // ใช้ระบบ Spawn เสกจานใบแรกมาวางบนเคาน์เตอร์ตอนเริ่มเกม
        KitchenObject.SpawnKitchenObject(kitchenObjectSO, this);
    }

    public void Interact(Player player)
    {
        // ถ้าผู้เล่นมือว่าง และบนโต๊ะนี้มีจานวางอยู่
        if (!player.HasKitchenObject() && HasKitchenObject())
        {
            // 1. ย้ายจานจากโต๊ะ ไปใส่มือผู้เล่น 
            // (คำสั่งนี้จะจัดการย้าย Parent และอัปเดตตัวแปรทั้งหมดให้อัตโนมัติ ป้องกันบัคภาพหลอนครับ)
            GetKitchenObject().SetKitchenObjectParent(player);

            // 2. เสกจานใบใหม่มาวางรอไว้บนเคาน์เตอร์ทันที
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, this);
        }
    }

    // ====================================================================
    // ส่วนประกอบของ IKitchenObjectParent
    // ====================================================================

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.plateKitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return plateKitchenObject;
    }

    public void ClearKitchenObject()
    {
        plateKitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return plateKitchenObject != null;
    }
}