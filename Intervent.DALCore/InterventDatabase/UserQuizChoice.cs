namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class UserQuizChoice
    {
        public int Id { get; set; }

        public int QuizId { get; set; }

        public int KitsInUserProgramsId { get; set; }

        [StringLength(100)]
        public string? Value { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual KitsinUserProgram KitsinUserProgram { get; set; }

        public virtual QuizinStep QuizinStep { get; set; }
    }
}
