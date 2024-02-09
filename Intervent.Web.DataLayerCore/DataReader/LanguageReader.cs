using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public enum TableName
    {
        Step,
        Activity,
        Question,
        Option
    }
    public enum LanguageType
    {
        //StepName
        SN,
        //StepDescription
        ST,
        //Activity TopText
        AT,
        //Activity BottomText
        AB,
        //Question Text
        QET,
        //Passive question text
        PQET,
        //Option Text
        OT,
        //Quiz Text
        QZT,
        //Quiz Option Text
        QZOT,
        //Profile Text
        PRO,
        //Program Name
        PRGNAME,
        //Program Desc
        PRGDESC
    }
    public class LanguageReader
    {

        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public List<LanguageItems> GetLanguageItems(List<string> itemCodes, string languageCode)
        {
            // var itemCodes = GetLanguageItemCodes(id, name);
            return context.LanguageItems.Where(x => x.LanguageCode == languageCode && itemCodes.Contains(x.ItemCode)).ToList();

        }

        public Dictionary<string, LanguageItems> GetLanguageItemDict(List<string> itemCodes, string languageCode)
        {
            // var itemCodes = GetLanguageItemCodes(id, name);
            Dictionary<string, LanguageItems> languageItems = new Dictionary<string, LanguageItems>();
            var items = context.LanguageItems.Where(x => x.LanguageCode == languageCode && itemCodes.Contains(x.ItemCode)).ToList();
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.Text))
                    languageItems.Add(item.ItemCode, item);
            }
            return languageItems;
        }

        public LanguageItemDto GetLanguageItem(LanguageType type, int id, string languageCode)
        {
            var itemCode = type.ToString() + id;
            // var itemCodes = GetLanguageItemCodes(id, name);
            var language = context.LanguageItems.Where(x => x.LanguageCode == languageCode && x.ItemCode == itemCode).FirstOrDefault();
            return Utility.mapper.Map<DAL.LanguageItems, LanguageItemDto>(language);
        }

        public List<LanguageItemDto> GetLanguageItemByCode(string itemCode)
        { // var itemCodes = GetLanguageItemCodes(id, name);
            var list = context.LanguageItems.Where(x => x.ItemCode == itemCode).ToList();
            return Utility.mapper.Map<List<DAL.LanguageItems>, List<LanguageItemDto>>(list);
        }

        public void SaveLanguageItems(Dictionary<string, string> itemCodes, string languageCode)
        {
            if (languageCode == ListOptions.DefaultLanguage) return;
            foreach (KeyValuePair<string, string> entry in itemCodes)
            {
                if (!string.IsNullOrEmpty(entry.Value))
                {
                    var item = context.LanguageItems.Where(x => x.ItemCode == entry.Key && x.LanguageCode == languageCode).FirstOrDefault();
                    if (item != null)
                    {

                        item.Text = entry.Value;
                        context.LanguageItems.Attach(item);
                        context.Entry(item).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    else
                    {
                        item = new LanguageItems();
                        item.Text = entry.Value;
                        item.ItemCode = entry.Key;
                        item.LanguageCode = languageCode;
                        context.LanguageItems.Add(item);
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
