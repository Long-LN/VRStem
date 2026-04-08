using UnityEngine;


public class LaserSource : MonoBehaviour
{
    public Transform sourceTransform;
    public LaserBeam laserBeam;


    private void Update()
    {
        Vector3 startPosition = sourceTransform.position;
        Vector3 direction = sourceTransform.forward;

        laserBeam.Propagate(startPosition, direction);
    }

}

//using UnityEngine;

//public class LaserSource : MonoBehaviour
//{
//    public Transform sourceTransform;
//    public LaserBeam laserBeam;

//    private bool isOn = false;

//    void Update()
//    {
//        if (!isOn)
//        {
//            laserBeam.gameObject.SetActive(false);
//            return;
//        }

//        laserBeam.gameObject.SetActive(true);

//        Vector3 startPosition = sourceTransform.position;
//        Vector3 direction = sourceTransform.forward;

//        laserBeam.Propagate(startPosition, direction);
//    }

//    // bật / tắt đèn
//    public void ToggleLight()
//    {
//        isOn = !isOn;
//    }

//    public void TurnOn()
//    {
//        isOn = true;
//    }

//    public void TurnOff()
//    {
//        isOn = false;
//    }
//}