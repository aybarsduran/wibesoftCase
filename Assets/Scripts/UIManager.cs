using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Crop Selection UI")]
    public GameObject CropSelectionPanel;

    [Header("Crop Timer UI")]
    public GameObject CropTimerPanel;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI NameText;
    public Image progressBar;

    [Header("Harvest UI")]
    public GameObject HarvestPanel;

    [Header("Building UI")]
    public Button BuildingButton;
    public Button BuildingPanelCloseButton;
    public RectTransform BuildingPanel;

    [Header("Building UI Elements")]
    public Button HouseButton;
    public BuildingData HouseBuilding;

    public Button TowerButton;
    public BuildingData TowerBuilding;



    private Coroutine _timerCoroutine;
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
        CropSelectionPanel.transform.localScale = Vector3.zero;
        CropTimerPanel.transform.localScale = Vector3.zero;
        HarvestPanel.transform.localScale = Vector3.zero;
        HideAllPanels();
        BuildingPanel.localScale = new Vector3(0, 1, 1);
        BuildingPanel.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        BuildingButton.onClick.AddListener(ShowBuildingPanel);
        BuildingPanelCloseButton.onClick.AddListener(HideBuildingPanel);

        HouseButton.onClick.AddListener(() => OnBuildingButtonClick(HouseBuilding));
        TowerButton.onClick.AddListener(() => OnBuildingButtonClick(TowerBuilding));

    }
    private void OnDisable()
    {
        BuildingButton.onClick.RemoveListener(ShowBuildingPanel);
        BuildingPanelCloseButton.onClick.RemoveListener(HideBuildingPanel);

        HouseButton.onClick.RemoveListener(() => OnBuildingButtonClick(HouseBuilding));
        TowerButton.onClick.RemoveListener(() => OnBuildingButtonClick(TowerBuilding));
    }

    public void ShowCropSelectionPanel(Vector3 worldPosition)
    {
        HideAllPanels();

        CropSelectionPanel.SetActive(true);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        ShowPanelWithEffect(CropSelectionPanel, screenPosition + new Vector3(-200f, 100f, 0f));
    }


    public bool IsUIActive()
    {
        return CropSelectionPanel.activeSelf || CropTimerPanel.activeSelf || HarvestPanel.activeSelf;
    }
    public void ShowCropTimerPanel(Vector3 worldPosition, BaseCrop crop)
    {
        if (crop == null || crop.IsFullyGrown()) return;
        HideAllPanels();

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        ShowPanelWithEffect(CropTimerPanel, screenPosition + new Vector3(0f, -150f, 0f));

        NameText.text = crop.CropName;

        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }
        _timerCoroutine = StartCoroutine(UpdateCropTimer(crop));
    }

    private IEnumerator UpdateCropTimer(BaseCrop crop)
    {
        float totalTime = crop.GrowthTime;
        float endTime = crop.GetGrowthEndTime();

        while (Time.time < endTime)
        {
            float remainingTime = endTime - Time.time;

            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            TimerText.text = $"{minutes}m {seconds}s";

            progressBar.fillAmount = 1f - (remainingTime / totalTime);

            yield return new WaitForSeconds(1f);
        }

        HideCropTimerPanel();
    }
    public void ShowHarvestPanel(Vector3 worldPosition)
    {
        HideAllPanels();


        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        ShowPanelWithEffect(HarvestPanel, screenPosition + new Vector3(-200f, 100f, 0f));
    }

    public void HideCropSelectionPanel() => CropSelectionPanel.SetActive(false);
    public void HideHarvestPanel() => HarvestPanel.SetActive(false);

    public void HideCropTimerPanel()
    {
        CropTimerPanel.SetActive(false);
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
    }

    private void HideAllPanels()
    {
        HideCropTimerPanel();
        HideCropSelectionPanel();
        HideHarvestPanel();
    }

    private void ShowPanelWithEffect(GameObject panel, Vector3 position)
    {
        panel.SetActive(true);
        panel.transform.position = position;
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    public void ShowBuildingPanel()
    {
        BuildingPanel.gameObject.SetActive(true);
        BuildingPanel.DOScaleX(1, 0.3f).SetEase(Ease.OutBack);
    }
    public void HideBuildingPanel()
    {
        BuildingPanel.gameObject.SetActive(false);
        BuildingPanel.DOScaleX(0, 0.3f).SetEase(Ease.InBack);
    }

    private void OnBuildingButtonClick(BuildingData building)
    {
        BuildingPanel.transform.DOScaleX(0, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            BuildingPanel.gameObject.SetActive(false);
            BuildingPlacementManager.Instance.StartPlacingBuilding(building);
        });
    }


}
