using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using link.magic.unity.sdk;
using link.magic.unity.sdk.Provider;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC;
using Nethereum.RPC.Eth;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.RPC.Personal;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Nethereum.Web3;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

public class EthereumButton : MonoBehaviour
{
    private string _account;
    
    public TMP_InputField result;
    public TMP_InputField accountOutput;

    public TMP_InputField accountInput;

    public TMP_InputField fromAccount;
    public TMP_InputField toAccount;
    public TMP_InputField amount;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Update is called once per frame
    public async void SendTransaction()
    {
        result.text = "";
        var ethAccounts = new EthAccounts(Magic.Instance.Provider);
        var accounts = await ethAccounts.SendRequestAsync();
        var transaction = new EthSendTransaction(Magic.Instance.Provider);
        var transactionInput = new TransactionInput
            { To = toAccount.text, Value = new HexBigInteger(UnitConversion.Convert.ToWei(amount.text)), From = fromAccount.text};
        var hash = await transaction.SendRequestAsync(transactionInput);
        Debug.Log(hash);
        result.text = hash;
    }
    
    public async void GetAccount()
    {
        result.text = "";
        var ethAccounts = new EthAccounts(Magic.Instance.Provider);
        var accounts = await ethAccounts.SendRequestAsync();
        _account = accounts[0];
        Debug.Log(accounts[0]);
        result.text = accounts[0];
        accountOutput.text = accounts[0];
    }

    public async void EthSign()
    {
        result.text = "";
        
        var ethAccounts = new EthAccounts(Magic.Instance.Provider);
        var accounts = await ethAccounts.SendRequestAsync();
        var personalSign = new EthSign(Magic.Instance.Provider);
        var transactionInput = new TransactionInput{Data = "Hello world"};
        var res = await personalSign.SendRequestAsync(accounts[0], "hello world");
        result.text = res;
    }

    public async void GetBalance()
    {
        var ethApiService = new EthApiService(Magic.Instance.Provider);
        var accounts = await ethApiService.Accounts.SendRequestAsync();
        var web3 = new Web3(Magic.Instance.Provider);
        var balance = await web3.Eth.GetBalance.SendRequestAsync(accountInput.text);
        //var balance = await ethApiService.GetBalance.SendRequestAsync(accountInput.text);
        result.text = balance.ToString();
    }
    
    public async void GetChainId()
    {
        var ethApiService = new EthApiService(Magic.Instance.Provider);
        var chainId = await ethApiService.ChainId.SendRequestAsync();
        result.text = chainId.ToString();
    }
}
