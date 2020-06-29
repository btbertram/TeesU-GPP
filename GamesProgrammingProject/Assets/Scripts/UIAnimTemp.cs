using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimTemp : MonoBehaviour
{

    public Image loadSymbol;
    private Vector3 rotateBy;

    // Start is called before the first frame update
    void Start()
    {
        rotateBy = new Vector3(0, 0, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        loadSymbol.rectTransform.Rotate(rotateBy);
    }
}
