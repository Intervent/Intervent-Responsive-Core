using ClaimDataAnalytics.Claims.Model;
using System;
using System.Text;

namespace ClaimDataAnalytics.Claims.SqlTranslator
{
    public static class ClaimSqlTranslator
    {
        const string DATE_FORMAT = "MM/dd/yyyy";
        public static string CreateClaimSql(ClaimsInputFlatFileModel model)
        {
            StringBuilder sb = new StringBuilder("INSERT INTO Claim (");
            sb.Append("[FirstName]");
            sb.Append(",[LastName]");
            sb.Append(",[DOB]");
            sb.Append(",[SSN]");
            sb.Append(",[UniqueId]");
            sb.Append(",[ProviderName]");
            sb.Append(",[RowId]");
            sb.Append(",[CreateDate]");
            sb.Append(",[ServiceStartDate]");
            sb.Append(",[BilledAmount]");
            sb.Append(",[Copay]");
            sb.Append(",[Deductible]");
            sb.Append(",[Coinsurance]");
            sb.Append(",[NetPaid]");
            sb.Append(",[TotalPaidByAllSources]");
            sb.Append(",[RelationshipToSubscriber]");
            sb.Append(",[NewUniqueId]");
            sb.Append(",[ServiceEndDate]");
            sb.Append(",[SubscriberSSN]");
            sb.Append(") VALUES (");
            if (String.IsNullOrEmpty(model.MemberFirstName))
                sb.Append("NULL,");
            else
                sb.Append($"'{model.MemberFirstName.Replace("'", "''")}' ,");
            if (String.IsNullOrEmpty(model.MemberLastName))
                sb.Append("NULL,");
            else
                sb.Append($"'{model.MemberLastName.Replace("'", "''")}' ,");
            if (!model.MemberDateOfBirth.HasValue)
                sb.Append("NULL,");
            else
                sb.Append($"'{model.MemberDateOfBirth.Value.ToString(DATE_FORMAT)}',");
            if (String.IsNullOrEmpty(model.MemberSSN))
                sb.Append("NULL,");
            else
                sb.Append($"'{model.MemberSSN}' ,");
            if (String.IsNullOrEmpty(model.UniqueId))
                sb.Append("NULL,");
            else
                sb.Append($"'{model.UniqueId}' ,");
            if (String.IsNullOrEmpty(model.ProviderName))
                sb.Append("NULL,");
            else
                sb.Append($"'{model.ProviderName}' ,");

            sb.Append($"{model.InputFileRowId},");
            sb.Append($"'{model.CreateDate.ToString(DATE_FORMAT)}',");
            if (!model.ServiceStartDate.HasValue)
                sb.Append("NULL,");
            else
                sb.Append($"'{model.ServiceStartDate.Value.ToString(DATE_FORMAT)}' ,");

            if (!model.BilledAmount.HasValue)
                sb.Append("NULL,");
            else
                sb.Append($"{model.BilledAmount.Value},");
            if (!model.Copay.HasValue)
                sb.Append("NULL,");
            else
                sb.Append($"{model.Copay.Value},");
            if (!model.Deductible.HasValue)
                sb.Append("NULL,");
            else
                sb.Append($"{model.Deductible.Value},");
            if (!model.Coinsurance.HasValue)
                sb.Append("NULL,");
            else
                sb.Append($"{model.Coinsurance.Value},");
            if (!model.NetPaid.HasValue)
                sb.Append("NULL,");
            else
                sb.Append($"{model.NetPaid.Value},");
            if (!model.TotalAmountPaidbyAllSource.HasValue)
                sb.Append("NULL,");
            else
                sb.Append($"{model.TotalAmountPaidbyAllSource.Value},");
            if (!model.RelationshipToSubscriberCode.HasValue)
                sb.Append("NULL,");
            else
                sb.Append($"'{model.RelationshipToSubscriberCode.Value.ToString()}', ");
            if (String.IsNullOrEmpty(model.NewUniqueId))
                sb.Append("NULL,");
            else
                sb.Append($"'{model.NewUniqueId}',");
            if (!model.ServiceEndDate.HasValue)
                sb.Append("NULL,");
            else
                sb.Append($"'{model.ServiceEndDate.Value.ToString(DATE_FORMAT)}' ,");
            if (String.IsNullOrEmpty(model.SubscriberSSN))
                sb.Append("NULL");
            else
                sb.Append($"'{model.SubscriberSSN}'");
            sb.Append(");");
            return sb.ToString();
        }
    }
}
