using UnityEngine;
using UnityEngine.InputSystem;

public class GlassController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Collider glassCollider;

    private Camera mainCamera;
    private SegmentScript[] segments;

    private int completedSegments;

    public int CompletedSegments => completedSegments;
    public int TotalSegments => segments != null ? segments.Length : 0;

    private void Awake()
    {
        mainCamera = Camera.main;
        segments = GetComponentsInChildren<SegmentScript>(true);
    }

    private void Update()
    {
        if (gameManager == null || !gameManager.IsPlaying)
            return;

        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            DetectClick();
        }
    }

    public void ResetRound()
    {
        completedSegments = 0;

        if (segments == null)
            return;

        foreach (SegmentScript segment in segments)
        {
            segment.ResetSegment();
        }
    }

    private void DetectClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        RaycastHit[] hits = Physics.RaycastAll(ray);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        bool clickedGlass = false;

        foreach (RaycastHit hit in hits)
        {
            SegmentScript segment = hit.collider.GetComponentInParent<SegmentScript>();

            if (segment != null)
            {
                if (segment.Complete())
                {
                    completedSegments++;
                    gameManager.RegisterHit(completedSegments, segments.Length);
                }
                return;
            }

            if (hit.collider == glassCollider)
            {
                clickedGlass = true;
            }
        }

        if (clickedGlass)
        {
            gameManager.RegisterMiss();
        }
    }
}