using System;
using System.Collections;
using System.Collections.Generic;
using MagicSDK;
using Nethereum.JsonRpc.Client;
using UnityEngine;

public class MagicUnity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Magic magic = new Magic("pk_live_8472C38BAF655645", macCanvas: GameObject.Find("Magic Example 1"));
        Magic.Instance = magic;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
