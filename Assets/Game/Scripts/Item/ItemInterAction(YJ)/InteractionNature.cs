using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionNature : InteractionObject, IGlow
{
    private Renderer objectRenderer;
    private bool isLoop = false;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();

        if(transform.childCount != 0)
            for(int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Renderer>().material = objectRenderer.materials[i];
            }
        else { };
        //메테리얼 설정
        for(int i = 0; i < objectRenderer.materials.Length; i++)
        {
            objectRenderer.materials[i].SetColor("_RimLightColor", new Color(1, 1, 0, 0));
            objectRenderer.materials[i].SetFloat("_RimWidth", 1f);
            objectRenderer.materials[i].SetFloat("_RimSharpness", 0f);
            objectRenderer.materials[i].SetFloat("_RimStrength", 1f);
            objectRenderer.materials[i].SetFloat("_RimBrighten", 0.35f);
        }

    }


    private void Update()
    {
        PlayerSearch();
    }


    //근처에 플레이어가 있다면 빛을 내게 된다
    private void PlayerSearch()
    {
        if (isCollectable)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, 1 << LayerMask.NameToLayer("Player"));
            if (colliders.Length > 0)
                StartCoroutine(Glow());
        }
    }

    //시간 설정
    public override IEnumerator Timer(int time)
    {
        isCollectable = false;
        for (int i = 0; i < objectRenderer.materials.Length; i++)
            objectRenderer.materials[i].SetColor("_RimLightColor", new Color(1, 1, 0, 0));
        yield return new WaitForSeconds(time);
        isCollectable = true;
        CollectNumber = collectNumberOrigin;
    }



    //빛을 내는 코루틴
    public IEnumerator Glow()
    {
        if (!isLoop)
        {
            isLoop = true;
            for (float i = 0; i < 1;)
            {
                for (int j= 0; j < objectRenderer.materials.Length; j++)
                    objectRenderer.materials[j].SetColor("_RimLightColor", new Color(1, 1, 0, i));
                
                yield return new WaitForSeconds(0.07f);
                i += 0.05f;
            }
            yield return new WaitForSeconds(0.1f);
            for (float i = 1; i > 0;)
            {
                for (int j = 0; j < objectRenderer.materials.Length; j++)
                    objectRenderer.materials[j].SetColor("_RimLightColor", new Color(1, 1, 0, i));
                
                yield return new WaitForSeconds(0.07f);
                i -= 0.05f;
            }
            yield return new WaitForSeconds(0.4f);
            isLoop = false;
        }
    }
}
