using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Firebase.Storage;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                AuthSecret = "la4AnvWnDBr18EMKoCEu4RZL3f2ktjsC5tj1EqJt",
                BasePath = "https://testapp-848d7-default-rtdb.firebaseio.com"
            };

            client = new FirebaseClient(config);
        }

        //  private static string Bucket = "gtestapp-848d7.appspot.com";
        //  private static string ApiKey = "AIzaSyDUem-hNdG6BCiocltvBKlZZis2ieSSD3A";
        //
        //  public ActionResult File()
        //  {
        //      return View();
        //  }


        //  [HttpPost]
        //  public async Task <ActionResult> File(Dog dog, HttpPostedFileBase file)
        //  {
        //
        //      FileStream stream;
        //      if(file != null && file.ContentLength>0)
        //      {
        //          string path = Path.Combine(Server.MapPath("~/Images"), file.FileName);
        //          file.SaveAs(path);
        //          stream = new FileStream(Path.Combine(path), FileMode.Open);
        //          await Task.Run(() => Upload(stream, file.FileName));
        //      }
        //      return View();
        //  }


        //  public async void Upload (FileStream stream, string fileName)
        //  {
        //      var task = new FirebaseStorage(Bucket, new FirebaseStorageOptions {})
        //      .Child("images")
        //      .Child(fileName)
        //      .PutAsync(stream);
        //      try
        //      {
        //          string link = await task;
        //      }
        //      catch(Exception e)
        //      {
        //          Console.WriteLine(e.Message);
        //      }
        //
        //
        //  }

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
                // string path = Path.GetFullPath(item.Value.ImageImg);
                // byte[] imageArray = System.IO.File.ReadAllBytes(path);
                // string base64ImageRepresentation = Convert.ToBase64String(imageArray);


                //  ImageCodecInfo myImageCodecInfo = GetEncoderInfo(item.Value.);
                //  using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
                //      {
                //          using (MemoryStream m = new MemoryStream())
                //          {
                //              image.Save(m, image.RawFormat);
                //              byte[] imageBytes = m.ToArray();
                //              var base64String = Convert.ToBase64String(imageBytes);
                //          }
                //      }



                dogList.Add(new Dog() { Id=item.Key, Name=item.Value.Name, DateOfArrival=item.Value.DateOfArrival, ChildFriendly=item.Value.ChildFriendly, Size=item.Value.Size, Base64String=item.Value.Base64String, Img= "data:image/jpeg;base64," + item.Value.Base64String//Image = Convert.ToBase64String(item.Value.ImageImg)
                                                                                                                                                                                                                                                                         // base64ImageRepresentation = item.Value.Image
                });
            }
            return View(dogList);
        }


        // public ActionResult GetImage(string id)
        // {
        //     var dir = Server.MapPath("/Images");
        //     var path = Path.Combine(dir, id + ".jpg");
        //     return base.File(path, "image/jpeg");
        // }

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

       // [HttpPost]
       // public ActionResult Add(HttpPostedFileBase file)
       // {
       //
       //     if (file != null && file.ContentLength > 0)
       //         try
       //         {
       //             string path = Path.Combine(Server.MapPath("~/Images"), Path.GetFileName(file.FileName));
       //             file.SaveAs(path);
       //             ViewBag.Message = "File uploaded successfully";
       //         }
       //         catch (Exception ex)
       //         {
       //             ViewBag.Message = "ERROR:" + ex.Message.ToString();
       //         }
       //     else
       //     {
       //         ViewBag.Message = "You have not specified a file.";
       //     }
       //     return View();
       // }


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
            //byte[] newFileByte = Convert.FromBase64String(Base64String);

            string img = "data:image/jpg;base64," + dog.Base64String;
            dog.Img = img;



            //dog.Base64String = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Senectus et netus et malesuada fames ac turpis egestas. Netus et malesuada fames ac turpis. Dolor sed viverra ipsum nunc aliquet bibendum enim. At tempor commodo ullamcorper a lacus. Interdum velit laoreet id donec ultrices. Lectus nulla at volutpat diam ut venenatis tellus in metus. Porttitor eget dolor morbi non arcu risus quis varius. Amet risus nullam eget felis eget nunc. Elementum facilisis leo vel fringilla est. Phasellus faucibus scelerisque eleifend donec pretium. Fringilla est ullamcorper eget nulla facilisi etiam dignissim. Et ultrices neque ornare aenean. Sodales ut eu sem integer vitae justo eget. Vitae sapien pellentesque habitant morbi tristique. Felis donec et odio pellentesque diam volutpat commodo sed. Ut tortor pretium viverra suspendisse potenti. Egestas maecenas pharetra convallis posuere morbi. Sit amet purus gravida quis blandit turpis cursus. Nec dui nunc mattis enim. Cras pulvinar mattis nunc sed blandit libero volutpat sed.Condimentum mattis pellentesque id nibh tortor id aliquet lectus proin. Nullam non nisi est sit amet facilisis magna etiam tempor. Lacus luctus accumsan tortor posuere ac ut consequat semper.Augue interdum velit euismod in. Diam phasellus vestibulum lorem sed risus ultricies.Urna molestie at elementum eu.Eget arcu dictum varius duis at consectetur lorem. Mattis pellentesque id nibh tortor id aliquet lectus proin.Urna molestie at elementum eu facilisis sed.Tellus molestie nunc non blandit massa. Vivamus at augue eget arcu dictum varius duis at consectetur. Velit ut tortor pretium viverra.Nibh tortor id aliquet lectus proin nibh nisl condimentum id. Aliquam eleifend mi in nulla posuere. Feugiat sed lectus vestibulum mattis ullamcorper. Ut tortor pretium viverra suspendisse potenti nullam ac tortor.Quam viverra orci sagittis eu volutpat. Tellus pellentesque eu tincidunt tortor aliquam nulla facilisi cras fermentum. Sit amet mattis vulputate enim nulla aliquet.Aliquam purus sit amet luctus venenatis lectus.Scelerisque eu ultrices vitae auctor.Diam quis enim lobortis scelerisque fermentum dui faucibus. Nisi lacus sed viverra tellus in hac habitasse platea.Turpis egestas maecenas pharetra convallis.Consectetur purus ut faucibus pulvinar elementum integer enim neque.Cursus eget nunc scelerisque viverra mauris in aliquam sem fringilla.In cursus turpis massa tincidunt dui. Fames ac turpis egestas maecenas pharetra. Eu feugiat pretium nibh ipsum.Faucibus vitae aliquet nec ullamcorper.Facilisis mauris sit amet massa.Lobortis elementum nibh tellus molestie nunc. Turpis egestas pretium aenean pharetra magna ac.Augue mauris augue neque gravida in fermentum et sollicitudin ac. Odio tempor orci dapibus ultrices in. Vitae aliquet nec ullamcorper sit amet risus nullam eget.Aliquam faucibus purus in massa tempor nec feugiat nisl.Lacus vestibulum sed arcu non odio euismod.Blandit turpis cursus in hac.Placerat in egestas erat imperdiet sed euismod.Pellentesque elit eget gravida cum sociis natoque penatibus. Et molestie ac feugiat sed lectus vestibulum mattis. Tellus rutrum tellus pellentesque eu tincidunt tortor.Enim diam vulputate ut pharetra sit amet aliquam id diam. Maecenas accumsan lacus vel facilisis volutpat est.Auctor elit sed vulputate mi sit amet mauris commodo quis. Lorem mollis aliquam ut porttitor leo a diam sollicitudin.Donec pretium vulputate sapien nec sagittis aliquam malesuada bibendum.At ultrices mi tempus imperdiet nulla malesuada pellentesque elit.Ut pharetra sit amet aliquam id diam maecenas ultricies mi. Nunc id cursus metus aliquam eleifend mi.Pharetra sit amet aliquam id diam maecenas ultricies mi eget. Non consectetur a erat nam at lectus urna. Tristique senectus et netus et malesuada fames.Enim facilisis gravida neque convallis a. Aliquam ut porttitor leo a diam sollicitudin.Eros donec ac odio tempor orci dapibus ultrices in. Ut enim blandit volutpat maecenas volutpat blandit. Vitae proin sagittis nisl rhoncus mattis. Pretium vulputate sapien nec sagittis aliquam malesuada bibendum arcu.Lacus laoreet non curabitur gravida arcu ac tortor dignissim convallis. Neque volutpat ac tincidunt vitae semper quis lectus. Id donec ultrices tincidunt arcu non sodales.In tellus integer feugiat scelerisque varius morbi enim nunc faucibus. Pellentesque nec nam aliquam sem et tortor consequat id.Bibendum ut tristique et egestas quis. Adipiscing vitae proin sagittis nisl rhoncus mattis rhoncus urna.Aliquet risus feugiat in ante metus dictum at tempor commodo. Morbi tristique senectus et netus et malesuada fames. Est pellentesque elit ullamcorper dignissim cras. Sit amet volutpat consequat mauris.Arcu vitae elementum curabitur vitae.Nunc pulvinar sapien et ligula ullamcorper. Quis eleifend quam adipiscing vitae proin sagittis nisl rhoncus mattis. Eget aliquet nibh praesent tristique magna sit amet purus.Morbi quis commodo odio aenean sed adipiscing diam. Cursus sit amet dictum sit amet justo donec enim diam. Pellentesque pulvinar pellentesque habitant morbi.Pellentesque elit ullamcorper dignissim cras tincidunt. Dictumst quisque sagittis purus sit amet. Nec dui nunc mattis enim ut tellus elementum. Libero id faucibus nisl tincidunt eget nullam.Purus viverra accumsan in nisl nisi scelerisque eu ultrices.Et egestas quis ipsum suspendisse ultrices gravida dictum fusce.Accumsan sit amet nulla facilisi morbi tempus iaculis. Faucibus et molestie ac feugiat sed lectus.Dignissim suspendisse in est ante in. Ut faucibus pulvinar elementum integer enim neque volutpat ac tincidunt. Enim blandit volutpat maecenas volutpat blandit aliquam.Magna eget est lorem ipsum dolor sit.Condimentum id venenatis a condimentum vitae sapien pellentesque habitant. Platea dictumst quisque sagittis purus sit. Tincidunt nunc pulvinar sapien et.Commodo quis imperdiet massa tincidunt.Rhoncus dolor purus non enim praesent. Mauris pharetra et ultrices neque ornare aenean euismod elementum.Purus semper eget duis at tellus at urna. Tellus in hac habitasse platea dictumst vestibulum rhoncus. Phasellus faucibus scelerisque eleifend donec.Arcu odio ut sem nulla pharetra diam.Duis ut diam quam nulla porttitor massa.Bibendum neque egestas congue quisque egestas diam.Neque sodales ut etiam sit amet nisl purus. Curabitur vitae nunc sed velit dignissim sodales.Sapien nec sagittis aliquam malesuada bibendum. Est ultricies integer quis auctor elit sed vulputate. Semper auctor neque vitae tempus quam pellentesque nec nam.Posuere sollicitudin aliquam ultrices sagittis orci a.Auctor neque vitae tempus quam pellentesque nec nam aliquam sem. Risus sed vulputate odio ut enim blandit.Nibh cras pulvinar mattis nunc sed. Purus in mollis nunc sed id. Vestibulum rhoncus est pellentesque elit ullamcorper dignissim.Sem fringilla ut morbi tincidunt augue interdum velit euismod in. Aliquet porttitor lacus luctus accumsan tortor. Varius morbi enim nunc faucibus a pellentesque. Ut placerat orci nulla pellentesque dignissim enim sit. Enim facilisis gravida neque convallis.Mauris ultrices eros in cursus.Dictum fusce ut placerat orci nulla pellentesque dignissim. Justo eget magna fermentum iaculis eu non.Penatibus et magnis dis parturient montes nascetur ridiculus. Amet consectetur adipiscing elit pellentesque habitant morbi tristique senectus et. Nunc non blandit massa enim nec. At varius vel pharetra vel turpis nunc.Nisl nisi scelerisque eu ultrices vitae auctor.Lectus arcu bibendum at varius vel pharetra vel turpis.Enim sit amet venenatis urna cursus eget.Cursus risus at ultrices mi tempus imperdiet nulla malesuada.Diam volutpat commodo sed egestas egestas fringilla phasellus. Tristique senectus et netus et malesuada fames. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Senectus et netus et malesuada fames ac turpis egestas. Netus et malesuada fames ac turpis. Dolor sed viverra ipsum nunc aliquet bibendum enim. At tempor commodo ullamcorper a lacus. Interdum velit laoreet id donec ultrices. Lectus nulla at volutpat diam ut venenatis tellus in metus. Porttitor eget dolor morbi non arcu risus quis varius. Amet risus nullam eget felis eget nunc. Elementum facilisis leo vel fringilla est. Phasellus faucibus scelerisque eleifend donec pretium. Fringilla est ullamcorper eget nulla facilisi etiam dignissim. Et ultrices neque ornare aenean. Sodales ut eu sem integer vitae justo eget. Vitae sapien pellentesque habitant morbi tristique. Felis donec et odio pellentesque diam volutpat commodo sed. Ut tortor pretium viverra suspendisse potenti. Egestas maecenas pharetra convallis posuere morbi. Sit amet purus gravida quis blandit turpis cursus. Nec dui nunc mattis enim. Cras pulvinar mattis nunc sed blandit libero volutpat sed.Condimentum mattis pellentesque id nibh tortor id aliquet lectus proin. Nullam non nisi est sit amet facilisis magna etiam tempor. Lacus luctus accumsan tortor posuere ac ut consequat semper.Augue interdum velit euismod in. Diam phasellus vestibulum lorem sed risus ultricies.Urna molestie at elementum eu.Eget arcu dictum varius duis at consectetur lorem. Mattis pellentesque id nibh tortor id aliquet lectus proin.Urna molestie at elementum eu facilisis sed.Tellus molestie nunc non blandit massa. Vivamus at augue eget arcu dictum varius duis at consectetur. Velit ut tortor pretium viverra.Nibh tortor id aliquet lectus proin nibh nisl condimentum id. Aliquam eleifend mi in nulla posuere. Feugiat sed lectus vestibulum mattis ullamcorper. Ut tortor pretium viverra suspendisse potenti nullam ac tortor.Quam viverra orci sagittis eu volutpat. Tellus pellentesque eu tincidunt tortor aliquam nulla facilisi cras fermentum. Sit amet mattis vulputate enim nulla aliquet.Aliquam purus sit amet luctus venenatis lectus.Scelerisque eu ultrices vitae auctor.Diam quis enim lobortis scelerisque fermentum dui faucibus. Nisi lacus sed viverra tellus in hac habitasse platea.Turpis egestas maecenas pharetra convallis.Consectetur purus ut faucibus pulvinar elementum integer enim neque.Cursus eget nunc scelerisque viverra mauris in aliquam sem fringilla.In cursus turpis massa tincidunt dui. Fames ac turpis egestas maecenas pharetra. Eu feugiat pretium nibh ipsum.Faucibus vitae aliquet nec ullamcorper.Facilisis mauris sit amet massa.Lobortis elementum nibh tellus molestie nunc. Turpis egestas pretium aenean pharetra magna ac.Augue mauris augue neque gravida in fermentum et sollicitudin ac. Odio tempor orci dapibus ultrices in. Vitae aliquet nec ullamcorper sit amet risus nullam eget.Aliquam faucibus purus in massa tempor nec feugiat nisl.Lacus vestibulum sed arcu non odio euismod.Blandit turpis cursus in hac.Placerat in egestas erat imperdiet sed euismod.Pellentesque elit eget gravida cum sociis natoque penatibus. Et molestie ac feugiat sed lectus vestibulum mattis. Tellus rutrum tellus pellentesque eu tincidunt tortor.Enim diam vulputate ut pharetra sit amet aliquam id diam. Maecenas accumsan lacus vel facilisis volutpat est.Auctor elit sed vulputate mi sit amet mauris commodo quis. Lorem mollis aliquam ut porttitor leo a diam sollicitudin.Donec pretium vulputate sapien nec sagittis aliquam malesuada bibendum.At ultrices mi tempus imperdiet nulla malesuada pellentesque elit.Ut pharetra sit amet aliquam id diam maecenas ultricies mi. Nunc id cursus metus aliquam eleifend mi.Pharetra sit amet aliquam id diam maecenas ultricies mi eget. Non consectetur a erat nam at lectus urna. Tristique senectus et netus et malesuada fames.Enim facilisis gravida neque convallis a. Aliquam ut porttitor leo a diam sollicitudin.Eros donec ac odio tempor orci dapibus ultrices in. Ut enim blandit volutpat maecenas volutpat blandit. Vitae proin sagittis nisl rhoncus mattis. Pretium vulputate sapien nec sagittis aliquam malesuada bibendum arcu.Lacus laoreet non curabitur gravida arcu ac tortor dignissim convallis. Neque volutpat ac tincidunt vitae semper quis lectus. Id donec ultrices tincidunt arcu non sodales.In tellus integer feugiat scelerisque varius morbi enim nunc faucibus. Pellentesque nec nam aliquam sem et tortor consequat id.Bibendum ut tristique et egestas quis. Adipiscing vitae proin sagittis nisl rhoncus mattis rhoncus urna.Aliquet risus feugiat in ante metus dictum at tempor commodo. Morbi tristique senectus et netus et malesuada fames. Est pellentesque elit ullamcorper dignissim cras. Sit amet volutpat consequat mauris.Arcu vitae elementum curabitur vitae.Nunc pulvinar sapien et ligula ullamcorper. Quis eleifend quam adipiscing vitae proin sagittis nisl rhoncus mattis. Eget aliquet nibh praesent tristique magna sit amet purus.Morbi quis commodo odio aenean sed adipiscing diam. Cursus sit amet dictum sit amet justo donec enim diam. Pellentesque pulvinar pellentesque habitant morbi.Pellentesque elit ullamcorper dignissim cras tincidunt. Dictumst quisque sagittis purus sit amet. Nec dui nunc mattis enim ut tellus elementum. Libero id faucibus nisl tincidunt eget nullam.Purus viverra accumsan in nisl nisi scelerisque eu ultrices.Et egestas quis ipsum suspendisse ultrices gravida dictum fusce.Accumsan sit amet nulla facilisi morbi tempus iaculis. Faucibus et molestie ac feugiat sed lectus.Dignissim suspendisse in est ante in. Ut faucibus pulvinar elementum integer enim neque volutpat ac tincidunt. Enim blandit volutpat maecenas volutpat blandit aliquam.Magna eget est lorem ipsum dolor sit.Condimentum id venenatis a condimentum vitae sapien pellentesque habitant. Platea dictumst quisque sagittis purus sit. Tincidunt nunc pulvinar sapien et.Commodo quis imperdiet massa tincidunt.Rhoncus dolor purus non enim praesent. Mauris pharetra et ultrices neque ornare aenean euismod elementum.Purus semper eget duis at tellus at urna. Tellus in hac habitasse platea dictumst vestibulum rhoncus. Phasellus faucibus scelerisque eleifend donec.Arcu odio ut sem nulla pharetra diam.Duis ut diam quam nulla porttitor massa.Bibendum neque egestas congue quisque egestas diam.Neque sodales ut etiam sit amet nisl purus. Curabitur vitae nunc sed velit dignissim sodales.Sapien nec sagittis aliquam malesuada bibendum. Est ultricies integer quis auctor elit sed vulputate. Semper auctor neque vitae tempus quam pellentesque nec nam.Posuere sollicitudin aliquam ultrices sagittis orci a.Auctor neque vitae tempus quam pellentesque nec nam aliquam sem. Risus sed vulputate odio ut enim blandit.Nibh cras pulvinar mattis nunc sed. Purus in mollis nunc sed id. Vestibulum rhoncus est pellentesque elit ullamcorper dignissim.Sem fringilla ut morbi tincidunt augue interdum velit euismod in. Aliquet porttitor lacus luctus accumsan tortor. Varius morbi enim nunc faucibus a pellentesque. Ut placerat orci nulla pellentesque dignissim enim sit. Enim facilisis gravida neque convallis.Mauris ultrices eros in cursus.Dictum fusce ut placerat Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Senectus et netus et malesuada fames ac turpis egestas. Netus et malesuada fames ac turpis. Dolor sed viverra ipsum nunc aliquet bibendum enim. At tempor commodo ullamcorper a lacus. Interdum velit laoreet id donec ultrices. Lectus nulla at volutpat diam ut venenatis tellus in metus. Porttitor eget dolor morbi non arcu risus quis varius. Amet risus nullam eget felis eget nunc. Elementum facilisis leo vel fringilla est. Phasellus faucibus scelerisque eleifend donec pretium. Fringilla est ullamcorper eget nulla facilisi etiam dignissim. Et ultrices neque ornare aenean. Sodales ut eu sem integer vitae justo eget. Vitae sapien pellentesque habitant morbi tristique. Felis donec et odio pellentesque diam volutpat commodo sed. Ut tortor pretium viverra suspendisse potenti. Egestas maecenas pharetra convallis posuere morbi. Sit amet purus gravida quis blandit turpis cursus. Nec dui nunc mattis enim. Cras pulvinar mattis nunc sed blandit libero volutpat sed.Condimentum mattis pellentesque id nibh tortor id aliquet lectus proin. Nullam non nisi est sit amet facilisis magna etiam tempor. Lacus luctus accumsan tortor posuere ac ut consequat semper.Augue interdum velit euismod in. Diam phasellus vestibulum lorem sed risus ultricies.Urna molestie at elementum eu.Eget arcu dictum varius duis at consectetur lorem. Mattis pellentesque id nibh tortor id aliquet lectus proin.Urna molestie at elementum eu facilisis sed.Tellus molestie nunc non blandit massa. Vivamus at augue eget arcu dictum varius duis at consectetur. Velit ut tortor pretium viverra.Nibh tortor id aliquet lectus proin nibh nisl condimentum id. Aliquam eleifend mi in nulla posuere. Feugiat sed lectus vestibulum mattis ullamcorper. Ut tortor pretium viverra suspendisse potenti nullam ac tortor.Quam viverra orci sagittis eu volutpat. Tellus pellentesque eu tincidunt tortor aliquam nulla facilisi cras fermentum. Sit amet mattis vulputate enim nulla aliquet.Aliquam purus sit amet luctus venenatis lectus.Scelerisque eu ultrices vitae auctor.Diam quis enim lobortis scelerisque fermentum dui faucibus. Nisi lacus sed viverra tellus in hac habitasse platea.Turpis egestas maecenas pharetra convallis.Consectetur purus ut faucibus pulvinar elementum integer enim neque.Cursus eget nunc scelerisque viverra mauris in aliquam sem fringilla.In cursus turpis massa tincidunt dui. Fames ac turpis egestas maecenas pharetra. Eu feugiat pretium nibh ipsum.Faucibus vitae aliquet nec ullamcorper.Facilisis mauris sit amet massa.Lobortis elementum nibh tellus molestie nunc. Turpis egestas pretium aenean pharetra magna ac.Augue mauris augue neque gravida in fermentum et sollicitudin ac. Odio tempor orci dapibus ultrices in. Vitae aliquet nec ullamcorper sit amet risus nullam eget.Aliquam faucibus purus in massa tempor nec feugiat nisl.Lacus vestibulum sed arcu non odio euismod.Blandit turpis cursus in hac.Placerat in egestas erat imperdiet sed euismod.Pellentesque elit eget gravida cum sociis natoque penatibus. Et molestie ac feugiat sed lectus vestibulum mattis. Tellus rutrum tellus pellentesque eu tincidunt tortor.Enim diam vulputate ut pharetra sit amet aliquam id diam. Maecenas accumsan lacus vel facilisis volutpat est.Auctor elit sed vulputate mi sit amet mauris commodo quis. Lorem mollis aliquam ut porttitor leo a diam sollicitudin.Donec pretium vulputate sapien nec sagittis aliquam malesuada bibendum.At ultrices mi tempus imperdiet nulla malesuada pellentesque elit.Ut pharetra sit amet aliquam id diam maecenas ultricies mi. Nunc id cursus metus aliquam eleifend mi.Pharetra sit amet aliquam id diam maecenas ultricies mi eget. Non consectetur a erat nam at lectus urna. Tristique senectus et netus et malesuada fames.Enim facilisis gravida neque convallis a. Aliquam ut porttitor leo a diam sollicitudin.Eros donec ac odio tempor orci dapibus ultrices in. Ut enim blandit volutpat maecenas volutpat blandit. Vitae proin sagittis nisl rhoncus mattis. Pretium vulputate sapien nec sagittis aliquam malesuada bibendum arcu.Lacus laoreet non curabitur gravida arcu ac tortor dignissim convallis. Neque volutpat ac tincidunt vitae semper quis lectus. Id donec ultrices tincidunt arcu non sodales.In tellus integer feugiat scelerisque varius morbi enim nunc faucibus. Pellentesque nec nam aliquam sem et tortor consequat id.Bibendum ut tristique et egestas quis. Adipiscing vitae proin sagittis nisl rhoncus mattis rhoncus urna.Aliquet risus feugiat in ante metus dictum at tempor commodo. Morbi tristique senectus et netus et malesuada fames. Est pellentesque elit ullamcorper dignissim cras. Sit amet volutpat consequat mauris.Arcu vitae elementum curabitur vitae.Nunc pulvinar sapien et ligula ullamcorper. Quis eleifend quam adipiscing vitae proin sagittis nisl rhoncus mattis. Eget aliquet nibh praesent tristique magna sit amet purus.Morbi quis commodo odio aenean sed adipiscing diam. Cursus sit amet dictum sit amet justo donec enim diam. Pellentesque pulvinar pellentesque habitant morbi.Pellentesque elit ullamcorper dignissim cras tincidunt. Dictumst quisque sagittis purus sit amet. Nec dui nunc mattis enim ut tellus elementum. Libero id faucibus nisl tincidunt eget nullam.Purus viverra accumsan in nisl nisi scelerisque eu ultrices.Et egestas quis ipsum suspendisse ultrices gravida dictum fusce.Accumsan sit amet nulla facilisi morbi tempus iaculis. Faucibus et molestie ac feugiat sed lectus.Dignissim suspendisse in est ante in. Ut faucibus pulvinar elementum integer enim neque volutpat ac tincidunt. Enim blandit volutpat maecenas volutpat blandit aliquam.Magna eget est lorem ipsum dolor sit.Condimentum id venenatis a condimentum vitae sapien pellentesque habitant. Platea dictumst quisque sagittis purus sit. Tincidunt nunc pulvinar sapien et.Commodo quis imperdiet massa tincidunt.Rhoncus dolor purus non enim praesent. Mauris pharetra et ultrices neque ornare aenean euismod elementum.Purus semper eget duis at tellus at urna. Tellus in hac habitasse platea dictumst vestibulum rhoncus. Phasellus faucibus scelerisque eleifend donec.Arcu odio ut sem nulla pharetra diam.Duis ut diam quam nulla porttitor massa.Bibendum neque egestas congue quisque egestas diam.Neque sodales ut etiam sit amet nisl purus. Curabitur vitae nunc sed velit dignissim sodales.Sapien nec sagittis aliquam malesuada bibendum. Est ultricies integer quis auctor elit sed vulputate. Semper auctor neque vitae tempus quam pellentesque nec nam.Posuere sollicitudin aliquam ultrices sagittis orci a.Auctor neque vitae tempus quam pellentesque nec nam aliquam sem. Risus sed vulputate odio ut enim blandit.Nibh cras pulvinar mattis nunc sed. Purus in mollis nunc sed id. Vestibulum rhoncus est pellentesque elit ullamcorper dignissim.Sem fringilla ut morbi tincidunt augue interdum velit euismod in. Aliquet porttitor lacus luctus accumsan tortor. Varius morbi enim nunc faucibus a pellentesque. Ut placerat orci nulla pellentesque dignissim enim sit. Enim facilisis gravida neque convallis.Mauris ultrices eros in cursus.Dictum fusce ut placerat Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Senectus et netus et malesuada fames ac turpis egestas. Netus et malesuada fames ac turpis. Dolor sed viverra ipsum nunc aliquet bibendum enim. At tempor commodo ullamcorper a lacus. Interdum velit laoreet id donec ultrices. Lectus nulla at volutpat diam ut venenatis tellus in metus. Porttitor eget dolor morbi non arcu risus quis varius. Amet risus nullam eget felis eget nunc. Elementum facilisis leo vel fringilla est. Phasellus faucibus scelerisque eleifend donec pretium. Fringilla est ullamcorper eget nulla facilisi etiam dignissim. Et ultrices neque ornare aenean. Sodales ut eu sem integer vitae justo eget. Vitae sapien pellentesque habitant morbi tristique. Felis donec et odio pellentesque diam volutpat commodo sed. Ut tortor pretium viverra suspendisse potenti. Egestas maecenas pharetra convallis posuere morbi. Sit amet purus gravida quis blandit turpis cursus. Nec dui nunc mattis enim. Cras pulvinar mattis nunc sed blandit libero volutpat sed.Condimentum mattis pellentesque id nibh tortor id aliquet lectus proin. Nullam non nisi est sit amet facilisis magna etiam tempor. Lacus luctus accumsan tortor posuere ac ut consequat semper.Augue interdum velit euismod in. Diam phasellus vestibulum lorem sed risus ultricies.Urna molestie at elementum eu.Eget arcu dictum varius duis at consectetur lorem. Mattis pellentesque id nibh tortor id aliquet lectus proin.Urna molestie at elementum eu facilisis sed.Tellus molestie nunc non blandit massa. Vivamus at augue eget arcu dictum varius duis at consectetur. Velit ut tortor pretium viverra.Nibh tortor id aliquet lectus proin nibh nisl condimentum id. Aliquam eleifend mi in nulla posuere. Feugiat sed lectus vestibulum mattis ullamcorper. Ut tortor pretium viverra suspendisse potenti nullam ac tortor.Quam viverra orci sagittis eu volutpat. Tellus pellentesque eu tincidunt tortor aliquam nulla facilisi cras fermentum. Sit amet mattis vulputate enim nulla aliquet.Aliquam purus sit amet luctus venenatis lectus.Scelerisque eu ultrices vitae auctor.Diam quis enim lobortis scelerisque fermentum dui faucibus. Nisi lacus sed viverra tellus in hac habitasse platea.Turpis egestas maecenas pharetra convallis.Consectetur purus ut faucibus pulvinar elementum integer enim neque.Cursus eget nunc scelerisque viverra mauris in aliquam sem fringilla.In cursus turpis massa tincidunt dui. Fames ac turpis egestas maecenas pharetra. Eu feugiat pretium nibh ipsum.Faucibus vitae aliquet nec ullamcorper.Facilisis mauris sit amet massa.Lobortis elementum nibh tellus molestie nunc. Turpis egestas pretium aenean pharetra magna ac.Augue mauris augue neque gravida in fermentum et sollicitudin ac. Odio tempor orci dapibus ultrices in. Vitae aliquet nec ullamcorper sit amet risus nullam eget.Aliquam faucibus purus in massa tempor nec feugiat nisl.Lacus vestibulum sed arcu non odio euismod.Blandit turpis cursus in hac.Placerat in egestas erat imperdiet sed euismod.Pellentesque elit eget gravida cum sociis natoque penatibus. Et molestie ac feugiat sed lectus vestibulum mattis. Tellus rutrum tellus pellentesque eu tincidunt tortor.Enim diam vulputate ut pharetra sit amet aliquam id diam. Maecenas accumsan lacus vel facilisis volutpat est.Auctor elit sed vulputate mi sit amet mauris commodo quis. Lorem mollis aliquam ut porttitor leo a diam sollicitudin.Donec pretium vulputate sapien nec sagittis aliquam malesuada bibendum.At ultrices mi tempus imperdiet nulla malesuada pellentesque elit.Ut pharetra sit amet aliquam id diam maecenas ultricies mi. Nunc id cursus metus aliquam eleifend mi.Pharetra sit amet aliquam id diam maecenas ultricies mi eget. Non consectetur a erat nam at lectus urna. Tristique senectus et netus et malesuada fames.Enim facilisis gravida neque convallis a. Aliquam ut porttitor leo a diam sollicitudin.Eros donec ac odio tempor orci dapibus ultrices in. Ut enim blandit volutpat maecenas volutpat blandit. Vitae proin sagittis nisl rhoncus mattis. Pretium vulputate sapien nec sagittis aliquam malesuada bibendum arcu.Lacus laoreet non curabitur gravida arcu ac tortor dignissim convallis. Neque volutpat ac tincidunt vitae semper quis lectus. Id donec ultrices tincidunt arcu non sodales.In tellus integer feugiat scelerisque varius morbi enim nunc faucibus. Pellentesque nec nam aliquam sem et tortor consequat id.Bibendum ut tristique et egestas quis. Adipiscing vitae proin sagittis nisl rhoncus mattis rhoncus urna.Aliquet risus feugiat in ante metus dictum at tempor commodo. Morbi tristique senectus et netus et malesuada fames. Est pellentesque elit ullamcorper dignissim cras. Sit amet volutpat consequat mauris.Arcu vitae elementum curabitur vitae.Nunc pulvinar sapien et ligula ullamcorper. Quis eleifend quam adipiscing vitae proin sagittis nisl rhoncus mattis. Eget aliquet nibh praesent tristique magna sit amet purus.Morbi quis commodo odio aenean sed adipiscing diam. Cursus sit amet dictum sit amet justo donec enim diam. Pellentesque pulvinar pellentesque habitant morbi.Pellentesque elit ullamcorper dignissim cras tincidunt. Dictumst quisque sagittis purus sit amet. Nec dui nunc mattis enim ut tellus elementum. Libero id faucibus nisl tincidunt eget nullam.Purus viverra accumsan in nisl nisi scelerisque eu ultrices.Et egestas quis ipsum suspendisse ultrices gravida dictum fusce.Accumsan sit amet nulla facilisi morbi tempus iaculis. Faucibus et molestie ac feugiat sed lectus.Dignissim suspendisse in est ante in. Ut faucibus pulvinar elementum integer enim neque volutpat ac tincidunt. Enim blandit volutpat maecenas volutpat blandit aliquam.Magna eget est lorem ipsum dolor sit.Condimentum id venenatis a condimentum vitae sapien pellentesque habitant. Platea dictumst quisque sagittis purus sit. Tincidunt nunc pulvinar sapien et.Commodo quis imperdiet massa tincidunt.Rhoncus dolor purus non enim praesent. Mauris pharetra et ultrices neque ornare aenean euismod elementum.Purus semper eget duis at tellus at urna. Tellus in hac habitasse platea dictumst vestibulum rhoncus. Phasellus faucibus scelerisque eleifend donec.Arcu odio ut sem nulla pharetra diam.Duis ut diam quam nulla porttitor massa.Bibendum neque egestas congue quisque egestas diam.Neque sodales ut etiam sit amet nisl purus. Curabitur vitae nunc sed velit dignissim sodales.Sapien nec sagittis aliquam malesuada bibendum. Est ultricies integer quis auctor elit sed vulputate. Semper auctor neque vitae tempus quam pellentesque nec nam.Posuere sollicitudin aliquam ultrices sagittis orci a.Auctor neque vitae tempus quam pellentesque nec nam aliquam sem. Risus sed vulputate odio ut enim blandit.Nibh cras pulvinar mattis nunc sed. Purus in mollis nunc sed id. Vestibulum rhoncus est pellentesque elit ullamcorper dignissim.Sem fringilla ut morbi tincidunt augue interdum velit euismod in. Aliquet porttitor lacus luctus accumsan tortor. Varius morbi enim nunc faucibus a pellentesque. Ut placerat orci nulla pellentesque dignissim enim sit. Enim facilisis gravida neque convallis.Mauris ultrices eros in cursus.Dictum fusce ut placerat Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Senectus et netus et malesuada fames ac turpis egestas. Netus et malesuada fames ac turpis. Dolor sed viverra ipsum nunc aliquet bibendum enim. At tempor commodo ullamcorper a lacus. Interdum velit laoreet id donec ultrices. Lectus nulla at volutpat diam ut venenatis tellus in metus. Porttitor eget dolor morbi non arcu risus quis varius. Amet risus nullam eget felis eget nunc. Elementum facilisis leo vel fringilla est. Phasellus faucibus scelerisque eleifend donec pretium. Fringilla est ullamcorper eget nulla facilisi etiam dignissim. Et ultrices neque ornare aenean. Sodales ut eu sem integer vitae justo eget. Vitae sapien pellentesque habitant morbi tristique. Felis donec et odio pellentesque diam volutpat commodo sed. Ut tortor pretium viverra suspendisse potenti. Egestas maecenas pharetra convallis posuere morbi. Sit amet purus gravida quis blandit turpis cursus. Nec dui nunc mattis enim. Cras pulvinar mattis nunc sed blandit libero volutpat sed.Condimentum mattis pellentesque id nibh tortor id aliquet lectus proin. Nullam non nisi est sit amet facilisis magna etiam tempor. Lacus luctus accumsan tortor posuere ac ut consequat semper.Augue interdum velit euismod in. Diam phasellus vestibulum lorem sed risus ultricies.Urna molestie at elementum eu.Eget arcu dictum varius duis at consectetur lorem. Mattis pellentesque id nibh tortor id aliquet lectus proin.Urna molestie at elementum eu facilisis sed.Tellus molestie nunc non blandit massa. Vivamus at augue eget arcu dictum varius duis at consectetur. Velit ut tortor pretium viverra.Nibh tortor id aliquet lectus proin nibh nisl condimentum id. Aliquam eleifend mi in nulla posuere. Feugiat sed lectus vestibulum mattis ullamcorper. Ut tortor pretium viverra suspendisse potenti nullam ac tortor.Quam viverra orci sagittis eu volutpat. Tellus pellentesque eu tincidunt tortor aliquam nulla facilisi cras fermentum. Sit amet mattis vulputate enim nulla aliquet.Aliquam purus sit amet luctus venenatis lectus.Scelerisque eu ultrices vitae auctor.Diam quis enim lobortis scelerisque fermentum dui faucibus. Nisi lacus sed viverra tellus in hac habitasse platea.Turpis egestas maecenas pharetra convallis.Consectetur purus ut faucibus pulvinar elementum integer enim neque.Cursus eget nunc scelerisque viverra mauris in aliquam sem fringilla.In cursus turpis massa tincidunt dui. Fames ac turpis egestas maecenas pharetra. Eu feugiat pretium nibh ipsum.Faucibus vitae aliquet nec ullamcorper.Facilisis mauris sit amet massa.Lobortis elementum nibh tellus molestie nunc. Turpis egestas pretium aenean pharetra magna ac.Augue mauris augue neque gravida in fermentum et sollicitudin ac. Odio tempor orci dapibus ultrices in. Vitae aliquet nec ullamcorper sit amet risus nullam eget.Aliquam faucibus purus in massa tempor nec feugiat nisl.Lacus vestibulum sed arcu non odio euismod.Blandit turpis cursus in hac.Placerat in egestas erat imperdiet sed euismod.Pellentesque elit eget gravida cum sociis natoque penatibus. Et molestie ac feugiat sed lectus vestibulum mattis. Tellus rutrum tellus pellentesque eu tincidunt tortor.Enim diam vulputate ut pharetra sit amet aliquam id diam. Maecenas accumsan lacus vel facilisis volutpat est.Auctor elit sed vulputate mi sit amet mauris commodo quis. Lorem mollis aliquam ut porttitor leo a diam sollicitudin.Donec pretium vulputate sapien nec sagittis aliquam malesuada bibendum.At ultrices mi tempus imperdiet nulla malesuada pellentesque elit.Ut pharetra sit amet aliquam id diam maecenas ultricies mi. Nunc id cursus metus aliquam eleifend mi.Pharetra sit amet aliquam id diam maecenas ultricies mi eget. Non consectetur a erat nam at lectus urna. Tristique senectus et netus et malesuada fames.Enim facilisis gravida neque convallis a. Aliquam ut porttitor leo a diam sollicitudin.Eros donec ac odio tempor orci dapibus ultrices in. Ut enim blandit volutpat maecenas volutpat blandit. Vitae proin sagittis nisl rhoncus mattis. Pretium vulputate sapien nec sagittis aliquam malesuada bibendum arcu.Lacus laoreet non curabitur gravida arcu ac tortor dignissim convallis. Neque volutpat ac tincidunt vitae semper quis lectus. Id donec ultrices tincidunt arcu non sodales.In tellus integer feugiat scelerisque varius morbi enim nunc faucibus. Pellentesque nec nam aliquam sem et tortor consequat id.Bibendum ut tristique et egestas quis. Adipiscing vitae proin sagittis nisl rhoncus mattis rhoncus urna.Aliquet risus feugiat in ante metus dictum at tempor commodo. Morbi tristique senectus et netus et malesuada fames. Est pellentesque elit ullamcorper dignissim cras. Sit amet volutpat consequat mauris.Arcu vitae elementum curabitur vitae.Nunc pulvinar sapien et ligula ullamcorper. Quis eleifend quam adipiscing vitae proin sagittis nisl rhoncus mattis. Eget aliquet nibh praesent tristique magna sit amet purus.Morbi quis commodo odio aenean sed adipiscing diam. Cursus sit amet dictum sit amet justo donec enim diam. Pellentesque pulvinar pellentesque habitant morbi.Pellentesque elit ullamcorper dignissim cras tincidunt. Dictumst quisque sagittis purus sit amet. Nec dui nunc mattis enim ut tellus elementum. Libero id faucibus nisl tincidunt eget nullam.Purus viverra accumsan in nisl nisi scelerisque eu ultrices.Et egestas quis ipsum suspendisse ultrices gravida dictum fusce.Accumsan sit amet nulla facilisi morbi tempus iaculis. Faucibus et molestie ac feugiat sed lectus.Dignissim suspendisse in est ante in. Ut faucibus pulvinar elementum integer enim neque volutpat ac tincidunt. Enim blandit volutpat maecenas volutpat blandit aliquam.Magna eget est lorem ipsum dolor sit.Condimentum id venenatis a condimentum vitae sapien pellentesque habitant. Platea dictumst quisque sagittis purus sit. Tincidunt nunc pulvinar sapien et.Commodo quis imperdiet massa tincidunt.Rhoncus dolor purus non enim praesent. Mauris pharetra et ultrices neque ornare aenean euismod elementum.Purus semper eget duis at tellus at urna. Tellus in hac habitasse platea dictumst vestibulum rhoncus. Phasellus faucibus scelerisque eleifend donec.Arcu odio ut sem nulla pharetra diam.Duis ut diam quam nulla porttitor massa.Bibendum neque egestas congue quisque egestas diam.Neque sodales ut etiam sit amet nisl purus. Curabitur vitae nunc sed velit dignissim sodales.Sapien nec sagittis aliquam malesuada bibendum. Est ultricies integer quis auctor elit sed vulputate. Semper auctor neque vitae tempus quam pellentesque nec nam.Posuere sollicitudin aliquam ultrices sagittis orci a.Auctor neque vitae tempus quam pellentesque nec nam aliquam sem. Risus sed vulputate odio ut enim blandit.Nibh cras pulvinar mattis nunc sed. Purus in mollis nunc sed id. Vestibulum rhoncus est pellentesque elit ullamcorper dignissim.Sem fringilla ut morbi tincidunt augue interdum velit euismod in. Aliquet porttitor lacus luctus accumsan tortor. Varius morbi enim nunc faucibus a pellentesque. Ut placerat orci nulla pellentesque dignissim enim sit. Enim facilisis gravida neque convallis.Mauris ultrices eros in cursus.Dictum fusce ut placerat";
            // dog.Base64String = "/9j/4AAQSkZJRgABAQAAAQABAAD/4gHYSUNDX1BST0ZJTEUAAQEAAAHIAAAAAAQwAABtbnRyUkdCIFhZWiAH4AABAAEAAAAAAABhY3NwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQAA9tYAAQAAAADTLQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAlkZXNjAAAA8AAAACRyWFlaAAABFAAAABRnWFlaAAABKAAAABRiWFlaAAABPAAAABR3dHB0AAABUAAAABRyVFJDAAABZAAAAChnVFJDAAABZAAAAChiVFJDAAABZAAAAChjcHJ0AAABjAAAADxtbHVjAAAAAAAAAAEAAAAMZW5VUwAAAAgAAAAcAHMAUgBHAEJYWVogAAAAAAAAb6IAADj1AAADkFhZWiAAAAAAAABimQAAt4UAABjaWFlaIAAAAAAAACSgAAAPhAAAts9YWVogAAAAAAAA9tYAAQAAAADTLXBhcmEAAAAAAAQAAAACZmYAAPKnAAANWQAAE9AAAApbAAAAAAAAAABtbHVjAAAAAAAAAAEAAAAMZW5VUwAAACAAAAAcAEcAbwBvAGcAbABlACAASQBuAGMALgAgADIAMAAxADb/2wBDAP//////////////////////////////////////////////////////////////////////////////////////2wBDAf//////////////////////////////////////////////////////////////////////////////////////wAARCAFlAO4DASIAAhEBAxEB/8QAFwABAQEBAAAAAAAAAAAAAAAAAAECA//EACcQAQABAgUEAgMBAQAAAAAAAAABEfAhMUFhcQJRgfGR4aGx0cES/8QAFQEBAQAAAAAAAAAAAAAAAAAAAAH/xAAUEQEAAAAAAAAAAAAAAAAAAAAA/9oADAMBAAIRAxEAPwDAAAAAAAAAAAAAAAAAAAAAAEQ1giA1VJQAAABQQAAAAAAAAAAAAAAAAAAAAAAAAUBAAVAAAAAAAAAABQQWAEFgBAUEFAKLSAQSUWUUUAAEBUAAAAAAKLQEFoUBBaFASFEBap7WIq6REXiDmOoDkOk0yZmI05Biq1QBaiALKL4PAAeDwAi+AEFAQXEABAUABUEFBFEBY7g1lFGulis/WtG+nIFABjvKQuH7ZFWY1hldpQQABoSFQAAAAEVAAAAFABAFQAp4/c+GopmnVKjLVKxS5llrUFpGc9vmif8AU3hC9Wl4sA1We7cTXNyaicoBuYqxSHRmdvqQZpF5JK10MM0VkamGVQVFgFARUFBEAAAUAEAUBFADHL6SVie8ExqokNx3Yi9m408gS5tywA1DKwDorF8NVBMNfRSiTO/+kax5RRhtmQRYRb3VFAxRRAEAAAFABAAABQRqL7Jy1WNFGdWpyYlqMrwBmZnVGpSgIKUBMVrJrTgmMfgEahP0uopMsqgCosXuIoCKACAgCoAACgAgAAoigkw1GxN92oUShfCoKkR/FFBmaRNdfwmf9/xc52iPmey7QITGFIZpLUKDKTDdFFcqX3L8N0vuzMCIoIAICoAAAACgAgAANR992VBsIFC+UvssnIIzOOGn7bSgGUUj2nvhpAGkUAlCoM1vJb4+mbuS9wA9iAGAAgAAAAKACAKgCwjUXuDaKigIagoACLfLPf5BpWFrpINDN+2gY6ovsjc5MXsgCAKXyACLigAAIoKKiKgHAAYtR9Mr05qNorM3sBM6Rf8AViP7zKQATnTz4ExvXbgn+A17ZmcYSt7G/vkGomL02lJxIWASf5BE4rP2wDpX81+YZnC8krfaVv6BAXFBAAAAAARS+Tyoire6ABsALF8oXyDozMLH0oOd8tR9pMNR9gqUaQGaJr+G2Jz/AByCxT4urTEZ3My3AIxLcpOX5BhfQAXwvpF98IIACoF7AAKIpfK33qDKgAAgX4L5L5FG2nNuMgJABQARiY1n8Y4OgDFNcWlARJy/LTEgz7IAAL4EF9IF7QC35TG8g9+FAADERfQB532X4QBFAQhWoikAzGboxN7LILUc2oBuFZiWgAQFRKpX+A1OTm11ToyCKXxAAGl4AHsC+AAvhAX2GyAC3ugC3yACxF90iKtTNMATVZZqiKt+ClEWb5VEWEAabYjJoFZr/OCWQWWWphkBUAX+oLe4F8mBflAXc9iAoIC9xAAAFajp7/UJFK48t1gEnDhhZmrIAAC6IsdrqAjdJZnOAa0+ZVmv+rEgSw16QG2Jb0KVBzPSzFEABQPRfOyKABfgARcf9A3uIQABb7IAAAAAAAsZofQOrHU0k5SDAAAANxk0z05Ng59Wfwys532QBUAF3RQAgpedQRRL7gKgC4F7ot8AIAAAAAALGcA6Mzk0kg5gAAA10ujEZNA5znPlF1+UBUU4BFNC9+QC+Q45AQABffKAAAAAAoIogK109/HDP35a6Z0BtJGZnD5BgVAAAbjJpjp7eXQHKc58+RerNAMN/wB+DyGF6gi32g4y/aAoc/RfACACoKCAAB6AAAURQL5N0AWs3mgt8giiAAA103tDo5Q6gkxWKXDnMUwujqx1cY/iYBne5X/fmI0S/KAtL5Rb2AENwFDe4QFQAAABUAVAAUBFRQL9gAgt+dEAvuAC3w1E9+GFB0ZnTarKAAAewAFv0i3sCCoAAAAAAAAAAAoAn2AAAAAAAAAAoAgAKTmAIAAAD//Z";
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

            //Task<SetResponse> setResponse = client.SetAsync("dogs/" + ID, dog);
            //
            //if (setResponse.IsCanceled)
            //{
            //    return RedirectToAction("Beginning", "Dog");
            //}
            //else
            //{
            //    return View();
            //}


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
                // string path = Path.GetFullPath(item.Value.ImageImg);
                // byte[] imageArray = System.IO.File.ReadAllBytes(path);
                // string base64ImageRepresentation = Convert.ToBase64String(imageArray);


                //  ImageCodecInfo myImageCodecInfo = GetEncoderInfo(item.Value.);
                //  using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
                //      {
                //          using (MemoryStream m = new MemoryStream())
                //          {
                //              image.Save(m, image.RawFormat);
                //              byte[] imageBytes = m.ToArray();
                //              var base64String = Convert.ToBase64String(imageBytes);
                //          }
                //      }


                dogList.Add(new Dog()
                {
                    Id = item.Key,
                    Name = item.Value.Name,
                    Gender = item.Value.Gender,
                    DateOfArrival = item.Value.DateOfArrival,
                    ChildFriendly = item.Value.ChildFriendly,
                    Age = item.Value.Age,
                    Size = item.Value.Size,
                    File = item.Value.File,//Image = Convert.ToBase64String(item.Value.ImageImg)
                                          // base64ImageRepresentation = item.Value.Image
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
                            //return View(model);
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
                        //return View(model);
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
                    //return View(model);
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
                    //return View(model);
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
                    //return View(model);
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

            /*
            List<Dog> list1 = new List<Dog>();
            foreach (var element in list)
            {
                if (option1 == "Yes")
                {
                    if (element.ChildFriendly == "tak")
                    {
                        list1.Add(element);
                    }
                    else
                    {
                       
                    }
                    //return View(model);
                }
                else if (option1 == "No")
                {
                    if (element.ChildFriendly == "nie")
                    {
                        list1.Add(element);
                    }
                }

            }

            return View(list1.ToList());
            
            */

            // if (option == "Name")
            // {
            //     var model = dogList.Where(d => d.Name.StartsWith(search) || search == null).ToList();
            //     return View(model);
            //
            // }
            // else
            // {
            //     var model = dogList.Where(d => d.ChildFriendly.StartsWith(search) || search == null).ToList();
            //     return View(model);
            // }


            // if (option == "Name")
            // {
            //     //Index action method will return a view with a student records based on what a user specify the value in textbox  
            //     return View(dogList.Where(d => d.Name.StartsWith(search) || search == null).ToList());
            // }
            // else
            // {
            //     return View(dogList.Where(d => d.ChildFriendly.StartsWith(search) || search == null).ToList());
            // }
        }





    }
}