using ClaimDataAnalytics.Eligibility.CsvModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataAnalytics.Eligibility.SqlTranslator
{
    public static class EligibiltySql
    {
        const string DATE_FORMAT = "MM/dd/yyyy";
        public static string CreateSql(EligibilityCsvModel model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO ELIGIBILITY([NamePrefix],[FirstName],[LastName],[DOB],[Gender],[Email],");
            sb.Append("[HomeNumber],[WorkNumber],[CellNumber],");
            sb.Append("[Address],[City],[State],[Country],[Zip],[SSN],[UserEnrollmentType],[EmployeeUniqueId],[HireDate],");
            sb.Append("[TerminatedDate],[DeathDate],[BusinessUnit],[RegionCode],[UnionFlag],[MedicalPlanCode],");
            sb.Append("[MedicalPlanStartDate],[MedicalPlanEndDate],[TobaccoFlag],[PayType],[UserStatus],[UniqueId],");
            sb.Append("[EnrollmentStatus],[DentalPlanCode],[DentalPlanStartDate],[DentalPlanEndDate],[VisionPlanCode],");
            sb.Append("[VisionPlanStartDate],[VisionPlanEndDate],[CompanyName],[CreateDate])");
            sb.Append(" VALUES(NULL, ");
            sb.Append($"'{model.FirstName.Replace("'","''")}', '{model.LastName.Replace("'", "''")}',");
            if (model.DOB.HasValue)
                sb.Append($"'{model.DOB.Value.Date.ToString(DATE_FORMAT)}'");
            else
                sb.Append("NULL");
            sb.Append(",");
            if(model.Gender != null)
                sb.Append($"'{model.Gender.CsvDescription}', ");
            else
                sb.Append("NULL , ");
            if(String.IsNullOrEmpty(model.EmailAddress))
                sb.Append("NULL , ");
            else
            {
                sb.Append($"'{model.EmailAddress.Replace("'", "''")}' , ");
            }
                
            if (String.IsNullOrEmpty(model.HomePhone))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.HomePhone}' , ");
          
            sb.Append("NULL , ");//WORK PHONE
            sb.Append("NULL , ");//CELL PHONE

            if (String.IsNullOrEmpty(model.Address1))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.Address1.Replace("'", "''")} {model.Address2.Replace("'", "''")}' , ");
            if (String.IsNullOrEmpty(model.City))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.City.Replace("'", "''")}' , ");
            if (String.IsNullOrEmpty(model.StateOrProvince))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.StateOrProvince}' , ");
            if (String.IsNullOrEmpty(model.Country))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.Country}' , ");
            if (String.IsNullOrEmpty(model.ZipOrPostalCode))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.ZipOrPostalCode}' , ");
            if (String.IsNullOrEmpty(model.SSN))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.SSN}' , ");
            if (model.UserEnrollmentType == null)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.UserEnrollmentType.UserEnrollmentTypeKey}' , ");
            if (String.IsNullOrEmpty(model.EmployeeUniqueId))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.EmployeeUniqueId}' , ");
            if (!model.HireDate.HasValue)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.HireDate.Value.Date.ToString(DATE_FORMAT)}' , ");
            if (!model.TerminatedDate.HasValue)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.TerminatedDate.Value.Date.ToString(DATE_FORMAT)}' , ");
            if (!model.DeathDate.HasValue)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.DeathDate.Value.Date.ToString(DATE_FORMAT)}' , ");
            if (String.IsNullOrEmpty(model.BusinessUnit))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.BusinessUnit}' , ");
            if (String.IsNullOrEmpty(model.RegionCode))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.RegionCode}' , ");
            if (model.UnionFlag == null)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.UnionFlag.Key}' , ");
            if (String.IsNullOrEmpty(model.MedicalPlanCode))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.MedicalPlanCode}' , ");
            if (!model.MedicalPlanStartDate.HasValue)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.MedicalPlanStartDate.Value.Date.ToString(DATE_FORMAT)}' , ");
            if (!model.MedicalPlanEndDate.HasValue)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.MedicalPlanEndDate.Value.Date.ToString(DATE_FORMAT)}' , ");
            if (model.TobaccoFlag == null)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.TobaccoFlag.Key}' , ");
            if (model.PayType == null)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.PayType.Key}' , ");
            if (model.UserStatus == null)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.UserStatus.Key}' , ");
            if (String.IsNullOrEmpty(model.UniqueId))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.UniqueId}' , ");
            sb.Append("NULL , ");//EnrollmentStatus
            if (String.IsNullOrEmpty(model.DentalPlanCode))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.DentalPlanCode}' , ");
            if (!model.DentalPlanStartDate.HasValue)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.DentalPlanStartDate.Value.Date.ToString(DATE_FORMAT)}' , ");
            if (!model.DentalPlanEndDate.HasValue)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.DentalPlanEndDate.Value.Date.ToString(DATE_FORMAT)}' , ");
            if (String.IsNullOrEmpty(model.VisionPlanCode))
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.VisionPlanCode}' , ");
            if (!model.VisionPlanStartDate.HasValue)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.VisionPlanStartDate.Value.Date.ToString(DATE_FORMAT)}' , ");
            if (!model.VisionPlanEndDate.HasValue)
                sb.Append("NULL , ");
            else
                sb.Append($"'{model.VisionPlanEndDate.Value.Date.ToString(DATE_FORMAT)}' , ");
            sb.Append($"'{model.CompanyName}',");
            sb.Append($"'{model.CreateDate.ToString("MM/01/yyyy")}');");
            return sb.ToString();
        }
    }
}
