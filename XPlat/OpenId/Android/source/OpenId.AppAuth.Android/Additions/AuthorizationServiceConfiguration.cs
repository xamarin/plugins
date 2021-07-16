using System;
using System.Threading.Tasks;

namespace OpenId.AppAuth
{
	partial class AuthorizationServiceConfiguration
	{
		public static void FetchFromIssuer(Android.Net.Uri openIdConnectIssuerUri, Action<AuthorizationServiceConfiguration, AuthorizationException> callback)
		{
			FetchFromIssuer(openIdConnectIssuerUri, new RetrieveConfigurationCallback(callback));
		}

		public static Task<AuthorizationServiceConfiguration> FetchFromIssuerAsync(Android.Net.Uri openIdConnectIssuerUri)
		{
			var tcs = new TaskCompletionSource<AuthorizationServiceConfiguration>();

			FetchFromIssuer(openIdConnectIssuerUri, (resp, ex) =>
			{
				if (ex != null)
				{
					tcs.SetException(ex);
				}
				else
				{
					tcs.SetResult(resp);
				}
			});

			return tcs.Task;
		}

		public static void FetchFromUrl(Android.Net.Uri openIdConnectDiscoveryUri, Action<AuthorizationServiceConfiguration, AuthorizationException> callback)
		{
			FetchFromUrl(openIdConnectDiscoveryUri, new RetrieveConfigurationCallback(callback));
		}

		public static Task<AuthorizationServiceConfiguration> FetchFromUrlAsync(Android.Net.Uri openIdConnectDiscoveryUri)
		{
			var tcs = new TaskCompletionSource<AuthorizationServiceConfiguration>();

			FetchFromUrl(openIdConnectDiscoveryUri, (resp, ex) =>
			{
				if (ex != null)
				{
					tcs.SetException(ex);
				}
				else
				{
					tcs.SetResult(resp);
				}
			});

			return tcs.Task;
		}

		private class RetrieveConfigurationCallback : Java.Lang.Object, IRetrieveConfigurationCallback
		{
			private Action<AuthorizationServiceConfiguration, AuthorizationException> callback;

			public RetrieveConfigurationCallback(Action<AuthorizationServiceConfiguration, AuthorizationException> callback)
			{
				this.callback = callback;
			}

			void IRetrieveConfigurationCallback.OnFetchConfigurationCompleted(AuthorizationServiceConfiguration serviceConfiguration, AuthorizationException ex)
			{
				callback(serviceConfiguration, ex);
			}
		}
	}
}
