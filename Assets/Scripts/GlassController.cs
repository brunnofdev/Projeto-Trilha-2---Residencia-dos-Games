using UnityEngine;
using UnityEngine.InputSystem;

public class GlassController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameManager gameManager;
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

        ClearCracks();


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
    Vector3 glassHitPoint = Vector3.zero;

    foreach (RaycastHit hit in hits)
    {
        SegmentScript segment = hit.collider.GetComponentInParent<SegmentScript>();

        if (segment != null)
        {
            // 1. Gera o efeito de rachadura no acerto
            SpawnCrackEffect(hit.point);

            // 2. Atualiza o segmento e o progresso
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
            glassHitPoint = hit.point;
        }
    }

    if (clickedGlass)
    {
        // 1. Gera o efeito de rachadura no erro
        SpawnCrackEffect(glassHitPoint);

        // 2. Desconta a tentativa no GameManager
        gameManager.RegisterMiss();
    }
}

    public void SpawnCrackEffect(Vector3 spawnPosition)
{
    if (crackPrefab == null)
    {
        Debug.LogWarning("[GlassController] 'crackPrefab' não foi atribuído no Inspector!");
        return;
    }

    // Leve elevação no eixo Y (0.005f) para não piscar sobre o vidro/mesa
    Vector3 position = new Vector3(spawnPosition.x, spawnPosition.y + 0.05f, spawnPosition.z);
    Quaternion rotation = Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);

    GameObject newCrack = Instantiate(crackPrefab, position, rotation, crackContainer);
    newCrack.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
}

public void ClearCracks()
{
    if (crackContainer == null) return;

    // Destrói todas as rachaduras acumuladas no container
    for (int i = crackContainer.childCount - 1; i >= 0; i--)
    {
        Destroy(crackContainer.GetChild(i).gameObject);
    }
    Debug.Log("[GlassController] Rachaduras limpas da cena.");
}
}