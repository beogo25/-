using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionCreature : InteractionObject
{
    public GameObject particle;
    public GameObject mesh;
    private PlayerStatus player;
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
            mesh.transform.Translate(Vector3.forward * 0.1f);
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
            player.Hp += 30;
            CollectNumber--;
        }
    }
}
