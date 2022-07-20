
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public  int            attackValue;
    public  static bool    isLockedOn = false;
                           
    private Animator       animator;
    public  static bool    isMoveAble = true;
    public  Transform      attackParticleParent;
    public  Transform      useParticleParent;
                           
    public  GameObject     lockOnPrefab;
    public  GameObject     lockOnObject = null;
                           
    public  bool           isGround = true;
    private float          jumpInputTime;
    private float          movementSpeed;
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
                           
    public  bool           talkState  = false;
    public  bool           isRollAble = true;
    public  bool           isCollectable = false;

    WaitForSecondsRealtime colorDelay = new WaitForSecondsRealtime(0.0005f);
    public Renderer        renderers;
    public Quest?          orderQuest = null;
    private PlayerStatus   status;

    void Start()
    {
        Player player      = this;
        status             = transform.parent.GetComponent<PlayerStatus>();
        characterMove      = transform.parent.GetComponent<CharacterMove>();
        movementSpeed      = transform.parent.GetComponent<CharacterMove>().movementSpeed;
        playerRigidbody = GetComponent<Rigidbody>();
        animator           = GetComponent<Animator>();
        attackDatas        = Resources.LoadAll<AttackData>("AttackData");

        animator.SetBool("Land", true);

        foreach (AttackData attack in attackDatas)
            combo.Insert(attack.attackCommand, attack.attackName, attack.attackDelay);
    }

    void Update()
    {
        if (!talkState)
            InputSetting();
        else
            animator.SetBool("Dash", false);

        CheckOnGround();



    }

    private void FixedUpdate()
    {
        if(!talkState)
            PlayerMove();
    }

    IEnumerator DashReset()
    {
        yield return new WaitForSeconds(0.1f);
        if (moveDir == Vector3.zero)
            animator.SetBool("Dash", false);
    }

    //�ִϸ����� Ʈ���� üũ
    IEnumerator TriggerCheck(string skillName)
    {
        if (skillName != "")
        {
            if (skillName == "Evade" || !animator.GetBool("Land")) { }      // ������� �����϶��� ���� Y�� ����
            else playerRigidbody.constraints |= RigidbodyConstraints.FreezePositionY;

            animator.SetTrigger(skillName);
            yield return new WaitForSeconds(0.3f);
            animator.ResetTrigger(skillName);
        }
        else
        { }
    }

    //ĳ���Ͱ� �޴� ��� ��ǲ
    private void InputSetting()
    {
        if (Input.GetKeyDown(KeyCode.B))
            StartCoroutine(HitDown(3));

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dash_Loop") || animator.GetCurrentAnimatorStateInfo(0).IsName("Jump_Start") || animator.GetCurrentAnimatorStateInfo(0).IsName("Jump_Loop"))
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


        if (Input.GetButtonDown("Evade") && isGround && !animator.GetCurrentAnimatorStateInfo(0).IsName("Evade") && status.Stamina>=10 && isRollAble)
            StartCoroutine(TriggerCheck("Evade"));


        if (Input.GetButtonDown("L Bumper"))
            StartCoroutine(cameraMove.CameraFocus());

        if (Input.GetButtonDown("R 3"))
            LockOn();
    }

    #region �ִϸ��̼� �̺�Ʈ
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
        playerRigidbody.constraints |= RigidbodyConstraints.FreezePositionY;
    }
    public void Attack_AirSlashEnd()
    {
        playerRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
        playerRigidbody.AddForce(Vector3.down * 13, ForceMode.Impulse);
    }
    // �ִϸ��̼ǿ��� ����մϴ�.
    IEnumerator AttackMove(float distance)
    {
        for (int i = 0; i < 50; i++)
        {
            transform.Translate(transform.forward * distance / 50, Space.World);
            yield return new WaitForSeconds(0.0001f);
            //if (Input.GetKey(KeyCode.S))  �Ƹ� ��Ƽ���� -1�϶� �ڷ� �Ȱ��� �ϸ� ��
            //    break;
        }
    }
    IEnumerator HitDown(float power)
    {
        //������ٵ� ���� ���� �׶���üũ �߰��ϸ� �����ؾ��մϴ�.
        //�������� ���󰡴� �κе� ����� ���Ͱ� üũ�� �ؼ� �ùٸ� �������� ���ư����� ��������
        animator.SetBool("Down", true);
        StartCoroutine(TriggerCheck("DownTrigger"));
        StartCoroutine(AttackMove(-power));
        yield return new WaitForSeconds(3f);
        animator.SetBool("Down", false);
    }
    public void ParticleInstantiate(GameObject attack)
    {
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
        StartCoroutine(combo.DelayCheck(0.2f));
        Vector3 rollDir = moveDir;
        if (rollDir == Vector3.zero)
        {
            for (int i = 0; i < 35; i++)
            {
                transform.position += transform.forward * movementSpeed * 0.02f;
                yield return new WaitForSeconds(0.01f);
            }

        }
        else
        {
            transform.forward = rollDir;
            for (int i = 0; i < 35; i++)
            {
                transform.position += rollDir * movementSpeed * 0.02f;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    #endregion �ִϸ��̼� �̺�Ʈ

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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Map"))
        {
            movementSpeed = 0;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Map"))
        {
            movementSpeed = transform.parent.GetComponent<CharacterMove>().movementSpeed;
        }
    }

    public void StaminaDecrease(int value)
    {
        status.Stamina -= value;
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


        //������ 0�϶� Ű �Է� �ʱ�ȭ + ��� �ʱ�ȭ
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
            return; // ����Ű ���������� 0.2�ʰ� ������� (����ĳ��Ʈ�� �ݶ��̴����� ��⶧����)

        RaycastHit ground;
        Physics.Raycast(transform.position + (transform.up * 0.1f), Vector3.down, out ground, 20f, 1 << LayerMask.NameToLayer("Ground"));

        if (ground.distance > 0 && ground.distance <= 0.3f)
        {
            if (!isGround)  // ���� �ƴϿ��ٰ� ���� ������ "�ѹ�"�� ����ǵ���
            {
                playerRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
                playerRigidbody.velocity = Vector3.zero;
                animator.SetBool("Jump", false);
                animator.SetBool("Land", true);
            }
            isGround = true;
        }
        else if (ground.distance == 0 || ground.distance > 2f)   // ���� ���ų� ������ �Ÿ��� 1f �̻��϶�
        {
            if (isGround)   // ���� ���ٰ� �����϶� "�ѹ�"�� ����ǵ���
            {
                animator.SetBool("Land", false);    // �������� ����
            }
            isGround = false;
        }
    }
    
    //������ ���� ��ƸԾ ���� ������ ���� 1����
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

