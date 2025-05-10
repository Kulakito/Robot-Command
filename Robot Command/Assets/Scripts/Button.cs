using UnityEngine;
using System.Collections.Generic;

public class Button : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [SerializeField] private List<Interactable> interactables = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            _animator.SetBool("IsPressed", true);
            foreach (var interactable in interactables)
            {
                interactable.OnInteract();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            _animator.SetBool("IsPressed", false);
        }
    }
}
