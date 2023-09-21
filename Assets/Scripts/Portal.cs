using UnityEngine;

public class Portal : MonoBehaviour
{
    public bool IsPlaced { get => true; }

    public Renderer Renderer { get => GetComponent<Renderer>(); }
}
