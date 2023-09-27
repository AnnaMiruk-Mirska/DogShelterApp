using System.Web;

namespace TestApp.Models
{
    public class Dog
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DateOfArrival { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string ChildFriendly { get; set; }
        public string PetFriendly { get; set; }
        public string Size { get; set; }

        public HttpPostedFileBase File { get; set; }
        public string Base64String { get; set; }
        public string Img { get; set; }

    
    }
}