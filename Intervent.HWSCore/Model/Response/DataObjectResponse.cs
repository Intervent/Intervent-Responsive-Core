namespace Intervent.HWS
{
    public class Fitness
    {
        public DateTime dateTime;
        public string _id { get; set; }
        public string activity_category { get; set; }
        public double? calories { get; set; }
        public double? distance { get; set; }
        public double? duration { get; set; }
        public string intensity { get; set; }
        public string last_updated { get; set; }
        public string source { get; set; }
        public string source_name { get; set; }
        public string start_time { get; set; }
        public string timestamp { get; set; }
        public string type { get; set; }
        public string utc_offset { get; set; }
        public bool validated { get; set; }
        public int weeknumber { get; set; }
    }

    public class Weight
    {
        public string _id { get; set; }
        public double? bmi { get; set; }
        public object fat_percent { get; set; }
        public object free_mass { get; set; }
        public double? height { get; set; }
        public string last_updated { get; set; }
        public object mass_weight { get; set; }
        public string source { get; set; }
        public string source_name { get; set; }
        public string timestamp { get; set; }
        public string utc_offset { get; set; }
        public bool validated { get; set; }
        public double? weight { get; set; }
    }

    public class Diabetes
    {
        public string _id { get; set; }
        public double blood_glucose { get; set; }
        public object c_peptide { get; set; }
        public object fasting_plasma_glucose_test { get; set; }
        public object hba1c { get; set; }
        public object insulin { get; set; }
        public string last_updated { get; set; }
        public object oral_glucose_tolerance_test { get; set; }
        public object random_plasma_glucose_test { get; set; }
        public string relationship_to_meal { get; set; }
        public string source { get; set; }
        public string source_name { get; set; }
        public string timestamp { get; set; }
        public object triglyceride { get; set; }
        public string user_id { get; set; }
        public string utc_offset { get; set; }
        public bool validated { get; set; }
    }

    public class Biometric
    {
        public string _id { get; set; }
        public object blood_calcium { get; set; }
        public object blood_chromium { get; set; }
        public object blood_folic_acid { get; set; }
        public object blood_magnesium { get; set; }
        public object blood_potassium { get; set; }
        public object blood_sodium { get; set; }
        public object blood_vitamin_b12 { get; set; }
        public object blood_zinc { get; set; }
        public object creatine_kinase { get; set; }
        public object crp { get; set; }
        public double? diastolic { get; set; }
        public object ferritin { get; set; }
        public object hdl { get; set; }
        public object hscrp { get; set; }
        public object il6 { get; set; }
        public string last_updated { get; set; }
        public object ldl { get; set; }
        public object resting_heartrate { get; set; }
        public string source { get; set; }
        public string source_name { get; set; }
        public object spo2 { get; set; }
        public double? systolic { get; set; }
        public object temperature { get; set; }
        public object testosterone { get; set; }
        public string timestamp { get; set; }
        public object total_cholesterol { get; set; }
        public object tsh { get; set; }
        public object uric_acid { get; set; }
        public string user_id { get; set; }
        public string utc_offset { get; set; }
        public bool validated { get; set; }
        public object vitamin_d { get; set; }
        public object white_cell_count { get; set; }
    }

    public class DrugsResponse
    {
        public Meta meta { get; set; }
        public List<Results> results { get; set; }
    }

    public class Params
    {
        public string start_date { get; set; }
        public string end_date { get; set; }
        public bool? expanded { get; set; }
        public int? offset { get; set; }
        public int? limit { get; set; }
    }

    public class Meta
    {
        public string disclaimer { get; set; }
        public string results { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public string previous { get; set; }
        public string next { get; set; }
        public Params @params { get; set; }
    }

    public class Results
    {
        public OpenFda openfda { get; set; }
        public object dosage_and_administration { get; set; }
    }

    public class OpenFda
    {
        public object brand_name { get; set; }
    }
}