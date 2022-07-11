using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_oInstance = null;

    public static T Instance
    {
        get
        {
            if (m_oInstance == null)
            {
                var oGameObject = new GameObject(typeof(T).ToString());
                m_oInstance = Function.AddComponent<T>(oGameObject);

                DontDestroyOnLoad(oGameObject);
            }

            return m_oInstance;
        }
    }

    //인스턴스 생성

    public static T create()
    {
        return Singleton<T>.Instance;
    }
}
