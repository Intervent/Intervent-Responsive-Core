using Microsoft.AspNetCore.Mvc;

namespace InterventWebApp.Controllers
{
    public class AWVReportController : Controller
    {
        public JsonResult SaveComments(int AWVId, string Comments)
        {
            AWVUtility.SaveComments(AWVId, Comments);
            return Json(true);
        }

        public ActionResult ListAWVReports(int userId)
        {
            AWVModel model = new AWVModel();
            model.AWV = ParticipantUtility.ListAWVReports(userId).AWV;
            return PartialView("_AWVReport", model);

        }
    }
}