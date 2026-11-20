using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolController : MonoBehaviour
{
    [System.Serializable]
    public class HitboxSegment
    {
        public Vector3 localOffset = Vector3.zero;
        public Vector3 size = new Vector3(0.14f, 0.08f, 0.08f);
    }

    [System.Serializable]
    public class ChiselTool
    {
        public string toolName = "Goiva";
        public List<HitboxSegment> segments = new List<HitboxSegment>();
        public bool canRotate = false;
        public float rotationStep = 45f;
    }

    [Header("Ferramentas Disponíveis")]
    [SerializeField] private List<ChiselTool> tools = new List<ChiselTool>();

    [Header("Visual da Hitbox (Preview)")]
    [SerializeField] private Material previewMaterial;
    [SerializeField] private float previewHeightOffset = 0.008f;

    private int currentToolIndex;
    private float currentAngle;
    private GameObject previewRoot;

    public ChiselTool CurrentTool => (tools != null && tools.Count > 0) ? tools[currentToolIndex] : null;

    private void Reset()
    {
        tools = new List<ChiselTool>
        {
            new ChiselTool
            {
                toolName = "Goiva (Ponto)",
                segments = new List<HitboxSegment> { new HitboxSegment { size = new Vector3(0.08f, 0.08f, 0.08f) } }
            },
            new ChiselTool
            {
                toolName = "Cinzel (Linha)",
                canRotate = true,
                rotationStep = 45f,
                segments = new List<HitboxSegment>
                {
                    new HitboxSegment { localOffset = new Vector3(-0.15f, 0f, 0f), size = new Vector3(0.13f, 0.08f, 0.07f) },
                    new HitboxSegment { localOffset = Vector3.zero,                size = new Vector3(0.13f, 0.08f, 0.07f) },
                    new HitboxSegment { localOffset = new Vector3( 0.15f, 0f, 0f), size = new Vector3(0.13f, 0.08f, 0.07f) }
                }
            },
            new ChiselTool
            {
                toolName = "Formão em L",
                canRotate = true,
                rotationStep = 90f,
                segments = new List<HitboxSegment>
                {
                    new HitboxSegment { localOffset = Vector3.zero,               size = new Vector3(0.11f, 0.08f, 0.11f) },
                    new HitboxSegment { localOffset = new Vector3(0f, 0f, 0.13f), size = new Vector3(0.11f, 0.08f, 0.11f) },
                    new HitboxSegment { localOffset = new Vector3(0f, 0f, 0.26f), size = new Vector3(0.11f, 0.08f, 0.11f) },
                    new HitboxSegment { localOffset = new Vector3(0.13f, 0f, 0f), size = new Vector3(0.11f, 0.08f, 0.11f) }
                }
            }
        };
    }

    private void Start() => BuildPreviewVisual();

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectTool(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectTool(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectTool(2);
            if (Keyboard.current.rKey.wasPressedThisFrame) RotateTool();
        }

        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            RotateTool();
    }

    public Quaternion GetWorldRotation(Vector3 normal, Vector3 forward)
    {
        Vector3 projected = Vector3.ProjectOnPlane(forward, normal);
        if (projected.sqrMagnitude < 0.0001f)
            projected = Vector3.ProjectOnPlane(Vector3.forward, normal);

        return Quaternion.LookRotation(projected.normalized, normal) * Quaternion.Euler(0f, currentAngle, 0f);
    }

    public void SelectTool(int index)
    {
        if (tools == null || index < 0 || index >= tools.Count) return;

        currentToolIndex = index;
        currentAngle = 0f;
        BuildPreviewVisual();
    }

    public void RotateTool()
    {
        if (CurrentTool != null && CurrentTool.canRotate)
            currentAngle = (currentAngle + CurrentTool.rotationStep) % 360f;
    }

    public void UpdatePreview(Vector3 hitPoint, Vector3 normal, Vector3 forward)
    {
        if (previewRoot == null || CurrentTool == null) return;

        previewRoot.SetActive(true);
        previewRoot.transform.SetPositionAndRotation(
            hitPoint + (normal * previewHeightOffset),
            GetWorldRotation(normal, forward)
        );
    }

    public void HidePreview()
    {
        if (previewRoot != null && previewRoot.activeSelf)
            previewRoot.SetActive(false);
    }

    private void BuildPreviewVisual()
    {
        if (previewRoot != null) Destroy(previewRoot);
        if (CurrentTool == null || CurrentTool.segments == null) return;

        previewRoot = new GameObject("ToolHitboxPreview");
        previewRoot.transform.SetParent(transform);
        previewRoot.SetActive(false);

        foreach (var seg in CurrentTool.segments)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.transform.SetParent(previewRoot.transform);
            marker.transform.localPosition = seg.localOffset;
            marker.transform.localScale = new Vector3(seg.size.x, 0.003f, seg.size.z);

            Destroy(marker.GetComponent<Collider>());
            if (previewMaterial != null)
                marker.GetComponent<Renderer>().sharedMaterial = previewMaterial;
        }
    }
}