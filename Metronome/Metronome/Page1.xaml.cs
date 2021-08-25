using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.SimpleAudioPlayer;
using System.IO;

namespace Metronome
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        ISimpleAudioPlayer player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        CancellationTokenSource cts = new CancellationTokenSource();
        private int delayForColor = 100;
        public Page1()
        {
            InitializeComponent();
            btnPlay.IsVisible = true;
            btnStop.IsVisible = false;
            //Initialisierung Tempo / Taktart
            int BPM = 60;
            int Zaehlzeit = 4;
            int Notenwert = 4;
            TempoBPM.Text = BPM.ToString();
            entrZaehlzeit.Text = Zaehlzeit.ToString();
            entrNotenwert.Text = Notenwert.ToString();
            player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            Stream beepStream = GetType().Assembly.GetManifestResourceStream("Metronome.sounds.bottle_sound.mp3");
            bool isSuccess = player.Load(beepStream);
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            int step = 10;
            var newStep = Math.Round(e.NewValue / step);
            SliderBPM.Value = newStep * step;
            TempoBPM.Text = ((int)SliderBPM.Value).ToString();
        }

        private void TapGestureRecognizer_StopTapped(object sender, EventArgs e)
        {
            btnPlay.IsVisible = true;
            btnStop.IsVisible = false;
            cts.Cancel();
            cts.Dispose();
            cts = new CancellationTokenSource();
        }

        private async void TapGestureRecognizer_PlayTapped(object sender, EventArgs e)
        {
            btnPlay.IsVisible = false;
            btnStop.IsVisible = true;
            try
            {
                await TestAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {}
        }
        private async Task TestAsync(CancellationToken cancelToken)
        {
            while (true)
            {
                bool res = Int32.TryParse(TempoBPM.Text, out int interval);
                rect1.BackgroundColor = Color.Yellow;
                await Task.Delay(delayForColor, cancelToken);
                rect1.BackgroundColor = Color.LightGray;
                await Task.Delay(CalculateTempoToMilisecs(interval) - delayForColor, cancelToken);
                player.Play();
                cancelToken.ThrowIfCancellationRequested();
            }
        }

        private int CalculateTempoToMilisecs(int tempBPM)
        {
            if (tempBPM != 0)
            {
                return 60000 / tempBPM;
            }
            return 0;
        }

    }
}