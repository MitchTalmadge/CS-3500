﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Networking;
using Newtonsoft.Json;

namespace SpaceWars
{
    /// <summary>
    /// This class keeps track of a single client and handles all required communication to and from that client.
    /// </summary>
    /// <authors>Jiahui Chen, Mitch Talmadge</authors>
    internal class ClientCommunicator
    {
        /// <summary>
        /// This static counter ensures that all new instances have a unique id.
        /// </summary>
        private static int _idCounter;

        /// <summary>
        /// The Id of this client communicator.
        /// Used to match up communicators with ships.
        /// </summary>
        public int Id { get; } = _idCounter++;

        /// <summary>
        /// The SpaceWars server instance.
        /// </summary>
        private readonly GameServerController _gameServerController;

        /// <summary>
        /// The client's SocketState.
        /// </summary>
        private readonly SocketState _state;

        /// <summary>
        /// This event is invoked when the client disconnects.
        /// </summary>
        public event Action Disconnected;

        /// <summary>
        /// Determines if the first nickname packet has been received.
        /// </summary>
        private bool _nicknameReceived;

        /// <summary>
        /// Invoked when the client sends their nickname.
        /// </summary>
        public event Action<string> NicknameReceived;

        /// <summary>
        /// Maps ship commands to the current value as received by the client.
        /// For example, if a value of a command is true, then the client sent the command during the current tick.
        /// Commands are set to false at the end of every tick.
        /// </summary>
        public IDictionary<Ship.Command, bool> ClientCommands { get; } = new ConcurrentDictionary<Ship.Command, bool>
        {
            [Ship.Command.Thrust] = false,
            [Ship.Command.Left] = false,
            [Ship.Command.Right] = false,
            [Ship.Command.Fire] = false
        };

        /// <summary>
        /// Creates an instance from a connected client SocketState.
        /// </summary>
        /// <param name="gameServerController">The SpaceWars server instance.</param>
        /// <param name="state">The client's SocketState.</param>
        public ClientCommunicator(GameServerController gameServerController, SocketState state)
        {
            _gameServerController = gameServerController;

            _state = state;

            // Listen for socket state events.
            _state.DataReceived += OnDataReceived;
            _state.Disconnected += OnDisconnected;
        }

        /// <summary>
        /// Asynchronously begins listening for client data.
        /// </summary>
        public void BeginListeningAsync()
        {
            AbstractNetworking.GetData(_state);
        }

        /// <summary>
        /// Sends the first packet that a client should receive.
        /// </summary>
        private void SendFirstPacket()
        {
            var packet = new StringBuilder();

            // Player ID
            packet.Append(Id).Append('\n');

            // World Size
            packet.Append(_gameServerController.Configuration.WorldSize).Append('\n');

            // Send packet.
            AbstractNetworking.Send(_state, packet.ToString());
        }

        /// <summary>
        /// Called when the world is updated by the server.
        /// </summary>
        /// <param name="world">The world that was updated.</param>
        private void OnWorldUpdated(World world)
        {
            // Serialize the World to JSON.
            var worldData = new StringBuilder();
            foreach (var ship in world.GetComponents<Ship>())
            {
                worldData.Append(JsonConvert.SerializeObject(ship)).Append("\n");
            }
            foreach (var proj in world.GetComponents<Projectile>())
            {
                worldData.Append(JsonConvert.SerializeObject(proj)).Append("\n");
            }
            foreach (var star in world.GetComponents<Star>())
            {
                worldData.Append(JsonConvert.SerializeObject(star)).Append("\n");
            }

            // Clear the client's commands.
            foreach (var command in ClientCommands.Keys)
                ClientCommands[command] = false;

            AbstractNetworking.Send(_state, worldData.ToString());
        }

        /// <summary>
        /// Called when data is received from the client.
        /// </summary>
        /// <param name="data">The data from the client.</param>
        private void OnDataReceived(string data)
        {
            // Check for nickname packet.
            if (!_nicknameReceived)
            {
                _nicknameReceived = true;

                // Trim newline from nickname and invoke event.
                var nickname = data.Replace("\n", "");
                NicknameReceived?.Invoke(nickname);

                // Send the first packet to the client.
                SendFirstPacket();

                // Listen for server events.
                _gameServerController.WorldUpdated += OnWorldUpdated;
            }
            else
            {
                // Thrust
                if (data.Contains("T"))
                    ClientCommands[Ship.Command.Thrust] = true;

                // Left or Right
                if (data.Contains("L"))
                    ClientCommands[Ship.Command.Left] = true;
                else if (data.Contains("R"))
                    ClientCommands[Ship.Command.Right] = true;

                // Fire
                if (data.Contains("F"))
                    ClientCommands[Ship.Command.Fire] = true;
            }

            AbstractNetworking.GetData(_state);
        }

        /// <summary>
        /// Called when the client disconnects.
        /// </summary>
        private void OnDisconnected()
        {
            // Unsubscribe from event listeners
            _gameServerController.WorldUpdated -= OnWorldUpdated;
            _state.DataReceived -= OnDataReceived;
            _state.Disconnected -= OnDisconnected;

            Disconnected?.Invoke();
        }
    }
}