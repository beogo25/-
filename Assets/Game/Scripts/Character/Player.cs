
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;

public class Player : MonoBehaviour
{
    public  int            attackValue;
    public  static bool    isLockedOn = false;
                           
    public  Animator       animator;
    public  static bool    isMoveAble = true;
    public  Transform      attackParticleParent;
    public  Transform      useParticleParent;
                           
    public  GameObject     lockOnPrefab;
    public  GameObject     lockOnObject = null;
                           
    public  bool           isGround = true;
    private float          jumpInputTime;
    private float          backupSpeed;
    private CharacterMove  characterMove;
    private Rigidbody      playerRigidbody;
    
    [SerializeField]       
    private ComboSystem    combo;
    [SerializeField]       
    private CameraMove     cameraMove;
                           
    [SerializeField]       
    private AttackData[]   attackDatas;
                           
    private Vector3        moveDir;
    private bool           isClickAble;
                           
    public  bool           talkState     = false;
    public  static bool    isRollAble    = true;
    public  bool           isCollectable = false;

    WaitForSecondsRealtime colorDelay    = new WaitForSecondsRealtime(0.0005f);
    public  Renderer       renderers;
    public  Quest?         orderQuest    = null;
    public  PlayerStatus   status;

    //[SerializeField]
    //private GameObject     bigSizeMap;
    //[SerializeField]
    //private GameObject     miniMap;

    public  event Action   AttackStartDelegate;
    public   Action        rollDelegate;

    [SerializeField]
    private EventReference soundEvent;


