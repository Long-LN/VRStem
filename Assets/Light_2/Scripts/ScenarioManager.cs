using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ScenarioManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text menuBoardText;

    [Header("Dependencies")]
    public LightPhysics mainLaser;
    public Visualization visualization;
    public RefractiveMaterial targetWaterTank;
    public LightPhysics dispersionPrismLaser;

    [Header("Audio")]
    public AudioSource scenarioAudioSource;
    public AudioClip[] stepClips;

    [Header("Images")]
    public MeshRenderer image;
    public Texture[] stepTextures;

    [Header("Dynamic Objects")]
    public GameObject indexButtonPanel;
    public GameObject prismObject;

    [Header("Events")]
    public UnityEvent onStepCompleted;
    public UnityEvent onAllComplete;

    [Header("Runtime State")]
    [Range(0, 9)]
    public int currentStep = 0;

    private Coroutine autoAdvanceCoroutine;
    private bool isWaitingToAdvance = false;
    private bool isAudioFinished = false;

    void Start()
    {
        currentStep = 0;
        UpdateInstructions();
    }

    void Update()
    {
        CheckAutoProgress();
    }

    void AdvanceStep()
    {
        if (currentStep < 9)
        {
            currentStep++;
            onStepCompleted?.Invoke();
            UpdateInstructions();

            if (currentStep == 9)
            {
                onAllComplete?.Invoke();
            }
        }
    }

    void CheckAutoProgress()
    {
        // KHÔNG tiến triển nếu:
        // 1. Đang trong thời gian delay để nhảy bước (isWaitingToAdvance)
        // 2. Hoặc Audio hướng dẫn chưa đọc xong (!isAudioFinished)
        if (isWaitingToAdvance || !isAudioFinished)
            return;

        // THỬ THÁCH 1: Khúc xạ
        if (currentStep == 3)
        {
            if (visualization != null 
                && visualization.currentAngleR >= 35f 
                && visualization.currentAngleR <= 40f)
            {
                AdvanceStepWithDelay(1.5f);
            }
        }
        // THỬ THÁCH 2: Phản xạ toàn phần (TIR)
        else if (currentStep == 4)
        {
            if (mainLaser != null && targetWaterTank != null)
            {
                if (mainLaser.environmentRefractiveIndex > targetWaterTank.refractiveIndex
                    && visualization.isTIR)
                {
                    AdvanceStepWithDelay(2f);
                }
            }
        }
        // THỰC HÀNH: Tán sắc qua lăng kính (Prism)
        else if (currentStep == 8)
        {
            // Kiểm tra nếu laser đã chiếu trúng lăng kính
            if (dispersionPrismLaser != null && dispersionPrismLaser.isHittingPrism)
            {
                // Chỉ khi đã nghe xong audio ở bước 8 (isAudioFinished == true)
                // thì mới kích hoạt việc kết thúc bài học
                AdvanceStepWithDelay(2f);
            }
        }
    }

    void AdvanceStepWithDelay(float delay)
    {
        if (!isWaitingToAdvance)
        {
            isWaitingToAdvance = true;
            if (autoAdvanceCoroutine != null)
                StopCoroutine(autoAdvanceCoroutine);
            StartCoroutine(WaitAndAdvance(delay));
        }
    }

    IEnumerator WaitAndAdvance(float delay)
    {
        yield return new WaitForSeconds(delay);
        AdvanceStep();
    }

    public void UpdateInstructions()
    {
        if (menuBoardText == null)
            return;
        isWaitingToAdvance = false;

        // Quản lý Ẩn/Hiện vật thể
        ManageObjectVisibility();

        // Cập nhật nội dung bảng chữ
        UpdateTextContent();

        // LOGIC MỚI: Đợi Audio xong mới chuyển bước
        HandleAudioAndSequence();
    }

    private void HandleAudioAndSequence()
    {
        if (scenarioAudioSource == null || stepClips == null || currentStep >= stepClips.Length)
            return;

        AudioClip currentClip = stepClips[currentStep];
        if (currentClip != null)
        {
            isAudioFinished = false; // Reset cờ mỗi khi sang bước mới
            scenarioAudioSource.Stop();
            scenarioAudioSource.clip = currentClip;
            scenarioAudioSource.Play();

            // Bất kể là bước nào, cũng đợi audio xong mới bật cờ isAudioFinished
            StartCoroutine(WaitForAudioToEnd(currentClip.length));

            bool isTheoryStep = (currentStep != 3 && currentStep != 4 && currentStep != 8);
            if (isTheoryStep)
            {
                if (autoAdvanceCoroutine != null) StopCoroutine(autoAdvanceCoroutine);
                autoAdvanceCoroutine = StartCoroutine(AutoAdvanceAfterAudio(currentClip.length + 1.0f));
            }
        }
    }

    IEnumerator WaitForAudioToEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        isAudioFinished = true; // Đã nghe xong audio!
    }

    IEnumerator AutoAdvanceAfterAudio(float duration)
    {
        isWaitingToAdvance = true;
        yield return new WaitForSeconds(duration);
        AdvanceStep();
    }

    void UpdateTextContent()
    {
        switch (currentStep)
        {
            case 0:
                menuBoardText.text =
                    "<b>XIN CHÀO!</b>\n\nChào mừng bạn đến với\nPhòng thí nghiệm vật lý ánh sáng thực tế ảo (VR).";
                break;
            case 1:
                menuBoardText.text =
                    "<b>ĐẶT VẤN ĐỀ:</b>\n\nBạn có bao giờ nhìn thấy một chiếc đũa hoặc một vật thể\nkhi đi vào trong nước bị \"gãy\" hoặc cong đi chưa?\n\nTại sao ánh sáng lại không đi thẳng như bình thường?";
                break;
            case 2:
                menuBoardText.text =
                    "<b>GIẢI THÍCH:</b>\n\nĐó là do hiện tượng <b>Khúc Xạ Ánh Sáng</b>\n\nKhi ánh sáng đi từ môi trường này sang môi trường khác\n(ví dụ: không khí → nước), tốc độ ánh sáng thay đổi làm tia sáng\nbị bẻ cong. Định lý này được miêu tả bằng Định luật Snell.";
                break;
            case 3:
                menuBoardText.text =
                    "<b>THỬ THÁCH 1: TÌM GÓC KHÚC XẠ</b>\n\nHãy dùng tay xoay đèn Laser chiếu vào bể nước sao cho bắt được <b>góc khúc xạ r</b> ở nửa dưới mặt nước dao động trong khoảng <b>35° - 40°</b>.\n\n<i>Hệ thống tự động quét tia sáng của bạn...</i>";
                break;
            case 4:
                menuBoardText.text =
                    "<b>THỬ THÁCH 2: PHẢN XẠ TOÀN PHẦN</b>\n\nHãy bấm nút để biến đổi chiết suất của Môi trường bên ngoài sao cho\nđặc hơn cả khối Nước (n Môi trường > n Nước).Và quan sát hiện tượng.";
                break;
            case 5:
                menuBoardText.text =
                    "<b>GIẢI THÍCH PHẢN XẠ TOÀN PHẦN:</b>\n\nÁnh sáng bị dội ngược lại như một tấm gương soi do nó muốn đi từ\nmôi trường chiết suất Cao sang Thấp, nhưng lại va đập với ranh giới\nở góc quá lớn khiến tia sáng bị dội ngược trở lại.\n(Đây là nguyên lý của cáp quang truyền Internet).";
                break;
            case 6:
                menuBoardText.text =
                    "<b>ĐẶT VẤN ĐỀ:</b>\n\nBạn có bao giờ nhìn thấy cầu vồng lấp lánh xuất hiện sau cơn mưa rào?\nHoặc ánh sáng trắng khi đi qua những lăng kính\nlại tách thành 7 màu lấp lánh?";
                break;
            case 7:
                menuBoardText.text =
                    "<b>GIẢI THÍCH TÁN SẮC:</b>\n\nHiện tượng này gọi là <b>Tán sắc ánh sáng</b>.\nÁnh sáng trắng thực chất gồm 7 dải màu gộp lại (Đỏ -> Tím).\nMỗi màu có tần số và mức độ bẻ cong khác nhau:\nTím bẻ cong nhiều nhất, đỏ bẻ cong ít nhất.\nKết quả: Chúng tách rời nhau ra khi đi qua một môi trường khác";
                break;
            case 8:
                menuBoardText.text =
                    "<b>THỰC HÀNH TÁN SẮC:</b>\n\nHãy cầm đèn Laser và chiếu thẳng vào <b>Lăng Kính (Prism)</b>.\n\nQuan sát hiện tượng: Tán sắc ánh sáng!";
                break;
            case 9:
                menuBoardText.text =
                    "<b>KẾT THÚC BÀI HỌC!</b>\n\nBạn đã tự tay thực hành các thí nghiệm và quan sát các hiện tượng\nvề khúc xạ ánh sáng.\nCảm ơn bạn đã tham gia bài học này!\nHãy quay lại menu để tham khảo các bài học khác nhé!";
                break;
        }
    }

    void ManageObjectVisibility()
    {
        bool showWater = (currentStep >= 0 && currentStep <= 5);
        bool showButtons = (currentStep == 4 || currentStep == 5);
        bool showPrism = (currentStep >= 6);
        bool showImage = (
            currentStep == 1
            || currentStep == 2
            || currentStep == 3
            // || currentStep == 4
            || currentStep == 5
            // || currentStep == 6
            || currentStep == 7
            || currentStep == 8
        );

        if (targetWaterTank != null)
            targetWaterTank.gameObject.SetActive(showWater);
        if (indexButtonPanel != null)
            indexButtonPanel.gameObject.SetActive(showButtons);
        if (prismObject != null)
            prismObject.gameObject.SetActive(showPrism);
        if (image != null)
        {
            image.gameObject.SetActive(showImage);
            image.material.SetTexture("_BaseMap", stepTextures[currentStep]);
        }

        // Reset chiết suất môi trường nếu cần
        if (mainLaser != null && (currentStep <= 3 || currentStep >= 6))
            mainLaser.environmentRefractiveIndex = 1.0f;
    }
}
