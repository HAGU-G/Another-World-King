using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIMapSlider : MonoBehaviour
{
    public StageManager stageManager;
    public CameraManager cameraManager;
    private Transform mainCameraTransform;

    public Slider sliderCurrentCameraPoint;

    public Slider sliderPlayerPoint;
    public Toggle togglePlayerPoint;
    public TextMeshProUGUI textSelectPlayerPoint;

    public Slider sliderEnemyPoint;
    public Toggle toggleEnemyPoint;
    public TextMeshProUGUI textSelectEnemyPoint;

    public float autoMoveSpeed;

    private List<CharacterAI> playerUnits;
    private List<CharacterAI> enemyUnits;
    private Slider selectedSlider;
    private bool isMoveToSelected;

    private void Start()
    {
        mainCameraTransform = Camera.main.transform;

        var playerTower = stageManager.playerTower;
        var enemyTower = stageManager.enemyTower;

        playerUnits = playerTower.units;
        enemyUnits = enemyTower.units;

        sliderPlayerPoint.minValue = cameraManager.MinX;
        sliderEnemyPoint.minValue = cameraManager.MinX;
        sliderCurrentCameraPoint.minValue = cameraManager.MinX;
        sliderCurrentCameraPoint.value = cameraManager.MinX;

        sliderPlayerPoint.maxValue = cameraManager.MaxX;
        sliderEnemyPoint.maxValue = cameraManager.MaxX;
        sliderCurrentCameraPoint.maxValue = cameraManager.MaxX;

    }

    private void Update()
    {
        UpdateSliders();
    }

    private void UpdateSliders()
    {
        if (cameraManager.IsCameraMoved)
        {
            sliderCurrentCameraPoint.value = mainCameraTransform.position.x;
        }

        sliderPlayerPoint.gameObject.SetActive(playerUnits.Count > 0);
        if (sliderPlayerPoint.gameObject.activeSelf)
        {
            sliderPlayerPoint.value = playerUnits[0].transform.position.x;
        }
        else if (selectedSlider == sliderPlayerPoint)
        {
            selectedSlider = null;
        }

        sliderEnemyPoint.gameObject.SetActive(enemyUnits.Count > 0);
        if (sliderEnemyPoint.gameObject.activeSelf)
        {
            sliderEnemyPoint.value = enemyUnits[0].transform.position.x;
        }
        else if (selectedSlider == sliderEnemyPoint)
        {
            selectedSlider = null;
        }

        if (selectedSlider != null && Time.timeScale != 0)
        {
            var deltaValue = autoMoveSpeed * Time.deltaTime / Time.timeScale;
            var selectedValue = selectedSlider.value;

            isMoveToSelected = true;
            if (sliderCurrentCameraPoint.value > selectedValue)
            {
                sliderCurrentCameraPoint.value -= deltaValue;
                if (sliderCurrentCameraPoint.value < selectedValue)
                {
                    sliderCurrentCameraPoint.value = selectedValue;
                }
            }
            else if (sliderCurrentCameraPoint.value < selectedValue)
            {
                sliderCurrentCameraPoint.value += deltaValue;
                if (sliderCurrentCameraPoint.value > selectedValue)
                {
                    sliderCurrentCameraPoint.value = selectedValue;
                }
            }
            isMoveToSelected = false;
        }
    }

    public void SetCameraPosX()
    {
        if (!cameraManager.IsCameraMoved)
        {
            var cameraTransform = mainCameraTransform.position;
            cameraManager.SetCameraPosition(
                new Vector3(
                    sliderCurrentCameraPoint.value,
                    cameraTransform.y,
                    cameraTransform.z
                ));
        }
    }

    public void SelectPoint(Toggle selectedToggle)
    {
        if (selectedToggle.isOn)
        {
            if (selectedToggle == togglePlayerPoint)
            {
                selectedSlider = sliderPlayerPoint;
            }
            else if (selectedToggle == toggleEnemyPoint)
            {
                selectedSlider = sliderEnemyPoint;
            }
            isMoveToSelected = true;
            sliderCurrentCameraPoint.value = selectedSlider.value;
            isMoveToSelected = false;
        }
        else
        {
            selectedSlider = null;
        }
    }

    public void ToggleOff()
    {
        if (!isMoveToSelected)
        {
            togglePlayerPoint.isOn = false;
            toggleEnemyPoint.isOn = false;
        }
    }
}
