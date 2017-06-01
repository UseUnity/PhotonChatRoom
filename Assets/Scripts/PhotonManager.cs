﻿using UnityEngine;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using Code.Common;

public class PhotonManager : MonoBehaviour, IPhotonPeerListener
{

    private static PhotonManager instance = null;

    public static PhotonManager Instance
    {
        get { return instance; }
    }

    //连接服务器
    private PhotonPeer peer;
    private ConnectionProtocol protocol = ConnectionProtocol.Udp;
    private string serverAddress = "127.0.0.1:5055";
    private string applicationName = "MyGameServer";

    private bool connected = false;

    void Awake()
    {
        instance = this;
        peer = new PhotonPeer(this, protocol);
    }

    void Update()
    {
        if (!connected)
            peer.Connect(serverAddress, applicationName);
        peer.Service();
    }

    private void OnDestroy()
    {
        peer.Disconnect();
    }

    //向服务器发请求
    public void OnOperationRequest(byte opCode, Dictionary<byte, object> parameters = null, byte SubCode = 0)
    {
        parameters[80] = SubCode;
        peer.OpCustom(opCode, parameters, true);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        if (connected)
            Debug.Log(message);
    }

    //事件响应
    public void OnEvent(EventData eventData)
    {
        Debug.Log("OnEvent() Called : " + eventData.ToStringFull());
    }

    //服务端发送过来的响应
    public void OnOperationResponse(OperationResponse operationResponse)
    {
        Debug.Log("OnOperationResponse() Called : " + operationResponse.ToStringFull());
    }

    //状态改变
    public void OnStatusChanged(StatusCode statusCode)
    {
        Debug.Log("OnOperationResponse() Called : " + statusCode.ToString());
        switch (statusCode)
        {
            case StatusCode.Connect:
                connected = true;
                break;
            case StatusCode.Disconnect:
                connected = false;
                break;
            case StatusCode.Exception:
                connected = false;
                break;
        }
    }

}