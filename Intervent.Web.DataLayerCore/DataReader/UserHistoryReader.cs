using Intervent.DAL;
using Intervent.Web.DTO;
using Intervent.Web.DTO.Diff;
using Newtonsoft.Json;

namespace Intervent.Web.DataLayer
{
    public class UserHistoryReader
    {
        InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public void AddUserChanges(AddUserChangeRequest request)
        {
            DAL.UserHistory change = new UserHistory();
            change.Changes = request.UserChange.Changes;
            change.UpdatedBy = request.UserChange.UpdatedBy;
            change.LogDate = DateTime.UtcNow;
            change.UserHistoryCategoryId = request.UserChange.UserHistoryCategoryId;
            change.UserId = request.UserChange.UserId;
            context.UserHistories.Add(change);
            context.SaveChanges();

        }

        public static bool LogUserChanges(object existingObject, object newObject, int userId, int updatedByUserId, UserHistoryCategoryDto category)
        {
            bool dataUpdated = false;
            IEnumerable<PropertyCompare> properties = PropertyCompare.FindDifferences(existingObject, newObject, excludeVirtual: true);
            if (properties.Count() > 0)
            {
                AddUserChangeRequest change = new AddUserChangeRequest();
                change.UserChange = new UserHistoryDto();
                change.UserChange.UserId = userId;
                change.UserChange.UpdatedBy = updatedByUserId;

                change.UserChange.UserHistoryCategoryId = category.Id;
                change.UserChange.Changes = JsonConvert.SerializeObject(properties);
                new UserHistoryReader().AddUserChanges(change);
                dataUpdated = true;
            }
            return dataUpdated;
        }
    }
}
