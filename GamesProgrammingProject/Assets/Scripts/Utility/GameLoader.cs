using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{

    //A general game init class, meant to be in the pre-loader/title scene.
    //Starts up GameManager Singleton.
    //Mostly to remove worry about "when" various singletons will be init. Class may be removed later based on structure.

    void Awake()
    {
        ConnectionManager.GetCMInstance();


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
