namespace ChatCore.EFCore.ChatApp.Services
{
    public static class SignalRClientMethod
    {
        public const string ReceiveMessage = "receiveMessage";
        public const string ReceiveFriendRequest = "receiveFriendRequest";
        public const string ReceiveFriendRequestResponse = "receiveFriendRequestResponse";
        public const string ReceiveStatusChanged = "receiveStatusChanged";
    }
}
