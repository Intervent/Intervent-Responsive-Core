using ClaimDataAnalytics.Claims.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClaimDataAnalytics.Claims.DB
{
    public class Dblayer
    {
        public IEnumerable<CrothalIdDto> GetClaimsCrothalIDs()
        {
            //return Enumerable.Empty<CrothalIdDto>();
            using (var ctx = new AnalyticsContext())
            {
                return ctx.CrothalIds.Select(x => new CrothalIdDto()
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    OldUniqueId = x.OldUniqueId,
                    NewUniqueId = x.NewUniqueId
                }).ToList();
            }
        }

        public IEnumerable<ClaimProcessEligibilityDto> GetEligibilityIDs(DateTime createDate)
        {
            // return Enumerable.Empty<ClaimProcessEligibilityDto>();
            using (var ctx = new AnalyticsContext())
            {
                return (from e in ctx.EligibilityIds
                        where !ctx.ExcludeIds.Any(x => x.EligibilityId == e.Id)
                        where e.CreateDate == createDate
                        select new ClaimProcessEligibilityDto()
                        {
                            FirstName = e.FirstName,
                            LastName = e.LastName,
                            DateOfBirth = e.DateOfBirth,
                            EligibilityId = e.Id,
                            SSN = e.SSN,
                            UniqueID = e.UniqueID,
                            UserEnrollmentType = e.UserEnrollmentType,
                            MedicalPlanCode = e.MedicalPlanCode,
                        }).ToList();
            }
            //return ctx.EligibilityIds.Where(x=>x.CreateDate == createDate).Select(x => new ClaimProcessEligibilityDto()
            //{
            //    FirstName = x.FirstName,
            //    LastName = x.LastName,
            //    DateOfBirth = x.DateOfBirth,
            //    EligibilityId = x.Id,
            //    SSN = x.SSN,
            //    UniqueID = x.UniqueID,
            //    UserEnrollmentType = x.UserEnrollmentType,
            //    MedicalPlanCode = x.MedicalPlanCode,
            //}).ToList();


        }
    }
}
