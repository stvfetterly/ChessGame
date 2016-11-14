using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SimpleClient;
using SimpleServer;
using System.Net;
using System.Text.RegularExpressions;

namespace Chess.Networking
{
    public partial class NetworkingWindow : Form
    {
        enum CHECKDATA {PORT, IP_BYTE};
        Client _chessClient;
        Server _chessServer;

        Font _systemFont;
        Color _systemColor;
        Color _commandColor;
        Font _userFont;
        Color _userColor;

        bool _connected;
        Chess.MainChessWindow _mainWindowHandle;
        String _userName;

        static String _textMessageID = "Ã¶Å";
        static String _gameMessageID = "â€™";

        public NetworkingWindow(Chess.MainChessWindow mainWindow)
        {
            InitializeComponent();

            _mainWindowHandle = mainWindow;
            _mainWindowHandle.chessMoveEvent += _mainWindowHandle_chessMoveEvent;
            _mainWindowHandle.selectionMadeEvent += _mainWindowHandle_selectionMadeEvent;

            //Fonts and colours for main display window
            _systemFont = new Font("Terminal", 11, FontStyle.Italic);
            _userFont = new Font("Calisto MT", 11, FontStyle.Regular);
            _systemColor = Color.Blue;
            _userColor = Color.Black;
            _commandColor = Color.Cyan;

            _connected = false;
        }

        //Stops the server, sets all the controls back to initial state
        private void stopServer()
        {
            buttStartStop.Text = "Start";
            MainChessWindow.isNetworked = false;
            _connected = false;
            txtMain.Enabled = false;
            txtSend.Enabled = false;
            buttSend.Enabled = false;

            txtIPAddress1.Enabled = true;
            txtIPAddress2.Enabled = true;
            txtIPAddress3.Enabled = true;
            txtIPAddress4.Enabled = true;
            txtPortNum.Enabled = true;
            radioHost.Enabled = true;
            radioJoin.Enabled = true;

            processingTimer.Stop();

            if (radioHost.Checked)
            {
                //If we were the host, kill the server and then let the garbage collector take care of it
                _chessServer.killServer();
                _chessServer = null;
            }
            else
            {
                //If we were joining, kill the client and then let the garbage collector take care of it
                _chessClient.killClient();
                _chessClient = null;
            }
        }

        //Starts/Stops the server or client
        private void buttStartStop_Click(object sender, EventArgs e)
        {
            if (MainChessWindow.isNetworked)     //Stop networking
            {
                //if this is the client, notify server that you've disconnected

                if (_chessClient != null)
                {
                    sendGameplayMessage("CLIENT_DISCONNECT");
                }
                stopServer();
            }
            else                        //Start networking
            {
                //First we need to check if the IP and port numbers have been entered correctly
                if (isValidData(txtIPAddress1.Text, CHECKDATA.IP_BYTE) &&
                     isValidData(txtIPAddress2.Text, CHECKDATA.IP_BYTE) &&
                     isValidData(txtIPAddress3.Text, CHECKDATA.IP_BYTE) &&
                     isValidData(txtIPAddress4.Text, CHECKDATA.IP_BYTE) &&
                     isValidData(txtPortNum.Text, CHECKDATA.PORT))
                {
                    //If we're good, start everything up
                    buttStartStop.Text = "Stop";
                    MainChessWindow.isNetworked = true;
                    txtMain.Enabled = true;
                    txtSend.Enabled = true;
                    buttSend.Enabled = true;

                    //Clear the messages from the main window
                    txtMain.Text = String.Empty;

                    txtIPAddress1.Enabled = false;
                    txtIPAddress2.Enabled = false;
                    txtIPAddress3.Enabled = false;
                    txtIPAddress4.Enabled = false;
                    txtPortNum.Enabled = false;
                    radioHost.Enabled = false;
                    radioJoin.Enabled = false;

                    //Build the IPAddress
                    String newAddress = txtIPAddress1.Text + "." + txtIPAddress2.Text + "." + txtIPAddress3.Text + "." + txtIPAddress4.Text;
                    int PortNum = Int32.Parse(txtPortNum.Text);

                    //Start up client or server depending on what the user has selected
                    if (radioHost.Checked)//Start server
                    {
                        _userName = "SERVER";

                        txtMain.AppendText("Server started, waiting for client . . . \n", _systemColor, _systemFont);
                        _chessServer = new Server(PortNum, newAddress);

                        //if the server failed to start correctly, send a message and shut everything down
                        if ( _chessServer.serverFailed() )
                        {
                            txtMain.AppendText("Server Failed To Start.  Someone may already be serving on this port.  Try a new port and double check your IP address! \n", Color.Blue, _systemFont);
                            stopServer();
                        }

                        //listen for the event indicating that the server failed and won't start communicating
                        //_chessServer.ServerFailed += _chessServer_ServerFailed;

                        //Start the processing timer
                        processingTimer.Start();
                    }
                    else  //Start client
                    {
                        _userName = "CLIENT";
                        _chessClient = new Client(PortNum, newAddress);
                        txtMain.AppendText("Client started, waiting for server . . . \n", _systemColor, _systemFont);

                        //Start the processing timer
                        processingTimer.Start();
                    }
                }
                else
                {
                    MessageBox.Show("Error, please check port and IP address");
                }
            }
        }

