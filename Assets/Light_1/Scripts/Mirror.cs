using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Mirror : OpticalElement
{
    private List<LaserBeamPair> laserBeamPairs = new List<LaserBeamPair>();

    [Header("VR UI")]
    public VRReflectionUI ui;

    public GameObject infoCanvas; // UI kiến thức

    public TaskManager taskManager;
    public override void RegisterLaserBeam(LaserBeam laserBeam)
    {
        LaserBeam outgoingLaserBeam = Instantiate(laserBeam.Prefab, transform);
        laserBeamPairs.Add(new LaserBeamPair(laserBeam, outgoingLaserBeam));

        // 👉 ĐỒNG BỘ MÀU SẮC TỪ UI SANG TIA 3D
        if (ui != null)
        {
            // Tìm component vẽ tia sáng của cái tia vừa đẻ ra
            LineRenderer lr = outgoingLaserBeam.GetComponent<LineRenderer>();
            if (lr != null)
            {
                // Đổi màu 2 đầu của tia sáng cho giống với màu Reflected Color trong UI
                lr.startColor = ui.reflectedColor;
                lr.endColor = ui.reflectedColor;

                // (Tùy chọn) Đổi luôn màu của Material để đảm bảo nó phát sáng đúng màu
                if (lr.material != null)
                {
                    lr.material.color = ui.reflectedColor;
                    lr.material.SetColor("_EmissionColor", ui.reflectedColor * 2.5f); // Kích hoạt phát sáng (Bloom)
                }
            }
        }

        // 👉 trigger bắt đầu bài học
        if (GameFlowManager.Instance.currentState == GameFlowManager.GameState.Idle)
        {
            GameFlowManager.Instance.StartLesson();
        }

        // 👉 Khi có tia chiếu vào thì bật UI
        if (ui != null)
        {
            ui.ShowUI(true);
        }

        if (infoCanvas != null)
        {
            infoCanvas.SetActive(true);
        }
    }

    public override void UnregisterLaserBeam(LaserBeam laserBeam)
    {
        var pair = GetPairFromIncomingBeam(laserBeam);

        if (pair.outgoing.OpticalElementThatTheBeamHit != null)
        {
            pair.outgoing.OpticalElementThatTheBeamHit.UnregisterLaserBeam(pair.outgoing);
        }

        laserBeamPairs.Remove(pair);
        Destroy(pair.outgoing.gameObject);

        // 👉 Nếu không còn tia thì tắt UI
        if (laserBeamPairs.Count == 0 && ui != null)
        {
            ui.ShowUI(false);
        }

        if (laserBeamPairs.Count == 0 && infoCanvas != null)
        {
            infoCanvas.SetActive(false);
        }
    }

    public override void Propagate(LaserBeam laserBeam)
    {
        var pair = GetPairFromIncomingBeam(laserBeam);

        Vector3 incoming = pair.incoming.Direction;
        Vector3 normal = pair.incoming.HitNormal;
        Vector3 hitPos = pair.incoming.EndPosition;

        Vector3 outgoingDirection = Vector3.Reflect(incoming, normal);

        // ===== TÍNH GÓC =====
        float angleIn = Vector3.Angle(-incoming, normal);
        float angleOut = Vector3.Angle(outgoingDirection, normal);

        // ===== CHECK TASK 1 =====
        if (taskManager != null)
        {
            taskManager.CheckTask1(angleIn, angleOut);
        }

        // 👉 Update UI + truyền hit position
        if (ui != null)
        {
            ui.UpdateAngles(incoming, normal, outgoingDirection, hitPos);
        }

        // ===== ĐẾM SỐ LẦN PHẢN XẠ =====
        pair.outgoing.reflectionCount = pair.incoming.reflectionCount + 1;

        // ===== CHECK TASK 2 =====
        if (taskManager != null)
        {
            taskManager.CheckTask2(pair.outgoing.reflectionCount);
        }

        pair.outgoing.Propagate(hitPos, outgoingDirection);
    }

    private LaserBeamPair GetPairFromIncomingBeam(LaserBeam laserBeam)
    {
        return laserBeamPairs.Find(x => x.incoming == laserBeam);
    }
}