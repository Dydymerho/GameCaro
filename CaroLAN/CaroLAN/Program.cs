namespace CaroLAN
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using LoginForm loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                SocketManager socket = loginForm.GetSocket();
                string username = loginForm.GetUsername();
                string password = loginForm.GetPassword();
                Queue<string> pendingMessages = loginForm.GetPendingMessages();

                Application.Run(new sanhCho(username, password, socket, pendingMessages));
            }
        }
    }
}