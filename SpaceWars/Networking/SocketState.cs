﻿using System;
using System.Net.Sockets;
using System.Text;

namespace Networking
{
    /// <summary>
    /// Holds information about how data is handled when it reaches
    /// a Socket.
    /// </summary>
    /// <authors>Jiahui Chen, Mitch Talmadge</authors>
    public class SocketState
    {
        /// <summary>
        /// A unique ID for determining equality between socket states.
        /// </summary>
        private readonly Guid _id = new Guid();

        /// <summary>
        /// Socket that receives data
        /// </summary>
        internal Socket Socket { get; }

        /// <summary>
        /// Callback for an established connection.
        /// </summary>
        internal readonly AbstractNetworking.ConnectionEstablished ConnectionEstablished;

        /// <summary>
        /// Callback for a failed connection.
        /// </summary>
        internal readonly AbstractNetworking.ConnectionFailed ConnectionFailed;

        /// <summary>
        /// Callback for received data.
        /// </summary>
        internal readonly AbstractNetworking.DataReceived DataReceived;

        /// <summary>
        /// This is the buffer where we will receive data from the socket
        /// </summary>
        internal byte[] DataBuffer = new byte[1000];

        // This is a larger (growable) buffer, in case a single receive does not contain the full message.
        // holds pervious data 
        internal StringBuilder DataStringBuilder = new StringBuilder();

        /// <summary>
        /// Creates a new Socket State from a socket and callback.
        /// </summary>
        /// <param name="socket">The socket related to this state.</param>
        /// <param name="established">Callback for an established connection.</param>
        /// <param name="failed">Callback for a failed connection.</param>
        /// <param name="dataReceived">Callback for received data.</param>
        internal SocketState(
            Socket socket,
            AbstractNetworking.ConnectionEstablished established,
            AbstractNetworking.ConnectionFailed failed,
            AbstractNetworking.DataReceived dataReceived)
        {
            Socket = socket;
            ConnectionEstablished = established;
            ConnectionFailed = failed;
            DataReceived = dataReceived;
        }

        protected bool Equals(SocketState other)
        {
            return _id.Equals(other._id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SocketState) obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}