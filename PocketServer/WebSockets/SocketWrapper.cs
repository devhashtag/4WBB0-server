using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PocketServer.WebSockets
{
    public class SocketWrapper
    {
        public class Message
        {
            public string Command { get; set; }
            public string Argument { get; set; }
        }

        public bool IsClosed { get => _webSocket.State == WebSocketState.Closed; }

        public const string REQUEST_ID = "REQUEST_ID";
        public const string ID = "ID";

        private readonly WebSocket _webSocket;
        private readonly CancellationToken _cancellationToken;

        public SocketWrapper(WebSocket webSocket, IHostApplicationLifetime applicationLifetime)
        {
            _webSocket = webSocket;
            _cancellationToken = applicationLifetime.ApplicationStopping;
        }

        private async Task Send(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);

            await _webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, _cancellationToken);
        }

        private async Task<string> Receive()
        {
            byte[] buffer = new byte[4 * 1024];

            var result = await _webSocket.ReceiveAsync(buffer, _cancellationToken);

            return Encoding.UTF8.GetString( buffer.Take(result.Count).ToArray() );
        }

        public async Task<Message> ReceiveMessage()
        {
            var result = await Receive();
            var index = result.IndexOf('\n');

            var argument = index == -1
                ? null
                : result.Substring(index + 1);

            return new Message()
            {
                Command = result.Substring(0, index),
                Argument = argument
            };
        }

        public async Task SendMessage(Message message)
        {
            await Send(message.Command + "\n" + message.Argument);
        }

        public async Task HandleError(string error)
        {
            await Send(error);
            await _webSocket.CloseAsync(WebSocketCloseStatus.Empty, error, _cancellationToken);
        }

        // Returns true if the message is not malformatted, and closes the connection if it is
        public bool ValidateMessage(Message message)
        {
            switch (message.Command)
            {
                case REQUEST_ID:
                    return message.Argument == null;
                case ID:
                    return message.Argument != null;
                default:
                    HandleError("Command not recognized");
                    return false;
            }

            return false; 
        } 
    }
}
