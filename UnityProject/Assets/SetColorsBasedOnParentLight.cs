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
        parentLight.enabled = false;
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

        hit = new RaycastHit();
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f)) {
            transform.localScale = new Vector3(hit.distance / 6f, hit.distance / 6f, hit.distance/2);
        }
        else {
            transform.localScale = new Vector3(1f, 1f, 3f);
        }

        if(mat.material.color != parentLight.color) {
            mat.material.color = parentLight.color;
        }
    }
}
