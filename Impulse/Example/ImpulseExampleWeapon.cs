using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseExampleWeapon : MonoBehaviour
{
    public float radius;
    public float strength;
    public float offset;

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.tag == "ImpulseDestructive")
                {
                    hit.collider.GetComponent<ImpulsePart>().impulse.projectileHit(ray, hit, radius, strength, offset);
                }
            }
        }
    }
}
