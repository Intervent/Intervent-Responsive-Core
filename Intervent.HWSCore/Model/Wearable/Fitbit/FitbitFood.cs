namespace Intervent.HWS.Model
{
    public class FitbitFood : ProcessResponse
    {
        public List<Food> foods { get; set; }

        public List<Note> notes { get; set; }

        public Summary summary { get; set; }

        public class Food
        {
            public bool isFavorite { get; set; }

            public string logDate { get; set; }

            public object logId { get; set; }

            public LoggedFood loggedFood { get; set; }

            public NutritionalValues nutritionalValues { get; set; }
        }

        public class LoggedFood
        {
            public string accessLevel { get; set; }

            public double amount { get; set; }

            public string brand { get; set; }

            public double calories { get; set; }

            public int foodId { get; set; }

            public string locale { get; set; }

            public int mealTypeId { get; set; }

            public string name { get; set; }

            public Unit unit { get; set; }

            public List<int> units { get; set; }
        }

        public class Note
        {
            public long id { get; set; }

            public string mealType { get; set; }

            public string message { get; set; }

            public DateTime updatedTime { get; set; }
        }

        public class NutritionalValues
        {
            public double calories { get; set; }

            public double carbs { get; set; }

            public double fat { get; set; }

            public double fiber { get; set; }

            public double protein { get; set; }

            public double sodium { get; set; }
        }

        public class Summary
        {
            public double calories { get; set; }

            public double carbs { get; set; }

            public double fat { get; set; }

            public double fiber { get; set; }

            public double protein { get; set; }

            public double sodium { get; set; }

            public double water { get; set; }
        }

        public class Unit
        {
            public int id { get; set; }

            public string name { get; set; }

            public string plural { get; set; }
        }
    }
}
