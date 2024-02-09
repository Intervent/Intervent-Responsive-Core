using System.Reflection;

namespace Intervent.Web.DTO.Diff
{
    public sealed class PropertyCompare
    {
        public string PropertyName { get; set; }

        public string CurrentValue { get; set; }

        public string NewValue { get; set; }


        public static IEnumerable<PropertyCompare> FindDifferences(object existingObject, object newObject, string[] propertiesNotToCompareArray = null, bool excludeVirtual = false)
        {
            List<PropertyCompare> changes = new List<PropertyCompare>();

            foreach (PropertyInfo propertyInfo in newObject.GetType().GetProperties())
            {
                if (propertyInfo.CanRead && (propertiesNotToCompareArray == null || !propertiesNotToCompareArray.Contains(propertyInfo.Name)))
                {
                    //exclude virtual ignores properties (FK properties) with virtual accessor in the db poco
                    if (excludeVirtual && propertyInfo.GetAccessors()[0].IsVirtual)
                        continue;
                    object existingValue = propertyInfo.GetValue(existingObject, null);
                    object newValue = propertyInfo.GetValue(newObject, null);
                    if (!object.Equals(existingValue, newValue))
                    {
                        string currentValue = null;
                        string newValue2 = null;

                        if ((newValue != null && typeof(ITypeSafeEnum).IsAssignableFrom(newValue.GetType())) || (existingValue != null && typeof(ITypeSafeEnum).IsAssignableFrom(existingValue.GetType())))
                        {
                            if (existingValue != null)
                                currentValue = ((ITypeSafeEnum)existingValue).Description;
                            if (newValue != null)
                                newValue2 = ((ITypeSafeEnum)newValue).Description;
                        }
                        else
                        {
                            currentValue = Convert.ToString(existingValue);
                            newValue2 = Convert.ToString(newValue);
                        }

                        changes.Add(new PropertyCompare() { PropertyName = propertyInfo.Name, CurrentValue = currentValue, NewValue = newValue2 });
                    }
                }
            }


            return changes;
        }
    }
}
