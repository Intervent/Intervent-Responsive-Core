using System.Collections.Generic;
using System.Linq;

namespace ClaimDataAnalytics.Claims
{
    public class ClaimProviderDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string DirectoryName { get; set; }

        public bool IsActive { get; set; }

        public bool IsFixedLayout { get; set; }

        public string TransformerClassName { get; set; }

        public string Delimiter { get; set; }

        public bool HasHeader { get; set; }

        public bool SetSSN { get; set; }


        private ClaimProviderDto(int id, string name, string directoryName, bool isActive, bool isFixedLayout, string fixedLayoutTranformerClassName, string delimiter, bool hasHeader, bool setSSN)
        {
            Id = id;
            Name = name;
            DirectoryName = directoryName;
            IsActive = isActive;
            IsFixedLayout = isFixedLayout;
            TransformerClassName = fixedLayoutTranformerClassName;
            Delimiter = delimiter;
            HasHeader = hasHeader;
            SetSSN = setSSN;
        }

        public static ClaimProviderDto AETNA = new ClaimProviderDto(1, "AETNA", "AETNA", true, true, "ClaimDataAnalytics.Claims.LayoutTransformer.AetnaFixedLayoutTransformer", "|", false, false);
        public static ClaimProviderDto BCBS = new ClaimProviderDto(2, "BCBS", "BCBS", true, true, "ClaimDataAnalytics.Claims.LayoutTransformer.BCBSFixedLayoutTransformer", "|", false, false);
        public static ClaimProviderDto CVS_CAREMARK = new ClaimProviderDto(3, "CVS", "CVS", true, true, "ClaimDataAnalytics.Claims.LayoutTransformer.CvsFixedLayoutTransformer", "|", false, true);
        public static ClaimProviderDto UHC = new ClaimProviderDto(4, "UHC", "UHC", true, false, "", "~", true, false);

        public static string ProcessedDirectoryName = @"Processed";

        public static string ErrorDirectoryName = @"Error";


        public static IEnumerable<ClaimProviderDto> GetAll()
        {
            List<ClaimProviderDto> lst = new List<ClaimProviderDto>();
            lst.Add(ClaimProviderDto.AETNA);
            lst.Add(ClaimProviderDto.BCBS);
            lst.Add(ClaimProviderDto.CVS_CAREMARK);
            lst.Add(ClaimProviderDto.UHC);
            return lst.Where(x => x.IsActive);
        }

        public static ClaimProviderDto GetByKey(int id)
        {
            return GetAll().Where(x => x.Id == id).FirstOrDefault();
        }

        public static ClaimProviderDto GetByProviderName(string providerName)
        {
            return GetAll().Where(x => x.DirectoryName == providerName).FirstOrDefault();
        }
    }
}
