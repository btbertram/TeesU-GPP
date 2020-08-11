using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionCollision : MonoBehaviour
{
    CharacterController _characterControllerRef;
    public List<IInteractable> interacters = new List<IInteractable>();

    private bool canInteract = false;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.Equals(_characterControllerRef))
        {
            Debug.Log("Hit something: " + other);
            var otherInteractable = other.gameObject.GetComponent<IInteractable>();
            interacters.Add(otherInteractable);
            otherInteractable.ToggleInteractionText();
            if(interacters.Count > 0)
            {
                canInteract = true;

            }

        }        
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Left Something:" + other);

        if(interacters.Count > 0)
        {
            var otherInteractable = other.gameObject.GetComponent<IInteractable>();
            interacters.Remove(otherInteractable);
            if (otherInteractable.TextRenderer.enabled)
            {
                otherInteractable.ToggleInteractionText();
            }
            if(interacters.Count <= 0)
            {
                canInteract = false;
            }
        }

    }

    public bool CanInteract()
    {
        return canInteract;
    }

    public void ToggleCanInteract()
    {
        canInteract = !canInteract;
    }

    public IInteractable GetFirstInteractableFromCollisionCollection()
    {
        return interacters[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        _characterControllerRef = FindObjectOfType<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
