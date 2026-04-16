using System.Collections;
using UnityEngine;

public class QuizAndInforManager : MonoBehaviour
{
    SolarSystemFocus solarSystemFocus;
    public PlanetController planetController;
    public PlanetVisual bigPlanetVisual;
    public PlanetVisual smallPlanetVisual;

    void Start()
    {
        solarSystemFocus = SolarSystemFocus.Instance;
    }

    public void ShowPanel(string planetName)
    {
        Debug.Log(planetName);
        smallPlanetVisual = planetController.smallPlanets.Find(planet => planet.name == planetName);
        bigPlanetVisual   = planetController.bigPlanets.Find(planet => planet.name == planetName);

        bool isAnswered = PlanetQuiz.Instance.IsAnswered(planetName);
        Debug.Log($"[QuizAndInforManager] {planetName} isAnswered={isAnswered}");

        if (!isAnswered)
            StartCoroutine(ShowDelayQuiz());
        else
            StartCoroutine(ShowDelayInfo(planetName)); // Gọi coroutine mới thay vì ShowInfo trực tiếp
    }

    public void HidePanel(string planetName)
    {
        bigPlanetVisual = planetController.bigPlanets.Find(planet => planet.name == planetName);
        
        // Đảm bảo ẩn InfoPanel đi khi cần
        if (bigPlanetVisual != null && bigPlanetVisual.infoPanel != null)
        {
            bigPlanetVisual.infoPanel.SetActive(false);
        }
        else
        {
            bigPlanetVisual.HideInfo();
        }
    }

    IEnumerator ShowDelayQuiz()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("ShowQuiz");
        PlanetQuiz.Instance.StartQuiz(smallPlanetVisual.planetName, smallPlanetVisual);
    }

    // TH2: hành tinh đã trả lời → hiện info có chữ + phát thuyết minh + ẩn toàn bộ cụm slider
    IEnumerator ShowDelayInfo(string planetName)
    {
        yield return new WaitForSeconds(0.1f);

        Debug.Log($"[QuizAndInforManager] TH2 - Hiện info + phát thuyết minh cho: {planetName}");

        // 1. Dùng biến sliderPanel an toàn để ẩn khung
        if (SolarSystemFocus.Instance != null && SolarSystemFocus.Instance.sliderPanel != null)
            SolarSystemFocus.Instance.sliderPanel.SetActive(false);

        // 2. Hiện InfoPanel trực tiếp
        if (bigPlanetVisual != null && bigPlanetVisual.infoPanel != null)
        {
            bigPlanetVisual.infoPanel.SetActive(true);
            
            // 3. Gán nội dung thuyết minh vào TextMeshPro để không bị bảng trống
            if (bigPlanetVisual.descriptionText != null)
            {
                bigPlanetVisual.descriptionText.text = bigPlanetVisual.description;
            }

            // [THÊM MỚI] Cho phép người dùng xoay hành tinh TO
            if (PlanetRotator.Instance != null)
                PlanetRotator.Instance.SetPlanet(bigPlanetVisual.model);

            // [THÊM MỚI] Dừng script tự xoay để hành tinh đứng yên cho người dùng xoay
            PlanetOrbit orbit = bigPlanetVisual.GetComponentInParent<PlanetOrbit>();
            if (orbit != null) orbit.enabled = false;
        }

        // 4. Phát sequence mô tả hành tinh kèm callback chờ
        if (AnswerTrigger.Instance != null)
        {
            Debug.Log($"[QuizAndInforManager] Gọi AnswerTrigger.PlayDescription cho: {planetName}");
            AnswerTrigger.Instance.PlayDescription(planetName, () => 
            {
                // KHI AUDIO XONG: Hiện lại toàn bộ cụm khung xám của slider
                if (SolarSystemFocus.Instance != null && SolarSystemFocus.Instance.sliderPanel != null)
                    SolarSystemFocus.Instance.sliderPanel.SetActive(true);
            });
        }
        else
        {
            Debug.LogWarning("[QuizAndInforManager] AnswerTrigger.Instance là null!");
        }
    }
}