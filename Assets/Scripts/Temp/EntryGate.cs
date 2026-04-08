// 
using UnityEngine;

public class EntryGate : MonoBehaviour
{
    public BoxCollider boxCollider; // kéo BoxCollider của GlassBox_Group vào

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Molecule")) return;

        MoleculeFloat mf = other.GetComponent<MoleculeFloat>();
        if (mf == null || mf.IsConfined()) return;

        mf.SetConfined();

        // Gán boundary cho phân tử
        MoleculeBoundary boundary = other.GetComponent<MoleculeBoundary>();
        if (boundary == null)
            boundary = other.gameObject.AddComponent<MoleculeBoundary>();

        boundary.boxCollider = boxCollider;
        boundary.enabled = true;
    }
}