using System.Collections.Generic;
using System.Collections;
using UnityEngine;


/// <summary>콤보를 관리하는 클래스입니다.</summary>
public class ComboSystem : MonoBehaviour
{
    /// <summary>콤보의 내용이 들어갈 노드입니다.</summary>
    private class TrieNode
    {
        /// <summary>이 노드가 가리키고 있는 키입니다.</summary>
        EnumKey currentKey;
        /// <summary>이 노드에 할당된 기능이 있다면, 표시합니다.</summary>
        public string currentName = "";
        public float delay = 0;
        

        /// <summary>이 노드의 상위 노드입니다.</summary>
        TrieNode parent = null;
        /// <summary>이 노드의 하위 노드 리스트입니다.</summary>
        List<TrieNode> childList = new List<TrieNode>();

        /// <summary>입력 키를 사용해서 새로운 노드를 추가합니다.</summary>
        public TrieNode(EnumKey wantKey)
        {
            currentKey = wantKey;
        }
        public TrieNode(EnumKey wantKey, float inValue)
        {
            currentKey = wantKey;
            delay = inValue;
        }

        /// <summary>하위 노드 중에서 해당하는 키를 가진 노드를 찾습니다.</summary>
        public TrieNode FindChild(EnumKey wantKey)
        {
            for (int i = 0; i < childList.Count; ++i)
            {
                if (childList[i].currentKey == wantKey)
                {
                    return childList[i];
                };
            };

            return null;
        }

        /// <summary>해당하는 키를 가진 하위 노드를 만듭니다.</summary>
        public TrieNode NewChild(EnumKey wantKey, float inDelay)
        {
            TrieNode returnNode = FindChild(wantKey);

            if (returnNode == null)
            {
                returnNode = new TrieNode(wantKey, inDelay);
                returnNode.parent = this;
                childList.Add(returnNode);
            };

            return returnNode;
        }

        /// <summary>해당하는 배열과 순서가 일치하는 마지막 노드를 찾습니다.</summary>
        public TrieNode Find(EnumKey[] wantArray, int index)
        {
            //내 다음 위치를 찾기 위해 1을 더합니다.
            ++index;

            //다음 위치가 아직 남은 경우
            if (index < wantArray.Length)
            {
                //다음 위치에 해당하는 키를 가진 하위 노드를 찾습니다.
                TrieNode nextChild = FindChild(wantArray[index]);

                //있다면, 그 노드에게 나머지 탐색을 요청합니다.
                if (nextChild != null)
                {
                    return nextChild.Find(wantArray, index);
                }
                //없다면, null을 반환합니다.
                else
                {
                    return null;
                };
            }
            //다음 위치가 없는 경우
            else
            {
                //마지막 키가 본인과 같으면, 본인이라고 알리고 아니면 null을 반환합니다.
                if (wantArray[wantArray.Length - 1] == currentKey)
                {
                    return this;
                }
                else
                {
                    return null;
                };
            };
        }

        /// <summary>해당하는 배열의 순서에 맞춰 새로운 노드들을 만듭니다.</summary>
        public void Insert(EnumKey[] wantArray, int index, string wantName, float inDelay)
        {
            //내 다음 위치를 찾기 위해 1을 더합니다.
            ++index;

            //다음 위치가 존재하는 경우
            if (index < wantArray.Length)
            {
                //다음 위치에 해당하는 키를 가진 하위 노드를 찾고, 없으면 새로 만듭니다.
                TrieNode nextChild = NewChild(wantArray[index], 0);

                //다음 위치의 하위 노드에게 계속 생성해달라고 요청합니다.
                nextChild.Insert(wantArray, index, wantName, inDelay);
            }
            //다음 위치가 없다면, 여기가 대상이므로 스킬이 없었을 때 새로운 스킬을 받습니다.
            else if (currentName == "")
            {
                currentName = wantName;
                delay = inDelay;
            };
        }

        /// <summary>해당 노드를 삭제합니다.</summary>
        public void Delete()
        {
            //스킬을 비웁니다.
            currentName = "";

            //하위 노드를 가지고 있지 않았을 때에만 실행합니다.
            if (childList.Count <= 0)
            {
                //상위 노드가 있는 경우
                if (parent != null)
                {
                    //상위 노드가 가진 하위 노드가 이것뿐이라면, 그 노드도 삭제합니다.
                    if (parent.childList.Count <= 1 && parent.currentName == "")
                    {
                        parent.Delete();
                    }
                    //다른 하위 노드가 있었다면, 이 노드만 빼달라고 요청합니다.
                    else
                    {
                        parent.childList.Remove(this);
                    };
                    //상위 노드와 연결을 끊습니다.
                    parent = null;
                };

                //하위 노드와 연결을 끊습니다.
                childList.Clear();
            };
        }

        /// <summary>해당 노드에 이어진 모든 하위 노드들을 삭제합니다.</summary>
        public void Destroy()
        {
            if (parent != null)
            {
                //하위 노드들에게 모두 삭제 요청을 돌립니다.
                for (int i = 0; i < childList.Count; ++i)
                {
                    childList[i].Destroy();
                };

                //상위, 하위 노드의 연결을 해제합니다.
                parent = null;
                childList.Clear();
            };
        }

        /// <summary>현재 노드가 가지고 있는 키를 가져옵니다.</summary>
        public EnumKey GetKey()
        {
            return currentKey;
        }
    }

