using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private Color color;
    [SerializeField]
    private MeshRenderer frontPortal;
    [SerializeField]
    private MeshRenderer behindPortal;

    public bool IsPlaced { get => true; }
    public Color Color { get => color; }
    public MeshRenderer FrontPortal { get => frontPortal; }
    public MeshRenderer BehindPortal { get => behindPortal; }

    void Awake()
    {
        frontPortal.material.SetColor("_BorderColor", color);
    }
}
