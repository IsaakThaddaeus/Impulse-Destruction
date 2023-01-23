using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulsePart : MonoBehaviour
{
    public Impulse impulse;

    public bool connected;
    public bool active = true;
    public bool processed = false;

    public List<ImpulsePart> neighbors = new List<ImpulsePart>();

    void Start()
    {
        partIsConnectedToGround();
        findNeighbors();
    }

    void partIsConnectedToGround()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 0.5f);
     
        foreach (Collider collider in hitColliders)
        {
            if (collider.tag != "ImpulseDestructive" && Physics.ComputePenetration(this.GetComponent<MeshCollider>(), this.transform.position, this.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out Vector3 dir, out float dist) == true)
            {
                connected = true;
            }
        }
    }
    void findNeighbors()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 0.5f);

        Vector3 scale = this.transform.localScale;
        this.transform.localScale = this.transform.localScale * 1.001f;
        this.GetComponent<MeshCollider>().sharedMesh = this.gameObject.GetComponent<MeshFilter>().mesh;

        foreach (Collider collider in hitColliders)
        {
            if (Physics.ComputePenetration(this.GetComponent<MeshCollider>(), this.transform.position, this.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out Vector3 dir, out float dist) == true)
            {
                if(collider.tag == "ImpulseDestructive" && collider.gameObject != this.gameObject)
                {
                    neighbors.Add(collider.GetComponent<ImpulsePart>());
                }
            }
        }

        this.transform.localScale = scale;
        this.GetComponent<MeshCollider>().sharedMesh = this.gameObject.GetComponent<MeshFilter>().mesh;
    }

}
