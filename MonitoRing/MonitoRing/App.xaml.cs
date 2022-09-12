using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MonitoRing
{
    public partial class App : Application
    {
        IManagedMqttClient _mqttClient;
        public App()
        {
            InitializeComponent();

            MainPage = new StatePage();
        }

        protected override void OnStart()
        {
            MqttStart();
        }

        protected override void OnSleep()
        {
            MqttStop();
        }

        protected override void OnResume()
        {
            MqttStart();
        }

        private void MqttStart()
        {

            ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                  .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                  .WithClientOptions(new MqttClientOptionsBuilder()
                      .WithTcpServer("io.adafruit.com", 1883)
                      .WithCleanSession(true)
                      .WithClientId(Guid.NewGuid().ToString())
                      .WithCredentials("Mariyan", "aio_fGGc91l7W9T1HmCfxaS6v8QV1kQX")
                      .WithKeepAlivePeriod(TimeSpan.FromSeconds(120))
                      .Build())
                  .Build();
            _mqttClient = new MqttFactory().CreateManagedMqttClient();

            _mqttClient.UseDisconnectedHandler(m =>
            {
                Debug.WriteLine("Disconnencted from the server");
            });

            _mqttClient.UseConnectedHandler(async m =>
            {
                Debug.WriteLine("Connected to the server. Subcribing......");
                Debug.WriteLine("################ Subscribed to the topic #############");
            });

            Task.Run(async () =>
            {
                await _mqttClient.StartAsync(options);
                await _mqttClient.SubscribeAsync(new List<TopicFilter>
                    {
                        new TopicFilter()
                        {
                            Topic = "Mariyan/feeds/temperature-feed",
                            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce
                        }
                    });
            });
            _mqttClient.UseApplicationMessageReceivedHandler(m =>
            {
                var payload = Convert.ToSingle(Encoding.UTF8.GetString(m.ApplicationMessage.Payload));
                var conFloatVal = Convert.ToSingle(payload);

                Debug.WriteLine("Received sensor data: {0}", payload);
                if (m.ApplicationMessage.Topic == "Mariyan/feeds/temperature-feed")
                {
                    MessagingCenter.Instance.Send<App, float>(this, "Mariyan/feeds/temperature-feed", payload);
                }
            });
            
            MqttNetGlobalLogger.LogMessagePublished += (s, e) =>
            {
                var mess = e.TraceMessage;
                var trace = $">> [{mess.Timestamp:O}] [{e.TraceMessage.ThreadId}] [{e.TraceMessage.Source}] [{e.TraceMessage.Level}]: {e.TraceMessage.Message}";
                if (e.TraceMessage.Exception != null)
                {
                    trace += Environment.NewLine + e.TraceMessage.Exception.ToString();
                }
                Debug.WriteLine("@@@@@@@@@@@@@@@@@@@@  --- Listening to Logger ------------- @@@@@@@@@@@@@@@@@");
                Debug.WriteLine(trace);
            };
        }
        private void MqttStop()
        {
            Task.Run(async () => await _mqttClient?.StopAsync());
        }
    }
}

