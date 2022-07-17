using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionNature : InteractionObject, IGlow
{
    private Renderer objectRenderer;
    private Material material;
    private bool isLoop = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        objectRenderer = GetComponent<Renderer>();
        material       = objectRenderer.material;

    }

    private void Update()
    {
        PlayerSearch();
    }


    //��ó�� �÷��̾ �ִٸ� ���� ���� �ȴ�
    private void PlayerSearch()
    {
        if (isCollectable)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, 1 << LayerMask.NameToLayer("Player"));
            if (colliders.Length > 0)
                StartCoroutine(Glow());
        }
    }

    //�ð� ����
    public override IEnumerator Timer(int time)
    {
        isCollectable = false;
        material.SetColor("_EmissionColor", Color.yellow * 0);
        yield return new WaitForSeconds(time);
        isCollectable = true;
        CollectNumber = collectNumberOrigin;
    }



    //���� ���� �ڷ�ƾ
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
}