        //Regularly check for the connection status and for new messages if we're connected
        private void processingTimer_Tick(object sender, EventArgs e)
        {
            //Checks connection status
            updateConnectionStatus();

            //Check for new messages
            checkForNewMessages();
        }

        //This function will update the _connected variable if the state has changed on the server 
         //or client running, and outputs a message to the main window
        private void updateConnectionStatus()
        {
            bool newStatus;

            if (_chessServer != null)
            {
                newStatus = _chessServer.isConnected();

                //Only change the _connected status and send update if there has been a state change
                if (_connected != newStatus)
                {
                    if (newStatus)
                    {
                        txtMain.AppendText("Client connected to server! \n", Color.Blue, _systemFont);
                        _connected = true;

                        //Messages sent by server to prepare client to start a new game
                        sendGameplayMessage("RESTART");

                        //RESTART current chessboard
                        _mainWindowHandle.newGame();


                        if (MainChessWindow.firstPlayerColour == chessColour.BLACK)
                        {
                            sendGameplayMessage("BLACKFIRST");
                            
                            //By default, the server goes first
                            MainChessWindow.thisPlayerColour = chessColour.BLACK;
                        }
                        else
                        {
                            sendGameplayMessage("WHITEFIRST");

                            //By default, the server goes first
                            MainChessWindow.thisPlayerColour = chessColour.WHITE;
                        }
                    }
                    else
                    {
                        txtMain.AppendText("Client disconnected from server! \n", Color.Blue, _systemFont);
                        _connected = false;
                    }
                }
            }
            else if (_chessClient != null)
            {
                newStatus = _chessClient.isConnected();

                //Only change the _connected status and send update if there has been a state change
                if (_connected != newStatus )
                {
                    if (newStatus)
                    {
                        txtMain.AppendText("We have connected to server! \n", Color.Blue, _systemFont);
                        _connected = true;
                    }
                    else
                    {
                        txtMain.AppendText("The server has disconnected! \n", Color.Blue, _systemFont);
                        _connected = false;
                    }
                }
            }
        }

        //loops through available messages and calls processRecievedData when they're found
        private void checkForNewMessages()
        {
            String newMessage = "";

            if (_connected)     //no need to check for messages if we're not connected
            {
                if (null != _chessClient)
                {
                    //Get all the messages available
                    while (_chessClient.recvDataAvailable())
                    {
                        newMessage = _chessClient.recvData();
                        processRecievedData(newMessage);
                    }
                }
                else if (null != _chessServer)
                {
                    //Get all the messages available
                    while (_chessServer.recvDataAvailable())
                    {
                        newMessage = _chessServer.recvData();
                        processRecievedData(newMessage);
                    }
                }
            }
        }

