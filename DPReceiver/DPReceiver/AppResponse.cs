namespace DPReceiver
{

    public class AppResponse
    {
        public ResponseType Type { get; set; }
        public dynamic Data { get; set; }
    }

    public enum ResponseType
    {
        Connection = 0,
        Disconnect = 1,
        Message = 2,
        FingerData = 3, 
        Error = 255
    }
}
