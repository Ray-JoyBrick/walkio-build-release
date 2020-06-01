namespace JoyBrick.Walkio.Game
{
    public partial class Bootstrap
    {
        private Firebase.Auth.FirebaseAuth _auth;

        private void SetupAuth()
        {
            _auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        }

        private void Login()
        {
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
        }
    }
}
