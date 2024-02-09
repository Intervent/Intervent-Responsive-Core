using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using System.Globalization;

namespace InterventWebApp
{
    public class ChallengeUtility
    {
        public static UserActivitiesResponse GetUserActivitiesDetail(int userId, int portalId)
        {
            UserActivitiesRequest request = new UserActivitiesRequest();
            ChallengeReader reader = new ChallengeReader();
            request.userId = userId;
            request.portalId = portalId;
            return reader.GetUserActivitiesDetail(request);
        }

        public static List<RafflesTrackModel> GetRaffleTypes(List<RafflesinPortalsDto> rafflesinPortals, string languagePreference)
        {
            CultureInfo info = !string.IsNullOrEmpty(languagePreference) && languagePreference == "es" ?
                CultureInfo.CreateSpecificCulture("es-MX") : CultureInfo.CreateSpecificCulture("en-US");
            var raffles = new List<RafflesTrackModel>();
            foreach (var raffle in rafflesinPortals)
            {
                if (raffle.RaffleDates.Where(x => x.RaffleDate > DateTime.Now && x.RafflesinPortalsId == raffle.Id).Any())
                {
                    var portalRaffle = new RafflesTrackModel();
                    portalRaffle.Id = raffle.RaffleTypeId;
                    portalRaffle.Name = String.Format(Translate.Message(raffle.Name), raffle.Reward);
                    portalRaffle.Description = Translate.Message(raffle.Description);
                    portalRaffle.RaffleDate = raffle.RaffleDates.Where(x => x.RaffleDate > DateTime.Now && x.RafflesinPortalsId == raffle.Id).OrderBy(x => x.RaffleDate).FirstOrDefault().RaffleDate.ToString("MMMM dd yyyy", info);
                    portalRaffle.Points = raffle.Points;
                    raffles.Add(portalRaffle);
                }
            }
            return raffles;
        }

