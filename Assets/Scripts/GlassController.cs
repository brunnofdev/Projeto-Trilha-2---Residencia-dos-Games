using UnityEngine;
using UnityEngine.InputSystem;

public class GlassController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Collider glassCollider;
    [SerializeField] private Transform shapeContainer;

    private Camera mainCamera;
    private SegmentScript[] segments;

    private int completedSegments;

    public int CompletedSegments => completedSegments;
    public int TotalSegments => segments != null ? segments.Length : 0;

    private void Awake()
    {
        mainCamera = Camera.main;
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

    public void LoadShape(GameObject shapePrefab)
    {
        completedSegments = 0;


        for (int i = shapeContainer.childCount - 1; i >= 0; i--)
        {
            GameObject oldShape = shapeContainer.GetChild(i).gameObject;
            oldShape.SetActive(false);
            Destroy(oldShape);
        }

        GameObject newShape = Instantiate(shapePrefab, shapeContainer);
        newShape.transform.localPosition = Vector3.zero;
        newShape.transform.localRotation = Quaternion.identity;
        newShape.transform.localScale = shapePrefab.transform.localScale;

        segments = newShape.GetComponentsInChildren<SegmentScript>(true);
        Debug.Log($"Forma '{shapePrefab.name}' carregada com {segments.Length} segmentos.");
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
                    gameManager.RegisterHit();
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