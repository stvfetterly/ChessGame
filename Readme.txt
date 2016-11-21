Chess:

This is a chess program implemented in C#.  Two people can play on the same computer, or connect over a LAN (using IP and port).



Files:
ChessClockWindow.cs - used to display the total time taken by each player and colour of the person playing
Chesspiece.cs - Contains the Chesspieces and associated logic for movement, calculating position on chess board
Gameplay.cs - Contains information about whose turn it is, and timing information for use with the clock
MainChessWindow.cs - Displays the chessboard/pieces, takes user click events which drive the game
\Networking\Client.cs - Simple client connects to a server and allows for send/recieve of strings
\Networking\Server.cs - Simple server connects to a single client and allows for send/recieve of strings
\Networking\NetworkingWindow.cs - Displays the networking window, allows players to text each other, sends and handles gameplay commands
\Networking\RichTextboxExtensions.cs - Override rich text box extensions so that modifying font and colour is easier



TODO:
- User selectable move display (for future debugging), currently all commands sent are logged and displayed
- Error logging for future debugging



KNOWN BUGS TO FIX:
- Network connection issue (user starts a server, user quits, user starts a client, client connects to own server)