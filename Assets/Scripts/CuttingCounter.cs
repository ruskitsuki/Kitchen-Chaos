using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

// 1. นำ IKitchenObjectParent มาใช้
public class CuttingCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private float cuttingProcess;
    public float cuttingSpeed = 5f;
    public float knifeSpeed = 0.2f;
    private Animator animator;
    private float timer = 0f;
    private readonly string[] cuttableObjects = { "Tomato", "Cheese", "Cabbage" };

    // ตัวแปรเก็บของที่อยู่บนเขียง
    private KitchenObject kitchenObject;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // ทำงานเฉพาะตอนที่มีกระบวนการหั่น และ "มีวัตถุดิบอยู่บนเขียง" เท่านั้น
        if (cuttingProcess > 0 && HasKitchenObject())
        {
            KitchenObject currentObj = GetKitchenObject();
            cuttingProcess += cuttingSpeed * Time.deltaTime;
            Cutting_FX(Time.deltaTime);

            ProcessBar processBar = GetComponentInChildren<ProcessBar>();
            string objectName = currentObj.GetKitchenObjectname();

            if (cuttableObjects.Contains(objectName))
            {
                int index = System.Array.IndexOf(cuttableObjects, objectName);
                int cuttingMax = cuttingRecipeSOArray[0].cutCount; // Note: ตรงนี้ฟิกซ์ค่า Index 0 ไว้ อาจต้องปรับให้ดึงตามชนิดวัตถุดิบในอนาคตครับ
                float percent_process = cuttingProcess / cuttingMax;

                if (processBar != null) processBar.CuttingCounter_OnProcessChanged(percent_process);

                if (cuttingProcess >= cuttingMax)
                {
                    // ทำลายวัตถุดิบเดิมทิ้ง (ระบบจะ ClearKitchenObject ให้อัตโนมัติ)
                    currentObj.DestroySelf();

                    // เสกวัตถุดิบที่หั่นเสร็จแล้วลงบนเขียง
                    KitchenObject.SpawnKitchenObject(cuttingRecipeSOArray[index].to, this);

                    if (processBar != null) processBar.CuttingCounter_OnProcessChanged(0f);
                    cuttingProcess = 0;
                }
            }
        }
    }

    private void Cutting_FX(float duration)
    {
        timer += duration;
        if (timer >= knifeSpeed)
        {
            animator.SetTrigger("Cut");
            timer = 0f;
        }
    }

    public void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // เขียงว่างเปล่า เช็คว่าผู้เล่นถือของมาไหม
            if (player.HasKitchenObject())
            {
                KitchenObject playerObj = player.GetKitchenObject();
                string objectName = playerObj.GetKitchenObjectname();

                // ถ้าของที่ถือมา สามารถหั่นได้
                if (cuttableObjects.Contains(objectName))
                {
                    Debug.Log("Slice Item: " + objectName);

                    // วางของลงบนเขียง
                    playerObj.SetKitchenObjectParent(this);

                    // เริ่มต้น Process การหั่น
                    cuttingProcess = 0;
                    ProcessBar processBar = GetComponentInChildren<ProcessBar>();

                    int cuttingMax = cuttingRecipeSOArray[0].cutCount;
                    float percentProcess = (float)cuttingProcess / cuttingMax;
                    if (processBar != null) processBar.CuttingCounter_OnProcessChanged(percentProcess);

                    cuttingProcess++;
                    if (animator != null) animator.SetTrigger("Cut");
                    timer = 0f;
                }
            }
        }
        else
        {
            // บนเขียงมีของวางอยู่
            if (player.HasKitchenObject())
            {
                // ถ้าผู้เล่นถือจานมา ให้ลองเอาของบนเขียงใส่จาน
                if (player.HasPlate())
                {
                    if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plate))
                    {
                        if (plate.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                        {
                            GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {
                // ผู้เล่นมือว่างเปล่า ให้หยิบของจากเขียงไปถือ
                GetKitchenObject().SetKitchenObjectParent(player);
                cuttingProcess = 0; // รีเซ็ตการหั่นเผื่อหยิบออกกลางคัน

                ProcessBar processBar = GetComponentInChildren<ProcessBar>();
                if (processBar != null) processBar.CuttingCounter_OnProcessChanged(0f);
            }
        }
    }

    public void InterActAlter(Player player)
    {
        // มักจะใช้ปุ่มนี้สำหรับการกดหั่นรัวๆ แบบ Manual แทนการหั่นอัตโนมัติใน Update ครับ (เผื่อคุณอยากย้ายลอจิกมาไว้ที่นี่)
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