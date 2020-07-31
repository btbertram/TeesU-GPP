using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    EInteractableType InteractableType { get; set; }
    MeshRenderer TextRenderer { get; set; }

    void InteractionTriggered();

    void ToggleInteractionText();

}
