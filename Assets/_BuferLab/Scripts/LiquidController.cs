using UnityEngine;

public class LiquidController : MonoBehaviour
{
    [Tooltip("Kéo object Cylinder (nước) vào ô này")]
    public Transform liquidMesh;

    void Update()
    {
        if (liquidMesh != null)
        {
            // Lấy góc xoay hiện tại của cái cốc
            Vector3 parentRotation = transform.eulerAngles;

            // Ép khối nước luôn hướng thẳng đứng (trục X và Z bằng 0)
            // Nhưng vẫn cho phép xoay quanh trục dọc (trục Y) theo cái cốc
            liquidMesh.rotation = Quaternion.Euler(0f, parentRotation.y, 0f);
        }
    }
}