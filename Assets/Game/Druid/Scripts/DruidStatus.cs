using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class DruidStatus : MonsterStatus, IInteraction
{
    [HideInInspector] public MONSTER_BEHAVIOR_STATE behaviorState = MONSTER_BEHAVIOR_STATE.SerchingTarget;
    [HideInInspector] public MONSTER_STATE state = MONSTER_STATE.Idle;
    [HideInInspector] public Collider target;
    private DruidAction[] druidAction = new DruidAction[2];

    // ����
    [SerializeField] private EventReference battleBGM;
    [SerializeField] private EventReference[] attackSound;
    [SerializeField] private EventReference[] hitSound;
    [SerializeField] private EventReference[] stateSound;
    [SerializeField] private EventReference walkSound;

    private Player player;
    public override float Hp
    {
        get { return currentHp; }
        set
        {
            if (value < currentHp)  // ü���� ���� ��쿡�� ����ǵ��� 
            {
                if (behaviorState != MONSTER_BEHAVIOR_STATE.InBattle)       // �÷��̾ �������ݽ� ������尡 �ƴϸ� �������
                {
                    Debug.Log("ü���� ��Ҵٸ� Ÿ����� ��������");
                    Collider[] targets = Physics.OverlapSphere(transform.position, 30f, 1 << LayerMask.NameToLayer("Character"));
                    for (int i = 0; i < targets.Length; i++)
                    {
                        target = targets[i];
                        druidAction[0].ChangeState(MONSTER_BEHAVIOR_STATE.InBattle);
                    }
                }
            }

            currentHp = value;
            Debug.Log("����̵� ��ü ü�� : " + currentHp);
            if (currentHp <= 0 && !state.HasFlag(MONSTER_STATE.Dead))       // ü���� 0���ϰ� �ǰ� �������°� �ƴϸ�
            {
                ((Druid_InBattle)druidAction[1]).Dead();
                monsterInterActionCollider.enabled = true;                  // ��ȣ�ۿ��� Collider �ѱ� (���� ��������)
                
                //Destroy(gameObject, 25f);
            }
        }
    }
    private void Awake()
    {
        druidAction[0] = GetComponent<Druid_SerchingTarget>();
        druidAction[1] = GetComponent<Druid_InBattle>();
        monsterInterActionCollider = GetComponent<CapsuleCollider>();
        player = FindObjectOfType<Player>();
        table.Set();
        monsterName = "����̵�";
        maxHp = 1000;
        atk = 10;
        currentHp = maxHp;
        collectNumber = collectNumberOrigin;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
         //   Debug.Log("walk sound ���");
            PlayWalkSound();
        }


        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("bgm sound ���");
            PlaybattleBGM();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            int radomValue = Random.Range(0, 2);
            Debug.Log(radomValue + " Hit sound");
            PlayHitSound(radomValue);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            int radomValue = Random.Range(0, 6);
            Debug.Log(radomValue + " Attack sound");
            PlayAttackSound(radomValue);
        }


    }

    // ���� ������ ����
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
    [SerializeField] private int collectNumberOrigin;
    [SerializeField] private ItemTable table;
    private int collectNumber; 

    public void Interaction()
    {
        StartCoroutine(InteractionCo());
    }

    private IEnumerator InteractionCo()
    {
        while (collectNumber > 0)
        {
            string itemName = table.itemDic[table.Choose()];
            WarehouseManager.instance.itemDelegate(DataManager.instance.materialsDic[itemName]);
            collectNumber--;
            yield return new WaitForSeconds(0.2f);
        }
        monsterInterActionCollider.enabled = false;
    }

    /*
        battleBGM;
        attackSound;
        hitSound;
        walkSound;

     */
    #region ���ͻ���
    public void PlayAttackSound(int num)
    {
        RuntimeManager.PlayOneShot(attackSound[num].Path);
    }
    public void PlaybattleBGM()
    {
        RuntimeManager.PlayOneShot(battleBGM.Path);
    }
    public void PlayRoarSound()
    {
        Debug.Log("�ξ� ���� ����" + state);
        if (!state.HasFlag(MONSTER_STATE.Attack))
        {
            
            RuntimeManager.PlayOneShot(attackSound[5].Path);
        }
    }

    public void PlayHitSound(int num)
    {
        RuntimeManager.PlayOneShot(hitSound[num].Path);
    }
    public void PlayWalkSound()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 65f)
        {
            Debug.Log("�ȱ� ���� ���, �Ÿ� : " + Vector3.Distance(player.transform.position, transform.position));
            RuntimeManager.PlayOneShot(walkSound.Path);
        }

    }    
public void PlayStateSound(int num)
    {
        RuntimeManager.PlayOneShot(stateSound[num].Path);
    }
    
    #endregion

}
