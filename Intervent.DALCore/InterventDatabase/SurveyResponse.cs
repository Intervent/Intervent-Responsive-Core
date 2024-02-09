namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SurveyResponse")]
    public partial class SurveyResponse
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public int UsersinProgramsId { get; set; }

        public int Answer { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual SurveyQuestion SurveyQuestion { get; set; }

        public virtual UsersinProgram UsersinProgram { get; set; }
    }
}
