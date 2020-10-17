using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PocketServer.DataAccess.Entities;
using PocketServer.DataAccess.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace PocketServer.WebSockets
{
    public class WebSocketHandler
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ConcurrentDictionary<string, SocketWrapper> _webSockets = new ConcurrentDictionary<string, SocketWrapper>();
        private readonly IServiceScopeFactory _scopeFactory;

        public WebSocketHandler(IHostApplicationLifetime applicationLifetime, IServiceScopeFactory scopeFactory)
        {
            _applicationLifetime = applicationLifetime;
            _scopeFactory = scopeFactory;
        }

        public void Add(WebSocket webSocket, TaskCompletionSource<object> taskCompletionSource)
        {
            var socket = new SocketWrapper(webSocket, _applicationLifetime, _scopeFactory);

            Thread thread = new Thread(async () =>
            {
                try
                {
                    await HandleSocket(socket);
                } catch (Exception ex)
                {
                    // Probably a closed connection
                    Console.WriteLine("Exception: " + ex.ToString());
                }

                taskCompletionSource.SetResult(0);
            });
            thread.Start();
        }

        private async Task HandleSocket(SocketWrapper socket)
        {
            Console.WriteLine("In handle socket");
            while (! socket.IsClosed)
            {
                Console.WriteLine("socket is not closed");
                // Receive and handle commands
                var message = await socket.ReceiveMessage();

                Console.WriteLine("Received message:\n" + message.ToString());

                await socket.HandleMessage(message);

                Console.WriteLine("Message handled");

                // Check if user is identifier
                if (socket.UserId != null)
                {
                    _webSockets.GetOrAdd(socket.UserId, socket);
                }
                
                // Prevent 100% CPU-usage
                Thread.Sleep(50);
            }

            _webSockets.Remove(socket.UserId, out socket);
        }
    }
}