    /// <summary>트라이 노드의 시작점(root)입니다.</summary>
    private TrieNode rootTrie = new TrieNode(0);
    /// <summary>마지막으로 입력한 키의 노드입니다.</summary>
    private TrieNode lastChecked = null;
    /// <summary>이 클래스가 기다려주는 한계 시간입니다.</summary>
    private float expiredTime = 0.0f;
    /// <summary>입력을 받은 후, 다음 연계까지 몇 초의 시간을 기다려줄지 설정합니다.</summary>
    private float limitInputTime = 0.6f;

    public bool attackAble = true;

    Animator animator;
    Rigidbody characterRigidbody;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterRigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>유저가 키를 입력했을 때 실행하는 함수입니다.</summary>
    /// <param name="inputKey">유저가 입력한 키를 넣습니다.</param>
    /// <param name="inputTime">유저가 입력한 시간을 넣습니다.</param>
    public string Input(EnumKey inputKey, float inputTime)
    {
        //기다려주는 시간보다 입력한 시간이 더 빠르면
        if (expiredTime >= inputTime)
        {
            //직전에 확인한 노드가 있었는지 봅니다.
            if (lastChecked != null)
            {
                //있었다면, 하위 노드 중에 새로 입력된 키로 옮겨갑니다.
                lastChecked = lastChecked.FindChild(inputKey);
            };
        }
        //늦게 입력되었으면, 직전 노드를 비웁니다.
        else
        {   
            lastChecked = null;
        };

        //기다려주는 시간을 현재시간 + 한계시간으로 조정합니다.
        expiredTime = inputTime + limitInputTime;

        //확인할 노드가 없다면
        if (lastChecked == null)
        {
            //최상단 노드들 중에서 해당하는 키를 확인하도록 합니다.
            lastChecked = FindNodeStart(inputKey);
        };

        //모든 확인을 끝낸 뒤에 도출된 노드가 있다면
        if (lastChecked != null && attackAble)
        {
            //해당 노드의 이름을 받아옵니다.
            StartCoroutine(DelayCheck(lastChecked.delay));
            return lastChecked.currentName;
        }
        //최상단 노드에도 해당하는 키가 없었다면, 빈 문자열을 반환합니다.
        else
        {
            return "";
        };
    }
    public IEnumerator DelayCheck(float value)
    {
        attackAble = false;
        animator.SetBool("EvadeBool",false);
        yield return new WaitForSeconds(value);
        attackAble = true;
        animator.SetBool("EvadeBool", true);

        if (animator.GetBool("Land"))    // 땅일때만 Y축고정 해제 ()
            characterRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
    }

    /// <summary>한계 대기 시간을 다시 조정합니다.</summary>
    public void SetTimeLimit(float wantTime)
    {
        limitInputTime = wantTime;
    }
    public float GetTimeLimit()
    {
        return limitInputTime;
    }

    /// <summary>다음 연계까지 기다려주는 시간을 직접 정합니다.</summary>
    public void SetExpireTime(float wantTime)
    {
        expiredTime = wantTime;
    }
    public float GetExpireTime()
    {
        return expiredTime;
    }

    /// <summary>해당 배열의 순서에 맞게 스킬을 넣습니다. 이미 그 자리에 스킬이 있다면, 실패합니다.</summary>
    public void Insert(EnumKey[] wantArray, string wantName, float inDelay)
    {
        //첫 번째 진입 노드를 찾습니다. 없으면 새로 만듭니다.
        TrieNode targetNode = rootTrie.NewChild(wantArray[0], inDelay);

        //진입 노드에 생성 요청을 전달합니다.
        targetNode.Insert(wantArray, 0, wantName, inDelay);
    }

    /// <summary>해당 배열과 순서가 같은 스킬이 있다면, 해당 스킬을 제거합니다.</summary>
    public void Delete(EnumKey[] wantArray)
    {
        //해당하는 스킬을 찾습니다.
        TrieNode targetNode = FindNodeEnd(wantArray);

        //있으면 제거합니다.
        if (targetNode != null)
        {
            targetNode.Delete();
        };
    }

    /// <summary>해당 배열과 순서가 같은 스킬이 있다면, 그 스킬의 이름을 바꿉니다.</summary>
    public void ChangeName(EnumKey[] wantArray, string wantName)
    {
        //해당하는 스킬을 찾습니다.
        TrieNode targetNode = FindNodeEnd(wantArray);

        //있으면 이름을 바꿉니다.
        if (targetNode != null)
        {
            targetNode.currentName = wantName;
        };
    }

    /// <summary>최상위 노드 중에서 원하는 키를 가진 노드를 찾습니다.</summary>
    private TrieNode FindNodeStart(EnumKey wantKey)
    {
        return rootTrie.FindChild(wantKey);
    }

    /// <summary>해당 배열의 순서와 같은 최하위 노드를 찾습니다.</summary>
    private TrieNode FindNodeEnd(EnumKey[] wantArray)
    {
        //진입 노드를 찾습니다.
        TrieNode targetNode = FindNodeStart(wantArray[0]);

        //진입 노드가 존재한다면
        if (targetNode != null)
        {
            //찾기 요청을 전달합니다.
            targetNode = targetNode.Find(wantArray, 0);

            //찾은 노드가 있다면 해당 노드를 반환합니다.
            if (targetNode != null)
            {
                return targetNode;
            };
        };

        //진입 노드가 없다면 그냥 null을 반환합니다.
        return null;
    }

}
