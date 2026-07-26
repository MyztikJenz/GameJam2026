using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace GameJam2026 {

    public class GameDetail {
        internal string id { get; set; }
        internal string name { get; set; }
        internal Texture2D icon { get; set; }
        internal float bestTime { get; set; }
        internal float currentTime { get; set; } // used when the game is being actively played
    }


    public static class GameDetails {
        public static List<GameDetail> games { get; }
        public static string DefuseTheBombScreenID = "5EA0717F-BFB7-4183-87DA-39957C39331E";

        static GameDetails() {
            ContentManager contentMgr = GameJam2026Game.GameObj.Content;
            games = new List<GameDetail>();

            // This isn't a game, but if we don't have a definition for it here, weird things happen.
            games.Add(new GameDetail { name = "Final Bomb Screen",
                                       icon = null,
                                       id = GameDetails.DefuseTheBombScreenID } );

            games.Add(new GameDetail { name = "Count Takedown",
                                       icon = contentMgr.Load<Texture2D>("icons/count_takedown"),
                                       id = "E6078C10-2ECE-437D-A984-EE93F7D9BE07" });

            games.Add(new GameDetail { name = "Down Feathers",
                                       icon = contentMgr.Load<Texture2D>("icons/down_feathers"),
                                       id = "A696084A-375C-427A-9723-2913FCA99969" });

            games.Add(new GameDetail { name = "Long Way Down",
                                       icon = contentMgr.Load<Texture2D>("icons/long_way_down"),
                                       id = "B6FA04CE-B76B-4772-BCFA-C0993965EBE5" });

            games.Add(new GameDetail { name = "Can You Count",
                                       icon = contentMgr.Load<Texture2D>("icons/can_you_count"),
                                       id = "8D656081-A9CB-4465-99E6-8B1C01CC0738" } );
        }

        public static GameDetail FindGameWithID(string id) {
            return games.Find(g => g.id == id);
        }
    }
}