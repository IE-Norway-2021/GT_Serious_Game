using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ProgressBar : MonoBehaviour
{

    public int current;
    public int maximum;
    public Image mask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }
    
    void GetCurrentFill()
    {
        float currentFill = (float)current / (float)maximum;
        mask.fillAmount = currentFill;
    }
}
