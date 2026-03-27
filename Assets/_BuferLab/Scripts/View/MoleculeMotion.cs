using UnityEngine;

public class MoleculeMotion : MonoBehaviour
{
    private HoloView myHoloView;
    private Vector3 targetPosition;
    private float changeTargetTimer = 0f;

    public void Setup(HoloView holoView)
    {
        myHoloView = holoView;
        PickNewTarget();
    }

    void Update()
    {
        if (myHoloView == null) return;

        float currentSpeed = myHoloView.simulationSpeed;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
        transform.Rotate(Vector3.one * currentSpeed * 10f * Time.deltaTime);

        changeTargetTimer -= Time.deltaTime;
        if (changeTargetTimer <= 0f || Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            PickNewTarget();
        }
    }

    private void PickNewTarget()
    {
        if (myHoloView != null)
        {
            targetPosition = myHoloView.hologramCenter.position + Random.insideUnitSphere * myHoloView.spawnRadius;
            changeTargetTimer = Random.Range(2f, 5f);
        }
    }
}