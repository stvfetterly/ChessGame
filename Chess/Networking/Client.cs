using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace SimpleClient
{
    class Client
    {
        //System.Net.IPAddress _ipAddress;
        String _ipAddress;
        int _port;

        TcpClient _client;

        bool _connected;
        bool _killClient;
        bool _readingFromStream;

        Queue _sendQueue;
        Object _sendLock;       //prevents access problems with the send queue
        Queue _recvQueue;
        Object _recvLock;       //prevents access problems with the recv queue

        //Constructors - default sets the IP to localhost and the port to 1981
        public Client()
        {
            _ipAddress = "127.0.0.1";
            _port = 1981;

            // creates a new thread to handle client operations
            Thread runThread = new Thread(init);
            runThread.IsBackground = true;
            runThread.Start();
        }
        public Client(int port, String address)
        {
            _port = port;
            _ipAddress = address;

            // creates a new thread to handle client operations
            Thread runThread = new Thread(init);
            runThread.IsBackground = true;
            runThread.Start();
        }

        /* In this function, the _client socket is used to attempt to connect to the server.  After connection, 
         * the main 'run' function is called for read/write operations.  When a client has a read error, it 
         * closes all connections and returns here.  */
        private void init()
        {
            _killClient = false;

            //keep restarting the client until the user has indicated that they want to stop it
            while (!_killClient)
            {
                _connected = false;
                _readingFromStream = false;
                _recvQueue = new Queue();
                _sendQueue = new Queue();
                _sendLock = new Object();
                _recvLock = new Object();

                //Client is waiting to connect
                Console.WriteLine("Client waiting to connect!");
                while (!_connected && !_killClient)
                {
                    try
                    {
                        //Actual connection to server happens here
                        _client = new TcpClient(_ipAddress, _port);
                        //_client.ReceiveTimeout = 1000;      //timeout after a second
                        Console.WriteLine("Create TcpClient!");
                        _connected = true;
                    }
                    catch
                    {
                        //Failed to connect, retry ten times a second
                        Thread.Sleep(100);
                    }
                }

                run();
            }
        }

        /* Creates a network stream using the socket we've connected to, and reads/writes data to it.
         * Reading is a blocking call, so it's threaded off.*/
        public void run()
        {
            try
            {
                // Create stream, reader/writer to access it
                NetworkStream s = _client.GetStream();

                StreamReader sr = new StreamReader(s);
                //sr.BaseStream.ReadTimeout = 1000;   //Don't hang forever waiting for input, time out after a second

                StreamWriter sw = new StreamWriter(s);
                sw.AutoFlush = true;
                Console.WriteLine("Creating stream, reader, and writer!");

                while (_connected && !_killClient)
                {
                    //WRITE
                    putDataOnStream(sw);

                    //READ
                    if (!_readingFromStream)            //if we're already waiting for a read stream thread to complete, keep waiting
                    {
                        _readingFromStream = true;

                        //Create thread
                        Thread readThread = new Thread( ()=>getDataFromStream(sr) );
                        readThread.IsBackground = true;
                        readThread.Start();
                    }

                    if (!_client.Connected)
                    {
                        _connected = false;
                        Console.WriteLine("No longer connected!");
                    }

                    Thread.Sleep(100);      //send/recieve data ten times a second
                }

                //No longer connected, close stream
                s.Close();
                Console.WriteLine("Closign stream!");
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }
            finally
            {
                //No longer connected, close socket if it was created
                if (_client != null)
                {
                    _client.Close();
                }
                Console.WriteLine("Closing client!");
            }
        }

        // Reads the next line from the stream, blocking.
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

        // Sends data to the server
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

        // Command from user to terminate the client
        public void killClient()
        {
            _killClient = true;
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

        //returns true if the client is able to send/recieve to the server
        public bool isConnected()
        {
            return _connected;
        }

        // Gets data from the queue (if available) or return an empty string
        public String recvData()
        {
            if (_connected)
            {
                lock (_recvLock)    //don't read data unless it's safe
                {
                    if (_recvQueue.Count > 0)
                    {
                        return (String)_recvQueue.Dequeue();
                    }
                }
            }
            return "";
        }

        // Takes in data to be sent to the server (if available)
        // note - empty strings will not be sent
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
