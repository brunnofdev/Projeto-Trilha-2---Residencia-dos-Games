
using UnityEngine;

public class ShapePivot : MonoBehaviour
{
    [ContextMenu("Centralizar Pivot")]
    public void CenterPivot()
    {
        Renderer[] renderers =
            GetComponentsInChildren<Renderer>(true);

        if (renderers.Length == 0)
        {
            Debug.LogWarning("Nenhum Renderer encontrado!");
            return;
        }

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        Vector3 center = bounds.center;

        Vector3 offset = center - transform.position;

        transform.position = center;

        foreach (Transform child in transform)
        {
            child.position -= offset;
        }

        Debug.Log("Pivot centralizado!");
    }
}
