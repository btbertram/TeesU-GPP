using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A script attached to a gameobject, which defines the game object as a Gathering Point
/// for the player to interact with and harvest resouces from.
/// </summary>
public class GatheringPoint : MonoBehaviour, IInteractable
{
    int _pointID;
    public EGatherPointType _type;
    //In Seconds, for comparing to Unix time
    long _respawnTimer;
    bool _isActive;
    GatheringConnection gatheringPointConneciton;
    private EInteractableType _interactableType;
    private MeshRenderer _textRenderer;
    EInteractableType IInteractable.InteractableType { get => _interactableType; set => _interactableType = value; }
    MeshRenderer IInteractable.TextRenderer { get => _textRenderer; set => _textRenderer = value; }

    public void LoadPoint(int ID, EGatherPointType type)
    {
        _pointID = ID;
        _type = type;
    }

    public EGatherPointType GetPointType()
    {
        return _type;
    }

    public async void CheckIfRegrownAsync()
    {
        if (!_isActive)
        {
            Task<long> queryNowTimeTask = ConnectionManager.QueryTimeNowAsync();
            Task<long> queryGatherTimeTask = gatheringPointConneciton.QueryGatherTimeAsync(_pointID);

            long currenttime = await queryNowTimeTask;
            long lastGatheredTime = await queryGatherTimeTask;
            if(currenttime - lastGatheredTime >= _respawnTimer)
            {
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
        _textRenderer = this.gameObject.GetComponentInChildren<TextMesh>().gameObject.GetComponent<MeshRenderer>();

        switch (_type)
        {
            case EGatherPointType.GoldGatherType:
            {
                    _respawnTimer = 60;
                break;
            }

            default:
            {
                break;
            }
        }

        GameObject.FindObjectOfType<WorldManager>().AddToGatheringPointsList(this);

    }

    // Update is called once per frame
    void Update()
    {
        TextFaceCamera();
    }

    public async Task InteractionTriggeredAsync()
    {

        switch (_type)
        {
            case (int)EGatherPointType.GoldGatherType:
                Task<long> queryTimeTask = ConnectionManager.QueryTimeNowAsync();
                _isActive = false;
                ToggleInteractionText();
                this.gameObject.GetComponent<MeshRenderer>().enabled = _isActive;
                this.gameObject.GetComponent<BoxCollider>().enabled = _isActive;
                GameObject.FindObjectOfType<PlayerData>().gameObject.SendMessage(EMessagedFunc.UpdatePlayerGold.ToString(), 10);
                GameObject.FindObjectOfType<PlayerStats>().gameObject.SendMessage(EMessagedFunc.UpdateGoldTotal.ToString(), 10);
                GameObject.FindObjectOfType<PlayerStats>().gameObject.SendMessage(EMessagedFunc.UpdateGatheringPointsTotal.ToString(), 1);
                long currentTime = await queryTimeTask;
                await Task.Run(() => gatheringPointConneciton.AsyncRecordGatherAsync(currentTime, _pointID));

                break;

            default:
                break;
        }
    }

    public void ToggleInteractionText()
    {
        _textRenderer.enabled = !_textRenderer.enabled;
    }

    public void TextFaceCamera()
    {
        if (_textRenderer.enabled)
        {
            Vector3 camFacing = GameObject.FindObjectOfType<Camera>().transform.position;

            _textRenderer.transform.LookAt(camFacing);
            _textRenderer.transform.Rotate(Vector3.up, 180);

        }
    }

}
