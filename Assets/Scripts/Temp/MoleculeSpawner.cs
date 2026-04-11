using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MoleculeSpawner : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject moleculePrefab;
    public int spawnCount = 10;
    public float spawnRadius = 0.2f;

    [Header("MoleculeFloat Params")]
    public float brownianForce = 0.05f;
    public float maxSpeed = 0.3f;
    public float moveRadius = 1.5f;
    public float collisionBoost = 1.3f;

    [Header("In-Box Speed")]
    public float inBoxSpeed = 2f;
    public float maxSpeedCap = 5f;

    bool spawned = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hand")) return;
        if (spawned) return;

        SpawnMolecules();
        spawned = true;
        gameObject.SetActive(false);
    }

    void SpawnMolecules()
    {
        TempAudioManager.Instance?.PlaySFX(TempAudioManager.Instance.moleculeSpawnSound);
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            GameObject m = Instantiate(moleculePrefab, pos, Quaternion.identity);
            m.tag = "Molecule";

            Rigidbody rb = m.GetComponent<Rigidbody>();
            if (rb == null) rb = m.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.linearVelocity = Vector3.zero;

            MoleculeFloat mf = m.GetComponent<MoleculeFloat>();
            if (mf == null) mf = m.AddComponent<MoleculeFloat>();
            mf.brownianForce = brownianForce;
            mf.maxSpeed = maxSpeed;
            mf.moveRadius = moveRadius;
            mf.collisionBoost = collisionBoost;
            mf.maxSpeedCap = maxSpeedCap;
            mf.enabled = false;

            XRGrabInteractable grab = m.GetComponent<XRGrabInteractable>();
            if (grab == null) grab = m.AddComponent<XRGrabInteractable>();
            grab.throwOnDetach = false;

            grab.selectEntered.AddListener(_ =>
            {
                mf.enabled = false;
                // ← chuyển âm thanh sang đây: phát khi BẮT ĐẦU grab
                TempAudioManager.Instance?.PlaySFX(
                    TempAudioManager.Instance.moleculeGrabSound, 0.6f);
            });

            grab.selectExited.AddListener(_ =>
            {
                rb.isKinematic = false;
                rb.linearDamping = 0f;
                rb.angularDamping = 0f;
                Debug.Log($"[Spawner] Released! Damping={rb.linearDamping}");
                mf.ActivateInBox(inBoxSpeed);
            });
        }
    }
}