using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ImpulseExampleWeapon : MonoBehaviour
{
    public float radius = 0.15f;
    public float strength = 5;
    public float offset = 0.25f;

    public InputActionProperty trigger;
    public LayerMask mask;

    void Update()
    {
        if(trigger.action.ReadValue<float>() == 1)
        {
            Debug.Log("trooger");

            Ray ray = new Ray(this.transform.position, this.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray,out hit, Mathf.Infinity, mask))
            {
                Debug.Log("hit");
                if(hit.collider.tag == "ImpulseDestructive")
                {
                    hit.collider.GetComponent<ImpulsePart>().impulse.projectileHit(ray, hit, radius, strength, offset);
                }
            }
        }
    }
}
