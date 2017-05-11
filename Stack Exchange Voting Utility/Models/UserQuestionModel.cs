using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Stack_Exchange_Voting_Utility.Models
{
    public class UserQuestionModel
    {
        [Key, Column(Order = 1)]
        [MaxLength(128)]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        [Key, Column(Order = 2)]
        public int QuestionId { get; set; }

        [Key, Column(Order = 3)]
        public string Site { get; set; }

        public string Action { get; set; }
    }
}
