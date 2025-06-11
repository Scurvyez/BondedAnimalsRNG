using UnityEngine;
using Verse;

namespace BondedAnimalsRNG
{
    public class BARNGLog
    {
        public static Color ErrorMsgCol = new (0.4f, 0.54902f, 1.0f);
        public static Color WarningMsgCol = new (0.70196f, 0.4f, 1.0f);
        public static Color MessageMsgCol = new (0.4f, 1.0f, 0.54902f);

        public static void Error(string msg)
        {
            Log.Error("[Bonded Animals RNG] ".Colorize(ErrorMsgCol) + msg);
        }

        public static void Warning(string msg)
        {
            Log.Warning("[Bonded Animals RNG] ".Colorize(WarningMsgCol) + msg);
        }

        public static void Message(string msg)
        {
            Log.Message("[Bonded Animals RNG] ".Colorize(MessageMsgCol) + msg);
        }
    }
}