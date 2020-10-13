using Microsoft.Extensions.Hosting;
using PocketServer.DataAccess.Entities;
using PocketServer.DataAccess.Services;
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
        private readonly UserService _userService;

        public WebSocketHandler(IHostApplicationLifetime applicationLifetime, UserService userService)
        {
            _applicationLifetime = applicationLifetime;
            _userService = userService;
        }

        public async Task Add(WebSocket webSocket)
        {
            var socket = new SocketWrapper(webSocket, _applicationLifetime);

            var userId = await GetUserId(socket);

            if (userId == null || _webSockets.ContainsKey(userId))
            {
                await socket.HandleError("A user with this ID is already connected");
                return;
            }

            _webSockets.GetOrAdd(userId, socket);

            Thread thread = new Thread(async () => await HandleSocket(userId));
            thread.Start();
        }

        private async Task<string> GetUserId(SocketWrapper socket)
        {
            // We wait on a message that contains either an Id request, or an Id
            var message = await socket.ReceiveMessage();

            if (! socket.ValidateMessage(message))
            {
                return null;
            }

            switch (message.Command)
            {
                case SocketWrapper.REQUEST_ID:
                    var user = new User();

                    await _userService.AddAsync(user);
                    await socket.SendMessage(new SocketWrapper.Message()
                    {
                        Command = SocketWrapper.ID,
                        Argument = user.Id
                    });
                    break;
                case SocketWrapper.ID:
                    if (await _userService.ExistsAsync(message.Argument)) return message.Argument;

                    await socket.HandleError("Provided ID does not exist in the database. Please request a new ID");
                    break;
                default:
                    await socket.HandleError("Unexpected command: " + message.Command);
                    break;
            }

            return null;
        }

        private async Task HandleSocket(string userId)
        {
            var socket = _webSockets[userId];

            while (! socket.IsClosed)
            {
                // Receive and handle commands
                Thread.Sleep(50);
            }

            _webSockets.Remove(userId, out socket);
        }
    }
}
