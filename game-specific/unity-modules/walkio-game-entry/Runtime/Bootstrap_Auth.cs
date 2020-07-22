namespace JoyBrick.Walkio.Game
{
    public partial class Bootstrap
    {
#if COMPLETE_PROJECT
        private Firebase.Auth.FirebaseAuth _auth;
#endif

        private void SetupAuth()
        {
#if COMPLETE_PROJECT
            _auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
#endif
        }

        private void Login()
        {
#if COMPLETE_PROJECT
            _auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                     _logger.Error("SignInAnonymouslyAsync was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    _logger.Error($"SignInAnonymouslyAsync encountered an error: {task.Exception}");
                    return;
                }

                var newUser = task.Result;
                _logger.Debug($"User signed in successfully: {newUser.DisplayName} ({newUser.UserId})");
            });
#endif
        }
    }
}
