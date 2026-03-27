using UnityEngine;

public class TrashZone : MonoBehaviour
{
    [Tooltip("Phat am thanh hoac hieu ung khoi bui khi xoa coc (co the de trong)")]
    public GameObject destroyEffect;

    // Ham nay tu dong chay khi co mot vat the cham vao vung khong gian cua thung rac
    private void OnTriggerEnter(Collider other)
    {
        // Kiem tra xem vat the roi vao co dung la coc hoa chat khong
        LiquidContainer container = other.GetComponentInParent<LiquidContainer>();

        if (container != null)
        {
            // Neu ban co cai dat hieu ung (hinh anh hoac am thanh), phat no ra
            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, container.transform.position, Quaternion.identity);
            }

            // Lenh Destroy se xoa so hoan toan Game Object nay khoi bo nho cua Unity
            Destroy(container.gameObject);

            Debug.Log("Da tieu huy an toan: " + container.gameObject.name);
        }
    }
}