using System;
using System.Threading.Tasks;

namespace OpenId.AppAuth
{
	partial class AuthorizationService
	{
		public void PerformRegistrationRequest(RegistrationRequest request, Action<RegistrationResponse, AuthorizationException> callback)
		{
			PerformRegistrationRequest(request, new RegistrationResponseCallback(callback));
		}

		public Task<RegistrationResponse> PerformRegistrationRequestAsync(RegistrationRequest request)
		{
			var tcs = new TaskCompletionSource<RegistrationResponse>();

			PerformRegistrationRequest(request, (resp, ex) =>
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

		public void PerformTokenRequest(TokenRequest request, IClientAuthentication clientAuthentication, Action<TokenResponse, AuthorizationException> callback)
		{
			PerformTokenRequest(request, clientAuthentication, new TokenResponseCallback(callback));
		}

		public Task<TokenResponse> PerformTokenRequestAsync(TokenRequest request, IClientAuthentication clientAuthentication)
		{
			var tcs = new TaskCompletionSource<TokenResponse>();

			PerformTokenRequest(request, clientAuthentication, (resp, ex) =>
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

		public void PerformTokenRequest(TokenRequest request, Action<TokenResponse, AuthorizationException> callback)
		{
			PerformTokenRequest(request, new TokenResponseCallback(callback));
		}

		public Task<TokenResponse> PerformTokenRequestAsync(TokenRequest request)
		{
			var tcs = new TaskCompletionSource<TokenResponse>();

			PerformTokenRequest(request, (resp, ex) =>
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

		private class RegistrationResponseCallback : Java.Lang.Object, IRegistrationResponseCallback
		{
			private Action<RegistrationResponse, AuthorizationException> callback;

			public RegistrationResponseCallback(Action<RegistrationResponse, AuthorizationException> callback)
			{
				this.callback = callback;
			}

			void IRegistrationResponseCallback.OnRegistrationRequestCompleted(RegistrationResponse registrationResponse, AuthorizationException ex)
			{
				callback(registrationResponse, ex);
			}
		}

		private class TokenResponseCallback : Java.Lang.Object, ITokenResponseCallback
		{
			private Action<TokenResponse, AuthorizationException> callback;

			public TokenResponseCallback(Action<TokenResponse, AuthorizationException> callback)
			{
				this.callback = callback;
			}

			void ITokenResponseCallback.OnTokenRequestCompleted(TokenResponse tokenResponse, AuthorizationException ex)
			{
				callback(tokenResponse, ex);
			}
		}
	}
}
