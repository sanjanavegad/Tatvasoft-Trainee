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
    public class ProviderPagesController : Controller
    {
        private readonly HelperlandContext _helperlandContext;

        public ProviderPagesController(HelperlandContext helperlandContext)
        {
            _helperlandContext = helperlandContext;
        }

        public IActionResult Provider_Dashboard()
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            var user = _helperlandContext.Users.Where(x => x.UserId == id).FirstOrDefault();
            return View(user);
        }

        [HttpGet]
        public IActionResult ProviderNewService()
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                int id = (int)HttpContext.Session.GetInt32("UserId");
                List<ServiceRequest> serviceRequest = ObjHelperlandContext.ServiceRequests.Where(x =>x.Status == 4).ToList();
                //status=> 1=Complete, 2=Cancel, 3=Pending, 4=NewRequest


                List<User> user = new List<User>();
                foreach (ServiceRequest users in serviceRequest)
                {
                    var u = _helperlandContext.Users.Where(x => x.UserId == users.UserId).FirstOrDefault();
                    users.Name = u.FirstName + " " + u.LastName;
                }

                List<ServiceRequestAddress> add = new List<ServiceRequestAddress>();
                foreach (ServiceRequest users in serviceRequest)
                {
                    var address = _helperlandContext.ServiceRequestAddresses.Where(x => x.ServiceRequestId == users.ServiceRequestId).FirstOrDefault();
                    users.AddressLine1 = address.AddressLine1;
                    users.AddressLine2 = address.AddressLine2;
                    users.City = address.City;
                    users.ZipCode = address.PostalCode;
                }

                ViewBag.services = serviceRequest;
                return PartialView("_SPNewService");
            }
            
        }
        [HttpGet]
        public IActionResult NewServiceDetailes(int id)
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                var servicerequest = ObjHelperlandContext.ServiceRequests.Where(x => x.ServiceRequestId == id).FirstOrDefault();
                var address = ObjHelperlandContext.ServiceRequestAddresses.Where(x => x.ServiceRequestId == id).FirstOrDefault();
                var u = _helperlandContext.Users.Where(x => x.UserId == servicerequest.UserId).FirstOrDefault();
                servicerequest.Name = u.FirstName + " " + u.LastName;
                servicerequest.AddressLine1 = address.AddressLine1;
                servicerequest.AddressLine2 = address.AddressLine2;
                servicerequest.Email = address.Email;
                servicerequest.Mobile = address.Mobile;

                var extraservice = _helperlandContext.ServiceRequestExtras.Where(c => c.ServiceRequestId == id).FirstOrDefault();
                servicerequest.Extra = extraservice.ServiceExtraId;

                return PartialView("_ProviderModelPartial", servicerequest);
            }
        }
        [HttpPost]
        public IActionResult AcceptServiceButton(int id)
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                int spid = (int)HttpContext.Session.GetInt32("UserId");
                var servicerequest = ObjHelperlandContext.ServiceRequests.Where(x => x.ServiceRequestId == id).FirstOrDefault();
                servicerequest.Status = 3;
                servicerequest.ServiceProviderId = spid;
                var result = _helperlandContext.ServiceRequests.Update(servicerequest);
                _helperlandContext.SaveChanges();
                if (result != null)
                {
                    return Ok(Json("true"));
                }
                return Ok(Json("False"));
            }
        }
        
        [HttpGet]
        public IActionResult ProviderUpCommingService()
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                int id = (int)HttpContext.Session.GetInt32("UserId");
                List<ServiceRequest> serviceRequest = ObjHelperlandContext.ServiceRequests.Where(x => x.ServiceProviderId == id && x.Status == 3).ToList();
                //status=> 1=Complete, 2=Cancel, 3=Pending, 4=NewRequest


                List<User> user = new List<User>();
                foreach (ServiceRequest users in serviceRequest)
                {
                    var u = _helperlandContext.Users.Where(x => x.UserId == users.UserId).FirstOrDefault();
                    users.Name = u.FirstName + " " + u.LastName;
                }

                List<ServiceRequestAddress> add = new List<ServiceRequestAddress>();
                foreach (ServiceRequest users in serviceRequest)
                {
                    var address = _helperlandContext.ServiceRequestAddresses.Where(x => x.ServiceRequestId == users.ServiceRequestId).FirstOrDefault();
                    users.AddressLine1 = address.AddressLine1;
                    users.AddressLine2 = address.AddressLine2;
                    users.City = address.City;
                    users.ZipCode = address.PostalCode;
                }

                ViewBag.services = serviceRequest;
                return PartialView("_SPUpcomingService");
            }

        }
        
        [HttpPost]
        public IActionResult CancelServiceButton(int id)
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                int spid = (int)HttpContext.Session.GetInt32("UserId");
                var servicerequest = ObjHelperlandContext.ServiceRequests.Where(x => x.ServiceRequestId == id).FirstOrDefault();
                servicerequest.Status = 4;
                servicerequest.ServiceProviderId = null;
                var result = _helperlandContext.ServiceRequests.Update(servicerequest);
                _helperlandContext.SaveChanges();
                if (result != null)
                {
                    return Ok(Json("true"));
                }
                return Ok(Json("False"));
            }
        }

        [HttpPost]
        public IActionResult CompaleteServiceButton(int id)
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                int spid = (int)HttpContext.Session.GetInt32("UserId");
                var servicerequest = ObjHelperlandContext.ServiceRequests.Where(x => x.ServiceRequestId == id).FirstOrDefault();
                servicerequest.Status = 1;
                var result = _helperlandContext.ServiceRequests.Update(servicerequest);
                _helperlandContext.SaveChanges();
                if (result != null)
                {
                    return Ok(Json("true"));
                }
                return Ok(Json("False"));
            }
        }

        [HttpGet]
        public IActionResult ProviderServiceSchedule()
        {
            return PartialView("_SPServiceSchedule");
        } 

        [HttpGet]
        public IActionResult ProviderServiceHistory()
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                int id = (int)HttpContext.Session.GetInt32("UserId");
                List<ServiceRequest> serviceRequest = ObjHelperlandContext.ServiceRequests.Where(x => x.ServiceProviderId == id && x.Status == 1).ToList();
                //status=> 1=Complete, 2=Cancel, 3=Pending, 4=NewRequest
                List<User> user = new List<User>();
                foreach (ServiceRequest users in serviceRequest)
                {
                    var u = _helperlandContext.Users.Where(x => x.UserId == users.UserId).FirstOrDefault();
                    users.Name = u.FirstName + " " + u.LastName;
                }

                List<ServiceRequestAddress> add = new List<ServiceRequestAddress>();
                foreach (ServiceRequest users in serviceRequest)
                {
                    var address = _helperlandContext.ServiceRequestAddresses.Where(x => x.ServiceRequestId == users.ServiceRequestId).FirstOrDefault();
                    users.AddressLine1 = address.AddressLine1;
                    users.AddressLine2 = address.AddressLine2;
                    users.City = address.City;
                    users.ZipCode = address.PostalCode;
                }
                ViewBag.services = serviceRequest;
                return PartialView("_SPServiceHistory");
            }
        }

        [HttpGet]
        public IActionResult ProviderRatings()
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                int id = (int)HttpContext.Session.GetInt32("UserId");
                List<ServiceRequest> serviceRequest = ObjHelperlandContext.ServiceRequests.Where(x => x.ServiceProviderId == id && x.Status == 1).ToList();
                //status=> 1=Complete, 2=Cancel, 3=Pending

                List<User> user = new List<User>();
                foreach (ServiceRequest users in serviceRequest)
                {
                    if (users.ServiceProviderId != null)
                    {
                        var u = _helperlandContext.Users.Where(x => x.UserId == users.UserId).FirstOrDefault();
                        users.Name = u.FirstName + " " + u.LastName;
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
                return PartialView("_SPServiceRatings");
            }
        }

        public IActionResult ProviderBlockCustomer()
        {
            var userid = (int)HttpContext.Session.GetInt32("UserId");

            var p = _helperlandContext.ServiceRequests.Where(c => c.ServiceProviderId == userid && c.Status == 1).AsEnumerable().GroupBy(x => x.UserId).ToList();

            List<User> users = new List<User>();
            List<string> blocklist = new List<string>();
            foreach (var i in p)
            {
                User temp = _helperlandContext.Users.Find(i.Key);
                users.Add(temp);

                var blockornot = _helperlandContext.FavoriteAndBlockeds.Where(c => c.UserId == userid && c.TargetUserId == i.Key).FirstOrDefault();
                if (blockornot != null)
                {
                    if (blockornot.IsBlocked == true)
                    {
                        string value = "yes";
                        blocklist.Add(value);
                    }
                    else
                    {
                        string value = "no";
                        blocklist.Add(value);
                    }
                }
                else
                {

                    string value = "no";
                    blocklist.Add(value);
                }
            }

            ViewBag.blockedornot = blocklist;
            return PartialView("_SPServiceBlockCustomer", users);
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
                _helperlandContext.FavoriteAndBlockeds.Add(block);
                _helperlandContext.SaveChanges();
            }
            else
            {
                p.IsBlocked = true;
                _helperlandContext.SaveChanges();
            }
            return PartialView("_SPServiceBlockCustomer");
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
            return PartialView("_SPServiceBlockCustomer");
        }

        [HttpGet]
        public IActionResult ProviderSettings()
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                int id = (int)HttpContext.Session.GetInt32("UserId");
                User user = ObjHelperlandContext.Users.FirstOrDefault(x => x.UserId == id);

                UserAddress addsressrequest = ObjHelperlandContext.UserAddresses.Where(x => x.UserId == id).FirstOrDefault();
                if (addsressrequest != null)
                {
                    ViewBag.AddressLine1 = addsressrequest.AddressLine1;
                    ViewBag.AddressLine2 = addsressrequest.AddressLine2;
                    ViewBag.City = addsressrequest.City;
                    ViewBag.PostalCode = addsressrequest.PostalCode;
                }
                ViewBag.users = user;
            }
            return PartialView("_SPSettings");
        }

        [HttpPost]
        public IActionResult UpdateProviderDetailes(User user)
        {
            int? Id = HttpContext.Session.GetInt32("UserId");
            if (Id != null)
            {
                User updated = _helperlandContext.Users.FirstOrDefault(x => x.UserId == Id);
                updated.FirstName = user.FirstName;
                updated.LastName = user.LastName;
                updated.Mobile = user.Mobile;
                updated.NationalityId = user.NationalityId;
                updated.Gender = user.Gender;
                updated.UserProfilePicture = user.UserProfilePicture;
                updated.ZipCode = user.PostalCode;
                if (user.Date != null && user.Month != null && user.Year != null)
                {
                    var dob = user.Date + "-" + user.Month + "-" + user.Year;
                    updated.DateOfBirth = Convert.ToDateTime(dob);
                }
                updated.ModifiedDate = DateTime.Now;
                var result = _helperlandContext.Users.Update(updated);
                _helperlandContext.SaveChanges();


                var useraddress = _helperlandContext.UserAddresses.Where(c => c.UserId == Id).FirstOrDefault();
                if (useraddress != null)
                {
                    useraddress.PostalCode = user.PostalCode;
                    useraddress.AddressLine1 = user.AddressLine1;
                    useraddress.AddressLine2 = user.AddressLine2;
                    useraddress.City = user.City;
                    var serviceaddResult = _helperlandContext.UserAddresses.Update(useraddress);
                    _helperlandContext.SaveChanges();
                }
                else
                {
                    UserAddress serviceadd = new UserAddress();
                    serviceadd.UserId = (int)Id;
                    serviceadd.AddressLine1 = user.AddressLine1;
                    serviceadd.AddressLine2 = user.AddressLine2;
                    serviceadd.City = user.City;
                    serviceadd.Email = user.Email;
                    serviceadd.Mobile = user.Mobile;
                    serviceadd.PostalCode = user.PostalCode;
                    var serviceaddResult = _helperlandContext.UserAddresses.Add(serviceadd);
                    _helperlandContext.SaveChanges();
                }
                
            }
            return PartialView("_SPSettings");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(User user)
        {
            var userid = (int)HttpContext.Session.GetInt32("UserId");

            var cuser = _helperlandContext.Users.Where(c => c.UserId == userid).FirstOrDefault();

            if (user.Password == cuser.Password)
            {
                cuser.Password = user.NewPassword;

                await _helperlandContext.SaveChangesAsync();
                //ModelState.Clear();


                return PartialView("_SPSettings");
            }
            else
            {
                //please check your current password
                return StatusCode(500);
            }



        }
    }
}
