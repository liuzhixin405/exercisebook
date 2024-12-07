using Mapster;

namespace MapsterDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
           var followModel = new FollowModel()
            {
                Name = "John",
                Email = "john@gmail.com",
                Phone = "1234567890"
            };

            var followerDTO = followModel.Adapt<FollowerDTO>();
            Console.WriteLine(followerDTO.Name);
            Console.WriteLine(followerDTO.Email);
            Console.WriteLine(followerDTO.Phone);
        }
    }

    public class FollowModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
    public class FollowerDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
