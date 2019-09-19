using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
    public class UserPrivateChat
    {
        [Key, Column(Order = 1)]
        public int UserID { get; set; }
        public User User { get; set; }

        [Key, Column(Order = 2)]
        public int PrivateChatID{ get; set; }
        public PrivateChat PrivateChat { get; set; }
    }
}
