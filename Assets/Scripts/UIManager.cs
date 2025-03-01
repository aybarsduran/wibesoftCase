using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject CropSelectionPanel;

    private void Awake()
    {
        Instance = this;
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
        CropSelectionPanel.SetActive(false);
    }
}
