using UnityEngine;
using UnityEngine.UI;

public class NetworkUIManager : MonoBehaviour
{
    [SerializeField] private Button createHostBtn;
    
    [SerializeField] private SteamManager steamManager;

    private void Awake()
    {
        createHostBtn.onClick.AddListener(() =>
        {
            steamManager.StartHost();
        });
    }
}