        //Takes in a new message, determines if it's a text message or gameplay data, and handles it
        private void processRecievedData(String newMessage)
        {
            String[] tempStr;

            //Check if this is a text message from user to user first
            tempStr = Regex.Split(newMessage, _textMessageID);

            // The _textMessageID is located at the start of the message.
            // if it has been found, the first element of tempStr will be ""
            // and the second element will be the sent message.
            if (tempStr.Length > 1)
            {
                newMessage = tempStr[1] + "\n";     //add a new line character as this is stripped out by client/server sending
                txtMain.AppendText(newMessage, _userColor, _userFont);      //add the message to the main window
            }
            else
            {
                //check if this is a gameplay message with move information next
                tempStr = Regex.Split(newMessage, _gameMessageID);


                // The _gameMessageID is located at the start of the message.
                // If it has been found, the first element of tempStr will be ""
                // and the second element will be the message.
                if (tempStr.Length > 1)
                {
                    newMessage = tempStr[1];     //add a new line character as this is stripped out by client/server sending
                    processGameplayMessage(newMessage);
                }
            }
        }

        /*Gameplay commands supported:
         - RESTART
         - BLACKFIRST
         - WHITEFIRST
         - MOVE,1,1,2,2
         - DSEL
         - SEL,1,1*/
        private void processGameplayMessage(String message)
        {
            //only process messages if we're connected
            if (_connected)
            {
                switch (message)
                {
                    //restart the game
                    case "RESTART":
                        _mainWindowHandle.newGame();
                        break;

                    //black moves first - message recieved only by client
                    case "BLACKFIRST":
                        MainChessWindow.firstPlayerColour = chessColour.BLACK;

                        //By default, the server is the first player
                        MainChessWindow.thisPlayerColour = chessColour.WHITE;
                        break;

                    //white moves first - message recieved only by client
                    case "WHITEFIRST":
                        MainChessWindow.firstPlayerColour = chessColour.WHITE;

                        //By default, the server is the first player
                        MainChessWindow.thisPlayerColour = chessColour.BLACK;
                        break;

                    case "CLIENT_DISCONNECT":
                        txtMain.AppendText("Client has manually disconnected from server.\n");
                        break;

                    case "DSEL":
                        _mainWindowHandle.opponentSelMade = false;
                        _mainWindowHandle.updateDisplay();
                        break;

                    default:
                        //MOVE,oldX,oldY,newX,newY
                        processMoveCommand(message);

                        //SEL,1,2
                        processSelCommand(message);
                        break;
                }

                //Print message
                txtMain.AppendText(message + "\n", _commandColor, _systemFont);
            }
        }

        //MOVE,oldX,oldY,newX,newY
        private void processMoveCommand(String message)
        {
            String[] tempStr;
            tempStr = Regex.Split(message, ",");

            //Check for valid move command, assume we're good if there are five strings created
            if (tempStr.Length > 0 && tempStr[0] == "MOVE")
            {
                int oldx, oldy, newx, newy;
                oldx = int.Parse(tempStr[1]);
                oldy = int.Parse(tempStr[2]);
                newx = int.Parse(tempStr[3]);
                newy = int.Parse(tempStr[4]);

                //Move the piece to the place specified
                _mainWindowHandle.movePiece(oldx, oldy, newx, newy);
            }
        }

        //SEL,intX,intY
        private void processSelCommand(String message)
        {
            String[] tempStr;
            tempStr = Regex.Split(message, ",");

            //Check for valid move command, assume we're good if there are five strings created
            if (tempStr.Length > 0 && tempStr[0] == "SEL")
            {
                int newX = int.Parse(tempStr[1]);
                int newY = int.Parse(tempStr[2]);

                //Update the display of the selection
                _mainWindowHandle.opponentSelMade = true;
                _mainWindowHandle.opponentSelX = newX;
                _mainWindowHandle.opponentSelY = newY;
                _mainWindowHandle.updateDisplay();
            }
        }

        /*Gameplay commands supported:
         - RESTART
         - BLACKFIRST
         - WHITEFIRST
         - MOVE,oldX,oldY,newX,newY
         - SEL,intX,intY
         - DSEL*/
        private void sendGameplayMessage(String message)
        {
            //only send messages if we get a valid one
            if ( isValidGameplayMessage(message) )
            {
                message = message + "\n";  //all messages must be terminated with a newline to be read properly

                //only send to the client or server that has been created
                if (_chessClient != null)
                {
                    _chessClient.sendData(_gameMessageID + message);
                }
                else if (_chessServer != null)
                {
                    _chessServer.sendData(_gameMessageID + message);
                }

                //after sending the message to the other player, display it for the
                //user who typed it to see as well
                txtMain.AppendText(message, _commandColor, _systemFont);
            }
        }

