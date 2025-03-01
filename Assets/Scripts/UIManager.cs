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
        HideCropSelectionPanel();
    }

    public void ShowCropSelectionPanel(Vector3 worldPosition)
    {
        CropSelectionPanel.SetActive(true);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        CropSelectionPanel.transform.position = screenPosition + new Vector3(-200f, 100f, 0f);
    }


    public bool IsUIActive()
    {
        return CropSelectionPanel.activeSelf ||CropTimerPanel.activeSelf;
    }
    public void ShowCropTimerPanel(Vector3 worldPosition, BaseCrop crop)
    {
        if (crop == null || crop.IsFullyGrown()) return;

        CropTimerPanel.SetActive(true);

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        CropTimerPanel.transform.position = screenPosition + new Vector3(0f, -200f, 0f);

        NameText.text = crop.name;

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

    public void HideCropSelectionPanel() => CropSelectionPanel.SetActive(false);
    public void HideCropTimerPanel()
    {
        CropTimerPanel.SetActive(false);
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
    }

}
