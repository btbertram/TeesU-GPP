using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    EInteractableType InteractableType { get; set; }
    void InteractionTriggered();
}
