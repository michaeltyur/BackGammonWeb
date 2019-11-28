using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
    [DataContract]
    public class ErrorMessage
    {
        [DataMember]
        public int MessageId { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public DateTime Date { get; set; }


    }

}
