using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
   public class ChatClosing
    {
        [DataMember]
        public int CloserID { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Error { get; set; }
    }
}
