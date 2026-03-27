using UnityEngine;

public class ChemicalSource : MonoBehaviour
{
    public string chemicalName = "Nuoc Cat";
    public Color chemicalColor = new Color(1f, 1f, 1f, 0.1f);
    public float phValue = 7.0f;
    public float molarity = 0f; 
    public float volumePerClick = 10f; 

    public LiquidContainer currentTarget; 

    [Header("Am Thanh Nguon")]
    [Tooltip("Keo component Audio Source cua voi bom vao day")]
    public AudioSource sourceAudio;

    public void DispenseChemical()
    {
        if (currentTarget != null && currentTarget.liquidData.volume < currentTarget.maxVolume)
        {
            LiquidData incomingData = new LiquidData();
            incomingData.liquidName = chemicalName;
            incomingData.volume = volumePerClick;
            incomingData.liquidColor = chemicalColor;
            incomingData.phValue = phValue;

            if (molarity > 0f)
            {
                float moles = molarity * (volumePerClick / 1000f);
                incomingData.AddChemical(chemicalName, moles);
            }

            currentTarget.ReceiveLiquid(incomingData);

            // --- PHAT AM THANH KHI BOM ---
            if (sourceAudio != null && sourceAudio.clip != null)
            {
                // PlayOneShot giup am thanh khong bi ngat khi ban bam lien tuc
                sourceAudio.PlayOneShot(sourceAudio.clip);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        LiquidContainer container = other.GetComponentInParent<LiquidContainer>();
        if (container != null) currentTarget = container;
    }

    private void OnTriggerExit(Collider other)
    {
        LiquidContainer container = other.GetComponentInParent<LiquidContainer>();
        if (container != null && currentTarget == container) currentTarget = null;
    }
}