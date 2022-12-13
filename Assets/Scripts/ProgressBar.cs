using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ProgressBar : MonoBehaviour
{

    public float current;
    public float maximum;
    public Image mask;



    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }
    
    public void GetCurrentFill()
    {
        float currentFill = current / maximum;
        mask.fillAmount = currentFill;
    }
}
    