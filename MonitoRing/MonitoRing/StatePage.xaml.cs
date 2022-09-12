using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MQTTnet.Client;
using MQTTnet;
using System.Threading;
using MonitoRing.ViewModel;

namespace MonitoRing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatePage : ContentPage
    {
        public StatePage()
        {
            InitializeComponent();
            BindingContext = new ViewModel.ViewModel();
        }
    }
}