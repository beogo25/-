using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    
public static class Function
{
    //! ������Ʈ�� Ž���Ѵ�.
    public static T FindComponent<T>(string objectName) where T : Component
    {
        var tempGameObject = GameObject.Find(objectName);

        return tempGameObject?.GetComponent<T>();
    }

    //! ������Ʈ�� �߰��Ѵ�.
    public static T AddComponent<T>(GameObject objectGameObject) where T : Component
    {
        var tempComponent = objectGameObject.GetComponent<T>();

        if (tempComponent == null)
        {
            tempComponent = objectGameObject.AddComponent<T>();
        }

        return tempComponent;
    }

    
}
