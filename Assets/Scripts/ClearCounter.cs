using UnityEngine;

// ต้องระบุว่านำ IKitchenObjectParent มาใช้งานด้วย
public class ClearCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    // จุดอ้างอิงบนเคาน์เตอร์ที่จะให้วัตถุดิบไปวาง (อย่าลืมไปสร้าง Empty GameObject กำหนดตำแหน่งบนเคาน์เตอร์ใน Unity แล้วลากมาใส่ตัวแปรนี้นะครับ)
    [SerializeField] private Transform counterTopPoint;

    // ตัวแปรสำหรับจำว่าตอนนี้มีวัตถุดิบอะไรวางอยู่บนเคาน์เตอร์นี้บ้าง
    private KitchenObject kitchenObject;

    public void Interact(Player player)
    {
        Debug.Log("--- เริ่มเช็ค ClearCounter ---");
        Debug.Log("1. โต๊ะมีของวางอยู่ไหม? : " + HasKitchenObject());
        Debug.Log("2. ผู้เล่นถือของอยู่ไหม? : " + player.HasKitchenObject());

        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                Debug.Log("3. เงื่อนไขผ่าน! กำลังวางของลงบนโต๊ะ...");
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                Debug.Log("3. ผู้เล่นมือว่างเปล่า ไม่มีอะไรให้วาง");
            }
        }
        else
        {
            Debug.Log("3. โต๊ะไม่ว่าง! มีของวางอยู่แล้ว");
            // ... (ลอจิกใส่จาน/หยิบของ ส่วนอื่นๆ ด้านล่าง ปล่อยไว้เหมือนเดิมได้เลยครับ)

            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    // ====================================================================
    // ส่วนที่เพิ่มเข้ามาเพื่อให้ทำงานร่วมกับ IKitchenObjectParent ได้สมบูรณ์
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