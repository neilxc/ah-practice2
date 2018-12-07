namespace Domain
{
    public class UserPhoto : PhotoBase
    {
        public bool IsMain { get; set; }
        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }
    }
}