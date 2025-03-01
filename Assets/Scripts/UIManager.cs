using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public GameObject CropSelectionPanel;
    public GameObject CropTimerPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }
    private void Start()
    {
        HideCropSelectionPanel();
    }

    public void ShowCropSelectionPanel(Vector3 worldPosition)
    {
        CropSelectionPanel.SetActive(true);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        CropSelectionPanel.transform.position = screenPosition + new Vector3(-200f, 100f, 0f);
    }

    public void HideCropSelectionPanel()
    {
        if (CropSelectionPanel != null)
            CropSelectionPanel.SetActive(false);
    }
    public bool IsUIActive()
    {
        return CropSelectionPanel.activeSelf;
    }
    public void ShowCropTimerPanel(Vector3 worldPosition, float remainingTime)
    {
        CropTimerPanel.SetActive(true);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        CropTimerPanel.transform.position = screenPosition + new Vector3(-200f, 100f, 0f);

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
    }


}
