using Helperland.Data;
using Helperland.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult ValidZip(Zipcodeviewmodel ForZip)
        {
            HttpContext.Session.SetString("ZipcodeValue", ForZip.ZipcodeValue);
            var Zipcode = _helperlandContext.Zipcodes.Where(z => z.ZipcodeValue == ForZip.ZipcodeValue);
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

            List<UserAddress> Address = new List<UserAddress>();
            int? Id = HttpContext.Session.GetInt32("UserId");
            var result = _helperlandContext.UserAddresses.Where(x => x.UserId == Id && x.IsDeleted == false).ToList();
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
            if (result != null)
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
            add.Status = 3;
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
            if (result != null && serviceaddResult != null)
            {
                return Ok(Json(result.Entity.ServiceRequestId));
            }
            return Ok(Json("False"));
        }

        [HttpGet]
        public IActionResult Customer_Dashboard()
        {
            return View();
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
                updated.ServiceStartDate = startDateTime;
                updated.ModifiedDate = DateTime.Now;
                var result = _helperlandContext.ServiceRequests.Update(updated);
                _helperlandContext.SaveChanges();
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
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                int? Id = HttpContext.Session.GetInt32("UserId");
                if (Id != null)
                {
                    User updated = _helperlandContext.Users.FirstOrDefault(x => x.UserId == Id);
                    updated.FirstName = user.FirstName;
                    updated.LastName = user.LastName;
                    updated.Mobile = user.Mobile;
                    if (user.Date != null && user.Month != null && user.Year != null)
                    {
                        var DateTime = user.Date + "-" + user.Month + "-" + user.Year;
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
                return PartialView("CustomerSettingPartial");
            }
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
                var result = _helperlandContext.UserAddresses.Update(updated);
                _helperlandContext.SaveChanges();
                if (result != null)
                {
                    return Ok(Json("true"));
                }
                return Ok(Json("False"));
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
            rating.Ratings = (rating.OnTimeArrival + rating.QualityOfService + rating.Friendly) / 3;
            rating.RatingFrom = userid;
            rating.RatingDate = DateTime.Now;

            if (p != null)
            {
                p.OnTimeArrival = rating.OnTimeArrival;
                p.QualityOfService = rating.QualityOfService;
                p.Friendly = rating.Friendly;
                p.Ratings = rating.Ratings;
                p.RatingDate = rating.RatingDate;
                p.RatingFrom = rating.RatingFrom;
                p.RatingTo = rating.RatingTo;
                _helperlandContext.SaveChanges();

            }
            else
            {
                p.Comments = rating.Comments;
                _helperlandContext.Ratings.Add(rating);
                _helperlandContext.SaveChanges();

            }
            return PartialView("CustomerHistoryPartial");
        }
    }
}


 