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

    // 사운드
    [SerializeField] private EventReference battleBGM;
    [SerializeField] private EventReference[] attackSound;
    [SerializeField] private EventReference[] hitSound;
    [SerializeField] private EventReference[] stateSound;
    [SerializeField] private EventReference walkSound;
    private FMOD.Studio.EventInstance soundInstance;

    private Player player;
    public override float Hp
    {
        get { return currentHp; }
        set
        {
            if (value < currentHp)  // 체력이 닳은 경우에만 실행되도록 
            {
                if (behaviorState != MONSTER_BEHAVIOR_STATE.InBattle)       // 플레이어가 선제공격시 전투모드가 아니면 전투모드
                {
                    Collider[] targets = Physics.OverlapSphere(transform.position, 30f, 1 << LayerMask.NameToLayer("Character"));
                    for (int i = 0; i < targets.Length; i++)
                    {
                        target = targets[i];
                        druidAction[0].ChangeState(MONSTER_BEHAVIOR_STATE.InBattle);
                    }
                }
            }

            currentHp = value;
            Debug.Log("드루이드 전체 체력 : " + currentHp);
            if (currentHp <= 0 && !state.HasFlag(MONSTER_STATE.Dead))       // 체력이 0이하가 되고 죽은상태가 아니면
            {
                ((Druid_InBattle)druidAction[1]).Dead();
                QuestSuccess();
                StartCoroutine(OFFBattleBGM());
                monsterInterActionCollider.enabled = true;                  // 상호작용할 Collider 켜기 (몬스터 갈무리용)
                
                Destroy(gameObject, 25f);
            }
        }
    }
    public void QuestSuccess()
    {
        if (player.orderQuest != null)
        {
            Quest quest = player.orderQuest ?? new Quest();
            if (quest.target == monsterName)
            {
                if (player.questCount > 0)
                    player.questCount--;
            }
        }
    }
    private void Awake()
    {
        druidAction[0] = GetComponent<Druid_SerchingTarget>();
        druidAction[1] = GetComponent<Druid_InBattle>();
        soundInstance = RuntimeManager.CreateInstance(battleBGM.Path);
        player = FindObjectOfType<Player>();
        
        table.Set();
        monsterName = "사슴";
        maxHp = 1000;
        atk = 10;
        currentHp = maxHp;
        collectNumber = collectNumberOrigin;
    }

    // 몬스터 갈무리 관련
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

    #region 몬스터사운드
    IEnumerator OFFBattleBGM()
    {

        yield return new WaitForSecondsRealtime(0.5f);
        soundInstance.setPaused(true);
    }

    public void PlayAttackSound(int num)
    {
        RuntimeManager.PlayOneShot(attackSound[num].Path);
    }
    public void PlaybattleBGM()
    {
        soundInstance.start();
        soundInstance.release();
    }
    public void PlayRoarSound()
    {
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
            RuntimeManager.PlayOneShot(walkSound.Path);
        }

    }    
public void PlayStateSound(int num)
    {
        RuntimeManager.PlayOneShot(stateSound[num].Path);
    }
    
    #endregion

}
