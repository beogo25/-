using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class InteractionObject: MonoBehaviour, IInteraction
{

    [System.Serializable]
    public class ItemTable
    {
        [Tooltip("��� �������� �̸�")]
        public string[] name = new string[0];
        [Tooltip("��� �������� Ȯ��(������ �������� Ȯ���� 100�� �ǰ� ����")]
        public int[] percent = new int[0];
        public Dictionary<int, string> itemDic = new Dictionary<int, string>();

        //Ȯ���� �������� �����ϴ� �Լ�
        public void Set()
        {
            for (int i = 0; i < name.Length; i++)
                itemDic.Add(percent[i], name[i]);
        }

        //ä���� �����Ҷ����� �̴� �Լ�
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


    [Tooltip("������ ���̺�")]
    public    ItemTable table;
    public    bool      isCollectable;
    [Tooltip ("ä�� ���� Ƚ���� ����")]
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


    //�ð� ����
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
   
    //���� ä��
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
