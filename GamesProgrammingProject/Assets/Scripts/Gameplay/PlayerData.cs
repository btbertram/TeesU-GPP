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
    }

    public int GetGoldHeld()
    {
        return goldHeld;
    } 

    public async Task AsyncUpdatePlayerDataTable()
    {
        await new Task(() => { UpdatePlayerQuery(); });
            
    }

    void LoadPlayerData()
    {
        _sConnection.LoadPlayerStatus();
    }
     
    void UpdatePlayerQuery()
    {
        
    } 

    // Start is called before the first frame update
    void Start()
    {
        ConnectionManager.GetCMInstance();
        _sConnection = GameObject.FindObjectOfType<SerializationConnection>();
        LoadPlayerData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
