using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ConsoleColor;

namespace AsciiEngine
{
    partial class Core
    {
        public static string[] ThrusterParticles =
        {
            "+"
        };

        public static ConsoleColor[,] ThrusterParticlesColorMatrix =
        {
            { DarkRed }
        };

        public static string[] Missile =
        {
            "."
        };

        public static ConsoleColor[,] MissileColorMatrix =
        {
            { Gray }
        };

        public static string[] StarDust =
        {
            "*"
        };
        public static string[] DefaultGraphics =
        {
            " ",
            " "
        };
        public static string[] PlayerGraphic = {
            "   OXXO   ",
            "  XOXXOX  ",
            " XXXXXXXX ",
            "XX++XX++XX",
            "XX++XX++XX",
        };
        public static string[] AsteroidGraphic = {
            " #######  ",
            "######### ",
            "##########",
            " ######## "
        };

        public static string[] AsteroidGraphic2 = {
            "   ############    ",
            " ################  ",
            "#################  ",
            "   ################",
            "  ###############  ",
            " ###############   ",
            "   ##########      "
        };

        public static string[] AsteroidGraphic3 =
        {
            " ####",
            "#### ",
            "#####"

        };

        public static string[] AsteroidGraphic4 =
        {

            "             ###############################                   ",
            "            ##################################                 ",
            "         ########################################              ",
            "        ###########################################            ",
            "       ##############################################          ",
            "     ###################################################       ",
            "   ##########################################################  ",
            "  ###########################################################  ",
            "     ######################################################### ",
            "          ###################################################  ",
            "         ##################################################    ",
            "             ###############################################   ",
            "            ################################################## ",
            "            ################################################## ",
            "           ##################################################  ",
            "         ##################################################    ",
            "       ################################################        ",
            "         ##########################################            ",
            "          #######################################              ",
            "             #################################                 "


        };

        public static ConsoleColor[,] PlayerGraphicColorMatrix =
            {
                {Gray, Gray, Gray, Blue, Gray, Gray, Blue, Gray, Gray, Gray },
                {Gray, Gray, Gray, Blue, Gray, Gray, Blue, Gray, Gray, Gray },
                {Gray, Gray, Gray, Gray, Gray, Gray, Gray, Gray, Gray, Gray },
                {Gray, Gray, DarkRed, DarkRed, Gray, Gray, DarkRed, DarkRed, Gray, Gray },
                {Gray, Gray, Red, Red, Gray, Gray, Red, Red, Gray, Gray }
            };
    }
}
