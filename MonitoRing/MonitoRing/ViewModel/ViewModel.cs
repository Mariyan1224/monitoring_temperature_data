using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MonitoRing.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        public List<ChartEntry> Sensordata_List;
        private Chart chart;
        private float sensorData;

        public event PropertyChangedEventHandler PropertyChanged;
        public Chart Chart
        {
            get
            {
                return chart;
            }
            private set
            {
                chart = value;
                OnPropertyChanged();
            }
        }
        public float SensorData
        {
            get => sensorData;
            private set
            {
                sensorData = value;
                AddNewDataInList();
            }
        }
        private void OnPropertyChanged([CallerMemberName]  string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public ViewModel()
        {
            Sensordata_List = new List<ChartEntry>();
            MessagingCenter.Instance.Subscribe<App, float>(this, "Mariyan/feeds/temperature-feed", (sender, temperatureData) => SensorData = temperatureData);
            Chart = new BarChart
            {
                Entries = Sensordata_List,
                BackgroundColor = SKColor.Parse("#0a3a0a"),
                LabelTextSize = 50,
                LabelColor = SKColor.Parse("#3498db"),
                PointMode = PointMode.Circle
            };

            Task.Run(async () => await Chart.AnimateAsync(true));
        }
        private void KeepAClearDisplay()
        {
            if(Sensordata_List.Count >= 30)
            {
                Sensordata_List.RemoveAt(0);
            }
        }
        private void AddNewDataInList()
        {
            Sensordata_List.Add(
                            new ChartEntry(sensorData)
                            {
                                Label = Convert.ToString(DateTime.Now.Hour),
                                ValueLabel = Convert.ToString(sensorData),
                                ValueLabelColor = SKColor.Parse("#05fc16"),
                                Color = SKColor.Parse("#05fc16")
                            });
            Chart.Entries = Sensordata_List;
            KeepAClearDisplay();
        }
    }
}