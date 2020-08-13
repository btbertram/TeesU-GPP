using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    SerializationConnection _sConnection;
    int goldHeld = 0;

    public void UpdatePlayerGold(int amount)
    {
        goldHeld += amount;
        Debug.Log(goldHeld);
    }

    public int GetGoldHeld()
    {
        return goldHeld;
    }

    // Start is called before the first frame update
    async void Start()
    {
        ConnectionManager.GetCMInstance();
        _sConnection = GameObject.FindObjectOfType<SerializationConnection>();
        await _sConnection.LoadPlayerStatusAsync();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
