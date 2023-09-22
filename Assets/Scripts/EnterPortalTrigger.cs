using UnityEngine;

public class EnterPortalTrigger : MonoBehaviour
{
    [SerializeField]
    private Portal otherPortal;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerMovement>().TeleportTo(otherPortal.transform.position + (other.transform.position - transform.position));
    }
} 
