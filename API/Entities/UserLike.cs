namespace API.Entities
{
    public class UserLike
    {
        public User SourceUser { get; set; }

        public int SourceUserID { get; set; }

        public User TargetUser { get; set; }

        public int TargetUserID { get; set; }
    }
}