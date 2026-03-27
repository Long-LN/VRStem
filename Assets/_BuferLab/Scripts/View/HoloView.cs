using UnityEngine;
using System.Collections.Generic;

public class HoloView : MonoBehaviour
{
    [Header("Khu vuc Hien thi")]
    public Transform hologramCenter;
    public float spawnRadius = 2.0f;

    [Header("Tuy chinh Mo phong")]
    [Tooltip("Tong so luong mo hinh toi da trong phong")]
    public int maxParticles = 20;

    [Tooltip("Toc do bay cua cac phan tu")]
    [Range(0f, 5f)]
    public float simulationSpeed = 0.5f;

    [Header("Slot cho Prefab")]
    public GameObject prefabH_Plus;
    public GameObject prefabOH_Minus;
    public GameObject prefabNa_Plus;
    public GameObject prefabCl_Minus;
    public GameObject prefabNH3;
    public GameObject prefabNH4_Plus;

    private LiquidContainer currentContainer;
    private List<GameObject> activeMolecules = new List<GameObject>();
    private float updateTimer = 0f;
    private float updateInterval = 0.5f;

    void Update()
    {
        if (currentContainer != null)
        {
            updateTimer += Time.deltaTime;
            if (updateTimer >= updateInterval)
            {
                updateTimer = 0f;
                RefreshHologram();
            }
        }
    }

    private void RefreshHologram()
    {
        ClearHologram();

        LiquidData data = currentContainer.liquidData;
        if (data.volume <= 0) return;

        float totalMoles = 0f;
        foreach (float moles in data.chemicalComponents.Values)
        {
            totalMoles += moles;
        }

        if (totalMoles == 0f)
        {
            if (data.phValue < 6.5f) SpawnMolecules(prefabH_Plus, 3);
            else if (data.phValue > 7.5f) SpawnMolecules(prefabOH_Minus, 3);
            return;
        }

        foreach (KeyValuePair<string, float> chem in data.chemicalComponents)
        {
            string name = chem.Key;
            float moles = chem.Value;

            float ratio = moles / totalMoles;
            int spawnCount = Mathf.Clamp(Mathf.RoundToInt(ratio * maxParticles / 2f), 1, maxParticles);

            switch (name)
            {
                case "HCl":
                    SpawnMolecules(prefabH_Plus, spawnCount);
                    SpawnMolecules(prefabCl_Minus, spawnCount);
                    break;
                case "NaOH":
                    SpawnMolecules(prefabNa_Plus, spawnCount);
                    SpawnMolecules(prefabOH_Minus, spawnCount);
                    break;
                case "NH3":
                    SpawnMolecules(prefabNH3, spawnCount);
                    break;
                case "NH4Cl":
                    SpawnMolecules(prefabNH4_Plus, spawnCount);
                    SpawnMolecules(prefabCl_Minus, spawnCount);
                    break;
            }
        }
    }

    private void SpawnMolecules(GameObject prefab, int count)
    {
        if (prefab == null) return;

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = hologramCenter.position + Random.insideUnitSphere * spawnRadius;
            GameObject mol = Instantiate(prefab, randomPos, Random.rotation);

            MoleculeMotion motionCode = mol.GetComponent<MoleculeMotion>();
            if (motionCode == null) motionCode = mol.AddComponent<MoleculeMotion>();

            motionCode.Setup(this);

            activeMolecules.Add(mol);
        }
    }

    private void ClearHologram()
    {
        foreach (GameObject mol in activeMolecules)
        {
            if (mol != null) Destroy(mol);
        }
        activeMolecules.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        LiquidContainer container = other.GetComponentInParent<LiquidContainer>();
        if (container != null)
        {
            currentContainer = container;
            RefreshHologram();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LiquidContainer container = other.GetComponentInParent<LiquidContainer>();
        if (container != null && container == currentContainer)
        {
            currentContainer = null;
            ClearHologram();
        }
    }

    // Cung cap thong tin coc cho cac nut bam
    public LiquidContainer GetCurrentContainer()
    {
        return currentContainer;
    }
}