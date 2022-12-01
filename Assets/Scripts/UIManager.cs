using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public int resource;

    // Start is called before the first frame update
    void Start()
    {
        resource = GameObject.Find("ResourceProgressBar").GetComponent<ProgressBar>().current;
        Debug.Log("Current resource: " + resource);
    }

    // Update is called once per frame
    void Update()
    {
        resource = 10;
    }
}
