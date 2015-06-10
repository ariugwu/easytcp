using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace easyTcp.Common.Model.Server
{
    public class AsyncListener
    {
        public List<StateObject> Connections { get; set; }

        private IProcessStrategy _processStrategy;

        private BinaryFormatter _binaryFormatter = new BinaryFormatter();

        private ManualResetEvent _allDone = new ManualResetEvent(false);

        public AsyncListener(IProcessStrategy processStrategy)
        {
            _processStrategy = processStrategy;
        }

        public AsyncListener() : this(new DefaultProcessStartegy())
        {
            
        }

        public void StartListening(IPAddress ip, int port)
        {
            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ip ?? ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                Console.WriteLine("Listening on: {0}", listener.LocalEndPoint);
                Console.WriteLine("Waiting for a connection...");

                while (true)
                {
                    // Set the event to nonsignaled state.
                    _allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.
                    _allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            _allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;

            Connections.Add(state);

            Console.WriteLine("Connected to: {0}", state.workSocket.RemoteEndPoint);

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);

            Console.WriteLine("Waiting for a connection..."); // Let the console know that we've processed that connection and we're waiting again.
        }

        public void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = 0;

            // Read data from the client socket.
            // If the client closes it will throw an exception here. 
            try
            {
                bytesRead = handler.EndReceive(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection with client [{0}] lost with message: {1}", handler.RemoteEndPoint, ex.Message);

                //Do some cleanup: Remove any connections that are closed.
                foreach (var c in Connections.Where(x => !x.workSocket.Connected)) Connections.Remove(c);

                return;
            }
            
            
            if (bytesRead > 0)
            {
                Request request = ByteArrayToRequest(state.buffer);

                // All the data has been read from the 
                // client. Display it on the console.
                Console.WriteLine("Request Received from socket {0}. \n Command : {1}", state.workSocket.RemoteEndPoint, request.Payload.ToString());

                // Process the command.
                Send(handler, ProcessRequest(request));

            }
            else
            {
                Send(handler, OnEmptyRequest());
            }

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        private void Send(Socket handler, Response response)
        {
            byte[] byteData = ObjectToByteArray(response);
            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes (response) to client.", bytesSent);

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public Response OnConnect()
        {
            return new Response() { Type = typeof(String), Payload = "Welcome!" };
        }

        public void OnDisconnect()
        {

        }

        public Response OnEmptyRequest()
        {
            return new Response() { Type = typeof(String), Payload = "This is the response to an empty request." };
        }

        public Response ProcessRequest(Request request)
        {
            return _processStrategy.ProcessRequest(request);
        }

        private byte[] ObjectToByteArray(Object obj)
        {

            using (var ms = new MemoryStream())
            {
                _binaryFormatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private Request ByteArrayToRequest(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            {
                return (Request)_binaryFormatter.Deserialize(ms);
            }
        }
    }
}
