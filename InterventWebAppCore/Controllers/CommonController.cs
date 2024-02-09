using Intervent.DAL;
using Intervent.Web.DataLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLog;
using System.Text.RegularExpressions;

namespace InterventWebApp
{
    public class CommonController : BaseController
    {
        public const int ImageMinimumBytes = 512;

        private readonly IHostEnvironment environment;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommonController(UserManager<ApplicationUser> userManager, IHostEnvironment environment)
        {
            _userManager = userManager;
            this.environment = environment;
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> UploadFile(int? id, string action, int? formType, string langCode)
        {
            IFormFile image = Request.Form.Files["FileUpload"];
            if (image != null && !string.IsNullOrEmpty(image.FileName))
            {
                if (CheckIfImage(image))
                {
                    var postedFileExtension = Path.GetExtension(image.FileName);
                    var fileName = System.DateTime.Now.ToString("_ddMMyyhhmmssFFF") + postedFileExtension;
                    string path = "";
                    string uri = Request.Headers["Referer"].ToString();
                    string absolutePath = "";

                    if (action.ToLower().Contains("recipe") && image.ContentType.Contains("image"))
                    {
                        var recipeId = uri.Substring(uri.LastIndexOf("/") + 1);
                        path = Path.Combine(environment.ContentRootPath, "~/images/upload", fileName);
                        absolutePath = Url.Content("~/images/upload/" + fileName);
                        RecipeUtility.UpdateImageUrl(int.Parse(recipeId), fileName);
                    }
                    else if (action.ToLower().Contains("kit") && image.ContentType.Contains("pdf"))
                    {
                        var kitId = uri.Substring(uri.LastIndexOf("/") + 1);
                        path = Path.Combine(environment.ContentRootPath, "~/Pdf", image.FileName);
                        absolutePath = Url.Content("~/Pdf/" + image.FileName);
                        string pdfFiles = KitUtility.UploadPdf(int.Parse(kitId), image.FileName, langCode).UploadedPdf;

                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }
                            //image.SaveAs(path);
                            return Json(new { data = pdfFiles });
                        }
                    }
                    else if (action.ToLower().Contains("lab"))
                    {
                        if (image.ContentType.Contains("pdf") || image.ContentType.Contains("image"))
                        {
                            path = Path.Combine(environment.ContentRootPath, "~/Lab", fileName);
                            LabUtility.UpdateLabResultFile(id, fileName, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue ? HttpContext.Session.GetInt32(SessionContext.AdminId).Value : null, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), null, true);
                            if (!string.IsNullOrWhiteSpace(path))
                            {
                                using (var stream = new FileStream(path, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }
                                //image.SaveAs(path);
                                return Json(new { data = fileName, AdminView = TempData["AdminView"] != null ? TempData["AdminView"].ToString() : "False" });
                            }
                        }
                    }
                    else if (action.ToLower().Contains("incentive"))
                    {
                        if (image.ContentType.Contains("pdf") || image.ContentType.Contains("image"))
                        {
                            var userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                            path = Path.Combine(environment.ContentRootPath, "~/IncentiveUploads", fileName);
                            PortalUtility.AddTobaccoIncentive(fileName, userId, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetInt32(SessionContext.ProgramsInPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ProgramsInPortalId).Value : null);
                        }
                    }
                    else if (action.ToLower().Contains("form"))
                    {
                        if (image.ContentType.Contains("pdf") || image.ContentType.Contains("image"))
                        {
                            var userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                            path = Path.Combine(environment.ContentRootPath, "~/FormUploads", fileName);
                            absolutePath = fileName;
                            ParticipantUtility.AddUserForm(fileName, userId, formType.Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value);
                        }
                        else
                        {
                            return Json(new { data = "Failed" });
                        }

                    }
                    else if (image.ContentType.Contains("image"))
                    {
                        path = Path.Combine(environment.ContentRootPath, "~/ProfilePictures", fileName);
                        absolutePath = fileName;
                        await AccountUtility.UploadPicture(_userManager, id.Value, fileName, environment.ContentRootPath + "~/ProfilePictures");
                    }
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }
                        //image.SaveAs(path);
                        return Json(new { data = absolutePath });
                    }
                    else
                    {
                        return Json(new { data = "Failed" });
                    }
                }
                else
                {
                    return Json(new { data = "Failed" });
                }
            }
            else
                return Json(new { data = "Failed" });
        }
        [Authorize]
        [HttpPost]
        public async Task<JsonResult> ValidateImage()
        {
            IFormFile image = Request.Form.Files["FileUpload"];
            var response = CheckIfImage(image) ? "Success" : "Failed";
            return Json(new { data = response });
        }
        private bool CheckIfImage(IFormFile postedFile)
        {
            var postedFileExtension = Path.GetExtension(postedFile.FileName);
            if (string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
           !string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
           !string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
           !string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
           !string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
           !string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                try
                {
                    //TODO
                    /*if (!postedFile.InputStream.CanRead)
                    {
                        return false;
                    }
                    //------------------------------------------
                    //   Check whether the image size exceeding the limit or not
                    //------------------------------------------ 
                    if (postedFile.ContentLength < ImageMinimumBytes)
                    {
                        return false;
                    }*/

                    byte[] buffer = new byte[ImageMinimumBytes];
                    //TODO
                    // postedFile.InputStream.Read(buffer, 0, ImageMinimumBytes);
                    string content = System.Text.Encoding.UTF8.GetString(buffer);
                    if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }

                //-------------------------------------------
                //  Try to instantiate new Bitmap, if .NET will throw exception
                //  we can assume that it's not a valid image
                //-------------------------------------------

                try
                {
                    //TODO
                    /*using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                    {
                    }*/
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    //TODO
                    //  postedFile.InputStream.Position = 0;
                }
            }
            else
            {
                return true;
            }
            return true;
        }

        [Authorize]
        public ActionResult GetRPEChart()
        {
            return PartialView("_FullRPE");
        }

        [Authorize]
        public JsonResult ListStates(int CountryId)
        {
            var states = CommonUtility.ListStates(CountryId);
            return Json(new { Result = "OK", Records = states });
        }

        [Authorize]
        public JsonResult CheckIfCountryHasZipCode(int CountryId)
        {
            var country = CommonUtility.ListCountries().Where(x => x.Id == CountryId).FirstOrDefault();
            return Json(new { Result = "OK", HasZipCode = country.HasZipCode });
        }

        [Authorize]
        public JsonResult ListProviderDetails(int organizationId)
        {
            var providers = CommonUtility.GetProvidersList(organizationId).Where(x => x.Active).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(y => y.Text);
            var portal = PortalUtility.ListPortals(organizationId).portals.Where(x => x.Active == true).FirstOrDefault();
            return Json(new { Result = "OK", Records = providers, hasProviderDropDown = portal != null && portal.ProviderDetails == (byte)ProviderDetails.DropDown });
        }

        [Authorize]
        public JsonResult ListRace(int CountryId)
        {
            var race = CommonUtility.ListRace(CountryId);
            return Json(new
            {
                Result = "OK",
                Records = race.Select(x => new
                {
                    Id = x.Id,
                    Name = Translate.Message(x.LanguageCode)
                })
            });
        }

        [HttpPost]
        public JsonResult CheckIfOther(int raceType)
        {
            return Json(new { Result = CommonUtility.CheckIfOther(raceType) });
        }

        [Authorize]
        public JsonResult GetRaffleTypes()
        {
            var raffles = CommonUtility.GetRaffleTypes();
            return Json(new { raffles });
        }

        [Authorize]
        public JsonResult GetGiftCards(int incentiveType)
        {
            var giftCards = CommonUtility.GetGiftCards(incentiveType, Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value));
            return Json(new { giftCards });
        }

        [Authorize]
        [HttpPost]
        public JsonResult Logger(string loggerName, string param1, string param2, string param3)
        {
            LogReader logReader = new LogReader();
            int userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            string ParticipantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value != userId ? " ,P:" + HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value : "";
            loggerName = !string.IsNullOrEmpty(param1) ? loggerName : "LoggerController";
            param1 = !string.IsNullOrEmpty(param1) ? " ,Param 1 : " + param1 : "";
            param2 = !string.IsNullOrEmpty(param2) ? " ,Param 2 : " + param2 : "";
            param3 = !string.IsNullOrEmpty(param3) ? " ,Param 3 : " + param3 : "";

            var logEvent = new LogEventInfo(NLog.LogLevel.Info, "U:" + userId + ParticipantId + param1 + param2 + param3, null, loggerName, null, null);
            logReader.WriteLogMessage(logEvent);

            return Json(new { Result = true });
        }

        public async Task<JsonResult> RotateImage(string image, int direction, int userId)
        {
            /*WebImage photo = new WebImage("../ProfilePictures/" + image);
            if (photo != null)
            {
                var imagePath = "~/ProfilePictures/" + image;
                if (direction == 1)
                {
                    photo.FileName = imagePath;
                    photo.RotateLeft();
                }
                else
                {
                    photo.RotateRight();
                }
                System.IO.File.Delete(Path.Combine(environment.ContentRootPath, "~/ProfilePictures", image));

                image = System.DateTime.Now.ToString("_ddMMyyhhmmss") + image.Substring(13);
                imagePath = "~/ProfilePictures/" + image;
                photo.Save(imagePath);
                await AccountUtility.UploadPicture(userId, image, environment.ContentRootPath + "~/ProfilePictures");
            }*/
            return Json(new { Image = image });
        }
    }
}