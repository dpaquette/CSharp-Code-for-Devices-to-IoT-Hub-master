﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace SendDirectMethodCall
{
    class Program
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=YourIoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=JustCopyTheWholeConnectionString";
        static string deviceName = "mySimulatedDevice";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

            Console.WriteLine("Press any key to send a C2D direct message.");
            while (true)
            {
                Console.ReadLine();
                await SendCloudToDeviceDirectAsync();
            }
        }
        static async Task SendCloudToDeviceDirectAsync()
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device direct."));
            commandMessage.Ack = DeliveryAcknowledgement.Full;

            LEDStatusCommand ledStatusCommand = new LEDStatusCommand
            {
                Time = DateTime.Now.ToString(),
                Source = "Azure Function Direct Method Call"
            };

            CloudToDeviceMethod methodInvocation = new CloudToDeviceMethod("DMToggleLED") { ResponseTimeout = TimeSpan.FromSeconds(30) };

            CloudToDeviceMethodResult r = await serviceClient.InvokeDeviceMethodAsync(deviceName, methodInvocation);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Direct Method result '{r.Status}', Payload '{r.GetPayloadAsJson()}' ");
            Console.ResetColor();
        }
    }
}

