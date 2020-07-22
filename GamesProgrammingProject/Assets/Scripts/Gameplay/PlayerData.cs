using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerData : MonoBehaviour
{

    int goldHeld;

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

    void UpdatePlayerQuery()
    {
        //Query Logic here: Use UserSessionManager and Connection manager functions to update the player table.
    } 

    // Start is called before the first frame update
    void Start()
    {

    } 

    // Update is called once per frame
    void Update()
    {
        
    }
}
