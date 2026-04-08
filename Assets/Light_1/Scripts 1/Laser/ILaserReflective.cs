using UnityEngine;

public interface ILaserReflective
{
    public void Reflect(Laser laser, Ray incomingRay, RaycastHit hitInfo);
}
