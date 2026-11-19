using UnityEngine;

public class SegmentScript : MonoBehaviour
{
    private bool completed;
    private Renderer segmentRenderer;

    private void Awake()
    {
        segmentRenderer = GetComponent<Renderer>();
    }

    public bool Complete()
    {
        if (completed)
            return false;

        completed = true;

        if (segmentRenderer != null)
        {
            segmentRenderer.material.color = Color.black;
        }

        Debug.Log("Segmento clicado");
        return true;
    }
}