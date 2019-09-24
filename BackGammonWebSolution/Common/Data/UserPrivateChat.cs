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

        public int UserID { get; set; }
        public  User User { get; set; }

        public int PrivateChatID{ get; set; }
        public  PrivateChat PrivateChat { get; set; }
    }
}
