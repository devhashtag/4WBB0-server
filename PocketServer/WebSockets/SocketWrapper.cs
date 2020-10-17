using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PocketServer.DataAccess.Entities;
using PocketServer.DataAccess.Services;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PocketServer.DataAccess;
using Newtonsoft.Json;
using System.Text.Json;
using PocketServer.DataAccess.Core;

namespace PocketServer.WebSockets
{
    public enum Request
    {
        REQUEST_ID,
        ID,
        GET_DEVICES_INFORMATION,
        GET_DEVICE_INFORMATION,
        GET_DEVICE_HISTORY,
        GET_DEVICE_LOCATION,
        START_LIVE_LOCATION,
        STOP_LIVE_LOCATION,
        ADD_DEVICE,
        UPDATE_DEVICE_INFORMATION,
        SEND_MEDICINE_REMINDER,
        REMOVE_MEDICINE_REMINDER
    }

    public enum Response
    {
        OK,
        BAD_REQUEST,
        ID,
        DEVICES_INFORMATION,
        DEVICE_HISTORY,
        DEVICE_LOCATION,
        LIVE_LOCATION,
        ALERT,
        CANCEL_ALERT,
    }

    public class SocketWrapper
    {
        public class Message
        {
            public Message()
            {
                MessageId = Guid.NewGuid().ToString();
            }

            public Message(string messageId)
            {
                MessageId = messageId;
            }

            public string MessageId { get; set; }
            public string Command { get; set; }
            public string Argument { get; set; }

            public static Message FromString(string text)
            {
                // Get ID
                int index = text.IndexOf("\n");

                if (index == -1)
                {
                    throw new ArgumentException("Given string must contain an Id and a command");
                }

                var message = new Message(text.Substring(0, index));

                // Get Command
                text = text.Substring(index + 1);
                index = text.IndexOf("\n");

                if (index == -1)
                {
                    message.Command = text;
                    return message;
                }

                // Get Argument
                message.Command = text.Substring(0, index);
                message.Argument = text.Substring(index + 1);

                return message;
            }

            public override string ToString()
            {
                return Argument == null
                    ? MessageId + "\n" + Command
                    : MessageId + "\n" + Command + "\n" + Argument;
            }
        }

        public bool IsClosed { get => _webSocket.State == WebSocketState.Closed; }

