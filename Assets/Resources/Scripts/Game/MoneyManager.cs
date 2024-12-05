using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public static class MoneyManager
{
    private static float _money;
    public static float GetMoney() => _money;
    public static float ResetMoney() => _money = 0; 

    private static List<float> _safeAccount;
    public static List<float> GetSafeAccount() => _safeAccount;

    private static float _gameAccount=2000f;
    public static float GetGameAccount() => _gameAccount;

    public static UnityAction OnMoneyWon;
    public static UnityAction OnLevelEnd;


    public static void UpdateGameAccount(float amount)
    {
        _gameAccount += amount;
        SaveGameAccount();  // Save the updated game account to ParticipantData
    }

    private static void SaveGameAccount()
    {
        ParticipantSettings.Instance.WholeGamePair.Invoke(new KeyValuePair<string, float>("GameAccount", _gameAccount));
    }
    private static void SaveSafeAccount()
    {
        ParticipantSettings.Instance.WholeGamePair.Invoke(new KeyValuePair<string, float>("SafeAccount", _gameAccount));
    }


    public static void InitializeSafeAccount(int numberOfLevels)
    {
        _safeAccount = new List<float>(new float[numberOfLevels]);
    }

    public static void UpdateMoney(float amount)
    {
        _money += amount;
        OnMoneyWon.Invoke();  // Notify listeners that the money has been updated
    }
    public static void StoreMoneyInSafeAccount(int levelIndex)
    {
        if (_safeAccount != null && levelIndex >= 0 && levelIndex < _safeAccount.Count)
        {
            _safeAccount[levelIndex] = _money;
            _money = 0;  // Reset the money for the next level
        }
    }

    
}

