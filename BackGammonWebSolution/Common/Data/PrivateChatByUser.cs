using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
   public class PrivateChatByUser
    {
        [DataMember]
        public int PrivateChatID { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public DateTime TimeCreation { get; set; }

        [DataMember]
        public int UserID { get; set; }

        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public int OpponentID { get; set; }
        [DataMember]
        public string OpponentName { get; set; }



    }
}
