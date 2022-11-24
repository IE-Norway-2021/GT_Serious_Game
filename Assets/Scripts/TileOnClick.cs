using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileOnClick : MonoBehaviour
{

    // // Start is called before the first frame update
    void Start()
    {
        
    }

    // // Update is called once per frame
    void Update()
    {
    if (Input.GetMouseButtonDown(0))
    {
         Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
         RaycastHit hit;
         
         if (Physics.Raycast(ray, out hit, 100))
         {
            // toggleOutline();
             hit.transform.gameObject.GetComponent<Outline>().enabled = false;
         }
     }
    }
}
