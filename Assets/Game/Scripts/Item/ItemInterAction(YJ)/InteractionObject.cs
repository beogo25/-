using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class InteractionObject: MonoBehaviour, IInteraction
{

    [System.Serializable]
    public class ItemTable
    {
        public string[] name = new string[0];
        public int[] percent = new int[0];
        public Dictionary<int, string> itemDic = new Dictionary<int, string>();
        public void Set()
        {
            for (int i = 0; i < name.Length; i++)
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
            return percent[percent.Length - 1];
        }
    }


    public    ItemTable       table;
    protected bool            isCollectable;
    public    int             collectNumber = 3;
    public    virtual int     CollectNumber
    {
        get { return collectNumber; }
        set { collectNumber = value; }
    }



    public virtual void Start()
    {
        table.Set();
        isCollectable = true;
    }
   
    //실제 채집
    public void Interaction()
    {
        if(isCollectable)
        {
            Debug.Log(table.itemDic[table.Choose()]);
            WarehouseManager.instance.itemDelegate(ItemListManager.instance.materialsDic[table.itemDic[table.Choose()]]);
            CollectNumber--;
        }
    }

}
