using System;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(menuName = "Save Data/Error Logic Forwarding/Target Counter")]

/// <summary>
/// Ведет счет ошибкам, до указанного значения. После чего, запрещает продолжать переотпровять запросы
/// (пока что то не произодет, и кто то не вызовет мето RemoveCount())
/// </summary>
public class SD_ErrorLogicForwardingDataCouner : SD_ErrorLogicForwardingData
{
    public override event Action OnUpdateData;
    public override bool IsContinue => _isContinue;
    [SerializeField]
    private bool _isContinue = true;

    private int _currentCoutn = 0;
    [SerializeField] 
    private int _targetCount = 3;
    
    public override void OnAddError()
    {
        _currentCoutn++;

        if (_currentCoutn >= _targetCount)
        {
            if (_isContinue == true)
            {
                _isContinue = false;
                OnUpdateData?.Invoke();
            }
            
        }
    }

    public override void OnRemoveAllError()
    {
        RemoveBlock();
    }


    public void RemoveCount()
    {
        RemoveBlock();
    }

    private void RemoveBlock()
    {
        _currentCoutn = 0;
        
        if (_isContinue == false)
        {
            _isContinue = true;
            OnUpdateData?.Invoke();
        }
    }

    //для тестов
    private void OnValidate()
    {
        OnUpdateData?.Invoke();
    }
}
