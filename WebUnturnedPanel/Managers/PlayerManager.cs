using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using RestoreMonarchy.WebUnturnedPanel.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Microsoft.AspNetCore.Http;
using RestoreMonarchy.WebUnturnedPanel.Helpers;

namespace RestoreMonarchy.WebUnturnedPanel.Managers
{
    public class PlayerManager
    {
        public Player Player => GetPlayer();
        public bool IsSigned
        {
            get
            {
                if (context.User?.Identity?.IsAuthenticated ?? false)
                    return true;
                else
                    return false;
            }
        }
        private Dictionary<ulong, Player> sessionPlayers = new Dictionary<ulong, Player>();
        private readonly IHttpContextAccessor httpContextAccessor;
        private HttpContext context => httpContextAccessor.HttpContext;

        private readonly MySqlConnection connection;        
        private readonly IConfiguration configuration;
        private readonly ImageManager imageManager;
        private readonly SteamManager steamManager;
        private readonly BanManager banManager;
        private readonly ulong owner;
        private readonly List<ulong> admins;

        public PlayerManager(IHttpContextAccessor httpContextAccessor, MySqlConnection connection, IConfiguration configuration, 
            ImageManager imageManager, SteamManager steamManager, BanManager banManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.connection = connection;
            this.imageManager = imageManager;
            this.configuration = configuration;
            this.steamManager = steamManager;
            this.banManager = banManager;
            this.owner = configuration.GetValue<ulong>("Owner");
            this.admins = configuration.GetSection("Admins").Get<List<ulong>>();
        }

        public Player GetPlayer()
        {
            if (context.User?.Identity?.IsAuthenticated ?? false)
            {
                ulong steamId = context.User.Claims.FirstOrDefault().Value.GetSteamId();
                
                if (sessionPlayers.TryGetValue(steamId, out Player player))
                {
                    if ((DateTime.Now - player.LastRefresh).TotalMinutes < 30)
                    {
                        player = QueryPlayer(steamId);
                        player.LastRefresh = DateTime.Now;
                        sessionPlayers[steamId] = player;
                    }
                    
                } else
                {
                    player = QueryPlayer(steamId);
                    sessionPlayers.Add(player.PlayerId, player);
                }

                return player;
            }
            return null;
        }

        private Player QueryPlayer(ulong steamId)
        {
            Player player;
            using (connection)
            {
                player = connection.QueryFirstOrDefault<Player>("SELECT * FROM Players WHERE PlayerId = @PlayerId;", new { PlayerId = steamId });
            }

            if (player == null)
                player = CreatePlayer(steamId);

            GetPlayerRole(ref player);

            return player;
        }

        private Player CreatePlayer(ulong steamId)
        {
            Player player = new Player();
            SteamPlayer steamPlayer = steamManager.GetSteamPlayer(steamId);

            player.PlayerId = steamId;
            player.PlayerName = steamPlayer.personaname;

            imageManager.SaveAvatar(steamPlayer.avatarfull, steamId);

            using (connection)
            {
                connection.Execute("INSERT INTO Players (PlayerId, PlayerName, PlayerCountry) VALUES (@PlayerId, @PlayerName, @PlayerCountry);", new
                {
                    PlayerId = steamId,
                    PlayerName = player.PlayerName,
                    PlayerCountry = player.PlayerCountry
                });
            }

            return player;
        }

        public void UpdatePlayerBans()
        {
            Player.PlayerBans = banManager.GetPlayerBans(Player.PlayerId);
        }

        private void GetPlayerRole(ref Player player)
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
