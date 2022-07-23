using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeFly : MonoBehaviour
{
    private bool coolTime = false;
    private int randNum;


    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 25f, 1 << LayerMask.NameToLayer("Player"));
        if(colliders.Length > 0)
        {
            
            if(!coolTime)
                StartCoroutine(FlyCo(colliders[0].gameObject));
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 25f);
    }


    private IEnumerator FlyCo(GameObject player)
    {
        transform.forward = player.transform.forward;
        coolTime = true;
        int randNum = Random.Range(20, transform.childCount);
        for(int i = 0; i < randNum; i++)
            transform.GetChild(i).gameObject.SetActive(true);

        yield return new WaitForSeconds(10f);
        for (int i = 0; i < randNum; i++)
            transform.GetChild(i).gameObject.SetActive(false);
        yield return new WaitForSeconds(30f);
        coolTime = false;
    }
}
