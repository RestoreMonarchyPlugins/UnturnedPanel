using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUnturnedPanel.Models
{
    public class PlayerBan
    {
        public int BanId { get; set; }
        public ulong PlayerId { get; set; }
        public ulong PunisherId { get; set; }
        public string BanReason { get; set; }
        public int? BanDuration { get; set; }
        public DateTime BanCreated { get; set; }
        public bool SendFlag { get; set; }

        public virtual Player Punisher { get; set; }
        public virtual Player Player { get; set; }

        public bool IsExpired()
        {
            if (BanDuration != null)
            {
                if (BanCreated.AddSeconds(BanDuration.Value) > DateTime.Now)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
    }
}
