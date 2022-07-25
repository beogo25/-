using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionCreature : InteractionObject
{
    public GameObject particle;
    public GameObject mesh;
    [HideInInspector]
    public PlayerStatus player;
    private void Awake()
    {
        player = FindObjectOfType<PlayerStatus>();
    }
    public override IEnumerator Timer(int time)
    {
        isCollectable = false;
        yield return new WaitForSeconds(time);
        isCollectable = true;
        CollectNumber = collectNumberOrigin;
        mesh.transform.localPosition = Vector3.zero;    
        mesh.SetActive(true);
    }
    public IEnumerator MeshMove()
    {
        for(int i=0;i<50;i++)
        {
            mesh.transform.Translate(new Vector3(0,Random.Range(0f,1f),Random.Range(0.5f,1f)) * 0.15f);
            yield return new WaitForFixedUpdate();
        }
        mesh.SetActive(false);
    }

    public override void Interaction()
    {
        if (isCollectable)
        {
            StartCoroutine(MeshMove());
            particle.SetActive(true);
            CollectNumber--;
        }
    }
}
