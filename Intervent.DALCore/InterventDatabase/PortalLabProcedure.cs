namespace Intervent.DAL
{
    public partial class PortalLabProcedure
    {
        public int Id { get; set; }

        public int PortalId { get; set; }

        public int LabProcedureId { get; set; }

        public virtual LabProcedure LabProcedure { get; set; }

        public virtual Portal Portal { get; set; }
    }
}
