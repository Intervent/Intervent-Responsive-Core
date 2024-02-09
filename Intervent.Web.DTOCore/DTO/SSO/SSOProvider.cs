namespace Intervent.Web.DTO
{
    public class SSOProviderDto
    {
        public int Id { get; set; }
        public string ProviderName { get; set; }
        public int OrganizationId { get; set; }
        public bool HasEligibility { get; set; }
        public string LogoutUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string Issuer { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }

        public string Certificate { get; set; }

        public IList<SSOAttributeMappingDto> SSOAttributeMappings { get; set; }

        private Dictionary<int, SSOAttributeMappingDto> mapping = null;

        public Dictionary<int, SSOAttributeMappingDto> MappingDictionary
        {
            get
            {
                if (mapping == null)
                {
                    mapping = new Dictionary<int, SSOAttributeMappingDto>();
                    foreach (var map in SSOAttributeMappings)
                    {
                        if (!mapping.ContainsKey(map.TypeId))
                        {
                            mapping[map.TypeId] = map;
                        }
                    }
                }
                return mapping;
            }
        }

        public string FindNameByType(AttributeType type)
        {
            if (MappingDictionary.ContainsKey((int)type))
            {
                return MappingDictionary[(int)type].AttributeName;
            }
            else
            {
                return type.ToString();
            }
        }

        public string[] FindPossibleValue(AttributeType type)
        {
            if (MappingDictionary.ContainsKey((int)type))
            {
                return MappingDictionary[(int)type].Format.Split('~');
            }
            return null;
        }
    }
}
