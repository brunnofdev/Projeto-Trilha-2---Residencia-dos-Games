
using UnityEngine;
using UnityEngine.InputSystem;

public class GlassController : MonoBehaviour
{
    private Camera camera;
    private SegmentScript[] segments;

    private int completedSegments;
    private bool completed;

    void Start()
    {
        camera = Camera.main;
        segments = GetComponentsInChildren<SegmentScript>();
        Debug.Log("Segmentos: " + segments.Length);
    }

    void Update()
    {
        if (completed || Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            DetectClick();
        }
    }

    void DetectClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = camera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            SegmentScript segment =
                hit.collider.GetComponentInParent<SegmentScript>();

            if (segment != null)
            {
                if (segment.Complete())
                {
                    completedSegments++;

                    Debug.Log("Progresso: " +
                        completedSegments + "/" + segments.Length);

                    if (completedSegments == segments.Length)
                    {
                        completed = true;
                        Debug.Log("Forma completa!");
                    }
                }
            }
        }
    }
}
