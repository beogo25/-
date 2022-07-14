using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionNatureObject: MonoBehaviour, IGlow, IInteraction
{
    private Renderer objectRenderer;
    private Material material;
    private bool isLoop = false;
    public string[] itemList = new string[0];
    // Start is called before the first frame update
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        material = objectRenderer.material;
    }
    private void Update()
    { 
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, 1 << LayerMask.NameToLayer("Player"));
        if(colliders.Length > 0)
        {
            StartCoroutine(Glow());
        }
        else
        {
            material.SetColor("_EmissionColor", Color.yellow * 0);
            StopAllCoroutines();
            isLoop = false;
        }
    }


    public IEnumerator Glow()
    {
        if (!isLoop)
        {
            isLoop = true;
            for (float i = 0; i < 1;)
            {
                material.SetColor("_EmissionColor", Color.yellow * i);
                yield return new WaitForSeconds(0.06f);
                i += 0.1f;
            }
            yield return new WaitForSeconds(0.1f);
            for (float i = 1; i > 0;)
            {

                material.SetColor("_EmissionColor", Color.yellow * i);
                yield return new WaitForSeconds(0.1f);
                i -= 0.1f;
            }
            yield return new WaitForSeconds(0.4f);
            isLoop = false;
        }
    }

    public void Interaction()
    {

    }

}
