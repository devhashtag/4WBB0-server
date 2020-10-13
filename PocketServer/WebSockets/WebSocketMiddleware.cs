using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace PocketServer.WebSockets
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly WebSocketHandler handler;

        public WebSocketMiddleware(RequestDelegate next, IHostApplicationLifetime applicationLifetime, WebSocketHandler webSocketHandler)
        {
            _next = next;
            _applicationLifetime = applicationLifetime;
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


        }
    }
}