        //Tests if a gameplay message is valid
        private bool isValidGameplayMessage(String message)
        {
            bool retVal = false;

            if (_connected)  //don't process messages if we're not connected
            {
                switch (message)
                {
                    case "RESTART":
                    case "BLACKFIRST":
                    case "WHITEFIRST":
                    case "CLIENT_DISCONNECT":
                    case "DSEL":
                        retVal = true;
                        break;
                    default:
                        //CHECK FOR A MOVE COMMAND
                        String[] tempStr;
                        tempStr = Regex.Split(message, ",");

                        //There should be five strings after the split for a MOVE command
                        if (tempStr.Length == 5)
                        {
                            //A valid move command will contain 'MOVE' as the first string after the split command
                            if (tempStr[0] == "MOVE")
                            {
                                try
                                {
                                    //store the new and old X/y positions in an array for error testing
                                    int[] testInt = new int[5];

                                    for (int i = 1; i < 5; i++)
                                    {
                                        testInt[i] = int.Parse(tempStr[i]);

                                        //all chess positions must be from 0 - 7
                                        if (testInt[i] < 0 || testInt[i] > 8)
                                        {
                                            return false;
                                        }
                                    }
                                }
                                catch
                                {
                                    //Any error parsing means invalid data
                                    return false;
                                }

                                //Passed all the checks, this is a good value!
                                retVal = true;
                            }
                        }

                        //CHECK FOR A SEL COMMAND (should be 3 arguments)
                        else if (tempStr.Length == 3)
                        {
                            if (tempStr[0] == "SEL")
                            {
                                try
                                {
                                    int testX = int.Parse(tempStr[1]);
                                    int testY = int.Parse(tempStr[2]);

                                    //all chess positions must be from 0 - 7
                                    if (testX < 0 || testX > 8 || testY < 0 || testY > 8)
                                    {
                                        return false;
                                    }
                                }
                                catch
                                {
                                    //Any error parsing means invalid data
                                    return false;
                                }

                                //Passed all the checks, this is a good value!
                                retVal = true;
                            }
                        }
                        break;
                }
            }

            return retVal;
        }

//User input tooltip warnings-------------------------------------------------------------
        
