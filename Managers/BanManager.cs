using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUnturnedPanel.Models;

namespace WebUnturnedPanel.Managers
{
    public class BanManager
    {
        private readonly MySqlConnection connection;
        public BanManager(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public Player GetPlayerBans(ref Player player)
        {
            Player tPlayer = player;
            tPlayer.PlayerBans = new List<PlayerBan>();
            using (connection)
            {
                connection.Query<Player, PlayerBan, Player>("SELECT p.*, b.* FROM Bans AS b INNER JOIN Players AS p ON (b.PunisherId = p.PlayerId) WHERE b.PlayerId = @PlayerId;",
                            (p, b) =>
                            {
                                b.Punisher = p;                                    
                                tPlayer.PlayerBans.Add(b);
                                return null;
                            },
                            new { PlayerId = player.PlayerId },
                            splitOn: "BanId");
            }
            player = tPlayer;
            return tPlayer;
        }
        
        public List<PlayerBan> GetPlayersBans()
        {
            using (connection)
            {
                string sql = "SELECT b.*, p.*, m.* FROM Bans AS b INNER JOIN Players AS p ON (b.PlayerId = p.PlayerId) INNER JOIN Players AS m ON (b.PunisherId = m.PlayerId);";
                List<PlayerBan> bans = connection.Query<PlayerBan, Player, Player, PlayerBan>(sql, (b, p, m) => 
                {
                    b.Player = p;
                    b.Punisher = m;
                    return b;
                }, splitOn: "PlayerId,PlayerId").ToList();

                return bans;
            }
        }

        public bool AddBan(ulong playerId, ulong punisherId, string reason, int? duration)
        {
            int result = 0;
            try
            {                
                using (connection)
                {
                    if (connection.ExecuteScalar<bool>("SELECT COUNT(1) FROM Players WHERE PlayerId = @PlayerId;", new { PlayerId = playerId }))
                        result = connection.Execute("INSERT INTO Bans (PlayerId, PunisherId, BanReason, BanDuration) VALUES (@PlayerId, @PunisherId, @BanReason, @BanDuration);",
                            new { PlayerId = playerId, PunisherId = punisherId, BanReason = reason, BanDuration = duration });                    
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return result == 0 ? false : true;
        }

        public bool DeleteBan(int banId)
        {
            int result = 0;
            using (connection)
            {
                result = connection.Execute("DELETE FROM Bans WHERE BanId = @BanId;", new { BanId = banId });
            }
            return result == 0 ? false : true;
        }
    }
}
