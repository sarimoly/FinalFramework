
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security;
using FirServer;
using FirServer.Defines;
using FirServer.Interface;
using GameLibs.FirSango.Defines;
using GameLibs.FirSango.Interface;
using Utility;

namespace GameLibs.FirSango.Managers
{
    public class RoomManager : BaseBehaviour, IManager
    {
        private Dictionary<uint, Dictionary<uint, IRoom>> gameRooms = new Dictionary<uint, Dictionary<uint, IRoom>>();

        public void Initialize()
        {
            LoadParseXml();
        }

        void LoadParseXml()
        {
            var xml = XmlHelper.LoadXml("config/room.xml");
            if (xml != null)
            {
                var count = xml.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    var node = xml.Children[i] as SecurityElement;
                    if (node != null)
                    {
                        var id = uint.Parse(node.Attribute("id"));
                        var name = node.Attribute("name");
                        var roomCount = uint.Parse(node.Attribute("roomCount"));
                        var roomUserMax = uint.Parse(node.Attribute("roomUserMax"));
                        this.CreateRooms(id, name, roomCount, roomUserMax);
                    }
                }
            }
        }

        void CreateRooms(uint levelid, string name, uint roomCount, uint roomUserMax)
        {
            var rooms = new Dictionary<uint, IRoom>();
            for (uint i = 0; i < roomCount; i++)
            {
                var room = new GameRoom();
                if (room != null)
                {
                    room.Initialize(name, i);
                    rooms.Add(i, room);
                }
            }
            gameRooms.Add(levelid, rooms);
        }

        public Dictionary<uint, IRoom> GetRooms(uint levelid)
        {
            Dictionary<uint, IRoom> rooms = null;
            gameRooms.TryGetValue(levelid, out rooms);
            return rooms;
        }

        public IRoom GetRoom(uint levelid, uint roomid)
        {
            IRoom room = null;
            var rooms = GetRooms(levelid);
            if (rooms != null)
            {
                rooms.TryGetValue(roomid, out room);
            }
            return room;
        }

        public IRoom OnMate(uint levelid, WebSocket socket)
        {
            var rooms = GetRooms(levelid);
            if (rooms != null)
            {
                foreach (var r in rooms)
                {
                    if (r.Value.GetUsers().Count < 1)
                    {
                        return r.Value;
                    }
                }
            }
            return null;
        }

        public void OnDispose()
        {
        }
    }
}