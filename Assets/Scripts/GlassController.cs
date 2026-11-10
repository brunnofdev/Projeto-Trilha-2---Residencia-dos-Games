using UnityEngine;
using UnityEngine.InputSystem;

public class GlassController : MonoBehaviour
{
    private Camera camera;
    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            detectClick();
        }
    }

    void detectClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = camera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Acertou " + hit.point);
        }
    }
}

