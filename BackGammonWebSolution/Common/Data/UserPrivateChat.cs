using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
   // [Table("UserPrivateChats")]
    public class UserPrivateChat
    {
        public int UserPrivateChatID { get; set; }
        public int UserID { get; set; }
        public int PrivateChatID{ get; set; }

    }
}
