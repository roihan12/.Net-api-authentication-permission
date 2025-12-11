namespace Common.Responses.Identity
{
    public class RoleClaimViewModel
    {
        public string RoleId { get; set; }
        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public string Descriptions { get; set; }

        public string Group { get; set; }

        public bool IsAssignedToRole { get; set; }
    }
}
