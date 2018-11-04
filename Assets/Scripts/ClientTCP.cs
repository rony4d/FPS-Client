using System;
using System.Net.Sockets;
using UnityEngine;


public class ClientTCP
{
    private static TcpClient _clientSocket;
    private static NetworkStream _clientNetworkStream;
    private static byte[] _asyncBuffer;


    public static void InitClient(string address, int port)
    {
        _clientSocket = new TcpClient();
        _clientSocket.ReceiveBufferSize = Constants.MAX_BUFFER_SIZE;
        _clientSocket.SendBufferSize = Constants.MAX_BUFFER_SIZE;

        _asyncBuffer = new byte[Constants.MAX_BUFFER_SIZE * 2];
        _clientSocket.BeginConnect(address, port, ClientConnectCallback, _clientSocket);
    }

    static void ClientConnectCallback(IAsyncResult result)
    {
        try
        {
            _clientSocket.EndConnect(result);
            if (_clientSocket.Connected == false)
            {
                return;
            }
            else
            {
                _clientSocket.NoDelay = true;
                _clientNetworkStream = _clientSocket.GetStream();

                // Constants.MAX_BUFFER_SIZE is multiplied 2 is multiplied to have a much large '
                _clientNetworkStream.BeginRead(_asyncBuffer, Constants.NETWORK_STREAM_OFFSET, Constants.MAX_BUFFER_SIZE * 2, ReceiveCallback, null);
                Debug.Log("Successfully connected to server");

            }
        }
        catch (ArgumentException ex)
        {
            Debug.Log("ClientConnectCallback error: "+ ex.Message);

        }

    }

    private static void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            int readByteSize = _clientNetworkStream.EndRead(result);
            if (readByteSize <= 0)
            {
                return;
            }
            byte[] newBytes = new byte[readByteSize];
            Buffer.BlockCopy(_asyncBuffer, Constants.NETWORK_STREAM_OFFSET, newBytes, Constants.NETWORK_STREAM_OFFSET, readByteSize);

            //Add unity thread here
            UnityThread.executeInUpdate(() =>
                 //Hanlde data here   
                 ClientHandleData.HandleData(newBytes)
             );
            _clientNetworkStream.BeginRead(_asyncBuffer, Constants.NETWORK_STREAM_OFFSET, Constants.MAX_BUFFER_SIZE * 2, ReceiveCallback, null);

        }
        catch (ArgumentException ex)
        {
            Debug.Log("ClientConnectCallback error: " + ex.Message);
            return;
        }
    }

    public static void SendData(byte[] data)
    {

    }

    public static void DisconnectFromServer()
    {
        _clientSocket.Close();
        _clientSocket = null;
    }

}