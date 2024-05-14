
using UnityEngine;

public class UIWindow : MonoBehaviour
{
    public virtual void Open()
    {
        gameObject.SetActive(true);
        Refresh();
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual void Refresh()
    {

    }
}