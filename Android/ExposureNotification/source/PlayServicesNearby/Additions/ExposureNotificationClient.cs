using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AndroidTask = Android.Gms.Tasks.Task;

namespace Android.Gms.Nearby.ExposureNotification
{
	public partial interface IExposureNotificationClient
	{
		public Task StartAsync()
			=> NativeStart().CastTask();

		public Task StopAsync()
			=> NativeStop().CastTask();

		[Obsolete("Use GetExposureWindowsAsync() instead.")]
		public async Task<IList<ExposureInformation>> GetExposureInformationAsync(string token)
			=> await NativeExposureInformation(token).CastTask<JavaList<ExposureInformation>>();

		[Obsolete("Use GetDailySummariesAsync(DailySummariesConfig) instead.")]
		public Task<ExposureSummary> GetExposureSummaryAsync(string token)
			=> NativeExposureSummary(token).CastTask<ExposureSummary>();

		public async Task<bool> IsEnabledAsync()
			=> (bool)await NativeIsEnabled().CastTask<Java.Lang.Boolean>();

		[Obsolete("Use GetDailySummariesAsync(IList<Java.IO.File>) instead.")]
		public Task ProvideDiagnosisKeysAsync(IList<Java.IO.File> files, ExposureConfiguration config, string token)
			=> NativeProvideDiagnosisKeys(files, config, token).CastTask();

		[Obsolete("Use ProvideDiagnosisKeysAsync(DiagnosisKeyFileProvider) instead.")]
		public Task ProvideDiagnosisKeysAsync(IList<Java.IO.File> files)
			=> NativeProvideDiagnosisKeys(files).CastTask();

		public Task ProvideDiagnosisKeysAsync(DiagnosisKeyFileProvider provider)
			=> NativeProvideDiagnosisKeys(provider).CastTask();

		public async Task<IList<TemporaryExposureKey>> GetTemporaryExposureKeyHistoryAsync()
			=> await NativeTemporaryExposureKeyHistory().CastTask<JavaList<TemporaryExposureKey>>();

		public async Task<long> GetVersionAsync()
			=> (long)await NativeVersion().CastTask<Java.Lang.Long>();

		public async Task<int> GetCalibrationConfidenceAsync()
			=> (int)await NativeCalibrationConfidence().CastTask<Java.Lang.Integer>();

		public async Task<IList<DailySummary>> GetDailySummariesAsync(DailySummariesConfig config)
			=> await NativeDailySummaries(config).CastTask<JavaList<DailySummary>>();

		public async Task<DiagnosisKeysDataMapping> GetDiagnosisKeysDataMappingAsync()
			=> await NativeDiagnosisKeysDataMapping().CastTask<DiagnosisKeysDataMapping>();

		public async Task SetDiagnosisKeysDataMappingAsync(DiagnosisKeysDataMapping mapping)
			=> await NativeDiagnosisKeysDataMapping(mapping).CastTask();

		public async Task<IList<ExposureWindow>> GetExposureWindowsAsync()
			=> await NativeExposureWindows().CastTask<JavaList<ExposureWindow>>();

		[Obsolete("Use GetExposureWindowsAsync() instead.")]
		public async Task<IList<ExposureWindow>> GetExposureWindowsAsync(string token)
			=> await NativeExposureWindows(token).CastTask<JavaList<ExposureWindow>>();

		public async Task<PackageConfiguration> GetPackageConfigurationAsync()
			=> await NativeGetPackageConfiguration().CastTask<PackageConfiguration>();

		public async Task<ICollection<ExposureNotificationStatus>> GetStatusAsync()
			=> await NativeGetStatus().CastTask<JavaCollection<ExposureNotificationStatus>>();
	}

	internal static class GoogleTaskExtensions
	{
		public static Task CastTask(this AndroidTask androidTask)
		{
			var tcs = new TaskCompletionSource<bool>();

			androidTask.AddOnCompleteListener(new MyCompleteListener(
				t =>
				{
					if (t.Exception == null)
						tcs.TrySetResult(false);
					else
						tcs.TrySetException(t.Exception);
				}
			));

			return tcs.Task;
		}

		public static Task<TResult> CastTask<TResult>(this AndroidTask androidTask)
			where TResult : Java.Lang.Object
		{
			var tcs = new TaskCompletionSource<TResult>();

			androidTask.AddOnCompleteListener(new MyCompleteListener(
				t =>
				{
					if (t.Exception == null)
						tcs.TrySetResult(t.Result.JavaCast<TResult>());
					else
						tcs.TrySetException(t.Exception);
				}));

			return tcs.Task;
		}

		class MyCompleteListener : Java.Lang.Object, Android.Gms.Tasks.IOnCompleteListener
		{
			public MyCompleteListener(Action<AndroidTask> onComplete)
				=> OnCompleteHandler = onComplete;

			public Action<AndroidTask> OnCompleteHandler { get; }

			public void OnComplete(AndroidTask task)
				=> OnCompleteHandler?.Invoke(task);
		}
	}
}
