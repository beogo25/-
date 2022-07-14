using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NatureItemTable
{
    public string[]                 name    = new string[0];
    public int[]                    percent = new int[0];
    public Dictionary <int, string> itemDic = new Dictionary<int, string>();
    public void Set()
    {
        for(int i = 0; i < name.Length; i++)
            itemDic.Add(percent[i], name[i]);
    }
    public int Choose()
    {
        int randomPoint = Random.Range(0, 100);

        for (int i = 0; i < percent.Length; i++)
        {
            if (randomPoint < percent[i])
                return percent[i];
            else
                randomPoint += percent[i];
        }
        return percent[percent.Length-1];
    }
}


public class InteractionNatureObject: MonoBehaviour, IGlow, IInteraction
{
    public  NatureItemTable table;
    private Renderer        objectRenderer;
    private Material        material;
    private bool            isLoop   = false;
    private bool            isCollectable;
    private int             collectNumber = 3;
    private int CollectNumber
    {
        get { return collectNumber; }
        set 
        { 
            collectNumber = value; 

            if(collectNumber == 0)
            {
                StartCoroutine(Timer(5));
            }
        }
    }


    void Start()
    {
        isCollectable  = true;
        objectRenderer = GetComponent<Renderer>();
        material       = objectRenderer.material;
        table.Set();
    }
    private void Update()
    {
        PlayerSearch();
    }

    //시간 설정
    private IEnumerator Timer(int time)
    {
        isCollectable = false;
        material.SetColor("_EmissionColor", Color.yellow * 0);
        yield return new WaitForSeconds(time);
        isCollectable = true;
        CollectNumber = 3;
    }

    //근처에 플레이어가 있다면 빛을 내게 된다
    private void PlayerSearch()
    {
        if (isCollectable)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, 1 << LayerMask.NameToLayer("Player"));
            if (colliders.Length > 0)
            {
                StartCoroutine(Glow());
            }

         
        }
    }
    //빛을 내는 코루틴
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

    //실제 채집
    public void Interaction()
    {
        if(isCollectable)
        {
            Debug.Log(table.itemDic[table.Choose()]);
            WarehouseManager.instance.AddItem(ItemListManager.instance.materialsDic[table.itemDic[table.Choose()]]);
            CollectNumber--;
        }

    }

}
