//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Runtime.InteropServices.WindowsRuntime;
//using UnityEngine;


//[RequireComponent(typeof(Collider))]
//public class Mirror : OpticalElement
//{
//    private List<LaserBeamPair> laserBeamPairs = new List<LaserBeamPair>();

//    public override void RegisterLaserBeam(LaserBeam laserBeam)
//    {
//        LaserBeam outgoingLaserBeam = GameObject.Instantiate(laserBeam.Prefab, transform);
//        laserBeamPairs.Add(new LaserBeamPair(laserBeam, outgoingLaserBeam));
//    }
//    public override void UnregisterLaserBeam(LaserBeam laserBeam)
//    {
//        var pair = GetPairFromIncomingBeam(laserBeam);

//        if (pair.outgoing.OpticalElementThatTheBeamHit != null)
//        {
//            pair.outgoing.OpticalElementThatTheBeamHit.UnregisterLaserBeam(pair.outgoing);
//        }

//        laserBeamPairs.Remove(pair);
//        GameObject.Destroy(pair.outgoing.gameObject);
//    }

//    public override void Propagate(LaserBeam laserBeam)
//    {
//        var pair = GetPairFromIncomingBeam(laserBeam);
//        Vector3 outgoingDirection = Vector3.Reflect(pair.incoming.Direction, pair.incoming.HitNormal);
//        pair.outgoing.Propagate(pair.incoming.EndPosition, outgoingDirection);
//    }

//    private LaserBeamPair GetPairFromIncomingBeam(LaserBeam laserBeam) => laserBeamPairs.Find(x => x.incoming == laserBeam);
//}

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