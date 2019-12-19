using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
    [DataContract]
    public class UserAddInfo
    {
        [DataMember]
        public int UserAddInfoID { get; set; }
        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public bool IsOnline { get; set; }
        [DataMember]
        public DateTime LastVisitTime { get; set; }
    }
}
