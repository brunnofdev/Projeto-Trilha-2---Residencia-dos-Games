using UnityEngine;

public class SegmentScript : MonoBehaviour
{
    private bool completed;
    private Renderer segmentRenderer;
    private Color originalColor;

    private void Awake()
    {
        segmentRenderer = GetComponent<Renderer>();

        if (segmentRenderer != null)
            originalColor = segmentRenderer.sharedMaterial.color;
    }

    public bool Complete()
    {
        if (completed)
            return false;

        completed = true;

        if (segmentRenderer != null)
            segmentRenderer.material.color = Color.black;

        Debug.Log("Segmento clicado");

        return true;
    }

    public void ResetSegment()
    {
        completed = false;

        if (segmentRenderer != null)
            segmentRenderer.material.color = originalColor;
    }
}