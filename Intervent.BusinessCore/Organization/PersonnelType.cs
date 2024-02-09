namespace Intervent.Business.Organization
{
    public sealed class PersonnelType : PersonnelTypeBase
    {
        protected override string EmployeeCode
        {
            get { return "EH9"; }
        }

        protected override string SpouseCode
        {
            get { return "EH8"; }
        }

        protected override string DependentCode
        {
            get { return "EH7"; }
        }

        protected override string ChildCode
        {
            get { return "EH6"; }
        }
    }
}
