//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Intervent.DAL
{
    using System;
    public partial class GetMessages_Result
    {
        public int MsgId { get; set; }

        public int? ParentMessageId { get; set; }

        public int? Total { get; set; }

        public DateTime? CreateDate { get; set; }

        public string? Subject { get; set; }

        public string? MessageBody { get; set; }

        public string? RecentMessage { get; set; }

        public string? Attachment { get; set; }

        public bool? NoActionNeeded { get; set; }

        public bool IsSent { get; set; }

        public int? CreatorId { get; set; }

        public int? MessageCreatorId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Picture { get; set; }

        public byte? Gender { get; set; }

        public string? HasResentAttachment { get; set; }

        public int? RecipientId { get; set; }

        public int? ReadById { get; set; }

        public string? ReadByName { get; set; }

        public string? CreatorRole { get; set; }

        public bool? IsRead { get; set; }

        public DateTime? LastMessageDate { get; set; }
    }
}