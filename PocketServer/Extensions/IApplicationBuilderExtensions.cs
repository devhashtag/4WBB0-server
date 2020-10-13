using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PocketServer.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PocketServer.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder AddWebSockets(this IApplicationBuilder app)
        {
            // Explicitly assign default parameters
            return app.UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            })
            .UseMiddleware<WebSocketMiddleware>();
        }

        //private static async Task Echo(HttpContext context, WebSocket webSocket)
        //{
        //    var buffer = new byte[1024 * 4];
        //    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        //    while (!result.CloseStatus.HasValue)
        //    {
        //        Console.WriteLine("Received a message: " + Encoding.UTF8.GetString(buffer, 0, buffer.Length));
        //        await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

        //        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //    }

        //    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        //}
    }
}
