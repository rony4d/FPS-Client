using System;
using System.Collections.Generic;
using UnityEngine;


  public class ClientHandleData
    {
		public delegate void Packet_(byte[] data);

        public static Dictionary<int, Packet_> Packets = new Dictionary<int, Packet_>();

        public static int PacketLength;

		private static ByteBuffer _playerBuffer;

		public static void InitPackets()
        {
            Packets.Add((int)ServerPackets.SIngame, HandleIngame);
            Packets.Add((int)ServerPackets.SPlayerData, HandlePlayerData);

        }

    private static void HandleIngame(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger(); // reads the packet Identifier first

        ClientManager.instance.myConnectionId = buffer.ReadInteger();  // read player's connection ID
        buffer.Dispose();
    }

    private static void HandlePlayerData(byte [] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger(); // reads the packet Identifier first
        int connectionId = buffer.ReadInteger();// read player's connection ID

        Types.Players[connectionId].ConnectionId = connectionId;

        buffer.Dispose();

        ClientManager.instance.InstantiateNetworkPlayers(connectionId);

    }


    public static void HandleData(byte[] data)
        {
            byte[] buffer = (byte[])data.Clone();

            if (_playerBuffer == null)
            {
                _playerBuffer = new ByteBuffer();
            }

            _playerBuffer.WriteBytes(buffer);

            if (_playerBuffer.Count() == 0)
            {
                _playerBuffer.Clear();
                return;
            }

            if (_playerBuffer.Count() >= Constants.INT_SIZE)
            {
                PacketLength = _playerBuffer.ReadInteger(false);
                if (PacketLength <= 0)
                {
                    _playerBuffer.Clear();
                    return;
                }
            }

            while (PacketLength > 0 && PacketLength <= _playerBuffer.Length() - Constants.INT_SIZE)
            {
                if (PacketLength <= _playerBuffer.Length() - Constants.INT_SIZE)
                {
                    _playerBuffer.ReadInteger();

                    data = _playerBuffer.ReadBytes(PacketLength);
                    HandleDataPackets(data);
                }

                PacketLength = 0;
                if (_playerBuffer.Length() >= Constants.INT_SIZE)
                {
                    _playerBuffer.ReadInteger();

                    PacketLength = _playerBuffer.ReadInteger(false);

                    if (PacketLength <= 0)
                    {
                        _playerBuffer.Clear();
                        return;
                    }

                }

                if (PacketLength <= 1)
                {
                    _playerBuffer.Clear();
                }
            }

        }

        private static void HandleDataPackets(byte[] data)
        {
        try
        {
            int packetId;
            ByteBuffer byteBuffer = new ByteBuffer();
            byteBuffer.WriteBytes(data);
            packetId = byteBuffer.ReadInteger();
            byteBuffer.Dispose();
            //if (Packets.TryGetValue(packetId, out Packet_ packet))
            //{
            //    Debug.Log("<Packet>" + Enum.GetName(typeof(ClientPackets), packetId));
            //    packet.Invoke(data);
            //}

            if (Packets.ContainsKey(packetId))
            {
                Packet_ packet_ = Packets[packetId];
                Debug.Log("<Packet>" + Enum.GetName(typeof(ServerPackets), packetId));
                packet_.Invoke(data);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("HandleDataPackets error: " + ex.Message);
        }


        }
    }

