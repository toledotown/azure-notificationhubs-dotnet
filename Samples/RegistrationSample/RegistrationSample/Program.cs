// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Configuration;

namespace RegistrationSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Getting connection key from the new resource
            var config = LoadConfiguration(args);
            var nhClient = NotificationHubClient.CreateClientFromConnectionString(config.PrimaryConnectionString, config.HubName);
            await CreateInstallationAsync(nhClient);
            //await CreateAndDeleteInstallationAsync(nhClient);
            //await CreateAndDeleteRegistrationAsync(nhClient);
        }

        private static async Task CreateAndDeleteRegistrationAsync(NotificationHubClient nhClient)
        {
            var registrationId = await nhClient.CreateRegistrationIdAsync();
            //var registrationDescr = await nhClient.CreateFcmNativeRegistrationAsync(registrationId);
            var registrationDescr = await nhClient.CreateAppleNativeRegistrationAsync(registrationId);
            //Console.WriteLine($"Created APNS registration {registrationDescr.FcmRegistrationId}");
            Console.WriteLine($"Create APNS registration {registrationDescr.RegistrationId}");

            var allRegistrations = await nhClient.GetAllRegistrationsAsync(1000);
            foreach (var regFromServer in allRegistrations)
            {
                if (regFromServer.RegistrationId == registrationDescr.RegistrationId)
                {
                    //Console.WriteLine($"Found FCM registration {registrationDescr.FcmRegistrationId}");
                    Console.WriteLine($"Found APNS registration {registrationDescr.RegistrationId}");
                    break;
                }
            }

            //commented out by code creator (next 2 lines)
            //registrationDescr = await nhClient.GetRegistrationAsync<FcmRegistrationDescription>(registrationId);
            //Console.WriteLine($"Retrieved FCM registration {registrationDescr.FcmRegistrationId}");

            await nhClient.DeleteRegistrationAsync(registrationDescr);
            //Console.WriteLine($"Deleted FCM registration {registrationDescr.FcmRegistrationId}");
            Console.WriteLine($"Deleted APNS registration {registrationDescr.RegistrationId}");
        }

        private static async Task CreateInstallationAsync(NotificationHubClient nhClient)
        {
            // Register some fake devices
            //var fcmDeviceId = Guid.NewGuid().ToString();
            //var apnsDeviceId = "djPUqVGjd9I:APA91bE7UyqaMr0IAICU7Wz5eV5qUEaZ2nEA7YkFcz6UqCHv-nIkUNDBAE4cZdjjG5bw8WnT2Psu7-eV4GMURVXtMGzI_yju2VRAzaTShW-9xwW2qy9D_SPIRsD8xVLalVklQgy_xara";
            var apnsDeviceId = "c61cd9eea3d2def2";
            Console.WriteLine($"The APNS device ID is {apnsDeviceId}");
                //var fcmInstallation = new Installation
                var apnsInstallation = new Installation
                {
                    //InstallationId = fcmDeviceId,
                    InstallationId = apnsDeviceId,
                    //Platform = NotificationPlatform.Fcm,
                    Platform = NotificationPlatform.Apns,
                    //PushChannel = fcmDeviceId,
                    PushChannel = apnsDeviceId,
                    PushChannelExpired = false,
                    //Tags = new[] { "fcm" }
                    Tags = new[] { "apns" }
                };
                //await nhClient.CreateOrUpdateInstallationAsync(fcmInstallation);
                await nhClient.CreateOrUpdateInstallationAsync(apnsInstallation);

                while (true)
                {
                    try
                    {
                        //var installationFromServer = await nhClient.GetInstallationAsync(fcmInstallation.InstallationId);
                        var installationFromServer = await nhClient.GetInstallationAsync(apnsInstallation.InstallationId);
                        break;
                    }
                    catch (MessagingEntityNotFoundException)
                    {
                        // Wait for installation to be created
                        await Task.Delay(1000);
                    }
                }
                //Console.WriteLine($"Created FCM installation {fcmInstallation.InstallationId}");
                Console.WriteLine($"Created APNS installation {apnsInstallation.InstallationId}");
        }

        private static async Task CreateAndDeleteInstallationAsync(NotificationHubClient nhClient)
        {
            // Register some fake devices
            //var fcmDeviceId = Guid.NewGuid().ToString();
            var apnsDeviceId = Guid.NewGuid().ToString();
            Console.WriteLine($"The APNS device ID is {apnsDeviceId}");
            //var fcmInstallation = new Installation
            var apnsInstallation = new Installation
            {
                //InstallationId = fcmDeviceId,
                InstallationId = apnsDeviceId,
                Platform = NotificationPlatform.Fcm,
                //PushChannel = fcmDeviceId,
                PushChannel = apnsDeviceId,
                PushChannelExpired = false,
                //Tags = new[] { "fcm" }
                Tags = new[] { "apns" }
            };
            //await nhClient.CreateOrUpdateInstallationAsync(fcmInstallation);
            await nhClient.CreateOrUpdateInstallationAsync(apnsInstallation);

            while (true)
            {
                try
                {
                    //var installationFromServer = await nhClient.GetInstallationAsync(fcmInstallation.InstallationId);
                    var installationFromServer = await nhClient.GetInstallationAsync(apnsInstallation.InstallationId);
                    break;
                }
                catch (MessagingEntityNotFoundException)
                {
                    // Wait for installation to be created
                    await Task.Delay(1000);
                }
            }
            //Console.WriteLine($"Created FCM installation {fcmInstallation.InstallationId}");
            Console.WriteLine($"Created APNS installation {apnsInstallation.InstallationId}");
            //await nhClient.DeleteInstallationAsync(fcmInstallation.InstallationId);
            await nhClient.DeleteInstallationAsync(apnsInstallation.InstallationId);
            while (true)
            {
                try
                {
                    //var installationFromServer = await nhClient.GetInstallationAsync(fcmInstallation.InstallationId);
                    var installationFromServer = await nhClient.GetInstallationAsync(apnsInstallation.InstallationId);
                    await Task.Delay(1000);
                }
                catch (MessagingEntityNotFoundException)
                {
                    //Console.WriteLine($"Deleted FCM installation {fcmInstallation.InstallationId}");
                    Console.WriteLine($"Deleted APNS installation {apnsInstallation.InstallationId}");
                    break;
                }
            }
        }

        private static SampleConfiguration LoadConfiguration(string[] args)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("config.json", true);
            configurationBuilder.AddCommandLine(args);
            var configRoot = configurationBuilder.Build();
            var sampleConfig = new SampleConfiguration();
            configRoot.Bind(sampleConfig);
            return sampleConfig;
        }
    }        
}
