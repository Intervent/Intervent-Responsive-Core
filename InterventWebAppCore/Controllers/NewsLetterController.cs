using Microsoft.AspNetCore.Mvc;

namespace InterventWebApp
{
    public class NewsLetterController : AccountBaseController
    {
        private readonly IHostEnvironment environment;

        public NewsLetterController(IHostEnvironment environment)
        {
            this.environment = environment;
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public ActionResult NewsLetter()
        {
            return View();
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public JsonResult ListNewsLetters(int page, int pageSize, int? totalRecords)
        {
            var response = NewsLetterUtility.ListNewsletters(page, pageSize, totalRecords);
            return Json(new { Result = "OK", Records = response.Newsletters, TotalRecords = response.TotalRecords });
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public JsonResult ListAssignedNewsLetters(int newsletterId)
        {
            var result = NewsLetterUtility.ListAssignedNewsletters(newsletterId);
            return Json(new { Result = "OK", Records = result });
        }

        [ModuleControl(null, RoleCode.Administrator)]
        [HttpPost]
        public JsonResult AddEditNewsLetter(IFormFile FileUpload, int id, string name, string pdf)
        {
            bool Status = false;
            string targetpath = environment.ContentRootPath + "../Pdf/";
            if (FileUpload != null || id != 0)
            {
                if (FileUpload != null)
                {
                    string filename = FileUpload.FileName;
                    if (filename.EndsWith(".pdf"))
                    {
                        if (!Directory.Exists(targetpath))
                            Directory.CreateDirectory(targetpath);
                        string pathToFile = targetpath + filename;
                        //TODO
                        //FileUpload.SaveAs(pathToFile);
                    }
                }
                var response = NewsLetterUtility.AddEditNewsletter(id, name, pdf);
                if (id > 0 && FileUpload != null)
                {
                    string oldPDF = targetpath + response.OldPdf;
                    System.IO.File.Delete(oldPDF);
                }
                id = response.NewsletterId;
                Status = true;
            }
            return Json(new { Result = "OK", Status = Status, NewsletterId = id });
        }

        [ModuleControl(null, RoleCode.Administrator)]
        [HttpPost]
        public JsonResult AssignOrRemoveNewsLetter(int newsLetterId, string organizationIds, bool isRemove)
        {
            AssignNewsletterModel model = new AssignNewsletterModel() { NewsletterId = newsLetterId, OrganizationIds = organizationIds };
            var result = NewsLetterUtility.AssignNewsletter(model);
            return Json(new { Result = "OK", Status = result });
        }

    }
}