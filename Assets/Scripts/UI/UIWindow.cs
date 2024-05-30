
using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public AudioClip bgm;

    public virtual void Open()
    {
        if (bgm != null)
            GameManager.PlayMusic(bgm);
        gameObject.SetActive(true);
        Refresh();
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
        SaveManager.GameSave();
    }

    public virtual void Refresh()
    {

    }
}