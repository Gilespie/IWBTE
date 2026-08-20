using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
{
    public static T Instance { get; private set; }

    [SerializeField] bool _isDontDestroyOnLoaded = false;

    protected virtual void Awake()
    {
        if(!Instance)
        {
            Instance = (T)this;

            if (_isDontDestroyOnLoaded) 
                DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}