using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHit
{
    public void Hit();
}

//��ȣ�ۿ� �ϴ°�
public interface IInteraction
{
    public void Interaction();
}


//���� ���ǹ��� ������ �����ϱ� IGlow ���� �������ϴ�
public interface IGlow
{
    public IEnumerator Glow();
}