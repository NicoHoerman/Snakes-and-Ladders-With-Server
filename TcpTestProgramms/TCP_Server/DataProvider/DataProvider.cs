﻿using System.Collections.Generic;

namespace EandE_ServerModel.DataProvider
{ 
    public class DataProvider
    {
        private Dictionary<string, string> _text = new Dictionary<string, string>
        {
            {
				"mainmenuinfo",
                "Welcome at Eels and Escalators\n" +
                "You are in the MainMenu\n\nCommandlist\n"+
                "/startgame\n" +
                "/closegame\n" +
                "/rolldice (only ingame)\n" +
                "\n"+
                "rule commands:\n"+
                "/classic\n" +
                "\n" +
                "First choose a ruleset then start the game"
            },
            {
                "gameinfo",
                "Game started\n" +
                "Rules: Classic\n" +
                "\n" +
                "\n" +
                "Symbols:\n" +
                "Player 1 is 1\n" +
                "Player 2 is 2\n" +
                "[ | , | ] is a field\n" +
                "S Top of an Eel\n" +
                "s Bottom of a Eel\n" +
                "E Top of an Escalator\n" +
                "e Bottom of an Escalator"
            },
            {
                "afterboardinfo",
                "Player {0} Turn \n" +
                "Roll the dice!"
            },
            {
                "gamefinishedinfo",
                "Game finished\n"+
                "{0} wins!"
            },
            {
                "diceresultinfo",
                "Player {1} rolled a {0}"
            },
            {
                "playerexceedsboardinfo",
                "Your role was too high\n"+
                "Better Luck next time"
            },
            {
                "playerwins",
                "Player {0} Wins\n"+
                "Eeeeelssss"
            },
            {
                "finishskull1",
                    "███████████████████████████████████\n" +
                    "███████████████████████████████████\n" +
                    "███████████████████████████████████\n" +
                    "█████████████▒▒▒▒▒▒▒▒▒█████████████\n"+
                    "█████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█████████\n" +
                    "███████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒███████\n" +
                    "██████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██████\n" +
                    "█████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█████\n" +
                    "█████▒▒▒▒█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█▒▒▒▒█████\n" +
                    "████▒▒▒▒███▒▒▒▒▒▒▒▒▒▒▒▒▒███▒▒▒▒████\n" +
                    "███▒▒▒▒██████▒▒▒▒▒▒▒▒▒██████▒▒▒▒███\n" +
                    "███▒▒▒███▐▀███▒▒▒▒▒▒▒███▀▌███▒▒▒███\n" +
                    "███▒▒▒██▄▐▌▄███▒▒▒▒▒███▄▐▌▄██▒▒▒███\n" +
                    "███▒▒▒▒██▌███▒▒▒█▒█▒▒▒███▐██▒▒▒▒███\n" +
                    "██▒▒▒▒▒▒███▒▒▒▒██▒██▒▒▒▒███▒▒▒▒▒▒██\n" +
                    "█▒▒▒▒▒▒▒▒█▒▒▒▒██▒▒▒██▒▒▒▒█▒▒▒▒▒▒▒▒█\n" +
                    "█▒▒▒▒▒▒▒▒▒▒▒▒▒█▒▒▒▒▒█▒▒▒▒▒▒▒▒▒▒▒▒▒█\n" +
                    "█▒▒▒▒█▒▒█▒▒▒▒██▒▒▒▒▒██▒▒▒▒█▒▒█▒▒▒▒█\n" +
                    "██▒▒▒█▒▒█▒▒▒▒█▒██▒██▒█▒▒▒▒█▒▒█▒▒▒██\n" +
                    "███▒█▒▒█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█▒▒█▒███\n" +
                    "█████▒▒█▒▒▒▐███████████▌▒▒▒█▒▒█████\n" +
                    "███████▒▒▒▐█▀██▀███▀██▀█▌▒▒▒███████\n" +
                    "███████▒▒▒▒█▐██▐███▌██▌█▒▒▒▒███████\n" +
                    "███████▒▒▒▒▒▐▒▒▐▒▒▒▌▒▒▌▒▒▒▒▒███████\n" +
                    "████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒████████\n" +
                    "████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒████████\n" +
                    "█████████▒▒█▒█▒▒▒█▒▒▒█▒█▒▒█████████\n" +
                    "█████████▒██▒█▒▒▒█▒▒▒█▒██▒█████████\n" +
                    "██████████████▒▒███▒▒██████████████\n" +
                    "██████████████▒█████▒██████████████\n" +
                    "███████████████████████████████████\n" +
                    "███████████████████████████████████"
            },
            {
                "finishskull2",
                    "       ▄█          █         █▄       \n"+
                    "─────▐██      ▄█  ███  █▄     ██▌─────\n"+
                    "────▐██▀     █████████████    ▀██▌────\n"+
                    "───▐██▌     ██████████████     ▐██▌───\n"+
                    "───████    ████████████████    ████───\n"+
                    "──▐█████  ██████████████████  █████▌──\n"+
                    "───████████████████████████████████───\n"+
                    "────███████▀▀████████████▀▀███████────\n"+
                    "─────█████▌  ▄▄ ▀████▀ ▄▄  ▐█████─────\n"+
                    "───▄▄██████▄ ▀▀  ████  ▀▀ ▄██████▄▄───\n"+
                    "──██████████████████████████████████──\n"+
                    "─████████████████████████████████████─\n"+
                   "▐██████  ███████▀▄██▄▀███████  ██████▌\n"+
                    "▐█████    ██████████████████    █████▌\n"+
                    "▐█████     ██████▀  ▀██████     █████▌\n"+
                    "─█████▄     ███        ███     ▄█████─\n"+
                    "──██████     █          █     ██████──\n"+
                    "────█████                    █████────\n"+
                    "─────█████                  █████─────\n"+
                    "──────█████                █████──────\n"+
                    "───────████   ▄        ▄   ████───────\n"+
                    "────────████ ██        ██ ████────────\n"+
                    "────────████████ ▄██▄ ████████────────\n"+
                    "───────████████████████████████───────\n"+
                    "───────████████████████████████───────\n"+
                    "────────▀█████████▀▀█████████▀────────\n"+
                    "──────────▀███▀        ▀███▀──────────"

            }

        };

        public string GetText(string key) => _text[key];

        private string[] _numberLiterals = new string[]
        {
                "Zero",
                "One",
                "Two",
                "Three",

        };

        public string GetNumberLiteral(int value) => _numberLiterals[value];
        
    }
}
