namespace Common.Request.Identity
{
    public class ChangeUserStatusRequest
    {
        public string UserId { get; set; }
        public bool Activate { get; set; }
    }
}
