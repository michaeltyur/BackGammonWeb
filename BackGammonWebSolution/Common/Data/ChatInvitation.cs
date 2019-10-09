using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
    [DataContract]
    public  class ChatInvitation
    {

        [DataMember]
        public int InviterID { get; set; }
        [DataMember]
        public string InviterName { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Error { get; set; }
    }
}
