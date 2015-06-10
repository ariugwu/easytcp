using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using easyTcp.Common.Model.Client.Parse;
using easyTcp.Common.Model.Client.Render;

namespace easyTcp.Common.Model.Client.Connection
{

    public class Client
    {
        private BinaryFormatter _bf = new BinaryFormatter();

        // ManualResetEvent instances signal completion.
        private ManualResetEvent _connectDone =
            new ManualResetEvent(false);
        private ManualResetEvent _sendDone =
            new ManualResetEvent(false);
        private ManualResetEvent _receiveDone =
            new ManualResetEvent(false);

        private IRenderStrategy _renderStrategy;
        private IParseStrategy _parseStrategy;

        public void Start(IPAddress ip, int port, IRenderStrategy renderStrategy, IParseStrategy parseStrategy)
        {

            _renderStrategy = renderStrategy;
            _parseStrategy = parseStrategy;

            TcpClient client = new TcpClient();

            IPEndPoint serverEndPoint = new IPEndPoint(ip, port);

            client.Connect(serverEndPoint);
            
            //ReceiveAsync(client.Client); // Setup our async listener for broadcasts and such from the server.
            
            // Create our console.
            var trap = true;

            while (trap)
            {
                var data = _parseStrategy.CommandPrompt();

                if (data != null && data.Trim().ToLower().Equals("q"))
                {
                    trap = false; 
                    continue;
                }

                byte[] buffer = ObjectToByteArray(_parseStrategy.Parse(data));

                client.Client.Send(buffer);

                buffer = new byte[client.Client.ReceiveBufferSize];
                
                client.Client.Receive(buffer);

                Response response = ByteArrayToResponse(buffer);

                _renderStrategy.Render(response);

            }

            Console.WriteLine("Connection Terminated. Press any key to exit program.");
            Console.ReadKey();
            
        }

        private void ReceiveAsync(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                Response response = ByteArrayToResponse(state.buffer);

                _renderStrategy.Render(response);

                 _receiveDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private byte[] ObjectToByteArray(Object obj)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private Response ByteArrayToResponse(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            {
                return (Response)_bf.Deserialize(ms);
            }
        }
    }
}
