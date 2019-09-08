using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
   public class PrivateChat
    {
        public int PrivateChatID { get; set; }
        public int FirstUserID { get; set; }
        public string FirstUserName { get; set; }
        public int SecondUserID { get; set; }
        public string SecondUserName { get; set; }
        public string GroupName { get; set; }


    }
}
