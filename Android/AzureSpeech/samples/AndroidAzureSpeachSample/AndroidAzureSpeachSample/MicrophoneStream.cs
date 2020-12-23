using System;
using Android.Media;
using CognitiveServices.Speech.Audio;

namespace AndroidAzureSpeachSample
{
    public class MicrophoneStream : PullAudioInputStreamCallback
    {
        private static readonly int SAMPLE_RATE = 16000;
        private readonly AudioStreamFormat format;
        private AudioRecord recorder;

        public MicrophoneStream()
        {
            format = AudioStreamFormat.GetWaveFormatPCM(SAMPLE_RATE, 16, 1);
            InitMic();
        }

        public override void Close()
        {
            recorder.Release();
            recorder = null;
            format.Close();
        }

        public override int Read(byte[] p0)
        {
            long ret = recorder.Read(p0, 0, p0.Length);
            return (int)ret;
        }

        private void InitMic()
        {
            // Note: currently, the Speech SDK support 16 kHz sample rate, 16 bit samples, mono (single-channel) only.
            AudioFormat af = new AudioFormat.Builder()
                    .SetSampleRate(SAMPLE_RATE)
                    .SetEncoding(Encoding.Pcm16bit)
                    .SetChannelMask(ChannelOut.Default)
                    .Build();

            recorder = new AudioRecord.Builder()
                .SetAudioSource(AudioSource.VoiceRecognition)
                .SetAudioFormat(af)
                .Build();

            recorder.StartRecording();
        }
    }
}
