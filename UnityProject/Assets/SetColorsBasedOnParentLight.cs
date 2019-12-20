using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColorsBasedOnParentLight : MonoBehaviour
{
    public Renderer mat;
    public Light parentLight;

    RaycastHit hit;

    void Start()
    {
        
    }

    void Update()
    {
        if(parentLight.intensity < 0.1f && mat.enabled) {
            mat.enabled = false;
        }

        if (parentLight.intensity > 0.1f && !mat.enabled) {
            mat.enabled = true;
        }

        if (parentLight.intensity < 0.1f) {
            return;
        }

        if (Physics.Raycast(transform.position, transform.forward, out hit, 30f)) {

            transform.localScale = new Vector3(0.1f,0.1f,hit.distance);
        }

        if(mat.material.color != parentLight.color) {
            mat.material.color = parentLight.color;
        }
    }
}
