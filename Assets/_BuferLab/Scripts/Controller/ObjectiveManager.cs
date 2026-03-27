using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    [Header("Ket noi Giao dien")]
    [Tooltip("Keo dong text hien thi huong dan tren tuong vao day")]
    public TextMeshProUGUI instructionText;

    [Header("Ket noi Du lieu")]
    [Tooltip("Keo cai coc thi nghiem chinh tren ban vao day")]
    public LiquidContainer targetContainer;

    private int currentStep = 0;

    void Update()
    {
        // Kiem tra lien tuc neu da gan day du thanh phan
        if (targetContainer == null || instructionText == null) return;

        LiquidData data = targetContainer.liquidData;

        // Su dung Switch-case de chuyen doi giua cac buoc cua bai hoc
        switch (currentStep)
        {
            case 0:
                instructionText.text = "Bước 1: Đổ khoảng 50ml nước cất H2O vào cốc thí nghiệm.";

                // Kiem tra the tich va do tinh khiet (pH = 7)
                if (data.volume >= 45f && data.volume <= 55f && data.phValue > 6.9f && data.phValue < 7.1f)
                {
                    currentStep = 1; // Chuyen sang buoc tiep theo
                }
                // Neu lo do axit hay base vao ngay tu dau
                else if (data.volume > 0f && (data.phValue < 6.5f || data.phValue > 7.5f))
                {
                    instructionText.text = "Cảnh báo: Dung dịch không đúng. Vui lòng kiểm tra lại.";
                }
                break;

            case 1:
                instructionText.text = "Bước 2: Thêm dung dịch HCl vào cốc để giảm pH xuống dưới 3.0.";

                // Kiem tra xem pH da dat chuan acid chua
                if (data.phValue < 3.0f)
                {
                    currentStep = 2;
                }
                // Canh bao neu coc bi tran ma van chua dat chuan
                else if (data.volume > 200f)
                {
                    instructionText.text = "Cảnh báo: Cốc đã đầy mà pH vẫn chưa đạt. Vui lòng tiêu hủy và thử lại.";
                }
                break;

            case 2:
                instructionText.text = "Bước 3: Thêm dung dịch NaOH vào cốc để điều chỉnh pH về khoảng 7.0.";

                // Nguoi choi phai bom NaOH vao de keo pH len lai
                if (data.phValue >= 6.8f && data.phValue <= 7.2f && data.volume > 60f)
                {
                    currentStep = 3;
                }
                break;

            case 3:
                instructionText.text = "Xuất sắc! Bạn đã hoàn thành bài thực hành Mở đầu. Bạn có thể tiếp tục pha chế tự do.";
                break;
        }
    }
}