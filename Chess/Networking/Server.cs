using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Collections;

namespace SimpleServer
{
    class Server
    {
        const int MAX_CLIENTS = 1;
        bool _connected;
        bool _killServer;
        bool _serverFailed;
        bool _readingFromStream;

        //location of the client
        System.Net.IPAddress _ipAddress;
        int _port;

        TcpListener _listener;

        Queue _sendQueue;
        Object _sendLock;       //prevents access problems with the send queue
        Queue _recvQueue;
        Object _recvLock;       //prevents access problems with the recv queue

        //Created to notify whoever creates the server of stuff via events
        public event EventHandler ServerFailed;

        //Default Constructor (port 1981, localhost address)
        public Server()
        {
            _ipAddress = System.Net.IPAddress.Parse("127.0.0.1");
            _port = 1981;
            init();
        }

        //Constructor (port to listen on)
        public Server(int port, string address)
        {
            _ipAddress = System.Net.IPAddress.Parse(address);
            _port = port;

            init();
        }

        //EVENTS====================================================================

        protected virtual void OnServerFailed(EventArgs e)
        {
            EventHandler handler = ServerFailed;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        //EVENTS====================================================================

        //Initialize all variables, listen on an IP address and port for client to connect
        void init()
        {
            _serverFailed = false;
            _connected = false;
            _readingFromStream = false;
            _killServer = false;

            _sendLock = new Object();
            _recvLock = new Object();

            _sendQueue = new Queue();
            _recvQueue = new Queue();

            //Create new server/listener socket and start listening
            _listener = new TcpListener(_ipAddress, _port);
            _listener.Server.ReceiveTimeout = 1000;     //timeout after a second

            try
            {
                _listener.Start();
                Console.WriteLine("Server started and listening!");
                run();
            }
            catch (Exception e)
            {
                //Server failed - likely cause, another user on the socket
                _serverFailed = true;

                //Notify observers when the server fails to connect
                OnServerFailed(EventArgs.Empty);

                _listener.Stop();   //close the failed TCPListener
            }
        }

        //opens the port, starts threads to listen for connection
        private void run()
        {
            //start a new thread to handle each possible connecting client
            for (int i = 0; i < MAX_CLIENTS; i++)
            {
                Console.WriteLine("Creating new thread to connect to the client!");
                Thread t = new Thread(ConnectToClient);
                t.IsBackground = true;
                t.Start();
            }
        }

        //Continuously tries to open a socket to communicate with the client, then sets up
        private void ConnectToClient()
        {
            // Connect to client, once connected open stream and attempting to read/write.  If something goes wrong,
            // close the stream, close the socket, start over.
            while (!_killServer)
            {
                Socket soc = _listener.AcceptSocket();
                soc.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.ReuseAddress, true);      //allows user to reuse socket
                Console.WriteLine("Connecting to client!");
                
                _connected = true;

                try
                {
                    //read to/write from the connection
                    NetworkStream s = new NetworkStream(soc);

                    StreamReader sr = new StreamReader(s);
                    //sr.BaseStream.ReadTimeout = 1000;   //Don't hang forever waiting for input, time out after a second

                    StreamWriter sw = new StreamWriter(s);
                    sw.AutoFlush = true;    //each time you write, clear the buffer
                    Console.WriteLine("Creating stream, reader, and writer!");

                    //loop while our socket is connected and see if we need to send or recieve data
                    while (_connected && !_killServer)
                    {
                        //write data
                        putDataOnStream(sw);

                        //read data - only thread this if it's not already running!
                        if (!_readingFromStream)
                        {
                            _readingFromStream = true;

                            //Create thread
                            Thread readThread = new Thread(() => getDataFromStream(sr));
                            readThread.IsBackground = true;
                            readThread.Start();
                        }

                        if (!soc.Connected) //If we get disconnected this closes the stream and socket, then loops through to try again
                        {
                            _connected = false;
                            Console.WriteLine("No longer connected!");
                        }

                        Thread.Sleep(100);      //check for messages ten times a second, don't run loop forever eating clock cycles
                    }

                    s.Close(); //close the stream, release resources
                }
                 
                catch (Exception e)
                {
                    //problem with read/write operations
                    Console.WriteLine(e.Message);
                }
                soc.Close();

                _listener.Stop();   //close the TCPListener - We can only do this here because we're listening to just one client
                _killServer = true; //stop looping, we've just killed the server
            }
        }

        // Returns true if data has been stored in the queue
        public bool recvDataAvailable()
        {
            lock (_recvLock)        //access the queue safely, threaded operations are writing to it!
            {
                if (_recvQueue.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        //Gets data from stream, this is a blocking call so we should thread it off
        private void getDataFromStream(StreamReader sr)
        {
            try
            {
                //Read outside of the lock, so that the recieved queue can be accessed while waiting for new data
                String temp = sr.ReadLine();

                if (temp != "" && temp != null)     //only save data to queue if there's something to read
                {
                    //make sure nobody is writing to the data we're reading from
                    lock (_recvLock)
                    {
                        _recvQueue.Enqueue(temp);                   //Stick it in the queue
                        Console.WriteLine("READING: " + temp);
                    }
                }
            }
            catch (Exception e)
            {
                //problem reading from stream, do nothing.  Main code handles this scenario by closing the stream and socket.
            }
            _readingFromStream = false; //thread ends, allow creation of another
        }

        // Command from user to terminate the server
        public void killServer()
        {
            _killServer = true;
        }

        //returns true when the server failed to start.  User should try again on a different port.
        public bool serverFailed()
        {
            return _serverFailed;
        }

        //Sends everything in the sendQueue to the server
        private void putDataOnStream(StreamWriter sw)
        {
            //make sure nobody is writing to the data we're reading from
            lock (_sendLock)
            {
                while (_sendQueue.Count > 0)
                {
                    String temp = (String)_sendQueue.Dequeue();
                    Console.WriteLine("SENDING: " + temp);
                    sw.WriteLine(temp);
                }
            }
        }

        //returns true if the server is able to send/recieve to the client
        public bool isConnected()
        {
            return _connected;
        }

        //Gets data from the client (if available)
        public String recvData()
        {
            if (_connected)
            {
                lock (_recvLock)    //don't read data unless it's safe
                {
                    return (String)_recvQueue.Dequeue();
                }
            }
            return "";
        }

        //Takes in data to be sent to the client (if available)
        public void sendData(String newData)
        {
            if (_connected)
            {
                lock (_sendLock)    //don't allow data to be written unless it's safe
                {
                    _sendQueue.Enqueue(newData);
                }
            }
        }
    }
}
