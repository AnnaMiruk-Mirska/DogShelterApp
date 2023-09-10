using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

        //public byte[] ImageImg { get; set; }

        public HttpPostedFileBase File { get; set; }
        public string Base64String { get; set; }
        public string Img { get; set; }

     

        //public static System.Drawing.Imaging.ImageFormat Jpeg { get; }

        // public string ImageToBase64()
        // {
        //     string path = "D:\\SampleImage.jpg";
        //     using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
        //     {
        //         using (MemoryStream m = new MemoryStream())
        //         {
        //             image.Save(m, image.RawFormat);
        //             byte[] imageBytes = m.ToArray();
        //             var base64String = Convert.ToBase64String(imageBytes);
        //             return base64String;
        //         }
        //     }
        // }

        //public Image ImageImg { get; set; }
    }
}