        public static string GetUpcomingRaffleEligiblity(double points, List<RafflesinPortalsDto> raffles, string timeZone)
        {
            var str = Translate.Message("L2902");
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var date = Convert.ToDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone));
            string response = "";
            bool gotoNext = false;
            if (points < 250)
            {
                var nextRaffle = raffles.Where(x => x.RaffleTypes.Name.Equals(RaffleTypes.Monthly) && (x.RaffleDates.Where(y => y.RaffleDate > date).Count() > 0)).OrderBy(x => x.RaffleDates).FirstOrDefault();
                if (nextRaffle != null)
                {
                    var days = (nextRaffle.RaffleDates.Where(y => y.RaffleDate > date).OrderBy(x => x.RaffleDate).FirstOrDefault().RaffleDate - date).Days;
                    var reqpoints = 250 - points;
                    response = String.Format(str, reqpoints, days, nextRaffle.Reward);
                }
                else
                    gotoNext = true;
            }
            if (gotoNext || points == 250)
            {
                var nextRaffle = raffles.Where(x => x.RaffleTypes.Name.Equals(RaffleTypes.Quaterly) && (x.RaffleDates.Where(y => y.RaffleDate > date).Count() > 0)).OrderBy(x => x.RaffleDates).FirstOrDefault();
                if (nextRaffle != null)
                {
                    var days = (nextRaffle.RaffleDates.Where(y => y.RaffleDate > date).OrderBy(x => x.RaffleDate).FirstOrDefault().RaffleDate - date).Days;
                    var reqpoints = 251 - points;
                    response = String.Format(str, reqpoints, days, nextRaffle.Reward);
                }
                else
                    gotoNext = true;
            }
            if (gotoNext || points > 250 && points < 500)
            {
                var nextRaffle = raffles.Where(x => x.RaffleTypes.Name.Equals(RaffleTypes.SemiAnnual) && (x.RaffleDates.Where(y => y.RaffleDate > date).Count() > 0)).OrderBy(x => x.RaffleDates).FirstOrDefault();
                if (nextRaffle != null)
                {
                    var days = (nextRaffle.RaffleDates.Where(y => y.RaffleDate > date).OrderBy(x => x.RaffleDate).FirstOrDefault().RaffleDate - date).Days;
                    var reqpoints = 500 - points;
                    response = String.Format(str, reqpoints, days, nextRaffle.Reward);
                }
                else
                    gotoNext = true;
            }
            if (gotoNext || points >= 500 && points < 1000)
            {
                var nextRaffle = raffles.Where(x => x.RaffleTypes.Name.Equals(RaffleTypes.Annual) && (x.RaffleDates.Where(y => y.RaffleDate > date).Count() > 0)).OrderBy(x => x.RaffleDates).FirstOrDefault();
                if (nextRaffle != null)
                {
                    var days = (nextRaffle.RaffleDates.Where(y => y.RaffleDate > date).OrderBy(x => x.RaffleDate).FirstOrDefault().RaffleDate - date).Days;
                    var reqpoints = 1000 - points;
                    response = String.Format(str, reqpoints, days, nextRaffle.Reward);
                }
            }
            return response;
        }

        public static List<ChallengesViewModel> GetIncentiveforChallenges(bool isAdminView, int? programType, bool selfScheduling, int participantPortalId, int participantId)
        {
            List<int> existingTypes = new List<int>();
            var portalIncentive = PortalUtility.ReadIncentive(participantPortalId, participantId);
            List<ChallengesViewModel> incentives = new List<ChallengesViewModel>();
            foreach (var inc in portalIncentive.PortalIncentives.Where(x => !x.IsCompanyIncentive))
            {
                var incentive = inc;
                bool hasActiveIncentive = false;
                if (incentive.RefId.HasValue)
                {
                    var activeIncentive = portalIncentive.PortalIncentives.Where(x => x.IncentiveTypeId == incentive.IncentiveTypeId && x.UserIncentives.Count() > 0).FirstOrDefault();
                    if (activeIncentive != null)
                    {
                        incentive = activeIncentive;
                        hasActiveIncentive = true;
                    }
                }
                if (!existingTypes.Contains(incentive.IncentiveTypeId) && (!incentive.RefId.HasValue || incentive.RefId == portalIncentive.programsinportalId || hasActiveIncentive || portalIncentive.programsinportalId == 0 || programType.HasValue && programType.Value == 1))
                {
                    existingTypes.Add(incentive.IncentiveTypeId);
                    var challenge = new ChallengesViewModel();
                    challenge.IncentiveTypeId = incentive.IncentiveTypeId;
                    challenge.Attachment = incentive.Attachment;
                    challenge.ImageUrl = incentive.ImageUrl;
                    challenge.IsActive = incentive.IsActive;
                    challenge.IsCompanyIncentive = incentive.IsCompanyIncentive;
                    challenge.IsPoint = incentive.IsPoint;
                    challenge.Name = !String.IsNullOrEmpty(incentive.LanguageItemName) ? Translate.Message(incentive.LanguageItemName) : incentive.Name;
                    challenge.MoreInfo = !String.IsNullOrEmpty(incentive.LanguageItemMoreInfo) ? Translate.Message(incentive.LanguageItemMoreInfo) : incentive.MoreInfo;
                    challenge.PointsText = !String.IsNullOrEmpty(incentive.PointsText) ? incentive.PointsText : incentive.Points.ToString();
                    challenge.PortalIncentiveId = incentive.Id;
                    if (incentive.UserIncentives != null && incentive.UserIncentives.Count > 0)
                    {
                        challenge.userIncentives = incentive.UserIncentives.ToList();
                        challenge.userIncentiveId = incentive.UserIncentives.FirstOrDefault().Id;
                        challenge.completed = true;
                        challenge.Reference = incentive.UserIncentives.FirstOrDefault().Reference;
                    }
                    else if (incentive.IncentiveType.Id != (int)IncentiveTypes.Intervent_1stCoaching || selfScheduling)
                    {
                        challenge.Url = incentive.IncentiveType.Url;
                    }
                    incentives.Add(challenge);
                }
            }
            existingTypes = new List<int>();
            foreach (var inc in portalIncentive.PortalIncentives.Where(x => x.IsCompanyIncentive))
            {
                var incentive = inc;
                if (!existingTypes.Contains(incentive.IncentiveTypeId))
                {
                    existingTypes.Add(incentive.IncentiveTypeId);
                    var challenge = new ChallengesViewModel();
                    var existingIncentive = incentives.Where(x => x.IncentiveTypeId == incentive.IncentiveTypeId).FirstOrDefault();
                    if (existingIncentive != null && !isAdminView)
                    {
                        challenge = existingIncentive;
                        incentives.Remove(existingIncentive);
                    }
                    var programIncentive = portalIncentive.PortalIncentives.Where(x => x.IncentiveTypeId == incentive.IncentiveTypeId && x.RefId == portalIncentive.programsinportalId && x.IsCompanyIncentive).FirstOrDefault();
                    if (programIncentive != null)
                    {
                        incentive = programIncentive;
                    }
                    if (incentive.isGiftCard)
                    {
                        var hasUserincentive = portalIncentive.PortalIncentives.Where(x => x.UserIncentives.Any() && x.IncentiveTypeId == incentive.IncentiveTypeId && x.isGiftCard).FirstOrDefault();
                        if (hasUserincentive != null)
                        {
                            incentive = hasUserincentive;
                        }
                    }
                    challenge.IncentiveTypeId = incentive.IncentiveTypeId;
                    challenge.Attachment = incentive.Attachment;
                    challenge.ImageUrl = incentive.ImageUrl;
                    challenge.IsActive = incentive.IsActive;
                    challenge.IsCompanyIncentive = incentive.IsCompanyIncentive;
                    challenge.IsPoint = incentive.IsPoint;
                    challenge.Name = !String.IsNullOrEmpty(incentive.LanguageItemName) ? Translate.Message(incentive.LanguageItemName) : incentive.Name;
                    challenge.MoreInfo = !String.IsNullOrEmpty(incentive.LanguageItemMoreInfo) ? Translate.Message(incentive.LanguageItemMoreInfo) : incentive.MoreInfo;
                    challenge.CompanyPointsText = !String.IsNullOrEmpty(incentive.PointsText) ? incentive.PointsText : incentive.Points.ToString();
                    challenge.PortalIncentiveId = incentive.Id;
                    challenge.isGiftCard = incentive.isGiftCard;
                    if ((incentive.IncentiveTypeId == 5 || incentive.Attachment != null) && incentive.RefValue2 == 1) // (incentive.Pdf != null && incentive.RefValue2 == 1)
                    {
                        challenge.supportTobaccoUpload = true;
                        if (incentive.UserIncentives != null && incentive.UserIncentives.Count > 0)
                        {
                            challenge.userIncentiveId = incentive.UserIncentives.FirstOrDefault().Id;
                            if (!incentive.UserIncentives.FirstOrDefault().IsActive)
                            {
                                challenge.Reference = incentive.UserIncentives.FirstOrDefault().Reference;
                            }
                            else
                            {
                                challenge.Reference = incentive.UserIncentives.FirstOrDefault().Reference;
                                challenge.userIncentives = incentive.UserIncentives.ToList();
                                challenge.supportTobaccoUpload = false;
                                challenge.employerComplete = true;
                                if (incentive.IncentiveTypeId == 5)
                                    challenge.completed = true;
                            }
                        }
                    }
                    else if (incentive.UserIncentives != null && incentive.UserIncentives.Count > 0)
                    {
                        challenge.userIncentives = incentive.UserIncentives.ToList();
                        challenge.userIncentiveId = incentive.UserIncentives.FirstOrDefault().Id;
                        challenge.employerComplete = true;
                        challenge.Reference = incentive.UserIncentives.FirstOrDefault().Reference;
                    }
                    else if (incentive.UserIncentives == null || incentive.UserIncentives.Count() == 0)
                    {
                        challenge.Url = incentive.IncentiveType.Url;
                    }
                    incentives.Add(challenge);
                }
            }
            return incentives;
        }

        public static DeleteUserIncentiveResponse RemoveIncentiveReference(int userIncentiveId, int? adminId)
        {
            IncentiveReader reader = new IncentiveReader();
            DeleteUserIncentiveRequest request = new DeleteUserIncentiveRequest();
            request.userIncentiveId = userIncentiveId;
            if (adminId.HasValue)
            {
                request.adminId = adminId.Value;
            }
            return reader.DeleteUserIncentive(request);
        }

        public static bool AddEditIncentives(UserIncetiveModel model)
        {
            IncentiveReader reader = new IncentiveReader();
            var isSuccess = false;
            bool save = true;
            if (model.type == "Add")
            {
                if (model.isTobacco && model.userIncentiveId > 0)
                {
                    isSuccess = reader.ApproveTobaccoAffidavit(model.userIncentiveId);
                }
                else
                {
                    SaveUserIncentiveRequest request = new SaveUserIncentiveRequest();
                    request.userId = model.userId;
                    request.incentiveId = model.portalIncentiveId;
                    request.portalId = model.portalId;
                    request.adminId = model.adminId;
                    isSuccess = reader.SaveUserIncentive(request).success;
                }
            }
            else
            {
                save = false;
                DeleteUserIncentiveRequest request = new DeleteUserIncentiveRequest();
                request.userIncentiveId = model.userIncentiveId;
                request.adminId = model.adminId;
                isSuccess = reader.DeleteUserIncentive(request).success;
            }

            if (isSuccess)
            {
                CommonReader commonReader = new CommonReader();
                PortalReader portalReader = new PortalReader();
                object[] args = new object[2];
                var portalIncentive = reader.ReadPortalIncentivesByIncentiveId(model.portalIncentiveId);
                args[0] = portalIncentive.Points;
                string messageType = IncentiveMessageTypes.Incentive;
                if (portalIncentive.IsCompanyIncentive)
                {
                    if (portalIncentive.IncentiveTypeId == (int)IncentiveTypes.Tobacco_Initiative)
                        messageType = IncentiveMessageTypes.TC_Incentive;
                    else if (portalIncentive.IncentiveTypeId == (int)IncentiveTypes.Maternity_Completion)
                        messageType = IncentiveMessageTypes.PP_Incentive;
                    else if (portalIncentive.IncentiveTypeId == (int)IncentiveTypes.HRA_Completion)
                    {
                        messageType = IncentiveMessageTypes.HRA_Incentive;
                        args[1] = portalReader.GetPortalByName(portalIncentive.PortalId);
                    }
                }
                if (!save)
                {
                    commonReader.DeleteIncentiveMessages(portalIncentive, model.userId, messageType);
                }
                else
                {
                    commonReader.AddDashboardMessage(model.userId, messageType, null, null, args);
                }
            }
            return true;
        }

        public static bool AddCustomIncentive(int PortalIncentiveId, int CustomIncentiveType, double Points, string comments, int adminId, int participantId)
        {
            IncentiveReader reader = new IncentiveReader();
            CommonReader commonReader = new CommonReader();
            AddCustomIncentiveRequest request = new AddCustomIncentiveRequest();
            request.adminId = adminId;
            var userId = request.userId = participantId;
            request.PortalIncentiveId = PortalIncentiveId;
            request.CustomIncentiveType = CustomIncentiveType;
            request.Points = Points;
            request.comments = comments;
            var response = reader.AddCustomIncentive(request);
            if (response.success == true)
            {
                string LanguageCode = reader.GetCustomIncentiveTypes().Where(x => x.Id == CustomIncentiveType).FirstOrDefault().LanguageCode;
                object[] args = new object[2];
                args[0] = Points;
                args[1] = LanguageCode;
                commonReader.AddDashboardMessage(userId, IncentiveMessageTypes.Custom_Incentive, null, null, args);
            }
            return true;
        }
    }
}