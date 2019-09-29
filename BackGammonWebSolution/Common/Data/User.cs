using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
    [DataContract]
    public class User
    {
        public User()
        {
            UserPrivateChats = new HashSet<UserPrivateChat>();
        }
        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public string UserName { get; set; }
        // public ClaimsIdentity Username { get; set; }
        public string Password { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public bool IsOnline { get; set; }

        [ForeignKey("UserPrivateChatID")]
        public  ICollection<UserPrivateChat> UserPrivateChats { get; set; }

        public string SignalRConnectionID { get; set; }


    }

}
