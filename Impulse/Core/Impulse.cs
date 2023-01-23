using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Impulse : MonoBehaviour
{
    public PhysicMaterial impulsPartMaterial;
    public List<AudioClip> impactSounds;
    public List<ParticleSystem> impactParticles;

    private AudioSource audioSource;
    private List<ImpulsePart> parts = new List<ImpulsePart>();

    void Start()
    {
        partInstanciation();
        audioSource = this.gameObject.AddComponent<AudioSource>();
    }


    public void projectileHit(Ray ray, RaycastHit hit, float radius, float strength, float offset)
    {
        playAudio();
        showParticles(hit.point);

        Collider[] hitColliders = Physics.OverlapSphere(hit.point, radius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "ImpulseDestructive" && parts.Contains(hitCollider.gameObject.GetComponent<ImpulsePart>()))
            {
                physicsInstanciation(hitCollider.gameObject, hit.point, ray.direction, strength, offset);
            }
        }

        clusterPhysicsInstanciation(getClusters());
    }
    private void partInstanciation()
    {
        this.tag = "ImpulseDestructive";

        foreach (Transform child in this.transform)
        {
            child.tag = "ImpulseDestructive";

            GameObject part = child.gameObject;

            part.AddComponent<MeshCollider>().convex = true;

            if(impulsPartMaterial != null)
            {
                part.GetComponent<MeshCollider>().material = impulsPartMaterial;
            }

            part.AddComponent<ImpulsePart>().impulse = this;
            parts.Add(part.GetComponent<ImpulsePart>());
        }
    }
    private void physicsInstanciation(GameObject obj, Vector3 hitPoint, Vector3 rayDirection, float impulseForce,float offset)
    {
        obj.tag = "Untagged";
        Rigidbody rb = obj.AddComponent<Rigidbody>();

        obj.transform.localScale = new Vector3(obj.transform.localScale.x * 0.9f, obj.transform.localScale.y * 0.9f, obj.transform.localScale.z * 0.9f);
        
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.AddForce((obj.transform.position - (hitPoint - rayDirection * offset)).normalized * impulseForce, ForceMode.Impulse);
        
        obj.GetComponent<ImpulsePart>().active = false;
        parts.Remove(obj.GetComponent<ImpulsePart>()); 
    }

 
    private HashSet<HashSet<ImpulsePart>> getClusters()
    {
        setAllImpulsePartsUnprocessed();
        HashSet<HashSet<ImpulsePart>> clusters = new HashSet<HashSet<ImpulsePart>>();

        while(getActiveAndUnprocessedImpulsePart(out ImpulsePart part) == true)
        {
            clusters.Add(new HashSet<ImpulsePart>() { part });
            int i = 0;

            while(i != clusters.Last().Count)
            {
                clusters.Last().UnionWith(getAllActiveAndUnprocessedNeighbors(clusters.Last().ElementAt(i)));
                clusters.Last().ElementAt(i).processed = true;
                i++;
            }
        }

        return clusters;
    }
    private void clusterPhysicsInstanciation(HashSet<HashSet<ImpulsePart>> clusters)
    {
        foreach(HashSet<ImpulsePart> cluster in clusters)
        {
            if(!cluster.Any(x => x.connected == true))
            {
                foreach(ImpulsePart part in cluster)
                {
                    part.tag = "Untagged";
                    part.gameObject.AddComponent<Rigidbody>();
                    part.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                    part.GetComponent<ImpulsePart>().active = false;
                    parts.Remove(part.GetComponent<ImpulsePart>());
                }
            }
        }
    }
    private void setAllImpulsePartsUnprocessed()
    {
        foreach(ImpulsePart part in parts)
        {
            part.processed = false;
        }
    }
    private bool getActiveAndUnprocessedImpulsePart(out ImpulsePart outPart)
    {
        foreach(ImpulsePart part in parts)
        {
            if (part.processed == false && part.active == true)
            {
                outPart = part;
                return true;
            }
        }

        outPart = null;
        return false;
    }
    private List<ImpulsePart> getAllActiveAndUnprocessedNeighbors(ImpulsePart part)
    {
        List<ImpulsePart> parts = new List<ImpulsePart>();

        foreach(ImpulsePart neighbor in part.neighbors)
        {
            if(neighbor.active == true && neighbor.processed == false)
            {
                parts.Add(neighbor);
            }
        }

        return parts;
    }


    private void playAudio()
    {
        if (impactSounds.Count > 0)
        {
            audioSource.clip = impactSounds[Random.Range(0, impactSounds.Count)];
            audioSource.Play();
        }
    }
    private void showParticles(Vector3 position)
    {
        if (impactParticles.Count > 0)
        {
            ParticleSystem ps = impactParticles[Random.Range(0, impactParticles.Count)];
            ps.transform.position = position;
            Instantiate(ps);
        }
    }

}
