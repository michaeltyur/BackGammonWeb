using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
   public class PrivateChat
    {
        public PrivateChat()
        {
            UserPrivateChats = new HashSet<UserPrivateChat>();
        }
        [DataMember]
        public int PrivateChatID { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public DateTime TimeCreation { get; set; }

        [ForeignKey("UserPrivateChatID")]
        public ICollection<UserPrivateChat> UserPrivateChats { get; set; }


    }
}
