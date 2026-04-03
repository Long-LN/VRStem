using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeatBox : MonoBehaviour
{
    [Header("Nhiệt độ ban đầu")]
    public float initialSpeed = 3f;
    public float minSpeed = 0.5f;

    [Header("Truyền nhiệt")]
    public float transferDuration = 5f;
    public float transferSpeed = 2f;

    [Header("Bounds Settings")]
    public Vector3 innerHalfSize = new Vector3(0.65f, 0.65f, 0.65f);
    public Vector3 innerCenterOffset = new Vector3(0f, 0f, 0f);

    [Header("Canvas Cân Bằng")]
    public GameObject balanceCanvas;

    [Header("Debug")]
    public float temperature = 0f;

    readonly List<Rigidbody> molecules = new();
    HeatBox contactBox = null;
    bool balanceSoundPlayed = false;
    bool isTransferring = false;
    bool isPaused = false;
    Vector3 innerHalf;
    Vector3 innerCenter;

    void Start()
{
    BoxCollider bc = GetComponent<BoxCollider>();
    if (bc != null)
    {
        // Tính đúng half size theo local space (không nhân scale)
        innerHalf = new Vector3(
            bc.size.x * 0.5f - 0.05f,
            bc.size.y * 0.5f - 0.05f,
            bc.size.z * 0.5f - 0.05f
        );
        innerCenter = bc.center;
        Debug.Log($"{name}: innerHalf={innerHalf} center={innerCenter}");
    }
    else
    {
        innerHalf = innerHalfSize;
        innerCenter = innerCenterOffset;
        Debug.Log($"{name}: dùng manual innerHalf={innerHalf}");
    }

    if (balanceCanvas != null)
        balanceCanvas.SetActive(false);

        FindAndInitMolecules();
        InvokeRepeating(nameof(StuckCheck), 1f, 0.5f);
        StartCoroutine(DelaySetSpeed());
    }

    IEnumerator DelaySetSpeed()
    {
        yield return null;
        yield return null;

        foreach (var rb in molecules)
        {
            if (rb == null) continue;
            rb.linearVelocity = Random.onUnitSphere * initialSpeed;
        }
    }

    void OnValidate()
    {
        innerHalf = innerHalfSize;
        innerCenter = innerCenterOffset;
    }

    void FindAndInitMolecules()
    {
        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            if (!rb.CompareTag("Molecule")) continue;
            if (molecules.Contains(rb)) continue;

            molecules.Add(rb);
            rb.useGravity = false;
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        Debug.Log($"{name}: {molecules.Count} phân tử");
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }

    void Update()
    {
        if (!isPaused)
            temperature = CalcTemperature();

        if (contactBox != null && !isTransferring && !isPaused)
            StartCoroutine(TransferLoop());
    }

    void FixedUpdate()
    {
        if (!isPaused)
            KeepInBounds();
    }

    void KeepInBounds()
    {
        foreach (var rb in molecules)
        {
            if (rb == null) continue;
            if (rb.isKinematic) continue;

            Vector3 localPos = transform.InverseTransformPoint(rb.position) - innerCenter;
            Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
            bool hit = false;

            if (localPos.x >  innerHalf.x) { localPos.x =  innerHalf.x; localVel.x = -Mathf.Abs(localVel.x); hit = true; }
            if (localPos.x < -innerHalf.x) { localPos.x = -innerHalf.x; localVel.x =  Mathf.Abs(localVel.x); hit = true; }
            if (localPos.y >  innerHalf.y) { localPos.y =  innerHalf.y; localVel.y = -Mathf.Abs(localVel.y); hit = true; }
            if (localPos.y < -innerHalf.y) { localPos.y = -innerHalf.y; localVel.y =  Mathf.Abs(localVel.y); hit = true; }
            if (localPos.z >  innerHalf.z) { localPos.z =  innerHalf.z; localVel.z = -Mathf.Abs(localVel.z); hit = true; }
            if (localPos.z < -innerHalf.z) { localPos.z = -innerHalf.z; localVel.z =  Mathf.Abs(localVel.z); hit = true; }

            if (hit)
            {
                rb.MovePosition(transform.TransformPoint(localPos + innerCenter));
                rb.linearVelocity = transform.TransformDirection(localVel);
            }
        }
    }

    float CalcTemperature()
    {
        molecules.RemoveAll(rb => rb == null);
        if (molecules.Count == 0) return 0f;

        int count = 0;
        float sum = 0f;
        foreach (var rb in molecules)
        {
            if (rb.isKinematic) continue;
            sum += rb.linearVelocity.magnitude;
            count++;
        }

        return count > 0 ? sum / count : temperature;
    }

    IEnumerator TransferLoop()
    {
        isTransferring = true;

        Debug.Log($"🌡 Bắt đầu: {name}={temperature:F1} ↔ {contactBox.name}={contactBox.temperature:F1}");

        while (contactBox != null && !isPaused)
        {
            float tempA = temperature;
            float tempB = contactBox.temperature;
            float diff  = tempA - tempB;

            if (Mathf.Abs(diff) < 0.05f)
            {
                float avg = (tempA + tempB) / 2f;
                SetMoleculeSpeed(avg);
                contactBox.SetMoleculeSpeed(avg);
                Debug.Log($"✅ Cân bằng: {avg:F1}");

                // Chỉ phát âm 1 lần duy nhất
                if (!balanceSoundPlayed)
                {
                    balanceSoundPlayed = true;
                    TempAudioManager.Instance?.PlaySFX(TempAudioManager.Instance.balanceSound);
                }

                // Hiện canvas cân bằng của cả 2 hộp
                if (balanceCanvas != null)
                    balanceCanvas.SetActive(true);

                if (contactBox != null && contactBox.balanceCanvas != null)
                    contactBox.balanceCanvas.SetActive(true);

                break;
            }

            float delta = Mathf.Abs(diff) * Time.deltaTime / transferDuration;

            if (tempA > tempB)
            {
                SetMoleculeSpeed(tempA - delta);
                contactBox.SetMoleculeSpeed(tempB + delta);
            }
            else
            {
                SetMoleculeSpeed(tempA + delta);
                contactBox.SetMoleculeSpeed(tempB - delta);
            }

            yield return null;
        }

        isTransferring = false;
    }

    public void SetMoleculeSpeed(float targetSpeed)
    {
        targetSpeed = Mathf.Max(targetSpeed, minSpeed);

        foreach (var rb in molecules)
        {
            if (rb == null) continue;
            if (rb.isKinematic) continue;

            if (rb.linearVelocity.sqrMagnitude < 0.001f)
            {
                rb.linearVelocity = Random.onUnitSphere * targetSpeed;
                continue;
            }

            float next = Mathf.Lerp(
                rb.linearVelocity.magnitude,
                targetSpeed,
                Time.deltaTime * transferSpeed
            );
            rb.linearVelocity = rb.linearVelocity.normalized * next;
        }
    }

    void StuckCheck()
    {
        if (isPaused) return;
        foreach (var rb in molecules)
        {
            if (rb == null) continue;
            if (rb.isKinematic) continue;
            if (rb.linearVelocity.magnitude < 0.1f)
                rb.linearVelocity = Random.onUnitSphere
                    * Mathf.Max(temperature, minSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Molecule")) return;

        HeatBox otherBox = other.GetComponentInParent<HeatBox>();
        if (otherBox != null && otherBox != this && contactBox == null)
        {
            contactBox = otherBox;
            Debug.Log($"🌡 {name}({temperature:F1}) ↔ " +
                      $"{otherBox.name}({otherBox.temperature:F1})");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Molecule")) return;
        if (contactBox == null) return;

        HeatBox otherBox = other.GetComponentInParent<HeatBox>();
        if (otherBox != null && otherBox == contactBox)
        {
            contactBox = null;
            balanceSoundPlayed = false; // ← reset để lần sau vẫn phát được
            Debug.Log($"❄ {name} tách → dừng");
        }
    }

    public float GetTemperature() => temperature;
    public int GetMoleculeCount() => molecules.Count;

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        BoxCollider bc = GetComponent<BoxCollider>();
        if (bc != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(bc.center, bc.size - Vector3.one * 0.1f);
        }

        UnityEditor.Handles.color =
            Color.Lerp(Color.blue, Color.red, temperature / 5f);
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 0.6f,
            $"🌡 {temperature:F1} m/s"
        );
#endif
    }
}
