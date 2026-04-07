using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                var objects = FindObjectsByType<T>(FindObjectsSortMode.InstanceID);
                if (objects.Length > 0)
                {
                    foreach (var obj in objects)
                    {
                        if (_instance == null && obj != null)
                        {
                            _instance = objects[0];
                        }
                        else
                        {
                            Destroy(obj);
                        }
                    }
                }
                else
                {
                    var instanceHolder = new GameObject(typeof(T).Name);
                    _instance = instanceHolder.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    private Transform GetRootTransform(Transform targetTransform)
    {
        return targetTransform.parent == null ? targetTransform : GetRootTransform(targetTransform.parent);
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(GetRootTransform(transform));
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }
}