using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

	public ProgressBar resource;
	public float newResource;

	// Start is called before the first frame update
	void Start()
	{
        // Update resource every 1 second
        InvokeRepeating("UpdateResource", 0, 1);
	}

	void UpdateResource()
	{
        resource = GameObject.Find("ResourceProgressBar").GetComponent<ProgressBar>();
		newResource = GameObject.Find("GameManager").GetComponent<GameManager>().metalMineCount;
		resource.current = newResource;
	}
}
