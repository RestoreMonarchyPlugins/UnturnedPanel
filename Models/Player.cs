using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUnturnedPanel.Models
{
    public class Player
    {
        public ulong PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerAvatar { get; set; }
        public string PlayerCountry { get; set; }
        public DateTime PlayerCreated { get; set; }

        public virtual char Role { get; set; }
        public virtual List<PlayerBan> PlayerBans { get; set; }
    }
}
