using UnityEngine;
using UnityEngine.UI;

public class UILoadScene : MonoBehaviour
{
    private ResourceRequest test;
    public Slider slider;

    void Start()
    {
        test = Resources.LoadAsync("");
        test.completed += (x) => { GameManager.Instance.ChangeScene(GameManager.Instance.nextScene); };
    }

    private void Update()
    {
        slider.value = test.progress;
    }
}
