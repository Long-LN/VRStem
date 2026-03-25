using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LiquidData
{
    public string liquidName = "Empty";
    public float volume = 0f; 
    public Color liquidColor = new Color(1f, 1f, 1f, 0f);
    public float phValue = 7.0f;
    
    public Dictionary<string, float> chemicalComponents = new Dictionary<string, float>();

    public LiquidData() {}

    public LiquidData(string name, float vol, Color color, float ph)
    {
        liquidName = name;
        volume = vol;
        liquidColor = color;
        phValue = ph;
    }

    public void AddChemical(string chemName, float moles)
    {
        if (chemicalComponents.ContainsKey(chemName))
            chemicalComponents[chemName] += moles;
        else
            chemicalComponents.Add(chemName, moles);
    }

    // HAM MOI: Trich xuat nuoc va hat phan tu khi rot ra
    public LiquidData Extract(float extractVolume)
    {
        float ratio = extractVolume / volume;
        if (ratio > 1f) ratio = 1f;

        LiquidData extracted = new LiquidData();
        extracted.liquidName = this.liquidName;
        extracted.volume = extractVolume;
        extracted.liquidColor = this.liquidColor;
        extracted.phValue = this.phValue;

        List<string> keys = new List<string>(chemicalComponents.Keys);
        foreach (string key in keys)
        {
            float extractedMoles = chemicalComponents[key] * ratio;
            extracted.AddChemical(key, extractedMoles);
            
            // TRU MOL KHOI COC GOC
            this.chemicalComponents[key] -= extractedMoles;
            if (this.chemicalComponents[key] <= 1e-7f) 
                this.chemicalComponents.Remove(key);
        }

        this.volume -= extractVolume;

        // RESET SACH SE KHI COC CAN
        if (this.volume <= 0.001f)
        {
            this.volume = 0f;
            this.chemicalComponents.Clear();
            this.phValue = 7f;
            this.liquidName = "Empty";
            this.liquidColor = new Color(1f, 1f, 1f, 0f);
        }

        return extracted;
    }
}