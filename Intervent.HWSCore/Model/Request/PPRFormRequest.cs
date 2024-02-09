namespace Intervent.HWS
{
    public class PPRFormRequest
    {
        public string participant_first_name { get; set; }

        public string participant_last_name { get; set; }

        public string participant_birth_date { get; set; }

        public string participant_study_subject_ID { get; set; }

        public string session_date_time { get; set; }

        public int? session_length_minutes { get; set; }

        public string session_communication_type { get; set; }

        public string coach_name { get; set; }

        public CoachingGoals coaching_goals { get; set; }

        public ReviewedCoachingGoals reviewed_coaching_goals { get; set; }

        public ReviewedBpTrends reviewed_bp_trends { get; set; }

        public ReviewedBpMonitoringTrends reviewed_bp_monitoring_trends { get; set; }

        public ReviewedBpMedications reviewed_bp_medications { get; set; }

        public ReviewedBpMedicationAdherence reviewed_bp_medication_adherence { get; set; }

        public ReviewedPhysicalActivityTrends reviewed_physical_activity_trends { get; set; }

        public ReviewedSodiumIntake reviewed_sodium_intake { get; set; }

        public bool? reviewed_problem_list { get; set; }

        public ReviewedEdVisits reviewed_ed_visits { get; set; }

        public ReviewedHospitalAdmits reviewed_hospital_admits { get; set; }

        public ReviewedFalls reviewed_falls { get; set; }

        public ReviewedRehabTherapy reviewed_rehab_therapy { get; set; }

        public string community_or_social_services_utilized { get; set; }

        public string emergent_resources_needed { get; set; }

        public string session_notes { get; set; }

        public string e_sig { get; set; }

        public string e_sig_date_time { get; set; }

        public bool? reviewed_neurology_follow_up_appointments { get; set; }

        public bool? reviewed_triage_escalation_process { get; set; }

        public UpdatedCoachingGoals updated_coaching_goals { get; set; }

        public NextCoachingAppointment next_coaching_appointment { get; set; }

        public class CoachingGoals
        {
            public string monitoring_bp { get; set; }

            public string taking_bp_medications { get; set; }

            public string physical_activity { get; set; }

            public int? physical_activity_steps { get; set; }

            public int? physical_activity_minutes { get; set; }

            public string healthy_diet { get; set; }

            public string managing_stress { get; set; }

            public string managing_weight { get; set; }

            public string limiting_alcohol { get; set; }

            public string quit_smoking { get; set; }

            public string other { get; set; }
        }

        public class EdVisitList
        {
            public string date_of_visit { get; set; }

            public string reason { get; set; }
        }

        public class FallsList
        {
            public string injury { get; set; }
        }

        public class HospitalAdmitsList
        {
            public string date_of_admit { get; set; }

            public int? number_of_days { get; set; }

            public string reason { get; set; }
        }

        public class NextCoachingAppointment
        {
            public bool? confirmed { get; set; }

            public string date { get; set; }

            public string time { get; set; }

            public string method { get; set; }
        }

        public class ReviewedBpMedicationAdherence
        {
            public bool? taking_as_prescribed { get; set; }

            public int? if_not_num_days_missed { get; set; }

            public string if_not_why { get; set; }
        }

        public class ReviewedBpMedications
        {
            public bool? current { get; set; }

            public bool? any_changes { get; set; }

            public string list_changes { get; set; }
        }

        public class ReviewedBpMonitoringTrends
        {
            public bool? achieved_6_days_per_week { get; set; }

            public string if_achieved_whats_helpful { get; set; }

            public int? if_not_achieved_num_days { get; set; }

            public string if_not_achieved_why { get; set; }
        }

        public class ReviewedBpTrends
        {
            public string sbp_goal_attained { get; set; }
        }

        public class ReviewedCoachingGoals
        {
            public bool? monitoring_bp { get; set; }

            public bool? taking_bp_medications { get; set; }

            public bool? physical_activity { get; set; }

            public bool? healthy_diet { get; set; }

            public bool? managing_stress { get; set; }

            public bool? managing_weight { get; set; }

            public bool? limiting_alcohol { get; set; }

            public bool? quit_smoking { get; set; }

            public bool? other { get; set; }
        }

        public class ReviewedEdVisits
        {
            public bool? has_visited_ed { get; set; }

            public int? number_of_visits { get; set; }

            public List<EdVisitList> ed_visit_list { get; set; }
        }

        public class ReviewedFalls
        {
            public bool? has_fallen { get; set; }

            public int? number_of_falls { get; set; }

            public List<FallsList> falls_list { get; set; }
        }

        public class ReviewedHospitalAdmits
        {
            public bool? was_admitted_hospital { get; set; }

            public int? number_of_visits { get; set; }

            public List<HospitalAdmitsList> hospital_admits_list { get; set; }
        }

        public class ReviewedPhysicalActivityTrends
        {
            public bool? avg_steps_day_reviewed { get; set; }

            public bool? avg_minutes_day_reviewed { get; set; }

            public bool? steps_goal_attained { get; set; }

            public bool? mins_goal_attained { get; set; }

            public string if_goals_achieved_why { get; set; }

            public string if_goals_not_achieved_why { get; set; }
        }

        public class ReviewedRehabTherapy
        {
            public bool? referred_by_provider { get; set; }

            public bool? attending_as_scheduled { get; set; }

            public string if_not_why { get; set; }

            public bool? needs_provider_referral { get; set; }
        }

        public class ReviewedSodiumIntake
        {
            public bool? followed_low_sodium_diet { get; set; }

            public string if_not_why { get; set; }
        }

        public class UpdatedCoachingGoals
        {
            public string monitoring_bp { get; set; }

            public string taking_bp_medications { get; set; }

            public string physical_activity { get; set; }

            public int? physical_activity_steps { get; set; }

            public int? physical_activity_minutes { get; set; }

            public string healthy_diet { get; set; }

            public string managing_stress { get; set; }

            public string managing_weight { get; set; }

            public string limiting_alcohol { get; set; }

            public string quit_smoking { get; set; }

            public string other { get; set; }
        }
    }
}