    public bool TalkState
    {
        get { return talkState; }
        set 
        {
            talkState = value;
            if(talkState)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
              //  miniMap.SetActive(false);
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
               // miniMap.SetActive(true);
            }
        }
    }
    void Start()
    {
        Player player      = this;
        status             = transform.parent.GetComponent<PlayerStatus >();
        characterMove      = transform.parent.GetComponent<CharacterMove>();
        backupSpeed        = transform.parent.GetComponent<CharacterMove>().movementSpeed;
        playerRigidbody    = GetComponent<Rigidbody>();
        animator           = GetComponent<Animator >();
        attackDatas        = Resources.LoadAll<AttackData>("AttackData");

        animator.SetBool("Land", true);

        foreach (AttackData attack in attackDatas)
            combo.Insert(attack.attackCommand, attack.attackName, attack.attackDelay);
    }

    void Update()
    {
        if (!TalkState)
            InputSetting();
        else
            animator.SetBool("Dash", false);

        CheckOnGround();

        if(Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("isGround : " + isGround + ", isMoveAble : " + isMoveAble);
        }

    }

    private void FixedUpdate()
    {
        if(!TalkState)
            PlayerMove();
    }
    public void PlaySound(int num)
    {
        FMOD.Studio.EventInstance eventInstance = RuntimeManager.CreateInstance(soundEvent.Path);
        eventInstance.setParameterByName("Attack", num);
        eventInstance.start();
        eventInstance.release();
    }

    IEnumerator DashReset()
    {
        yield return new WaitForSeconds(0.1f);
        if (moveDir == Vector3.zero)
            animator.SetBool("Dash", false);
    }

    //애니메이터 트리거 체크
    IEnumerator TriggerCheck(string skillName)
    {
        if (skillName != "")
        {
            if (skillName == "Evade" || !animator.GetBool("Land")) { }      
            else playerRigidbody.constraints |= RigidbodyConstraints.FreezePositionY;

            animator.SetTrigger(skillName);
            yield return new WaitForSeconds(0.3f);
            animator.ResetTrigger(skillName);
        }
        else
        { }
    }

    //캐릭터가 받는 모든 인풋
    private void InputSetting()
    {
        //임시
        if (Input.GetKeyDown(KeyCode.B))
        {
            status.PlayerHit(1, 0, Vector3.zero, AttackType.BURN);
            status.PlayerHit(1, 0, Vector3.zero, AttackType.POISON);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dash_Loop") || animator.GetCurrentAnimatorStateInfo(0).IsName("Jump_Start") 
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Jump_Loop") || animator.GetCurrentAnimatorStateInfo(0).IsName("standing"))
            isMoveAble = true;
        else
            isMoveAble = false;

        if (Input.GetButtonDown("Fire1"))
            StartCoroutine(TriggerCheck(combo.Input(EnumKey.PUNCH, Time.time)));

        if (Input.GetButtonDown("Fire2") && !isCollectable)
            StartCoroutine(TriggerCheck(combo.Input(EnumKey.POWERPUNCH, Time.time)));

        if ((Input.GetButtonDown("Jump") || Input.GetAxis("Jump") > 0) && isGround)
        {
            jumpInputTime = Time.time;
            isGround = false;
            animator.SetBool("Jump", true);
            animator.SetBool("Land", false);
        }


        if (Input.GetButtonDown("Evade") && isGround && !animator.GetCurrentAnimatorStateInfo(0).IsName("Evade") && status.Stamina>=10 && isRollAble == true)
            StartCoroutine(TriggerCheck("Evade"));


        if (Input.GetButtonDown("L Bumper"))
            StartCoroutine(cameraMove.CameraFocus());

        if (Input.GetButtonDown("R 3"))
            LockOn();

       // if(Input.GetButtonDown("Select"))
            //bigSizeMap.SetActive(true);
    }

    #region 애니메이션 이벤트
    public void Jump()
    {
        characterMove.Jump();
    }

    public void DropKick()
    {
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.AddForce((transform.forward * 10) + (transform.up * -15), ForceMode.Impulse);
    }
    public void Attack_AirSlashStart()
    {
        playerRigidbody.constraints |=  RigidbodyConstraints.FreezePositionY | 
                                        RigidbodyConstraints.FreezePositionX |
                                        RigidbodyConstraints.FreezePositionZ;
    }
    public void Attack_AirSlashEnd()
    {
        playerRigidbody.constraints &=  ~RigidbodyConstraints.FreezePositionY &
                                        ~RigidbodyConstraints.FreezePositionX &
                                        ~RigidbodyConstraints.FreezePositionZ;

        playerRigidbody.AddForce(Vector3.down * 13, ForceMode.Impulse);
    }

    // 애니메이션에서 사용합니다.
    IEnumerator AttackMove(float distance)
    {
        for (int i = 0; i < 50; i++)
        {
            transform.Translate(transform.forward * distance / 50, Space.World);
            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator HitDown(float power)
    {
        //리지드바디 넣은 다음 그라운드체크 추가하면 수정해야합니다.
        //힘에따라 날라가는 부분도 상대방과 벡터값 체크를 해서 올바른 방향으로 날아가도록 만들어야함
        animator.SetBool("Down", true);
        StartCoroutine(TriggerCheck("DownTrigger"));
        StartCoroutine(AttackMove(-power));
        yield return new WaitForSeconds(3f);
        animator.SetBool("Down", false);
    }
    public void AttackStart()
    {
        //AttackStartDelegate();
    }
    public void ParticleInstantiate(GameObject attack)
    {
        Debug.Log(attack);
        GameObject temp = attackParticleParent.Find(attack.name).gameObject;
        if (temp != null)
        {
            temp.transform.position = transform.position;
            temp.transform.rotation = transform.rotation;
            temp.SetActive(true);
        }
    }

    IEnumerator Roll()
    {
        rollDelegate();
        StartCoroutine(combo.DelayCheck(0.2f));
        Vector3 rollDir = moveDir;
        float rollSpeed = 0.045f;
        if (rollDir == Vector3.zero)
        {
            for (int i = 0; i < 30; i++)
            {
                transform.position += transform.forward * characterMove.movementSpeed * rollSpeed;
                rollSpeed -= 0.0005f;
                yield return new WaitForFixedUpdate();
            }
            if (animator.GetBool("Dash"))
            {
                for (int i = 0; i < 20; i++)
                {
                    transform.position += transform.forward * characterMove.movementSpeed * rollSpeed;
                    rollSpeed -= 0.0005f;
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        else
        {
            transform.forward = rollDir;
            for (int i = 0; i < 30; i++)
            {
                transform.position += rollDir * characterMove.movementSpeed * rollSpeed;
                rollSpeed -= 0.0005f;
                yield return new WaitForFixedUpdate();
            }
            if (animator.GetBool("Dash"))
            {
                for (int i = 0; i < 20; i++)
                {
                    transform.position += transform.forward * characterMove.movementSpeed * rollSpeed;
                    rollSpeed -= 0.0005f;
                    yield return new WaitForFixedUpdate();
                }
            }
        }
    }
    #endregion 애니메이션 이벤트

    private void LockOn()
    {
        if (isLockedOn)
        {
            Destroy(lockOnObject);
            isLockedOn = false;
        }
        else
        {
            Collider[] tempColliders = Physics.OverlapSphere(transform.position, 40, 1 << LayerMask.NameToLayer("LockOn"));
            if (tempColliders.Length > 0)
            {
                if (lockOnObject == null)
                {
                    lockOnObject = Instantiate(lockOnPrefab, new Vector3(tempColliders[0].transform.position.x, tempColliders[0].transform.position.y, 17), Quaternion.Euler(Vector3.zero));
                    lockOnObject.GetComponent<LockOn>().targetTransform = tempColliders[0].transform;
                    isLockedOn = true;
                }
                else
                {
                    Destroy(lockOnObject);
                    lockOnObject = Instantiate(lockOnPrefab, new Vector3(tempColliders[0].transform.position.x, tempColliders[0].transform.position.y, 17), Quaternion.Euler(Vector3.zero));
                    lockOnObject.GetComponent<LockOn>().targetTransform = tempColliders[0].transform;
                    isLockedOn = false;
                }
            }
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Fence"))
        {
            characterMove.movementSpeed = 3;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Fence"))
        {
            characterMove.movementSpeed = backupSpeed;
        }
    }
    private void PlayerMove()
    {
        moveDir = new Vector3().GetDirectionByCameraRaw(Camera.main);

        if (transform.forward == moveDir)
            if (isClickAble == true)
            {
                StartCoroutine(TriggerCheck(combo.Input(EnumKey.FRONT, Time.time)));
                isClickAble = false;
            }
        if (transform.forward == -moveDir)
            if (isClickAble == true)
            {
                StartCoroutine(TriggerCheck(combo.Input(EnumKey.BACK, Time.time)));
                isClickAble = false;
            }
        //포지션 0일때 키 입력 초기화 + 모션 초기화
        if (moveDir == Vector3.zero)
        {
            isClickAble = true;
            if (animator.GetBool("Dash"))
                StartCoroutine(DashReset());
        }
        else
            animator.SetBool("Dash", true);
    }

    private void CheckOnGround()
    {
        if (Time.time < jumpInputTime + 0.2f)
            return; // 점프키 눌렀을때는 0.2초간 실행금지 (레이캐스트가 콜라이더보다 길기때문에)

        RaycastHit ground;
        Physics.Raycast(transform.position + (transform.up * 0.5f), Vector3.down, out ground, 20f, 1 << LayerMask.NameToLayer("Ground"));

        if (ground.distance > 0 && ground.distance <= 1f)           // 땅과의 거리가 1f 이하일때 땅에 닿고 있는걸로 취급
        {
            if (!isGround)  // 땅이 아니였다가 땅에 닿을시 "한번"만 실행되도록
            {
                playerRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
                playerRigidbody.velocity = Vector3.zero;
                animator.SetBool("Jump", false);
                animator.SetBool("Land", true);
            }
            isGround = true;
        }
        else if (ground.distance == 0 || ground.distance > 4f)      // 땅이 없거나 땅과의 거리가 4f 이상일때 공중으로 취급
        {
            if (isGround)   // 땅이 였다가 공중일때 "한번"만 실행되도록
            {
                animator.SetBool("Land", false);    // 떨어지는 상태
            }
            isGround = false;
        }
    }
    
    //성능에 문제 생길시 삭제 1순위
    public IEnumerator RimLight(Color color)
    {
        for (int i = 0; i < renderers.materials.Length; i++)
            renderers.materials[i].SetColor("_RimLightColor", color);
        Color changeColor = color;
        for (int i = 0; i < 15; i++) 
        {
            changeColor = new Color(changeColor.r, changeColor.g, changeColor.b, changeColor.a + 0.08f);
            for (int j = 0; j < renderers.materials.Length; j++)
                renderers.materials[j].SetColor("_RimLightColor", changeColor);
            yield return colorDelay;
        }
        for (int i = 0; i < 15; i++)
        {
            changeColor = new Color(changeColor.r, changeColor.g, changeColor.b, changeColor.a - 0.08f);
            for (int j = 0; j < renderers.materials.Length; j++)
                renderers.materials[j].SetColor("_RimLightColor", changeColor);
            yield return colorDelay;
        }
    }
   
}

