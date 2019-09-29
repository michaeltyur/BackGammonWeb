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

        public int UserPrivateChatID { get; set; }
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }


        public int PrivateChatID{ get; set; }

        [ForeignKey("PrivateChatID")]
        public  PrivateChat PrivateChat { get; set; }
    }
}
