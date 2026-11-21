using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GlassController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ToolController toolController;
    [SerializeField] private Collider glassCollider;
    [SerializeField] private Transform shapeContainer;

    [Header("Efeitos Visuais")]
    [SerializeField] private GameObject crackPrefab;
    [SerializeField] private Transform crackContainer;

    private Camera mainCamera;
    private SegmentScript[] segments;
    private int completedSegments;

    public int CompletedSegments => completedSegments;
    public int TotalSegments => segments != null ? segments.Length : 0;

    private void Awake() => mainCamera = Camera.main;

    private void Update()
    {
        if (gameManager == null || !gameManager.IsPlaying || Mouse.current == null || toolController == null || glassCollider == null)
        {
            if (toolController != null) toolController.HidePreview();
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            toolController.HidePreview();
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!glassCollider.Raycast(ray, out RaycastHit hit, 100f))
        {
            toolController.HidePreview();
            return;
        }

        Vector3 forward = shapeContainer != null ? shapeContainer.forward : glassCollider.transform.forward;
        toolController.UpdatePreview(hit.point, hit.normal, forward);

        if (Mouse.current.leftButton.wasPressedThisFrame)
            ApplyToolImpact(hit.point, hit.normal, forward);
    }

    public void LoadShape(GameObject shapePrefab)
    {
        completedSegments = 0;
        ClearChildren(crackContainer);
        ClearChildren(shapeContainer);

        GameObject newShape = Instantiate(shapePrefab, shapeContainer);
        newShape.transform.SetLocalPositionAndRotation(Vector3.zero, shapePrefab.transform.localRotation);
        newShape.transform.localScale = shapePrefab.transform.localScale;

        segments = newShape.GetComponentsInChildren<SegmentScript>(true);
    }

    private void ApplyToolImpact(Vector3 centerPoint, Vector3 normal, Vector3 forward)
    {
        var tool = toolController.CurrentTool;
        if (tool == null || tool.segments == null) return;

        Quaternion rot = toolController.GetWorldRotation(normal, forward);
        HashSet<SegmentScript> hitSegments = new HashSet<SegmentScript>();

        foreach (var seg in tool.segments)
        {
            Vector3 worldCenter = centerPoint + (rot * seg.localOffset);
            Collider[] overlaps = Physics.OverlapBox(worldCenter, seg.size * 0.5f, rot);

            bool hitAny = false;
            foreach (Collider col in overlaps)
            {
                SegmentScript segment = col.GetComponentInParent<SegmentScript>();
                if (segment != null)
                {
                    hitAny = true;
                    hitSegments.Add(segment);
                }
            }

            if (!hitAny)
            {
                SpawnCrackEffect(worldCenter, normal);
                gameManager.RegisterMiss();
                return;
            }
        }

        int newlyCompleted = 0;
        foreach (SegmentScript segment in hitSegments)
        {
            if (segment.Complete())
            {
                completedSegments++;
                newlyCompleted++;
            }
        }

        if (newlyCompleted > 0)
            gameManager.RegisterHit();
    }

    private void SpawnCrackEffect(Vector3 position, Vector3 normal)
    {
        if (crackPrefab == null) return;

        Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal) * Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);
        GameObject crack = Instantiate(crackPrefab, position + normal * 0.01f, rot, crackContainer);
        crack.transform.localScale = Vector3.one * 0.7f;
    }

    private void ClearChildren(Transform container)
    {
        if (container == null) return;

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            GameObject child = container.GetChild(i).gameObject;
            child.SetActive(false);
            Destroy(child);
        }
    }
}