namespace API.Data.DTOs
{
    public class PhotoDTO
    {
        public int Id { get; set; }
        public string Url { get; set; }

        public bool isMain { get; set; }

        public string PublicId { get; set; }

        public int UserId { get; set; }

        //public User User { get; set; }
    }
}