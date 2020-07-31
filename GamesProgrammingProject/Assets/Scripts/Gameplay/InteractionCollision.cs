using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionCollision : MonoBehaviour
{
    CharacterController _characterControllerRef;

    public List<Collider> colliders;

    private bool canInteract = false;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.Equals(_characterControllerRef))
        {
            Debug.Log("Hit something: " + other);
            colliders.Add(other);
            if(colliders.Count > 0)
            {
                canInteract = true;
            }
        }
        else
        {
            Debug.Log("Should not be hitting" + other);
        }

        
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Left Something:" + other);

        if(colliders.Count > 0)
        {
            colliders.Remove(other);
            //colliders.TrimExcess();
            if(colliders.Count <= 0)
            {
                canInteract = false;
            }
        }
        else
        {
            Debug.Log("Could not remove Collider, collection empty.");
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

    public Collider GetFirstColliderFromCollisionCollection()
    {
        return colliders[0];
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
