namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class UserChoice
    {
        public int Id { get; set; }

        public int KitsInUserProgramsId { get; set; }

        public int QuestionId { get; set; }

        public string? Value { get; set; }

        public DateTime DateCreated { get; set; }

        [StringLength(64)]
        public string? ValueLangItemCode { get; set; }

        public virtual KitsinUserProgram KitsinUserProgram { get; set; }

        public virtual QuestionsinActivity QuestionsinActivity { get; set; }
    }
}
