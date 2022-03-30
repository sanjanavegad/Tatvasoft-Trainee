using Helperland.Core;
using Helperland.Data;
using Helperland.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Helperland.Controllers
{
    public class CustomerPagesController : Controller
    {
        private readonly HelperlandContext _helperlandContext;

        public CustomerPagesController(HelperlandContext helperlandContext)
        {
            _helperlandContext = helperlandContext;
        }
        [HttpGet]
        public IActionResult Book_Now()
        {
            string loggedin = HttpContext.Session.GetString("isLoggedIn");

            if (loggedin == "true")
            {
                
                //ViewBag.loginreqmsg = "Login is Required";
                return View();
            }
            else
            {
                TempData["loginreq"] = "Login is Required";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult ValidZip(Zipcodeviewmodel ForZip)
        {
            HttpContext.Session.SetString("zipcode", ForZip.ZipcodeValue);
            //var Zipcode = _helperlandContext.Zipcodes.Where(z => z.ZipcodeValue == ForZip.ZipcodeValue);

            var Zipcode = _helperlandContext.Users.Where(x => x.ZipCode == ForZip.ZipcodeValue && x.UserTypeId == 2);
            if (Zipcode.Count() > 0)
            {
                return Ok(Json("true"));
            }
            return Ok(Json("false"));
        }

        [HttpPost]
        public ActionResult Scheduledetails()
        {
            if (ModelState.IsValid)
            {
                return Ok(Json("true"));
            }
            else
            {
                return Ok(Json("false"));
            }
        }
        
        [HttpGet]
        public JsonResult Loadaddress()
        { 
            var zipcode = HttpContext.Session.GetString("zipcode");
            List<UserAddress> Address = new List<UserAddress>();
            int? Id = HttpContext.Session.GetInt32("UserId");
            var result = _helperlandContext.UserAddresses.Where(x => x.UserId == Id && x.IsDeleted == false && x.PostalCode == zipcode).ToList();
            foreach (var add in result)
            {
                UserAddress useraddress = new UserAddress();
                useraddress.AddressId = add.AddressId;
                useraddress.AddressLine1 = add.AddressLine1;
                useraddress.AddressLine2 = add.AddressLine2;
                useraddress.City = add.City;
                useraddress.PostalCode = add.PostalCode;
                useraddress.IsDefault = add.IsDefault;
                useraddress.Mobile = add.Mobile;
                Address.Add(useraddress);
            }
            return new JsonResult(Address);
        }
        
        [HttpPost]
        public ActionResult NewAddress(UserAddress newuseradd)
        {
            int? Id = HttpContext.Session.GetInt32("UserId");
            newuseradd.UserId = (int)Id;
            newuseradd.IsDefault = true;
            newuseradd.IsDeleted = false;
            User user = _helperlandContext.Users.Where(x => x.UserId == Id).FirstOrDefault();
            newuseradd.Email = user.Email;
            var result = _helperlandContext.UserAddresses.Add(newuseradd);
            _helperlandContext.SaveChanges();

            User updated = _helperlandContext.Users.FirstOrDefault(x => x.UserId == Id);
            updated.ZipCode = newuseradd.PostalCode;
            updated.ModifiedDate = DateTime.Now;
            var result1 = _helperlandContext.Users.Update(updated);
            _helperlandContext.SaveChanges();
            if (result != null && result1 != null)
            {
                return Ok(Json("true"));
            }
            return Ok(Json("False"));

        }

        [HttpPost]
        public ActionResult Completebooking(Completebooking Finalbooking)
        {
            int Id = (int)HttpContext.Session.GetInt32("UserId");
            ServiceRequest add = new ServiceRequest();
            add.UserId = Id;
            add.ServiceId = Id;
            //add.ServiceStartDate = Finalbooking.ServiceStartDate;
            add.ZipCode = Finalbooking.ZipcodeValue;
            add.ServiceHourlyRate = 25;
            add.ServiceHours = Finalbooking.ServiceHours;
            add.ExtraHours = Finalbooking.ExtraHours;
            add.SubTotal = (decimal)(add.ServiceHours + add.ExtraHours);
            add.TotalCost = (decimal)(add.SubTotal * add.ServiceHourlyRate);
            add.Comments = Finalbooking.Comments;
            add.PaymentDue = false;
            add.PaymentDone = true;
            add.HasPets = Finalbooking.HasPets;
            add.CreatedDate = DateTime.Now;
            add.ModifiedDate = DateTime.Now;
            add.HasIssue = false;

            string date = Finalbooking.ServiceDate.ToString("yyyy-MM-dd");
            string time = Finalbooking.ServiceTime.ToString("HH:mm:ss");
            DateTime startDateTime = Convert.ToDateTime(date).Add(TimeSpan.Parse(time));
            add.ServiceStartDate = startDateTime;
            add.Status = 4;
            var result = _helperlandContext.ServiceRequests.Add(add);
            _helperlandContext.SaveChanges();
            int id = add.ServiceRequestId;

            UserAddress UserAddress = _helperlandContext.UserAddresses.Where(x => x.AddressId == Finalbooking.AddressId).FirstOrDefault();

            ServiceRequestAddress serviceadd = new ServiceRequestAddress();
            serviceadd.AddressLine1 = UserAddress.AddressLine1;
            serviceadd.AddressLine2 = UserAddress.AddressLine2;
            serviceadd.City = UserAddress.City;
            serviceadd.Email = UserAddress.Email;
            serviceadd.Mobile = UserAddress.Mobile;
            serviceadd.PostalCode = UserAddress.PostalCode;
            serviceadd.ServiceRequestId = id;
            serviceadd.State = UserAddress.State;
            var serviceaddResult = _helperlandContext.ServiceRequestAddresses.Add(serviceadd);
            _helperlandContext.SaveChanges();

            char[] extra = { '0', '0', '0', '0', '0' };
            if (Finalbooking.Cabinet == true)
            {
                extra[0] = '1';
            }
            if (Finalbooking.Oven == true)
            {
                extra[1] = '1';
            }
            if (Finalbooking.Window == true)
            {
                extra[2] = '1';
            }
            if (Finalbooking.Fridge == true)
            {
                extra[3] = '1';
            }
            if (Finalbooking.Laundry == true)
            {
                extra[4] = '1';
            }
            string etc = new string(extra);
            int extraids = Convert.ToInt32(etc);
            ServiceRequestExtra extraservice = new ServiceRequestExtra();
            extraservice.ServiceRequestId = id;
            extraservice.ServiceExtraId = extraids;
            _helperlandContext.ServiceRequestExtras.Add(extraservice);
            _helperlandContext.SaveChanges();

            List<User> availableSPsInGivenZipcode = (from u in _helperlandContext.Users
                                                     join fb in _helperlandContext.FavoriteAndBlockeds on u.UserId equals fb.UserId into fb1
                                                     from fb in fb1.DefaultIfEmpty()
                                                     where u.ZipCode == add.ZipCode && u.IsApproved == true && u.UserTypeId == 2 && Convert.ToInt32(add.UserId) != fb.TargetUserId
                                                     select u).ToList();

            foreach (User sps in availableSPsInGivenZipcode)
            {
                var subject = "New Service Request Available!!";
                var body = "Hi " + sps.FirstName + " " +sps.LastName + "<br/> A service request Id : " + add.ServiceRequestId + " has been directly assigned to you.";
                SendEmail(sps.Email, body, subject);
            }
            if (result != null && serviceaddResult != null)
                {
                    return Ok(Json(result.Entity.ServiceRequestId));
                }
                return Ok(Json("False"));
           
        }
        
        private void SendEmail(string emailaddress,string body, string subject)
        {
            using (MailMessage mm = new MailMessage("sanjanavegad123@gmail.com", emailaddress))
            {
                mm.Subject = subject;
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    EnableSsl = true
                };
                NetworkCredential NetworkCred = new NetworkCredential("sanjanavegad123@gmail.com", "s@sanju123");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
                ViewBag.message = "Email send to admin successfully";
            }
        }

        [HttpGet]
        public IActionResult Customer_Dashboard()
        {
              int id = (int)HttpContext.Session.GetInt32("UserId");
            var user = _helperlandContext.Users.Where(x => x.UserId == id).FirstOrDefault();
            return View(user);
        }

        [HttpGet]
        public IActionResult CustomerDashboard()
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                int id = (int)HttpContext.Session.GetInt32("UserId");
                List<ServiceRequest> serviceRequest = ObjHelperlandContext.ServiceRequests.Where(x => x.UserId == id && (x.Status == 3 || x.Status == 4)).ToList();
                //status=> 1=Complete, 2=Cancel, 3=Pending

                List<User> user = new List<User>();
                foreach (ServiceRequest users in serviceRequest)
                {
                    if (users.ServiceProviderId != null)
                    {
                        var providername = _helperlandContext.Users.Where(x => x.UserId == users.ServiceProviderId).FirstOrDefault();
                        users.Name = providername.FirstName + " " + providername.LastName;
                        users.UserProfilePicture = providername.UserProfilePicture;
                        var rate = _helperlandContext.Ratings.Where(c => c.ServiceRequestId == users.ServiceRequestId).ToList();
                        decimal temp = 0;
                        foreach (Rating rating in rate)
                        {
                            if (rating.Ratings != 0)
                            {
                                temp += rating.Ratings;
                            }
                        }
                        if (rate.Count() != 0)
                        {
                            temp /= rate.Count();
                        }
                        users.ratings = temp;
                    }
                    else
                    {
                        user.Add(null);
                    }
                }

                ViewBag.services = serviceRequest;
                return PartialView("CustomerDashboardPartial");
            }
        }

        public IActionResult ServiceDetailes(int id)
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                var servicerequest = ObjHelperlandContext.ServiceRequests.Where(x => x.ServiceRequestId == id).FirstOrDefault();
                var address = ObjHelperlandContext.ServiceRequestAddresses.Where(x => x.ServiceRequestId == id).FirstOrDefault();
                servicerequest.AddressLine1 = address.AddressLine1;
                servicerequest.AddressLine2 = address.AddressLine2;
                servicerequest.Email = address.Email;
                servicerequest.Mobile = address.Mobile;

                var extraservice = _helperlandContext.ServiceRequestExtras.Where(c => c.ServiceRequestId == id).FirstOrDefault();
                servicerequest.Extra = extraservice.ServiceExtraId;

                return PartialView("_CustomerModelPartial", servicerequest);
            }
        }

        [HttpGet]
        public IActionResult ServiceSchedule(int id)
        {
                var request = _helperlandContext.ServiceRequests.Where(x => x.ServiceRequestId == id).FirstOrDefault();
                return PartialView("_CustomerSchedulePartial", request);
        }

        [HttpPost]
        public IActionResult UpdateScheduleRequest(ServiceRequest schedule)
        {
            int? Id = HttpContext.Session.GetInt32("UserId");
            if (Id != null)
            {
                ServiceRequest updated = _helperlandContext.ServiceRequests.FirstOrDefault(x => x.ServiceRequestId == schedule.ServiceRequestId);
                string date = schedule.ServiceDate.ToString("yyyy-MM-dd");
                string time = schedule.ServiceTime.ToString("HH:mm:ss");
                DateTime startDateTime = Convert.ToDateTime(date).Add(TimeSpan.Parse(time));
                
                List<ServiceRequest> accept = _helperlandContext.ServiceRequests.Where(x => x.ServiceProviderId == updated.ServiceProviderId && x.Status == 3 && DateTime.Compare(x.ServiceStartDate.Date, startDateTime.Date) == 0).ToList();
                foreach (var test in accept)
                {
                    TimeSpan start = test.ServiceStartDate.TimeOfDay;
                    TimeSpan end = test.ServiceStartDate.AddHours(Convert.ToDouble(test.SubTotal)).TimeOfDay;
                    TimeSpan now = updated.ServiceStartDate.TimeOfDay;

                    if ((now >= start) && (now <= end))
                    {
                        return Ok(Json("false"));
                    }
                }

                updated.ServiceStartDate = startDateTime;
                updated.ModifiedDate = DateTime.Now;
                var result = _helperlandContext.ServiceRequests.Update(updated);
                _helperlandContext.SaveChanges();

                var user = _helperlandContext.ServiceRequests.Where(x => x.ServiceProviderId == updated.ServiceProviderId).FirstOrDefault();
                var email = _helperlandContext.Users.Where(sp => sp.UserId == user.ServiceProviderId).FirstOrDefault();

                var subject = "Recheduled Service by the customer";
                var body = "Hi " + "Service Request Id :" + updated.ServiceRequestId + "has been rescheduled by customer. New date and time are {" + date + time + "}";
                SendEmail(email.Email, body, subject);

                if (result != null)
                {
                    return Ok(Json("true"));
                }
                return Ok(Json("False"));
            }
            return PartialView("CustomerDashboardPartial");
        }

        [HttpGet]
        public IActionResult CancelServiceModel(int id)
        {
             var schedulerequest = _helperlandContext.ServiceRequests.Where(x => x.ServiceRequestId == id).FirstOrDefault();
             return PartialView("_CustomerServiceCancelPartial", schedulerequest);
        }

        [HttpPost]
        public IActionResult CancelServiceRequest(ServiceRequest cancel)
        {
            int? Id = HttpContext.Session.GetInt32("UserId");
            if (Id != null)
            {
                ServiceRequest cancelrequest = _helperlandContext.ServiceRequests.FirstOrDefault(x => x.ServiceRequestId == cancel.ServiceRequestId);
                cancelrequest.Status = 2;
                var result = _helperlandContext.ServiceRequests.Update(cancelrequest);
                _helperlandContext.SaveChanges();
                var email = _helperlandContext.Users.Where(sp => sp.UserId == cancelrequest.ServiceProviderId).FirstOrDefault();

                var subject = "Cancelled Service by the customer";
                var body = "Hi " + "Service Request Id :" + cancelrequest.ServiceRequestId + " has been cancelled by customer";
                SendEmail(email.Email, body, subject);
                if (result != null)
                {
                    return Ok(Json("true"));
                }
                return Ok(Json("False"));
            }
            return PartialView("CustomerDashboardPartial");
        }

        [HttpGet]
        public IActionResult CustomerSettings()
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                int id = (int)HttpContext.Session.GetInt32("UserId");
                User user = ObjHelperlandContext.Users.FirstOrDefault(x => x.UserId == id);

                List<UserAddress> addsressrequest = ObjHelperlandContext.UserAddresses.Where(x => x.UserId == id && x.IsDeleted == false).ToList();

                ViewBag.users = user;
                ViewBag.addresses = addsressrequest;
                return PartialView("CustomerSettingPartial");
            }
        }

        [HttpPost]
        public IActionResult UpdateUserDetailes(User user)
        {
            int? Id = HttpContext.Session.GetInt32("UserId");
            User updated = _helperlandContext.Users.FirstOrDefault(x => x.UserId == Id);
            updated.FirstName = user.FirstName;
            updated.LastName = user.LastName;
            updated.Mobile = user.Mobile;
            if (user.Day != null && user.Month != null && user.Year != null)
                {
                    var DateTime = user.Day + "-" + user.Month + "-" + user.Year;
                    updated.DateOfBirth = Convert.ToDateTime(DateTime);
                }
            updated.LanguageId = user.LanguageId;

            updated.ModifiedDate = DateTime.Now;
            var result = _helperlandContext.Users.Update(updated);
            _helperlandContext.SaveChanges();
            if (result != null)
            {
                return Ok(Json("true"));
            }
            return Ok(Json("False"));
        }

        [HttpGet]
        public IActionResult AddressModel(int id)
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                var user = ObjHelperlandContext.UserAddresses.Where(x => x.AddressId == id).FirstOrDefault();

                return PartialView("_CustomerAddressPartial", user);
            }
        }

        [HttpPost]
        public IActionResult UpdateUserAddress(UserAddress data)
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                UserAddress updated = ObjHelperlandContext.UserAddresses.FirstOrDefault(x => x.AddressId == data.AddressId);

                updated.AddressLine1 = data.AddressLine1;
                updated.AddressLine2 = data.AddressLine2;
                updated.PostalCode = data.PostalCode;
                updated.State = data.State;
                updated.Mobile = data.Mobile;
                updated.IsDefault = true;

                var result = _helperlandContext.UserAddresses.Update(updated);
                _helperlandContext.SaveChanges();

                User add = ObjHelperlandContext.Users.FirstOrDefault(x => x.UserId == updated.UserId);
                add.ZipCode = updated.PostalCode;
                var result1 = _helperlandContext.Users.Update(add);
                _helperlandContext.SaveChanges();
                return Ok(Json("true"));

            }
        }

        [HttpGet]
        public IActionResult DeleteAddressModel(int id)
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                var user = ObjHelperlandContext.UserAddresses.Where(x => x.AddressId == id).FirstOrDefault();

                return PartialView("_CustomerAddressDeletePartial", user);
            }
        }

        [HttpPost]
        public IActionResult DeleteUserAddress(UserAddress add)
        {
            int? Id = HttpContext.Session.GetInt32("UserId");
            if (Id != null)
            {
                UserAddress delete = _helperlandContext.UserAddresses.FirstOrDefault(x => x.AddressId == add.AddressId);
                delete.IsDeleted = true;
                var result = _helperlandContext.UserAddresses.Update(delete);
                _helperlandContext.SaveChanges();
                if (result != null)
                {
                    return Ok(Json("true"));
                }
                return Ok(Json("False"));
            }
            return PartialView("_CustomerAddressPartial");
        }

        [HttpPost]
        public IActionResult ChangePassword(User user)
        {
            var userid = (int)HttpContext.Session.GetInt32("UserId");

            var cuser = _helperlandContext.Users.Where(c => c.UserId == userid).FirstOrDefault();

            if (user.Password == cuser.Password)
            {
                cuser.Password = user.NewPassword;

                _helperlandContext.SaveChanges();
                ModelState.Clear();
                return PartialView("_CustomerAddressPartial");
            }
            else
            {
                return StatusCode(500);
            }

        }

        public IActionResult CustomerHistory()
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                int id = (int)HttpContext.Session.GetInt32("UserId");
                List<ServiceRequest> serviceRequest = ObjHelperlandContext.ServiceRequests.Where(x => x.UserId == id && (x.Status == 2 || x.Status == 1)).ToList();

                List<User> user = new List<User>();
                foreach (ServiceRequest users in serviceRequest)
                {
                    if (users.ServiceProviderId != null)
                    {
                        var providername = _helperlandContext.Users.Where(x => x.UserId == users.ServiceProviderId).FirstOrDefault();
                        users.Name = providername.FirstName + " " + providername.LastName;
                        users.UserProfilePicture = providername.UserProfilePicture;
                        var rate = _helperlandContext.Ratings.Where(c => c.ServiceRequestId == users.ServiceRequestId).ToList();
                        decimal temp = 0;
                        foreach (Rating rating in rate)
                        {
                            if (rating.Ratings != 0)
                            {
                                temp += rating.Ratings;
                            }
                        }
                        if (rate.Count() != 0)
                        {
                            temp /= rate.Count();
                        }
                        users.ratings = temp;
                    }
                    else
                    {
                        user.Add(null);
                    }
                }

                ViewBag.services = serviceRequest;
                return PartialView("CustomerHistoryPartial");
            }
        }

        [HttpGet]
        public IActionResult RateSPModal(int id)
        {
            var p = _helperlandContext.ServiceRequests.Where(c => c.ServiceRequestId == id).FirstOrDefault();
            var q = _helperlandContext.Users.Where(x => x.UserId == p.ServiceProviderId).FirstOrDefault();

            var rate = _helperlandContext.Ratings.Where(c => c.ServiceRequestId == p.ServiceRequestId).ToList();

            decimal temp = 0;

            foreach (Rating rating in rate)
            {
                if (rating.Ratings != 0)
                {
                    temp += rating.Ratings;
                }
            }
            if (rate.Count() != 0)
            {
                temp /= rate.Count();
            }
            ViewBag.servicerequestid = id;
            ViewBag.serviceproviderid = p.ServiceProviderId;
            ViewBag.rating = temp;
            ViewBag.Name = q.FirstName + " " + q.LastName;

            return View("_CustomerRateingPartial");
        }
        
        [HttpPost]
        public IActionResult AddRatings(Rating rating)
        {
            var userid = (int)HttpContext.Session.GetInt32("UserId");
            var p = _helperlandContext.Ratings.Where(c => c.ServiceRequestId == rating.ServiceRequestId).FirstOrDefault();

            if (p == null)
            {
                rating.Ratings = (rating.OnTimeArrival + rating.QualityOfService + rating.Friendly) / 3;
                rating.RatingFrom = userid;
                rating.RatingDate = DateTime.Now;
                var result = _helperlandContext.Ratings.Add(rating);
                _helperlandContext.SaveChanges();
                if (result != null)
                {
                    return Ok(Json("true"));
                }
                return Ok(Json("False"));
            }
            else
            {
                p.OnTimeArrival = rating.OnTimeArrival;
                p.Friendly = rating.Friendly;
                p.QualityOfService = rating.QualityOfService;
                p.Ratings = (p.OnTimeArrival + p.Friendly + p.QualityOfService) / 3;
                p.Comments = rating.Comments;
                p.RatingDate = DateTime.Now;
                var result1 = _helperlandContext.Ratings.Update(p);
                _helperlandContext.SaveChanges();
                if (result1 != null)
                {
                    return Ok(Json("true"));
                }
                return Ok(Json("False"));
            }
        }

        public IActionResult FavoriteAndPros()
        {
            var userid = (int)HttpContext.Session.GetInt32("UserId");

            var serviceRequests = _helperlandContext.ServiceRequests.Where(c => c.UserId == userid && c.Status == 1 && c.ServiceProviderId != null).AsEnumerable().GroupBy(x => x.ServiceProviderId).ToList();

            List<int> ids = new List<int>();
            List<int> counts = new List<int>();

            foreach (var i in serviceRequests)
            {
                counts.Add(i.Count());
                ids.Add(Convert.ToInt32(i.Key));
            }

            List<FavoriteAndBlocked> favoriteAndBlockeds = new List<FavoriteAndBlocked>();
            List<bool> hasEntry = new List<bool>();

            foreach (var i in ids)
            {
                FavoriteAndBlocked temp = _helperlandContext.FavoriteAndBlockeds.Where(x => x.UserId.Equals(userid) && x.TargetUserId.Equals(i)).FirstOrDefault();
                if (temp != null)
                {
                    favoriteAndBlockeds.Add(temp);
                    hasEntry.Add(true);
                }
                else
                {
                    favoriteAndBlockeds.Add(temp);
                    hasEntry.Add(false);
                }
            }

            ViewBag.favs = favoriteAndBlockeds;
            ViewBag.hasEntry = hasEntry;
            ViewBag.counts = counts;



            List<User> SPs = new List<User>();

            foreach (var i in ids)
            {
                var sp = _helperlandContext.Users.Find(i);
                SPs.Add(sp);
            }

            ViewBag.serviceProviders = SPs;
            return PartialView("CustomerFavoriteAndProsPartial");
        }

        public IActionResult FavoriteCustomer(int id)
        {
            var userid = (int)HttpContext.Session.GetInt32("UserId");
            var p = _helperlandContext.FavoriteAndBlockeds.Where(c => c.UserId == userid && c.TargetUserId == id).FirstOrDefault();
            if (p == null)
            {
                FavoriteAndBlocked block = new FavoriteAndBlocked
                {
                    UserId = userid,
                    TargetUserId = id,
                    IsBlocked = false,
                    IsFavorite = true
                };
                var result = _helperlandContext.FavoriteAndBlockeds.Add(block);
                
                _helperlandContext.SaveChanges();
                return Ok(Json("true"));
            }
            else
            {
                p.IsFavorite = true;
                var result1 = _helperlandContext.SaveChanges();
                return Ok(Json("true"));
            }
        }
        
        public IActionResult UnFavoriteCustomer(int id)
        {
            var userid = (int)HttpContext.Session.GetInt32("UserId");
            var p = _helperlandContext.FavoriteAndBlockeds.Where(c => c.UserId == userid && c.TargetUserId == id).FirstOrDefault();
            if (p != null)
            {
                p.IsFavorite = false;
                _helperlandContext.SaveChanges();
            }
            return Ok(Json("true"));
        }
        
        public IActionResult BlockCustomer(int id)
        {
            var userid = (int)HttpContext.Session.GetInt32("UserId");
            var p = _helperlandContext.FavoriteAndBlockeds.Where(c => c.UserId == userid && c.TargetUserId == id).FirstOrDefault();
            if (p == null)
            {
                FavoriteAndBlocked block = new FavoriteAndBlocked
                {
                    UserId = userid,
                    TargetUserId = id,
                    IsBlocked = true,
                    IsFavorite = false
                };
                var result = _helperlandContext.FavoriteAndBlockeds.Add(block);
                
                _helperlandContext.SaveChanges();
                return Ok(Json("true"));
            }
            else
            {
                p.IsBlocked = true;
                var result1 = _helperlandContext.SaveChanges();
                return Ok(Json("true"));
            }
        }
       
        public IActionResult UnBlockCustomer(int id)
        {
            var userid = (int)HttpContext.Session.GetInt32("UserId");
            var p = _helperlandContext.FavoriteAndBlockeds.Where(c => c.UserId == userid && c.TargetUserId == id).FirstOrDefault();
            if (p != null)
            {
                p.IsBlocked = false;
                _helperlandContext.SaveChanges();
            }
            return Ok(Json("true"));
        }

    }
}


 