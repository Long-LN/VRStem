using UnityEngine;
using System.Collections.Generic;

public enum ChemicalType { Acid, Base, Salt, Neutral }

[System.Serializable]
public class ChemicalProfile
{
    public string chemicalName;
    public ChemicalType type;
    public bool isStrong; 
    public float pKa_pKb; 
}

[System.Serializable]
public class ReactionRule
{
    public string reactionName; 
    public string reactant1;    
    public string reactant2;    
    public string product1;     
}

public class ChemistryEngine : MonoBehaviour
{
    public static ChemistryEngine Instance { get; private set; }

    [Header("Tu Dien Hoa Chat")]
    public List<ChemicalProfile> chemicalDatabase = new List<ChemicalProfile>();

    [Header("So Tay Phan Ung (Khong bat buoc voi Axit/Bazo manh)")]
    public List<ReactionRule> reactionRules = new List<ReactionRule>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // DA SUA: Ghep truc tiep, khong tinh sai ty le pha loang
    public void MixLiquids(LiquidData targetData, LiquidData incomingData)
    {
        if (incomingData.volume <= 0f) return;

        if (targetData.volume <= 0.001f || targetData.liquidName == "Empty")
        {
            targetData.liquidName = incomingData.liquidName;
            targetData.liquidColor = incomingData.liquidColor;
        }
        else if (targetData.liquidName != incomingData.liquidName)
        {
            targetData.liquidName = "Hon hop dung dich";
        }

        float totalVolume = targetData.volume + incomingData.volume;
        float incomingRatio = incomingData.volume / totalVolume;
        targetData.liquidColor = Color.Lerp(targetData.liquidColor, incomingData.liquidColor, incomingRatio);

        // Ghep toan bo Mol tu dong nuoc rot vao coc
        foreach (var kvp in incomingData.chemicalComponents)
        {
            targetData.AddChemical(kvp.Key, kvp.Value);
        }

        targetData.volume = totalVolume;
        ProcessDynamicReactions(targetData);
    }

    private void ProcessDynamicReactions(LiquidData data)
    {
        // 1. Xu ly luat phan ung tu khai bao
        foreach (ReactionRule rule in reactionRules)
        {
            if (data.chemicalComponents.ContainsKey(rule.reactant1) && data.chemicalComponents.ContainsKey(rule.reactant2))
            {
                float n1 = data.chemicalComponents[rule.reactant1];
                float n2 = data.chemicalComponents[rule.reactant2];

                if (n1 > 0 && n2 > 0)
                {
                    float reactedMoles = Mathf.Min(n1, n2);
                    data.chemicalComponents[rule.reactant1] -= reactedMoles;
                    data.chemicalComponents[rule.reactant2] -= reactedMoles;
                    if (!string.IsNullOrEmpty(rule.product1)) data.AddChemical(rule.product1, reactedMoles);
                }
            }
        }

        // 2. TU DONG TRUNG HOA AXIT MANH VA BAZO MANH (Khac phuc loi lech pH)
        List<string> strongAcids = new List<string>();
        List<string> strongBases = new List<string>();

        foreach (var kvp in data.chemicalComponents)
        {
            ChemicalProfile profile = chemicalDatabase.Find(x => x.chemicalName == kvp.Key);
            if (profile != null && profile.isStrong)
            {
                if (profile.type == ChemicalType.Acid) strongAcids.Add(kvp.Key);
                if (profile.type == ChemicalType.Base) strongBases.Add(kvp.Key);
            }
        }

        foreach (string acid in strongAcids)
        {
            foreach (string baseChem in strongBases)
            {
                float aMoles = data.chemicalComponents.ContainsKey(acid) ? data.chemicalComponents[acid] : 0;
                float bMoles = data.chemicalComponents.ContainsKey(baseChem) ? data.chemicalComponents[baseChem] : 0;
                
                if (aMoles > 1e-7f && bMoles > 1e-7f)
                {
                    float reactAmount = Mathf.Min(aMoles, bMoles);
                    data.chemicalComponents[acid] -= reactAmount;
                    data.chemicalComponents[baseChem] -= reactAmount;
                }
            }
        }

        CalculateDynamicPH(data);
    }

    private void CalculateDynamicPH(LiquidData data)
    {
        float volumeLiters = data.volume / 1000f;
        if (volumeLiters <= 0f) return;

        float epsilon = 1e-7f;

        ChemicalProfile strongAcid = null; float saConc = 0f;
        ChemicalProfile strongBase = null; float sbConc = 0f;
        ChemicalProfile weakAcid = null; float waConc = 0f;
        ChemicalProfile weakBase = null; float wbConc = 0f;

        // Phan loai cac chat dang co trong coc
        foreach (var kvp in data.chemicalComponents)
        {
            if (kvp.Value > epsilon)
            {
                ChemicalProfile profile = chemicalDatabase.Find(x => x.chemicalName == kvp.Key);
                if (profile != null)
                {
                    float concentration = kvp.Value / volumeLiters;
                    if (profile.type == ChemicalType.Acid)
                    {
                        if (profile.isStrong && concentration > saConc) { strongAcid = profile; saConc = concentration; }
                        else if (!profile.isStrong && concentration > waConc) { weakAcid = profile; waConc = concentration; }
                    }
                    else if (profile.type == ChemicalType.Base)
                    {
                        if (profile.isStrong && concentration > sbConc) { strongBase = profile; sbConc = concentration; }
                        else if (!profile.isStrong && concentration > wbConc) { weakBase = profile; wbConc = concentration; }
                    }
                }
            }
        }

        float finalPH = 7.0f;

        // Uu tien 1: Axit manh lan at
        if (strongAcid != null) 
        {
            finalPH = -Mathf.Log10(saConc);
        }
        // Uu tien 2: Bazo manh lan at
        else if (strongBase != null)
        {
            finalPH = 14f + Mathf.Log10(sbConc);
        }
        // Uu tien 3: HE DUNG DICH DEM (Co ca Axit yeu va Bazo yeu) - Kich hoat phuong trinh Henderson-Hasselbalch
        else if (weakAcid != null && weakBase != null)
        {
            finalPH = weakAcid.pKa_pKb + Mathf.Log10(wbConc / waConc);
        }
        // Uu tien 4: Chi co Axit yeu
        else if (weakAcid != null)
        {
            finalPH = 0.5f * (weakAcid.pKa_pKb - Mathf.Log10(waConc));
        }
        // Uu tien 5: Chi co Bazo yeu
        else if (weakBase != null)
        {
            finalPH = 14f - 0.5f * (weakBase.pKa_pKb - Mathf.Log10(wbConc));
        }

        data.phValue = Mathf.Clamp(finalPH, 0f, 14f);
    }

    public Color GetColorFromPH(float ph)
    {
        if (ph < 3f) return Color.Lerp(Color.red, new Color(1f, 0.5f, 0f), ph / 3f); 
        if (ph < 7f) return Color.Lerp(new Color(1f, 0.5f, 0f), Color.green, (ph - 3f) / 4f); 
        if (ph < 11f) return Color.Lerp(Color.green, Color.blue, (ph - 7f) / 4f); 
        return Color.Lerp(Color.blue, new Color(0.5f, 0f, 0.5f), (ph - 11f) / 3f); 
    }
}