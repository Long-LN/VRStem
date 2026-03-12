using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRScaleKnobDelta : XRBaseInteractable
{
    public Transform knobVisual;
    public Transform targetObject;

    [Header("Scale Limits")]
    public float minScale = 0.5f;
    public float maxScale = 10f;

    [Header("Speed")]
    public float scaleSpeed = 0.01f;

    private IXRSelectInteractor interactor;
    private float lastAngle;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        interactor = args.interactorObject;
        lastAngle = GetHandAngle();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        interactor = null;
    }

    void Update()
    {
        if (interactor == null) return;

        float currentAngle = GetHandAngle();
        float delta = Mathf.DeltaAngle(lastAngle, currentAngle);

        ChangeScale(delta);

        if (knobVisual != null)
            knobVisual.Rotate(Vector3.up, delta, Space.Self);

        lastAngle = currentAngle;
    }

    float GetHandAngle()
    {
        Transform hand = interactor.GetAttachTransform(this);
        Vector3 dir = hand.position - transform.position;
        return Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
    }

    public void ChangePivot(Transform pivot)
    {
        targetObject = pivot;
    }

    void ChangeScale(float delta)
    {
        if (targetObject == null) return;

        float scale = targetObject.localScale.x;
        scale += delta * scaleSpeed;

        scale = Mathf.Clamp(scale, minScale, maxScale);

        targetObject.localScale = Vector3.one * scale;
    }
}