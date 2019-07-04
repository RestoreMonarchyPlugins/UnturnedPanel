using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using WebUnturnedPanel.Models;

namespace WebUnturnedPanel.Managers
{
    public class PlayerManager
    {
        private readonly MySqlConnection connection;
        private readonly ulong owner;
        private readonly List<ulong> admins;
        public PlayerManager(MySqlConnection connection, IConfiguration configuration)
        {
            this.connection = connection;
            this.owner = configuration.GetValue<ulong>("Owner");
            this.admins = configuration.GetSection("Admins").Get<List<ulong>>();
        }

        public Player GetPlayer(ulong steamId)
        {
            Player player = null;
            using (connection)
            {
                player = connection.QueryFirstOrDefault<Player>("SELECT * FROM Players WHERE PlayerId = @PlayerId;", new { PlayerId = steamId });
            }

            if (player == null)
            {
                player = new Player();
                player.PlayerId = steamId;
                GetPlayerRole(ref player);
            }
            else
                GetPlayerRole(ref player);

            return player;
        }

        public void GetPlayerRole(ref Player player)
        {
            if (player.PlayerId == owner)
                player.Role = 'O';
            else if (admins.Contains(player.PlayerId))
                player.Role = 'A';
            else
                player.Role = 'D';
        }

        public List<Player> GetPlayers()
        {
            using (connection)
            {
                return connection.Query<Player>("SELECT * FROM Players;").ToList();
            }
        }
    }
}
