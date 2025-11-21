namespace EvolveThisMatch.Core
{
    public static class BattleContext
    {
        public static int category = 0;
        public static int chapter = 0;

        public static bool genesisCoin = false;
        public static bool originCrystal = false;
        public static bool fateRoneStone = false;
        public static bool heroSeal = false;

        public static void Clear()
        {
            category = 0;
            chapter = 0;

            genesisCoin = false;
            originCrystal = false;
            fateRoneStone = false;
            heroSeal = false;
        }
    }
}