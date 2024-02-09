using Intervent.Web.DTO;

namespace Intervent.Business.Organization
{
    public abstract class PersonnelTypeBase
    {
        protected abstract string EmployeeCode { get; }

        protected abstract string SpouseCode { get; }

        protected abstract string DependentCode { get; }

        protected abstract string ChildCode { get; }

        public string Code(EligibilityUserEnrollmentTypeDto userEnrollmentType)
        {
            if (userEnrollmentType == null)
                return null;
            else if (userEnrollmentType == EligibilityUserEnrollmentTypeDto.Employee)
                return EmployeeCode;
            else if (userEnrollmentType == EligibilityUserEnrollmentTypeDto.Spouse)
                return SpouseCode;
            else if (userEnrollmentType == EligibilityUserEnrollmentTypeDto.Dependent)
                return DependentCode;
            else if (userEnrollmentType == EligibilityUserEnrollmentTypeDto.Child)
                return ChildCode;
            else
                throw new ArgumentOutOfRangeException("Personnel type not defined");
        }

        public static PersonnelTypeBase GetOrganizationPersonnelType(int organizationId)
        {
            PersonnelTypeBase type = null;
            if (organizationId > 0)
                type = new PersonnelType();
            else
                throw new ArgumentOutOfRangeException("organization id not recognized");
            return type;
        }
    }
}
