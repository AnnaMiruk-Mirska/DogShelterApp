using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using TestApp.Models;



namespace TestApp.Controllers
{
    
    public class DogController : Controller
    {
        // GET: Dog
        IFirebaseClient client;


        public DogController()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "",
                BasePath = ""
            };

            client = new FirebaseClient(config);
        }

     

        List<Dog> dogList = new List<Dog>();


        public ActionResult Beginning()
        {
            


            Dictionary<string, Dog> dogs = new Dictionary<string, Dog>();
            FirebaseResponse response= client.Get("dogs");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dogs = JsonConvert.DeserializeObject<Dictionary<string, Dog>>(response.Body);
            }


            List<Dog> dogList = new List<Dog>();




            foreach(KeyValuePair<string, Dog> item in dogs)
            {

                dogList.Add(new Dog() { Id=item.Key, Name=item.Value.Name, DateOfArrival=item.Value.DateOfArrival, ChildFriendly=item.Value.ChildFriendly, Size=item.Value.Size, Base64String=item.Value.Base64String, Img= "data:image/jpeg;base64," + item.Value.Base64String//Image = Convert.ToBase64String(item.Value.ImageImg)
                                                                                                                                                                                                                                                                         // base64ImageRepresentation = item.Value.Image
                });
            }
            return View(dogList);
        }



        [Authorize]
        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Info(string id)
        {
            FirebaseResponse response = client.Get("dogs/" + id);
            Dog dog = response.ResultAs<Dog>();
            dog.Id = id;
            return View(dog);
        }



        [Authorize]
        public ActionResult Post()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(Dog dog)
        {
            string id = dog.Id;
            dog.Id = null;


            byte[] fileByte = new byte[dog.File.ContentLength];
            dog.File.InputStream.Read(fileByte, 0, dog.File.ContentLength);
            string Base64String = Convert.ToBase64String(fileByte);
            dog.Base64String = Base64String;
            dog.File.InputStream.Close();
            dog.File = null;

            string img = "data:image/jpeg;base64,"+dog.Base64String;
            dog.Img = img;

            FirebaseResponse response = client.Update("dogs/" + id, dog);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Beginning", "Dog");
            }
            else
            {
                return View();
            }

        }

        [Authorize]
        public ActionResult Edit(string id)
        {
            FirebaseResponse response = client.Get("dogs/" + id);
            Dog dog = response.ResultAs<Dog>();
            dog.Id = id;
            return View(dog);

        }

        [Authorize]
        public ActionResult Delete(string id)
        {
            FirebaseResponse response = client.Delete("dogs/" + id);
            return RedirectToAction("Beginning", "Dog");



        }


        [Authorize]
        [HttpPost]
        public ActionResult Add(Dog dog)
        {

             byte[] fileByte = new byte[dog.File.ContentLength];
             dog.File.InputStream.Read(fileByte, 0, dog.File.ContentLength);
             string Base64String = Convert.ToBase64String(fileByte);
             dog.Base64String = Base64String;
             dog.File.InputStream.Close();

            string img = "data:image/jpg;base64," + dog.Base64String;
            dog.Img = img;



            string ID = Guid.NewGuid().ToString("N");
            dog.File = null;
                SetResponse setResponse = client.Set("dogs/" + ID, dog);
            if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Beginning","Dog");
            }
            else
            {
                return View();
            }

        }




        public ActionResult Search(string option, string childfriendly, string petfriendly, string Age, string size1, string size2, string size3)
        {

            Dictionary<string, Dog> dogs = new Dictionary<string, Dog>();
            FirebaseResponse response = client.Get("dogs");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dogs = JsonConvert.DeserializeObject<Dictionary<string, Dog>>(response.Body);
            }


            List<Dog> dogList = new List<Dog>();

            foreach (KeyValuePair<string, Dog> item in dogs)
            {

                dogList.Add(new Dog()
                {
                    Id = item.Key,
                    Name = item.Value.Name,
                    Gender = item.Value.Gender,
                    DateOfArrival = item.Value.DateOfArrival,
                    ChildFriendly = item.Value.ChildFriendly,
                    Age = item.Value.Age,
                    Size = item.Value.Size,
                    File = item.Value.File,
                    Img = item.Value.Img
                }) ;
            }

            List<Dog> list = new List<Dog>();

            foreach (var element in dogList)
            {
                if (Age != "")
                {
                    if (element.Age == Age)
                    {
                        if (option == "Female")
                        {
                            if (element.Gender == "samica")
                            {
                                list.Add(element);
                            }
                        }
                        else if (option == "Male")
                        {
                            if (element.Gender == "samiec")
                            {
                                list.Add(element);
                            }
                        }
                    }
                }
                else
                {
                    if (option == "Female")
                    {
                        if (element.Gender == "samica")
                        {
                            list.Add(element);
                        }
                    }
                    else if (option == "Male")
                    {
                        if (element.Gender == "samiec")
                        {
                            list.Add(element);
                        }
                    }
                }
            }

            List<Dog> list1 = new List<Dog>();
            foreach (var element in list)
            {
                if (size1 == "true")
                {
                    if (element.Size == "mały")
                    {
                        list1.Add(element);
                    }
                }
                if (size2 == "true")
                {
                    if (element.Size == "średni")
                    {
                        list1.Add(element);
                    }
                }
                if (size3 == "true")
                {
                    if (element.Size == "duży")
                    {
                        list1.Add(element);
                    }
                }
                if (size1 == "false" && size2 == "false" && size3 == "false")
                {
                    list1.Add(element);
                }

            }



            List<Dog> list2 = new List<Dog>();
            foreach (var element in list1)
            {
                if (childfriendly == "true")
                {
                    if (element.ChildFriendly == "tak")
                    {
                        list2.Add(element);
                    }
                }
                else
                {
                    if (element.ChildFriendly == "nie")
                    {
                        list2.Add(element);
                    }
                }

            }

            List<Dog> list3 = new List<Dog>();
            foreach (var element in list2)
            {
                if (childfriendly == "true")
                {
                    if (element.ChildFriendly == "tak")
                    {
                        list3.Add(element);
                    }
                }
                else
                {
                    if (element.ChildFriendly == "nie")
                    {
                        list3.Add(element);
                    }
                }

            }

            return View(list3.ToList());

         
        }

    }
}