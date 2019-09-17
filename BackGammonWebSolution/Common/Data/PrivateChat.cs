using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
   public class PrivateChat
    {
        [DataMember]
        public int PrivateChatID { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public DateTime TimeCreation { get; set; }

        public virtual ICollection<User> Users { get; set; }


    }
}
