
using UnityEngine;

public class SegmentScript : MonoBehaviour
{
    private bool completed;

    public bool Complete()
    {
        if (completed)
            return false;

        completed = true;

        GetComponent<Renderer>().material.color = Color.black;

        Debug.Log("Segmento clicado");

        return true;
    }
}
