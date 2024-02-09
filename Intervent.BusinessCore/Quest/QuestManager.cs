using Intervent.HWS;
using Microsoft.Office.Interop.Excel;

namespace Intervent.Business.Quest
{
    public class QuestManager
    {
        public Dictionary<string, LabResponse> LoadQuestDataFromExcel(string questFile)
        {
            Application xl = new Application();
            Workbook workbook = xl.Workbooks.Open(questFile);
            Worksheet sheet = (Worksheet)workbook.Sheets[1];

            Dictionary<string, LabResponse> response = new Dictionary<string, LabResponse>();
            //index starts at 1 and 1 is a header; start reading from 2
            for (int rowIndex = 2; rowIndex <= sheet.Rows.Count; rowIndex++)
            {
                var ssn = ExtractString(sheet, rowIndex, 1);
                var bloodTestDate = ExtractDateTime(sheet, rowIndex, 14);
                var code = ExtractString(sheet, rowIndex, 16);
                var value = ExtractFloat(sheet, rowIndex, 18);
                response = MapLabValue(ssn, bloodTestDate, code, value, response);
            }
            return response;
        }

        Dictionary<string, LabResponse> MapLabValue(string uniqueId, DateTime? bloodTestDate, string code, float? value, Dictionary<string, LabResponse> response)
        {
            LabResponse lab = null;
            if (response.ContainsKey(uniqueId))
            {
                lab = response[uniqueId];
            }
            else
            {
                lab = new LabResponse();
                response.Add(uniqueId, lab);
            }
            if (bloodTestDate != null)
            {
                lab.BloodTestDate = bloodTestDate;
            }
            switch (code)
            {
                case "2345-7":
                    lab.Glucose = value;
                    break;
                case "1558-6":
                    lab.Glucose = value;
                    lab.DidYouFast = 1;
                    break;
                case "2093-3":
                    lab.TotalChol = value;
                    break;
                case "2571-8":
                    lab.Trig = value;
                    break;
                case "2085-9":
                    lab.HDL = value;
                    break;
                case "13457-7":
                    lab.LDL = value;
                    break;
                case "3137-7":
                    lab.Height = value;
                    break;
                case "3142-7":
                    lab.Weight = value;
                    break;
                case "39156-5":
                    lab.BMI = value;
                    break;
                //case "WAIST":
                //    lab.Waist = value;
                //    break;
                case "8480-6":
                    lab.SBP = short.Parse(value.ToString());
                    break;
                case "8462-4":
                    lab.DBP = short.Parse(value.ToString());
                    break;
            }
            return response;
        }

        string ExtractString(Worksheet sheet, int row, int col)
        {
            Microsoft.Office.Interop.Excel.Range cell = (Microsoft.Office.Interop.Excel.Range)sheet.Cells[row, col];
            if (cell.Value != null)
            {
                return Convert.ToString(cell.Value);
            }
            return null;
        }

        float? ExtractFloat(Worksheet sheet, int row, int col)
        {
            Microsoft.Office.Interop.Excel.Range cell = (Microsoft.Office.Interop.Excel.Range)sheet.Cells[row, col];
            if (cell.Value != null)
            {
                return float.Parse(cell.Value.ToString());
            }
            return null;
        }

        DateTime? ExtractDateTime(Worksheet sheet, int row, int col)
        {
            Microsoft.Office.Interop.Excel.Range cell = (Microsoft.Office.Interop.Excel.Range)sheet.Cells[row, col];
            if (cell.Value != null)
            {
                return DateTime.Parse(cell.Value.ToString());
            }

            return null;
        }

    }
}
