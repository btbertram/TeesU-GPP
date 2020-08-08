using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardDisplay : MonoBehaviour
{

    RectTransform rectTransform;
    public GameObject leaderboardDisplayBoxPrefab;
    public float leaderboardDisplayBoxSize = 100;
    List<GameObject> LeaderRowBoxes = new List<GameObject>();

    GameObject LoadAddLeaderRowToContent()
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.height + leaderboardDisplayBoxSize);
        var newLeaderBox = GameObject.Instantiate(leaderboardDisplayBoxPrefab, this.transform);

        ConnectionManager.GetCMInstance();

        return newLeaderBox;

    }

    void Awake()
    {
        rectTransform = this.gameObject.GetComponent<RectTransform>();
        LoadAddLeaderRowToContent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
