using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class MonoInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T s_instance = null;
    public static T Instance
    { 
        get
        {
            if( s_instance == null )
            {
                s_instance = GameObject.FindObjectOfType<T>();
            }

            return s_instance;
        }
    }

    protected virtual void Awake()
    {
        if( s_instance == null ) 
        {
            s_instance = this as T;
        }
    }

    protected virtual void OnDestroy()
    {
        ResetInstance();
    }

    public void ResetInstance()
    {
        s_instance = null;
	}
}
