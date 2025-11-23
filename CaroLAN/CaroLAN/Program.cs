namespace CaroLAN
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Hiển thị form đăng nhập 
            using LoginForm loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                SocketManager socket = loginForm.GetSocket();
                string username = loginForm.GetUsername();
                string password = loginForm.GetPassword(); // ✅ Lấy password để tự động đăng nhập lại

                // Mở sảnh chờ với thông tin đăng nhập và socket hiện tại
                Application.Run(new sanhCho(username, password, socket));
            }
        }
    }
}