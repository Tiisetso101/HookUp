namespace API.Helpers
{
    public class UserParams : PaginationParams
    {

        public string CurrentUser { get; set; }

        public string Gender { get; set; }

        public int minAge { get; set; } = 18;

        public int maxAge { get; set; } = 40;

        public string OrderBy { get; set; } = "lastActive";

    }
}