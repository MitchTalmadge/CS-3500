﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceWars
{
    /// <summary>
    /// The main controller for the SpaceWars game.
    /// Uses the networking library to maintain a connection with the game server and notify listeners of changes to game components.
    /// </summary>
    public class SpaceWars
    {
        /// <summary>
        /// This delegate handles cases where any game component is updated (a ship, projectile, etc.)
        /// </summary>
        public delegate void GameComponentsListener();

        /// <summary>
        /// This event is fired whenever a game component (ship, projectile, etc.) is updated from the server.
        /// </summary>
        public event GameComponentsListener OnGameComponentsUpdated;

        /// <summary>
        /// The dimensions of the game world (both sides use same length).
        /// </summary>
        public int WorldSize { get; }

        /// <summary>
        /// The current ship that the player controls. 
        /// May be null if there is no established connection.
        /// </summary>
        public Ship PlayerShip { get; }

        /// <summary>
        /// Determines if there is an established connection.
        /// </summary>
        /// <returns>True if a connection has been established.</returns>
        public bool IsConnected => PlayerShip != null;

        /// <summary>
        /// A mapping of each known ship in the game to that ship's ID.
        /// </summary>
        private Dictionary<int, Ship> _ships;

        /// <summary>
        /// All known ships in the game.
        /// </summary>
        public IEnumerable<Ship> Ships => _ships.Values.ToList().AsReadOnly();

        //TODO: Projectiles, etc.

        /// <summary>
        /// Attempts to establish a connection to the given game server, using the given nickname.
        /// If a connection cannot be established, an exception is thrown. 
        /// Otherwise, the connection has been established and the game is ready.
        /// </summary>
        /// <param name="hostName">The server address, excluding the port.</param>
        /// <param name="nickname">The nickname to use for the player connecting.</param>
        public SpaceWars(string hostName, string nickname)
        {
            throw new SpaceWarsConnectionFailedException("We didn't even try to connect :(");
        }

        /// <summary>
        /// Disconnects from the server. 
        /// This game instance should no longer be used once this method is called.
        /// </summary>
        public void Disconnect()
        {
            //TODO: Disconnect gracefully.
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// A custom exception for when a connection to the Space Wars server could not be established.
    /// </summary>
    /// <authors>Jiahui Chen, Mitch Talmadge</authors>
    public class SpaceWarsConnectionFailedException : Exception
    {
        /// <inheritdoc />
        public SpaceWarsConnectionFailedException()
        {
        }

        /// <inheritdoc />
        public SpaceWarsConnectionFailedException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public SpaceWarsConnectionFailedException(string message, Exception innerException) : base(message,
            innerException)
        {
        }
    }
}