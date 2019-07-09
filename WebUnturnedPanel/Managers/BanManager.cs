using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using RestoreMonarchy.WebUnturnedPanel.Models;

namespace RestoreMonarchy.WebUnturnedPanel.Managers
{
    public class BanManager
    {
        private readonly MySqlConnection connection;
        public BanManager(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public List<PlayerBan> GetPlayerBans(ulong steamId)
        {
            List<PlayerBan> bans;
            using (connection)
            {
                bans = connection.Query<PlayerBan, Player, Player, PlayerBan>("SELECT b.*, p.*, v.* FROM Bans AS b INNER JOIN Players AS p ON (b.PunisherId = p.PlayerId) INNER JOIN Players AS v ON (b.PlayerId = v.PlayerId) WHERE b.PlayerId = @PlayerId;",
                            (b, p, v) =>
                            {
                                b.Punisher = p;
                                b.Player = v;
                                return b;
                            },
                            new { PlayerId = steamId },
                            splitOn: "PlayerId,PlayerId").ToList();
            }
            return bans;
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
