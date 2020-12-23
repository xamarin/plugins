using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using CognitiveServices.Speech;
using CognitiveServices.Speech.Audio;
using Java.Util.Concurrent;

namespace AndroidAzureSpeachSample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        // Replace below with your own subscription key
        private static readonly string SpeechSubscriptionKey = "YourKey";
        // Replace below with your own service region (e.g., "westus").
        private static readonly string SpeechRegion = "westus";

        private TextView recognizedTextView;
        private Button recognizeButton;

        private MicrophoneStream microphoneStream;
        private SpeechConfig speechConfig;
        private SpeechRecognizer reco;
        private AudioConfig audioInput;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            recognizedTextView = (TextView)FindViewById(Resource.Id.recognizedText);
            recognizedTextView.Text = "Recognized text goes here!";

            recognizeButton = (Button)FindViewById(Resource.Id.buttonRecognize);

            // Initialize SpeechSDK and request required permissions.
            HandlePermissions();

            InitSpeechConfig();

            recognizeButton.Click += RecognizeButton_Click;
        }

        private void InitSpeechConfig()
        {
            try
            {
                speechConfig = SpeechConfig.FromSubscription(SpeechSubscriptionKey, SpeechRegion);
            }
            catch (Exception ex)
            {
                DisplayException(ex);
                return;
            }
        }

        private void HandlePermissions()
        {
            try
            {
                // a unique number within the application to allow
                // correlating permission request responses with the request.
                int permissionRequestId = 5;

                // Request permissions needed for speech recognition
                RequestPermissions(new string[]
                {
                    Android.Manifest.Permission.RecordAudio,
                    Android.Manifest.Permission.Internet },
                    permissionRequestId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SpeechSDK: ", ex.Message);
                recognizedTextView.Text = ex.ToString();
            }
        }

        private void RecognizeButton_Click(object sender, EventArgs e)
        {
            audioInput = AudioConfig.FromStreamInput(CreateMicrophoneStream());
            reco = new SpeechRecognizer(speechConfig, audioInput);

            DisableButtons();
            ClearTextBox();

            try
            {
                audioInput = AudioConfig.FromStreamInput(CreateMicrophoneStream());
                reco = new SpeechRecognizer(speechConfig, audioInput);

                IFuture task = reco.RecognizeOnceAsync();
                SpeechRecognitionResult obj = (SpeechRecognitionResult)task.Get();

                string s = obj.Text;
                if (obj.Reason != ResultReason.RecognizedSpeech)
                {
                    string errorDetails = (obj.Reason == ResultReason.Canceled) ? CancellationDetails.FromResult(obj).ErrorDetails : string.Empty;
                    s = "Recognition failed with " + obj.Reason + ". Did you enter your subscription?" + System.Environment.NewLine + errorDetails;
                }

                reco.Close();
                Console.WriteLine("Recognizer returned: " + s);

                SetRecognizedText(s);
                EnableButtons();
            }
            catch (Exception ex)
            {
                DisplayException(ex);
                EnableButtons();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void DisplayException(Exception ex)
        {
            recognizedTextView.Text = ex.Message + System.Environment.NewLine + string.Join(System.Environment.NewLine, ex.StackTrace);
        }

        private MicrophoneStream CreateMicrophoneStream()
        {
            if (microphoneStream != null)
            {
                microphoneStream.Close();
                microphoneStream = null;
            }

            microphoneStream = new MicrophoneStream();
            return microphoneStream;
        }

        private void ClearTextBox()
        {
            AppendTextLine(string.Empty, true);
        }

        private void SetRecognizedText(string s)
        {
            AppendTextLine(s, true);
        }

        private void AppendTextLine(string s, bool erase)
        {
            RunOnUiThread(() =>
            {
                if (erase)
                {
                    recognizedTextView.Text = s;
                }
                else
                {
                    string txt = recognizedTextView.Text;
                    recognizedTextView.Text = txt + System.Environment.NewLine + s;
                }
            });
        }

        private void DisableButtons()
        {
            RunOnUiThread(() =>
            {
                recognizeButton.Enabled = false;
            });
        }

        private void EnableButtons()
        {
            RunOnUiThread(() =>
            {
                recognizeButton.Enabled = true;
            });
        }
    }
}