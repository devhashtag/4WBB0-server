using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace PocketServer.WebSockets
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketHandler handler;

        public WebSocketMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            handler = webSocketHandler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (! (context.WebSockets.IsWebSocketRequest && context.Request.Path == "/websocket"))
            {
                await _next(context);
                return;
            }

            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var socketFinishedTcs = new TaskCompletionSource<object>();

            handler.Add(webSocket, socketFinishedTcs);

            await socketFinishedTcs.Task;
        }
    }
}
