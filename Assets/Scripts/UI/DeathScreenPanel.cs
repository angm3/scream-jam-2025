using UnityEngine;
using UnityEngine.UI;

public class DeathScreenPanel : MonoBehaviour
{

    [SerializeField] private Button backToGarageBtn;
    [SerializeField] private Button exitGameBtn;

    void Start()
    {
        backToGarageBtn.onClick.AddListener(OnGarageBtnClick);
        exitGameBtn.onClick.AddListener(OnExitGameBtnClick);
    }

    private void OnGarageBtnClick()
    {
        // trigger load to garage
        SceneController.Instance.LoadScene("Garage", includeUI: true);
        UIManager.Instance.SetDeathScreenPanelInactive();
    }

    private void OnExitGameBtnClick()
    {
        // exit game
        GameManager.Instance.EndGame();
    }
}
