using System;
using System.Collections.Generic;

namespace OpenId.AppAuth
{
	partial class AuthState
	{
		public void PerformActionWithFreshTokens(AuthorizationService service, Action<string, string, AuthorizationException> callback)
		{
			PerformActionWithFreshTokens(service, new AuthStateAction(callback));
		}

		public void PerformActionWithFreshTokens(AuthorizationService service, IDictionary<string, string> refreshTokenAdditionalParams, Action<string, string, AuthorizationException> callback)
		{
			PerformActionWithFreshTokens(service, refreshTokenAdditionalParams, new AuthStateAction(callback));
		}

		private class AuthStateAction : Java.Lang.Object, IAuthStateAction
		{
			private Action<string, string, AuthorizationException> callback;

			public AuthStateAction(Action<string, string, AuthorizationException> callback)
			{
				this.callback = callback;
			}

			void IAuthStateAction.Execute(string accessToken, string idToken, AuthorizationException ex)
			{
				callback(accessToken, idToken, ex);
			}
		}
	}
}
