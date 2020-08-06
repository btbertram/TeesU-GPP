using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IInteractable
{
    EInteractableType InteractableType { get; set; }
    MeshRenderer TextRenderer { get; set; }

    Task InteractionTriggered();

    void ToggleInteractionText();

}
