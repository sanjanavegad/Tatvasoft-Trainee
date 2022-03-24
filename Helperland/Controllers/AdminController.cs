using Helperland.Data;
using Helperland.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helperland.Controllers
{
    public class AdminController : Controller
    {
        private readonly HelperlandContext _helperlandContext;

        public AdminController(HelperlandContext helperlandContext)
        {
            _helperlandContext = helperlandContext;
        }
        public IActionResult AdminIndex()
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            var user = _helperlandContext.Users.Where(x => x.UserId == id).FirstOrDefault();
            user.Name = user.FirstName + " " + user.LastName;
            return View(user);
        }
        [HttpGet]
        public IActionResult AdminDashboard(SearchUser data)
        {
            List<User> userdetailes = _helperlandContext.Users.Where(x => x.UserTypeId != 3).ToList();
            userdetailes = userdetailes.Where(x => (string.IsNullOrEmpty(data.Name) || x.FirstName.ToLower().Contains(data.Name.ToLower())) &&
                                                   (!data.UserTypeId.HasValue || x.UserTypeId == data.UserTypeId) &&
                                                   (string.IsNullOrEmpty(data.Mobile) || x.Mobile.ToLower().Contains(data.Mobile.ToLower())) &&
                                                   (string.IsNullOrEmpty(data.ZipCode) || (!string.IsNullOrEmpty(x.ZipCode) ? x.ZipCode.ToLower().Contains(data.ZipCode.ToLower()) : false))).ToList();
            ViewBag.data = userdetailes;
            return PartialView("User_ManagementPartial");
        }
        //[HttpPost]
        //public IActionResult AdminDashboard(SearchUser data)
        //{
        //        List<User> userdetailes = _helperlandContext.Users.Where(x => x.UserTypeId != 3).ToList();
        //        userdetailes = userdetailes.Where(x => (string.IsNullOrEmpty(data.Name) || x.FirstName.ToLower().Contains(data.Name.ToLower())) &&
        //                                           (string.IsNullOrEmpty(data.Mobile) || x.Mobile.ToLower().Contains(data.Mobile.ToLower())) &&
        //                                           (string.IsNullOrEmpty(data.PostalCode) || x.PostalCode.ToLower().Contains(data.PostalCode.ToLower())) &&
        //                                           (!data.UserTypeId.HasValue || x.UserTypeId == data.UserTypeId)).ToList();
        //        ViewBag.data = userdetailes;
        //    return PartialView("User_ManagementPartial");
        //}
        public IActionResult ApproveUser(int id)
        {
            User user = _helperlandContext.Users.Where(x => x.UserId == id).FirstOrDefault();
            return PartialView("ApprovedModelPartial", user);
        }
        public IActionResult FinalApproveUser(User user)
        {
            User users = _helperlandContext.Users.Where(x => x.UserId == user.UserId).FirstOrDefault();
            users.IsActive = true;
            users.IsApproved = true;
            users.Status = 1;
            var aprroveuser = _helperlandContext.Users.Update(users);
            _helperlandContext.SaveChanges();
            if (aprroveuser != null)
            {
                return Ok(Json("true"));
            }
            return Ok(Json("false"));
        }
        public IActionResult DeactivateUser(int id)
        {
            User users = _helperlandContext.Users.Where(x => x.UserId == id).FirstOrDefault();
            users.IsActive = false;
            users.Status = 0;
            var aprroveuser = _helperlandContext.Users.Update(users);
            _helperlandContext.SaveChanges();
            if (aprroveuser != null)
            {
                return Ok(Json("true"));
            }
            return Ok(Json("false"));
        }
        public IActionResult ActivateUser(int id)
        {
            User users = _helperlandContext.Users.Where(x => x.UserId == id).FirstOrDefault();
            users.IsActive = true;
            users.Status = 1;
            var aprroveuser = _helperlandContext.Users.Update(users);
            _helperlandContext.SaveChanges();
            if (aprroveuser != null)
            {
                return Ok(Json("true"));
            }
            return Ok(Json("false"));
        }
        [HttpGet]
        public IActionResult User_Rquest(SearchUser data)
        {
            List<ServiceRequest> serviceRequest = _helperlandContext.ServiceRequests.ToList();

            List<ServiceRequestAddress> add = new List<ServiceRequestAddress>();
            foreach (ServiceRequest users in serviceRequest)
            {
                var address = _helperlandContext.ServiceRequestAddresses.Where(x => x.ServiceRequestId == users.ServiceRequestId).FirstOrDefault();
                users.AddressLine1 = address.AddressLine1;
                users.AddressLine2 = address.AddressLine2;
                users.City = address.City;
                users.ZipCode = address.PostalCode;
            }


            List<User> uname = new List<User>();
            foreach (ServiceRequest users in serviceRequest)
            {
                var username = _helperlandContext.Users.Where(x => x.UserId == users.UserId).FirstOrDefault();
                users.UserName = username.FirstName + " " + username.LastName;
            }
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

            if(serviceRequest.Count > 0)
            {
                serviceRequest = serviceRequest.Where(x => (!data.ServiceRequestId.HasValue || x.ServiceRequestId == data.ServiceRequestId) &&
                                                  (string.IsNullOrEmpty(data.UserName) || x.UserName.ToLower().Contains(data.UserName.ToLower())) &&
                                                  (string.IsNullOrEmpty(data.Name) || (!string.IsNullOrEmpty(x.Name) ? x.Name.ToLower().Contains(data.Name.ToLower()) : false)) &&
                                                  //(string.IsNullOrEmpty(data.PostalCode) || x.PostalCode.ToLower().Contains(data.PostalCode.ToLower())) &&
                                                  (!data.Status.HasValue || x.Status == data.Status)&&
                                                  (!data.ServiceStartDate.HasValue || x.ServiceStartDate.Date == data.ServiceStartDate.Value.Date)).ToList();
            }
           

            ViewBag.services = serviceRequest;
            return PartialView("User_RequestPartial");
        }
        [HttpPost]
        public IActionResult SearchRequest(ServiceRequest data)
        {
            List<ServiceRequest> serviceRequest = _helperlandContext.ServiceRequests.ToList();
            serviceRequest = serviceRequest.Where(x => (string.IsNullOrEmpty(data.UserName) || x.UserName.ToLower().Contains(data.UserName.ToLower()))).ToList();
            ViewBag.data = serviceRequest;
            return PartialView("User_RequestPartial");
        }
        [HttpGet]
        public IActionResult EditServiceModel(int id)
        {
            ServiceRequest user = _helperlandContext.ServiceRequests.Where(x => x.ServiceRequestId == id).FirstOrDefault();
            ServiceRequestAddress address = _helperlandContext.ServiceRequestAddresses.Where(x => x.ServiceRequestId == id).FirstOrDefault();
            user.AddressLine1 = address.AddressLine1;
            user.AddressLine2 = address.AddressLine2;
            user.ZipCode = address.PostalCode;
            user.City = address.City;
            return PartialView("EditServiceModelPartial", user);
        }
        [HttpPost]
        public IActionResult UpdateServiceRequest(ServiceRequest data)
        {
            ServiceRequest update = _helperlandContext.ServiceRequests.Where(x => x.ServiceRequestId == data.ServiceRequestId).FirstOrDefault();
            //string date = data.ServiceStartDate.ToString("yyyy-MM-dd");
            //string time = data.ServiceTime.ToString("HH:mm:ss");
            //DateTime startDateTime = Convert.ToDateTime(date).Add(TimeSpan.Parse(time));
            //update.ServiceStartDate = startDateTime;
            //update.ModifiedDate = DateTime.Now;
            //var result = _helperlandContext.ServiceRequests.Update(update);
            //_helperlandContext.SaveChanges();

            ServiceRequestAddress updateadd = _helperlandContext.ServiceRequestAddresses.Where(x => x.ServiceRequestId == data.ServiceRequestId).FirstOrDefault();
            updateadd.AddressLine1 = data.AddressLine1;
            updateadd.AddressLine2 = data.AddressLine2;
            updateadd.PostalCode = data.ZipCode;
            updateadd.City = data.City;
            var result1 = _helperlandContext.ServiceRequestAddresses.Update(updateadd);
            _helperlandContext.SaveChanges();
            return PartialView("User_RequestPartial");
        }

        [HttpGet]
        public IActionResult CancelSPbyCust(int id)
        {
            ServiceRequest sr = _helperlandContext.ServiceRequests.Where(x => x.ServiceRequestId == id).FirstOrDefault();
            sr.Status = 4;
            sr.ServiceProviderId = null;
            var cancelSP = _helperlandContext.ServiceRequests.Update(sr);
            _helperlandContext.SaveChanges();
            if (cancelSP != null)
            {
                return Ok(Json("true"));
            }
            return Ok(Json("false"));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Msg"] = "You have succesfully logged out";
            return RedirectToAction("Index", "Home");
        }
    }
}
