using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyController : MonoBehaviour
{

    public static CurrencyController Instance;

    public TextMeshProUGUI text_BankBalance;

    private int bankBalance;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;
        // TODO: Load/save
        SetBankBalance(60000);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetBankBalance()
    {
        return bankBalance;
    }

    public void SetBankBalance(int amount)
    {
        bankBalance = amount;
        text_BankBalance.text = string.Format("{0:C}", bankBalance); 

    }

    public void AddBankBalance(int amount)
    {
        bankBalance += amount;
        text_BankBalance.text = string.Format("{0:C}", bankBalance);
    }

    public void DeductBankBalance(int amount)
    {
        bankBalance -= amount;
        text_BankBalance.text = string.Format("{0:C}", bankBalance);
    }

}
