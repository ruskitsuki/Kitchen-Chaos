using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

// 1. เพิ่ม , IKitchenObjectParent หลัง MonoBehaviour
public class Player : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] private float movespeed = 5f;
    [SerializeField] private float rotatespeed = 8f;
    [SerializeField] private Transform holdPoint;

    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool isWalking = false;

    // 2. เพิ่มตัวแปรสำหรับเก็บของที่กำลังถืออยู่
    private KitchenObject kitchenObject;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled -= ctx => moveInput = Vector2.zero;
        inputActions.Player.Disable();
    }

    void Update()
    {
        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);

        transform.position += moveDir * movespeed * Time.deltaTime;

        isWalking = moveDir != Vector3.zero;

        if (isWalking)
        {
            transform.forward = Vector3.Slerp(
                transform.forward,
                moveDir,
                Time.deltaTime * rotatespeed
            );
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Counter")
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            if (selectedCounter != null) selectedCounter.gameObject.SetActive(true);

            ClearCounter clearcounter = collision.gameObject.GetComponent<ClearCounter>();
            if (clearcounter != null) clearcounter.Interact(this);
        }
        else if (collision.gameObject.tag == "Container")
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            if (selectedCounter != null) selectedCounter.gameObject.SetActive(true);

            ContainerCounter containercounter = collision.gameObject.GetComponent<ContainerCounter>();
            if (containercounter != null) containercounter.Interact(this);
        }
        else if (collision.gameObject.tag == "Cutting")
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            if (selectedCounter != null) selectedCounter.gameObject.SetActive(true);

            CuttingCounter cuttingcounter = collision.gameObject.GetComponent<CuttingCounter>();
            if (cuttingcounter != null) cuttingcounter.Interact(this);
        }
        else if (collision.gameObject.tag == "PlatesCounter")
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            if (selectedCounter != null) selectedCounter.gameObject.SetActive(true);

            PlateCounter platecounter = collision.gameObject.GetComponent<PlateCounter>();
            if (platecounter != null) platecounter.Interact(this);
        }
        else if (collision.gameObject.tag == "TrashCounter")
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            if (selectedCounter != null) selectedCounter.gameObject.SetActive(true);

            TrashCounter trashcounter = collision.gameObject.GetComponent<TrashCounter>();
            if (trashcounter != null) trashcounter.Interact(this);
        }
        else if (collision.gameObject.tag == "StoveCounter")
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            if (selectedCounter != null) selectedCounter.gameObject.SetActive(true);

            StoveCounter stovecounter = collision.gameObject.GetComponent<StoveCounter>();
            if (stovecounter != null) stovecounter.Interact(this);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        string[] validTags = { "Counter", "Container", "Cutting", "PlatesCounter", "TrashCounter", "StoveCounter" };
        if (validTags.Contains(collision.gameObject.tag))
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            if (selectedCounter != null) selectedCounter.gameObject.SetActive(false);
        }
    }

    // ====================================================================
    // 3. ปรับปรุงฟังก์ชันกลุ่มนี้ให้ตรงตามกฎของ IKitchenObjectParent ทั้งหมด
    // ====================================================================

    public Transform GetKitchenObjectFollowTransform()
    {
        return holdPoint;
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

    // ====================================================================

    public string[] GetKitchenObjectNames()
    {
        KitchenObject[] playerKitchenObject = this.GetComponentsInChildren<KitchenObject>();
        string[] listkitchenObject = new string[0];

        if (playerKitchenObject.Length > 0)
        {
            foreach (KitchenObject obj in playerKitchenObject)
            {
                System.Array.Resize(ref listkitchenObject, listkitchenObject.Length + 1);
                listkitchenObject[listkitchenObject.Length - 1] = obj.GetKitchenObjectname();
            }
        }
        return listkitchenObject;
    }

    public bool HasPlate()
    {
        if (HasKitchenObject())
        {
            return GetKitchenObject().GetKitchenObjectname() == "Plate";
        }
        return false;
    }
}