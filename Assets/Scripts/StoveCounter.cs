using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// 1. นำ IKitchenObjectParent มาใช้
public class StoveCounter : MonoBehaviour, IKitchenObjectParent
{
    // อ้างอิงตัวเอง (ถ้ามีไว้ใช้เปิด/ปิด Object)
    [SerializeField] private GameObject stoveCounterGameObject; // *เปลี่ยนจาก StoveCounter เป็น GameObject เพื่อให้ตรงกับการใช้ .SetActive(true)
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private KitchenObjectSO[] meatObjectOS; // [0] = สุก, [1] = ไหม้

    private float fryingProcess;
    public float fryingSpeed = 1f;
    public float fryingMax = 100f;
    private float timer = 0f;
    private int isCook = 0; // 0 = กำลังทอด, 1 = ทอดสุกแล้ว กำลังจะไหม้
    private Animator animator;

    // ตัวแปรเก็บวัตถุดิบที่อยู่บนเตา
    private KitchenObject kitchenObject;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // ทำงานเฉพาะตอนที่มีของบนเตา และกระบวนการทอดทำงานอยู่
        if (HasKitchenObject() && fryingProcess > 0)
        {
            fryingProcess += fryingSpeed * Time.deltaTime;

            FryingBar processBar = this.GetComponentInChildren<FryingBar>();
            float persent_process = fryingProcess / fryingMax;

            if (processBar != null) processBar.FryingCounter_OnProcessChanged(persent_process);

            if (fryingProcess >= fryingMax)
            {
                // ทำลายของเดิม (ดิบ / สุก)
                GetKitchenObject().DestroySelf();

                // เสกของใหม่ (สุก / ไหม้) ลงบนเตา
                KitchenObject.SpawnKitchenObject(meatObjectOS[isCook], this);

                if (processBar != null) processBar.FryingCounter_OnProcessChanged(0f);
                fryingProcess = 0f;

                if (isCook == 0)
                {
                    // เปลี่ยนสถานะเป็นสุกแล้ว และเริ่มนับเวลาไหม้ต่อ
                    isCook = 1;
                    fryingProcess = 0.01f;
                    if (animator != null) animator.SetBool("IsFlashing", true);

                    Transform warningUI = this.gameObject.transform.Find("StoveBurnWarningUI");
                    if (warningUI != null) warningUI.gameObject.SetActive(true);
                }
                else
                {
                    // ถ้าไหม้แล้ว ให้หยุดการทอดและปิดเตือน
                    ResetStoveUI();
                }
            }
        }
    }

    public void Interact(Player player)
    {
        // กรณีที่ 1: เตาว่างเปล่า
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                KitchenObject playerObj = player.GetKitchenObject();

                // เช็คว่าผู้เล่นถือเนื้อดิบมาไหม
                if (playerObj.GetKitchenObjectname() == "Meat")
                {
                    Debug.Log("Frying");

                    // วางเนื้อลงบนเตา
                    playerObj.SetKitchenObjectParent(this);

                    fryingProcess = 0.01f; // เริ่มต้นทอด
                    isCook = 0;
                    timer = 0f;

                    if (stoveCounterGameObject != null) stoveCounterGameObject.SetActive(true);
                }
            }
        }
        // กรณีที่ 2: มีของอยู่บนเตาแล้ว
        else
        {
            if (player.HasKitchenObject())
            {
                // ถ้าผู้เล่นถือจานมา
                if (player.HasPlate())
                {
                    if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plate))
                    {
                        // ลองเอาของบนเตา (เนื้อสุก/เนื้อไหม้) ใส่จาน
                        if (plate.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                        {
                            GetKitchenObject().DestroySelf();
                            ResetStoveUI();
                        }
                    }
                }
            }
            else
            {
                // ถ้าผู้เล่นมือว่าง ให้หยิบของออกจากเตา
                GetKitchenObject().SetKitchenObjectParent(player);
                ResetStoveUI();
            }
        }
    }

    // ฟังก์ชันสำหรับรีเซ็ต UI และสถานะเตาเมื่อหยิบของออก
    private void ResetStoveUI()
    {
        fryingProcess = 0f;
        isCook = 0;

        FryingBar processBar = this.GetComponentInChildren<FryingBar>();
        if (processBar != null) processBar.FryingCounter_OnProcessChanged(0f);

        if (animator != null) animator.SetBool("IsFlashing", false);

        Transform warningUI = this.gameObject.transform.Find("StoveBurnWarningUI");
        if (warningUI != null) warningUI.gameObject.SetActive(false);
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