        private static JsonSerializerSettings defaultSerializationSettings = new JsonSerializerSettings()
        {
            ContractResolver = new IgnoreVirtualResolver(),
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        private readonly WebSocket _webSocket;
        private readonly CancellationToken _cancellationToken;
        private readonly IServiceScopeFactory _scopeFactory;

        public string UserId { get; private set; } = null;

        public SocketWrapper(WebSocket webSocket, IHostApplicationLifetime applicationLifetime, IServiceScopeFactory scopeFactory)
        {
            _webSocket = webSocket;
            _cancellationToken = applicationLifetime.ApplicationStopping;
            _scopeFactory = scopeFactory;
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

            if (result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine("WebSocket state: " + _webSocket.State.ToString());
                // Bad practice, but we need to terminate the thread
                throw new Exception("Connection closed");
            }

            return Encoding.UTF8.GetString( buffer.Take(result.Count).ToArray() );
        }

        public async Task<Message> ReceiveMessage()
        {
            var result = await Receive();

            return Message.FromString(result);
        }

        public async Task SendMessage(Message message)
        {
            await Send(message.ToString());
        }

        public async Task HandleMessage(Message message)
        {
            switch (Enum.Parse(typeof(Request), message.Command))
            {
                case Request.REQUEST_ID: 
                    await RequestId(message);
                    break;
                case Request.ID:
                    await Id(message);
                    break;
                case Request.GET_DEVICES_INFORMATION:
                    await DevicesInformation(message);
                    break;
                case Request.GET_DEVICE_HISTORY:
                    await DeviceHistory(message);
                    break;
                case Request.GET_DEVICE_LOCATION:
                    await DeviceLocation(message);
                    break;
                case Request.ADD_DEVICE:
                    await AddDevice(message);
                    break;
                case Request.UPDATE_DEVICE_INFORMATION:
                    await UpdateDevice(message);
                    break;
                default:
                    Console.Error.WriteLine($"Should never happen: Command ${message.Command} in HandleMessage not recognized");
                    break;
            }
        }

        public async Task RequestId(Message message)
        {
            using var scope = _scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetService<UserService>();

            var user = new User();

            await userService.AddAsync(user);
            await SendMessage(new Message() {
                MessageId = message.MessageId,
                Command = Response.ID.ToString(),
                Argument = user.Id
            });

            UserId = user.Id;
        }

        public async Task Id(Message message)
        {
            using var scope = _scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetService<UserService>();

            if (await userService.ExistsAsync(message.Argument))
            {
                await SendMessage(new Message()
                {
                    MessageId = message.MessageId,
                    Command = Response.OK.ToString()
                });

                UserId = message.Argument;
            } else
            {
                await SendMessage(new Message()
                {
                    MessageId = message.MessageId,
                    Command = Response.BAD_REQUEST.ToString(),
                    Argument = "Provided ID does not exist in the database. Please request a new ID"
                });
            }
        }
    
        public async Task DevicesInformation(Message message)
        {
            using var scope = _scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetService<UserService>();

            var devices = userService.GetById(UserId)
                .Subscriptions.Select(s => s.Device)
                .ToList();

            string deviceInformation = JsonConvert.SerializeObject(devices, defaultSerializationSettings);

            await SendMessage(new Message()
            {
                MessageId = message.MessageId,
                Command = Response.DEVICES_INFORMATION.ToString(),
                Argument = deviceInformation
            });
        } 
        
        public async Task DeviceInformation(Message message)
        {
            using var scope = _scopeFactory.CreateScope();
            var deviceService = scope.ServiceProvider.GetService<DeviceService>();

            var device = deviceService.GetById(message.Argument);

            if (device == null)
            {
                await SendMessage(new Message()
                {
                    MessageId = message.MessageId,
                    Command = Response.BAD_REQUEST.ToString(),
                    Argument = "Device does not exist"
                });
            } else
            {
                var serializedDevice = JsonConvert.SerializeObject(device, defaultSerializationSettings);

                await SendMessage(new Message()
                {
                    MessageId = message.MessageId,
                    Command = Response.OK.ToString(),
                    Argument = serializedDevice
                });
            }
        }
    
        public async Task DeviceHistory(Message message)
        {
            using var scope = _scopeFactory.CreateScope();
            var deviceService = scope.ServiceProvider.GetService<DeviceService>();

            var deviceId = message.Argument;

            if (! deviceService.Exists(deviceId))
            {
                await SendMessage(new Message()
                {
                    MessageId = message.MessageId,
                    Command = Response.BAD_REQUEST.ToString(),
                    Argument = "Device with given Id does not exist"
                });

                return;
            }

            var alerts = deviceService.GetById(deviceId).Alerts;
            var serializedAlerts = JsonConvert.SerializeObject(alerts, defaultSerializationSettings);

            await SendMessage(new Message()
            {
                MessageId = message.MessageId,
                Command = Response.OK.ToString(),
                Argument = serializedAlerts
            });
        }
        
        public async Task DeviceLocation(Message message)
        {
            using var scope = _scopeFactory.CreateScope();
            var deviceService = scope.ServiceProvider.GetService<DeviceService>();

            var deviceId = message.Argument;

            var lastHeartbeat = deviceService.GetById(deviceId)
                .Heartbeats
                .OrderByDescending(h => h.Timestamp)
                .FirstOrDefault();

            if (! deviceService.Exists(deviceId) || lastHeartbeat == null)
            {
                await SendMessage(new Message()
                {
                    MessageId = message.MessageId,
                    Command = Response.BAD_REQUEST.ToString(),
                    Argument = "Given DeviceId does not exist or has no heartbeats"
                });

                return;
            }

            var serializedHeartbeat = JsonConvert.SerializeObject(lastHeartbeat, defaultSerializationSettings);

            await SendMessage(new Message()
            {
                MessageId = message.MessageId,
                Command = Response.OK.ToString(),
                Argument = serializedHeartbeat
            });
        }
    
        public async Task AddDevice(Message message)
        {
            using var scope = _scopeFactory.CreateScope();
            var deviceService = scope.ServiceProvider.GetService<DeviceService>();

            Device device;

            try
            {
                device = JsonConvert.DeserializeObject<Device>(message.Argument, defaultSerializationSettings);
                
            } catch (JsonSerializationException exception)
            {
                await SendMessage(new Message()
                {
                    MessageId = message.MessageId,
                    Command = Response.BAD_REQUEST.ToString(),
                    Argument = "Could not deserialize argument: " + exception.Message
                });

                return;
            }

            deviceService.Add(device);

            var serializedDevice = JsonConvert.SerializeObject(device, defaultSerializationSettings);

            await SendMessage(new Message()
            {
                MessageId = message.MessageId,
                Command = Response.OK.ToString(),
                Argument = serializedDevice
            });
        }

        public async Task UpdateDevice(Message message)
        {
            using var scope = _scopeFactory.CreateScope();
            var deviceService = scope.ServiceProvider.GetService<DeviceService>();

            Device device;

            try
            {
                device = JsonConvert.DeserializeObject<Device>(message.Argument, defaultSerializationSettings);
            } catch (JsonSerializationException exception)
            {
                await SendMessage(new Message()
                {
                    MessageId = message.MessageId,
                    Command = Response.BAD_REQUEST.ToString(),
                    Argument = "Could not deserialize argument: " + exception.Message
                });

                return;
            }

            deviceService.Update(device);

            await SendMessage(new Message()
            {
                MessageId = message.MessageId,
                Command = Response.OK.ToString(),
                Argument = message.Argument
            });
        }

        public async Task AddMedicineReminder(Message message)
        {

        }

        public async Task RemoveMedicineReminder(Message message)
        {

        }

        public async Task StartLiveLocation(Message message)
        {

        }

        public async Task StopLiveLocation(Message message)
        {

        }
    }
}
