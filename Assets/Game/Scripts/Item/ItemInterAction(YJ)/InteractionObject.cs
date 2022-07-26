using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class InteractionObject: MonoBehaviour, IInteraction
{

    [System.Serializable]
    public class ItemTable
    {
        [Tooltip("드랍 아이템의 이름")]
        public string[] name = new string[0];
        [Tooltip("드랍 아이템의 확률(마지막 아이템의 확률이 100이 되게 설정")]
        public int[] percent = new int[0];
        public Dictionary<int, string> itemDic = new Dictionary<int, string>();

        //확률과 아이템을 연결하는 함수
        public void Set()
        {
            for (int i = 0; i < name.Length; i++)
                itemDic.Add(percent[i], name[i]);
        }

        //채집을 시행할때마다 뽑는 함수
        public int Choose()
        {
            int randomPoint = Random.Range(0, 100);
            for (int i = 0; i < percent.Length; i++)
            {
                if (randomPoint < percent[i])
                    return percent[i];
            }
            return percent[percent.Length - 1];
        }
    }
    [SerializeField]
    private Texture2D objectImage;
    public string objectName;
    [HideInInspector]
    public Sprite imageSprite;


    [Tooltip("아이템 테이블")]
    public    ItemTable table;
    public    bool      isCollectable;
    [Tooltip ("채집 가능 횟수를 지정")]
    [SerializeField]
    protected int       collectNumberOrigin;
    protected int       collectNumber;
    public    int       CollectNumber
    {
        get { return collectNumber; }
        set
        {
            collectNumber = value;
            if (collectNumber == 0)
            {
                StartCoroutine(Timer(10));
            }
        }
    }


    //시간 설정
    public virtual IEnumerator Timer(int time)
    {
        isCollectable = false;
        yield return new WaitForSeconds(time);
        isCollectable = true;
        CollectNumber = collectNumberOrigin;
    }

    public virtual void Start()
    {
        table.Set();
        isCollectable = true;
        CollectNumber = collectNumberOrigin;
        imageSprite = objectImage.ToSprite();
    }
   
    //실제 채집
    public virtual void Interaction()
    {
        if (isCollectable)
        {
            string itemName = table.itemDic[table.Choose()];
            WarehouseManager.instance.itemDelegate(DataManager.instance.materialsDic[itemName]);
            CollectNumber--;
        }
    }

}
