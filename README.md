# EasyTcp

Based on the example from MSDN (https://msdn.microsoft.com/en-us/library/fx6588te(v=vs.110).aspx).

### My Changes:
* Instead of passing strings around there's a Request/Response model.
* The connections don't close but are instead stored in collection. It's the job of the server to burn stale connections (some how....)
* For extension there's a "ProcessStrategy" that allows you to write a preferred way for requests to be processed and responses to be generated.
* For the Client the Request and Response is syncronous. However it sets up an asyncronous listener to recieve from the server at any time.

### To test:
  * Build.
  * Run the server .exe 
  * Run the client.
  * Send messages.

### To extend:
* Use the IProcessStrategy and write something that can handle requests. Pass your strategy in when you new up the server.
