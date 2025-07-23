# Moddable Web Server

### About
This project is made because I used more than 2 project the NetCoreServer Library and each project is used to make a Server that support more C# DLL to be loaded.

I am really bad at making a Good Documentation. Please help!
# Using the Library

### Response Creator
Creating response simple.

Make sure you adding like this:\
Header\
Cookie\
Body

```csharp
//here you create a simple 200 (OK) response.
ResponseCreator response = new();
//adding one Header to it.
response.SetHeader("key","value");
//adding multiple header to it.
response.SetHeaders(
	new Dictionary<string, string>() 
	{
		{ "key", "Value" }
	}
);

//adding content to it.
response.SetBody("Hello World?");

//Getting the HttpResponse.
_ = response.GetResponse();

//we can use the already made creator for start to make a new one.
response.New(404);
```

If you ever will need, you can use `response.SetResponse`, but not suggesting it.
### Use of HTTPAttribute

```csharp
[HTTP("Method", "Url")]
```
In Method can be used any HTTP/S Method.\
GET, POST, HEAD, OPTION, DELETE is accepted.

In the Url you can use the full url, or making it as parameter\
If you use parameter you can use `?{!args}` to parse every arg into the parameters

```csharp
//url
[HTTP("GET", "/full/url")]

//parameter
[HTTP("GET", "/hey/{parameters}")]

//parameter and normal args
[HTTP("GET", "/hey/{parameters}?user={username}")]

//parameter and args
[HTTP("GET", "/hey/{parameters}?{!args}")]
```

Make sure your Function must be STATIC and must return a BOOL.


```csharp
[HTTP("GET", "/test")]
public static bool test(ServerSender sender)
{
	//simple response making
    sender.Response.MakeOkResponse();
    sender.SendResponse();
	//return is if the page exist or not. It will try to send 404 if false.
	return true;
}
```

Example or Return a false:

```csharp
[HTTP("GET", "/throw404")]
public static bool throw_404(ServerSender _)
{
	return false;
}
```


## WS(S)

### Use of WSAttribute

In the Url you can use the full url, or making it as parameter

Make sure your Function is STATIC, and must a VOID.

```csharp
 [WS("/ws/{test}")]
 public static void ws(WebSocketStruct webStruct)
 {
	 //bunch of stuff for printing.
     Console.WriteLine(webStruct.IsConnected);
     Console.WriteLine(webStruct.Request.Url);
     Console.WriteLine(webStruct.WSRequest?.buffer);
     Console.WriteLine(webStruct.WSRequest?.offset);
     Console.WriteLine(webStruct.WSRequest?.size);
	 //answering in the websocket.
     webStruct.SendWebSocketText("test");
 }
```

## Making a server.
Here you gonna see how we make a server and using it.
### Certificate for Secure server.

Make sure you have a PFX file in a disk, and a password for that pfx file (The PFX file must use a password!)

Using Fields Neccesary:

```csharp
//add this into the before the classname
using System.Security.Authentication;
using ModdableWebServer.Helper;
```

You can use any SslProtocol for this, I always use TLS 1.2

```csharp
//Get Context with Validation
CertHelper.GetContext( SslProtocols.Tls12, "mypfx.pfx", "asecurepassword");

//Get Context with No Validation
CertHelper.GetContextNoValidate( SslProtocols.Tls12, "mypfx.pfx", "asecurepassword");
```

### Creation

You can use any of it to make a sever

```csharp
var context = CertHelper.GetContext( SslProtocols.Tls12, "mypfx.pfx", "asecurepassword");
var ws_server = new WS_Server("127.0.0.1", 6666);
var wss_server = new WSS_Server(context, "127.0.0.1", 6667);
var http_server = new HTTP_Server("127.0.0.1", 6668);
var https_server = new HTTPS_Server(context, "127.0.0.1", 6669);
```

### Starting and Stopping.

Simply just use Start(), or Stop()

```csharp
ws_server.Start();
ws_server.Stop();
```

### Adding our created Attributes to the Server

Merging (Will fail if other DLL already contains url/method)
```csharp
// Loading Entry assembly contained HTTP/HTTPHeader/WS Attribute and merging.
ws_server.HTTPAttributeToMethods.Merge(Assembly.GetEntryAssembly());
// ONLY available for WebSocket servers.
ws_server.WSAttributeToMethods.Merge(Assembly.GetEntryAssembly()); 
```

Overriding.
It will replace already existing ones.
```csharp
// Loading Entry assembly contained HTTP/WS Attribute and merging.
ws_server.HTTPAttributeToMethods.Override(Assembly.GetEntryAssembly());
ws_server.WSAttributeToMethods.Override(Assembly.GetEntryAssembly());
```

### Assembly Difference
This is so Technical I needed to check like 5 times.

We have 4 Assembly:
ModdableWebServer (MWS)\
MyServerLib (Lib)\
MyServerExtenstion (Extension)\
MyServerConsole (Console)

Console starts a Lib.\
Lib starts MWS.\
Lib starts Extension.

Extenstion Contain a Class: EXT\
Lib Contains a Class: LIBC\
Console Contains a Class: CONC

GetEntryAssembly = Console\
GetCallingAssembly On Extension = Lib\
GetCallingAssembly On Lib = Console\
GetExecutingAssembly On Lib = Lib\
GetExecutingAssembly On Extension = Extension\
GetExecutingAssembly On Console = Console\
GetAssembly(typeof(EXT)) = Extension\
GetAssembly(typeof(LIBC)) = Lib\
GetAssembly(typeof(CONC)) = Console

Use the GetAssembly and TypeOf if you dont know what Assembly loaded what!
