using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHit
{
    public void Hit();
}

//상호작용 하는거
public interface IInteraction
{
    public void Interaction();
}


//보스 유실물은 빛나지 않으니까 IGlow 따로 나눴습니다
public interface IGlow
{
    public IEnumerator Glow();
}