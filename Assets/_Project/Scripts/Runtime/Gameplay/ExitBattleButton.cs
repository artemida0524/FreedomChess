using UnityEngine;
using UnityEngine.UI;

public class ExitBattleButton : MonoBehaviour
{

    [SerializeField] private ExitBattlePanel panel;
    [SerializeField] private Button button;
    [SerializeField] private GameObject buttonView;


    public void Init()
    {
        button.onClick.AddListener(() => panel.SetActive(true));
    }


    public void SetActive(bool active)
    {
        buttonView.SetActive(active);
    }

}