        //Checks if the string input is a valid byte for an IP Address (between 0 - 255)
        private bool isValidData(String newData, CHECKDATA type)
        {
            try
            {
                int newInt = Int32.Parse(newData);

                //testing for IP data
                if (CHECKDATA.IP_BYTE == type)
                {
                    if (newInt >= 0 && newInt < 255)
                    {
                        return true;
                    }
                }
                else if (CHECKDATA.PORT == type)
                {
                    if (newInt > 1024 && newInt <= 65535)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                //all user input errors will return false
            }


            return false;
        }

        private void txtIPAddress1_leave(object sender, EventArgs e)
        {
            if (!isValidData(txtIPAddress1.Text, CHECKDATA.IP_BYTE))
            {
                toolTip1.ToolTipTitle = "Invalid number entered";
                toolTip1.Show("Please enter a value between 0 and 255", txtIPAddress1);
            }
        }

        private void txtIPAddress2_leave(object sender, EventArgs e)
        {
            if (!isValidData(txtIPAddress1.Text, CHECKDATA.IP_BYTE))
            {
                toolTip1.ToolTipTitle = "Invalid number entered";
                toolTip1.Show("Please enter a value between 0 and 255", txtIPAddress1);
            }
        }

        private void txtIPAddress3_leave(object sender, EventArgs e)
        {
            if (!isValidData(txtIPAddress1.Text, CHECKDATA.IP_BYTE))
            {
                toolTip1.ToolTipTitle = "Invalid number entered";
                toolTip1.Show("Please enter a value between 0 and 255", txtIPAddress2);
            }
        }

        private void txtIPAddress4_leave(object sender, EventArgs e)
        {
            if (!isValidData(txtIPAddress1.Text, CHECKDATA.IP_BYTE))
            {
                toolTip1.ToolTipTitle = "Invalid number entered";
                toolTip1.Show("Please enter a value between 0 and 255", txtIPAddress3);
            }
        }

        private void txtPortNum_keydown(object sender, EventArgs e)
        {
            if (!isValidData(txtPortNum.Text, CHECKDATA.PORT))
            {
                toolTip1.ToolTipTitle = "Invalid port entered";
                toolTip1.Show("Please enter a value between 1025 and 65535", txtPortNum);
            }
        }

        private void txtIPAddress1_keydown(object sender, KeyEventArgs e)
        {
            toolTip1.Hide(txtIPAddress1);  //hides the tool tip if user starts typing again in the box
        }

        private void txtIPAddress2_keydown(object sender, KeyEventArgs e)
        {
            toolTip1.Hide(txtIPAddress2);  //hides the tool tip if user starts typing again in the box
        }

        private void txtIPAddress3_keydown(object sender, KeyEventArgs e)
        {
            toolTip1.Hide(txtIPAddress3);  //hides the tool tip if user starts typing again in the box
        }

        private void txtIPAddress4_keydown(object sender, KeyEventArgs e)
        {
            toolTip1.Hide(txtIPAddress4);  //hides the tool tip if user starts typing again in the box
        }

        private void txtPortNum_keydown(object sender, KeyEventArgs e)
        {
            toolTip1.Hide(txtPortNum); //hides the tool tip if user starts typing again in the box
        }

//End of User input tooltip warnings-------------------------------------------------------------    


//Text Messaging functions---------------------------------------------------------------------
        
        //When user presses send, send message
        private void buttSend_Click(object sender, EventArgs e)
        {
            sendTextMessage();
        }

        //When user types ENTER, send message
        private void txtSend_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                sendTextMessage();
            }
        }

        //Sends text message to client or server
        private void sendTextMessage()
        {
            String message = "[" + _userName + "]:" + txtSend.Text + "\n";

            //clear the txtSend message box for more messages to be written
            txtSend.Text = "";


            //only try to send messages if we're connected
            if (_connected)
            {
                //only send to the client or server that has been created
                if (_chessClient != null)
                {
                    _chessClient.sendData(_textMessageID + message);
                }
                else if (_chessServer != null)
                {
                    _chessServer.sendData(_textMessageID + message);
                }

                //after sending the message to the other player, display it for the
                //user who typed it to see as well
                txtMain.AppendText(message, _userColor, _userFont);
            }
        }

        //Always scroll to bottom of text messages in main display
        private void txtMain_TextChanged(object sender, EventArgs e)
        {
            txtMain.SelectionStart = txtMain.Text.Length;
            txtMain.ScrollToCaret();
        }

//End of Text Messaging functions---------------------------------------------------------------------

//Custom Event Handlers-------------------------------------------------------------------
        //Execute code when the server connects to the client
        void _chessServer_ServerFailed(object sender, EventArgs e)
        {
            txtMain.AppendText("Server Failed To Start.  Someone may already be serving on this port.  Try a new port and double check your IP address! \n", Color.Blue, _systemFont);
            stopServer();
        }

        //Send update to other player when a chess piece has been moved
        void _mainWindowHandle_chessMoveEvent(object sender, chessMoveArgs e)
        {
            // builds a move message in the form:
            // MOVE,oldX,oldY,newX,newY
            String message = "MOVE," + e.oldX + "," + e.oldY + "," + e.newX + "," + e.newY;

            sendGameplayMessage(message);
        }

        //Sends the selected position, or deselect based on your actions
        void _mainWindowHandle_selectionMadeEvent(object sender, selectionMadeArgs e)
        {
            String message;
            if (e.selection)
            {
                //SEL,intX,intY
                message = "SEL," + e.intX + "," + e.intY;
            }
            else
            {
                message = "DSEL";
            }

            sendGameplayMessage(message);
        }
//End of Custom Event Handlers-------------------------------------------------------------------
    }
}
