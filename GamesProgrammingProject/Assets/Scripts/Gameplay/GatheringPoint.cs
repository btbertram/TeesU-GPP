using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// A script attached to a gameobject, which defines the game object as a Gathering Point
/// for the player to interact with and harvest resouces from.
/// </summary>
public class GatheringPoint : MonoBehaviour, IInteractable
{
    int _pointID;
    public EGatherPointType _type;
    long _respawnTimer;
    bool _isActive;
    GatheringConnection gatheringPointConneciton;
    private EInteractableType _interactableType;
    EInteractableType IInteractable.InteractableType { get => _interactableType; set => _interactableType = value; }

    public void LoadPoint(int ID, EGatherPointType type)
    {
        _pointID = ID;
        _type = type;
    }

    public EGatherPointType GetPointType()
    {
        return _type;
    }

    public async void AsyncCheckIfRegrown()
    {
        if (!_isActive)
        {
            Debug.Log("Growin'!?" + this.gameObject.name);
            long currenttime = await ConnectionManager.AsyncQueryTimeNow();
            long lastGatheredTime = await gatheringPointConneciton.AsyncQueryGatherTime(_pointID);

            //Debug.Log("Currentime: " + currenttime + " LastTime: " + lastGatheredTime);

            if(currenttime - lastGatheredTime >= _respawnTimer)
            {
                Debug.Log("Sproutin'!");
                _isActive = true;
                this.gameObject.GetComponent<MeshRenderer>().enabled = _isActive;
                this.gameObject.GetComponent<BoxCollider>().enabled = _isActive;
            }

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gatheringPointConneciton = FindObjectOfType<GatheringConnection>();
        _interactableType = EInteractableType.GatheringPoint;

        switch (_type)
        {
            case EGatherPointType.GoldGatherType:
            {
                    _respawnTimer = 2000;
                    GameObject.FindObjectOfType<WorldManager>().AddToGatheringPointsList(this);
                break;
            }

            default:
            {
                break;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InteractionTriggered()
    {

        switch (_type)
        {
            case (int)EGatherPointType.GoldGatherType:
                GameObject.FindObjectOfType<PlayerData>().gameObject.SendMessage(EMessagedFunc.UpdatePlayerGold.ToString(), 10);
                gatheringPointConneciton.RecordGatherTime(_pointID);
                _isActive = false;
                this.gameObject.GetComponent<MeshRenderer>().enabled = _isActive;
                this.gameObject.GetComponent<BoxCollider>().enabled = _isActive;

                break;

            default:
                break;
        }
    }
}
