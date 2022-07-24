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
        coolTime = true;
        int randNum = Random.Range(35, transform.childCount);
        transform.forward = player.transform.forward;
        for(int i = 0; i < randNum; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(10f);
        for (int i = 0; i < randNum; i++)
            transform.GetChild(i).gameObject.SetActive(false);
        yield return new WaitForSeconds(50f);
        coolTime = false;
    }
}
