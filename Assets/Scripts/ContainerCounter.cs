using UnityEngine;

// 1. นำ IKitchenObjectParent มาใช้ เพื่อให้เคาน์เตอร์นี้สามารถถือของได้ตามมาตรฐาน
public class ContainerCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform counterTopPoint;

    private KitchenObject kitchenObject;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Interact(Player player)
    {
        // ถ้าเคาน์เตอร์นี้ยังไม่มีของวางอยู่ (เสกของใหม่)
        if (!HasKitchenObject())
        {
            if (animator != null) animator.SetTrigger("OpenClose");

            // ใช้ฟังก์ชัน SpawnKitchenObject ที่คุณสร้างไว้ใน KitchenObject.cs มาช่วยเสกและจัดการ Parent ให้อัตโนมัติ
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, this);
        }
        else // ถ้ามีของวางอยู่บนเคาน์เตอร์แล้ว
        {
            // เช็คว่ามือผู้เล่นว่างอยู่ใช่ไหม? (แก้ Error ตรงจุดนี้)
            if (!player.HasKitchenObject())
            {
                // สั่งให้วัตถุที่อยู่บนเคาน์เตอร์ ย้ายตัวเองไปอยู่กับ Player 
                // (ฟังก์ชันนี้จะลบของออกจากเคาน์เตอร์ และย้ายไปใส่มือผู้เล่นให้เองแบบไม่เกิดบัค)
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    // ====================================================================
    // ส่วนประกอบของ IKitchenObjectParent ที่บังคับต้องมี
    // ====================================================================

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}