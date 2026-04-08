using UnityEngine;

public class MoleculeCountTrigger : MonoBehaviour
{
    [Header("Settings")]
    public int requiredCount = 5;
    public GameObject explanationPanel;

    bool panelShown = false;

    void Update()
    {
        if (panelShown) return;

        int count = 0;
        foreach (var mf in FindObjectsByType<MoleculeFloat>(FindObjectsSortMode.None))
        {
            if (mf.IsConfined()) count++;
        }

        if (count >= requiredCount)
            ShowPanel();
    }

    void ShowPanel()
    {
        panelShown = true;
        if (explanationPanel != null)
            explanationPanel.SetActive(true);

        Debug.Log($"✅ Đủ {requiredCount} phân tử → hiện panel");
    }

    public void ClosePanel()
    {
        if (explanationPanel != null)
            explanationPanel.SetActive(false);
    }
}