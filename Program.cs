using System.Drawing;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

internal class Program
{

    static bool NobodyThere(List<PlacedName> placedNames, Rectangle proposed)
    {
        return true;
    }


    static double CostInDistanceFromFriends(List<PlacedName> placed, Rectangle option)
    {
        // add up the pixel-seconds from option to each placed,
        // and then average it.

        foreach (var p in placed)
        {
        }

        


        return 0.0;

    }


    static int RandomXOrY()
    {
        int LOWEST = int.MinValue + 1000;
        int HIGHEST = int.MaxValue - 1000;

        Random rnd = new Random();
    AGAIN:
        int v = rnd.Next() - (int.MaxValue / 2);
        if (v < LOWEST || v > HIGHEST)
            goto AGAIN;

        return v;
    }



    static Rectangle RandomlyPlacedRect(Size s)
    {
        int x = RandomXOrY();
        int y = RandomXOrY();

        return new Rectangle(x - s.Width / 2,
                                y - s.Height / 2,
                                s.Width,
                                s.Height);
    }


    private static void Main(string[] args)
    {
        const string TIME_TOGETHER = "timeTogether.json";
        const string GUID_NAME_PAIRS = "guidNamePairs.json";

        // Load data
        var timeTogether = new Dictionary<string, TimeSpan>();
        {
            var timeTogetherOnDisk = JsonSerializer.Deserialize<KeyValuePair<string, TimeSpan>[]>(File.ReadAllText(TIME_TOGETHER));
            foreach (var item in timeTogetherOnDisk) { timeTogether[item.Key] = item.Value; }
        }
        var namesFromGuids = new Dictionary<string, string>();
        {
            var namesFromGuidsOnDisk = JsonSerializer.Deserialize<KeyValuePair<string, string>[]>(File.ReadAllText(GUID_NAME_PAIRS));
            foreach (var item in namesFromGuidsOnDisk) { namesFromGuids[item.Key] = item.Value; }
        }

        // sort by time
        var sortedByLongestTogether = timeTogether.OrderByDescending(x => x.Value).ToList();
        // extract all that have me in the key.
        var allWhoMightAppear = new HashSet<string>();
        allWhoMightAppear.Add(args[0]); // me!
        foreach (var pair in sortedByLongestTogether)
        {
            if (pair.Key.Contains(args[0]))
                allWhoMightAppear.Add(pair.Key.Replace(args[0], ""));
        }

        // If I have no name for anyone, then they won't appear.
        var allWhoWillAppear = new HashSet<string>();
        foreach (string guid in allWhoMightAppear)
        {
            if (namesFromGuids.ContainsKey(guid))
                allWhoWillAppear.Add(guid);
        }
        allWhoMightAppear = null;

        // for every pair found in our data, I want their durations together.
        // HashSet<string> allInvolved = new HashSet<string>();
        Dictionary<string, TimeSpan> timeTogetherOfAllInvolved = new Dictionary<string, TimeSpan>();
        foreach (var pal in allWhoWillAppear)
        {
            foreach (var pair in timeTogether)
            {
                if (pair.Key.Contains(pal))
                {
                    // is the other party also involved here?
                    var otherPal = pair.Key.Replace(pal, "");
                    if (allWhoWillAppear.Contains(otherPal))
                    {
                        timeTogetherOfAllInvolved[pair.Key] = pair.Value;
                    }
                }
            }
        }

        // placement data structure
        List<PlacedName> placedNames = new List<PlacedName>();

        // now place them with weighting
        foreach (var humanGuid in allWhoWillAppear)
        {
            // Need a height and width of the name.
            // which I think derives from GDI somehow.
            // a choosing of a font.
            // but i insist on an unlimited canvas in this round.
            // rather than a fixed size.
            var name = namesFromGuids[humanGuid];

            // Get the size of the name string when rendered.
            // For now, just assume 200px x 50px
            Size size = new Size(200, 50); // https://github.com/mono/SkiaSharp

            Rectangle rect;
            do
            {
                rect = RandomlyPlacedRect(size);
            }
            while (false == NobodyThere(placedNames, rect));

            placedNames.Add(new PlacedName("foo", "guid", rect, 128));
        }

        // All have been placed somewhere. Now iterate everyone toward their highest time
        // by making it costliest to move far away from the highest timers.
        //for (int i = 0; i < allWhoWillAppear.Count; i++)
        bool fMovedOne = false;
        do
        {
            fMovedOne = false;
            foreach (var one in placedNames)
            {
                // for each iteration, just compare current pos against a random pos.
                // if it's not better, then just leave it.
                // if it's better, then migrate it.

                Rectangle newpossiblerect;
                do
                {
                    newpossiblerect = RandomlyPlacedRect(one.Rect.Size);
                }
                while (false == NobodyThere(placedNames, newpossiblerect));

                if (CostInDistanceFromFriends(placedNames, newpossiblerect) > CostInDistanceFromFriends(placedNames, one.Rect))
                {
                    one.Rect = newpossiblerect;
                    fMovedOne = true;
                }
            }
        }
        while (fMovedOne); // iterate until we don't find a better option for anyone.
    }
}




            // A migrated item is re-colored based on their relationship with neighbor colors
            // from furthest neighbor to nearest.
            // (go ahead and skip to the last 10%)

                // if it's unoccupied, then it's possible.
//                if () { possible.Add(proposed); }
  //          }

            // I have possibles. Pick the one closest to top friends, and 
            // 2. choose the place that pushes the fewest seconds far
            // weight = seconds * pixels ? 
            /*
            Rectangle? topPick = null;
            double goodnessOfTopPick = 0.0;
            foreach (var option in possible)
            {
                if (topPick == null)
                {
                    topPick = option;
                    goodnessOfTopPick = CalcGoodness(placedNames, option);
                    continue;
                }

                var goodnessOfThisPick = CalcGoodness(placedNames, option);
                if (goodnessOfThisPick < goodnessOfTopPick)
                {
                    topPick = option;
                    goodnessOfTopPick = goodnessOfThisPick;
                }
            }
            */

            // for this option, what's the spot weight?
            //Int64 spotWeight = 0;
            //foreach(var placed in placedNames)
            // add to spotWeight the seconds * pixels between tehse two
            //                    var newGuy = new PlacedName(name, humanGuid, proposed, rnd.Next() % 360);
            //                    placedNames.Add(newGuy);
            // 3. with additional gravitation toward 0.0?
            // 4. shuffle forward and back in the list a little.

 

class PlacedName
{
    public PlacedName(string name, string guid, Rectangle rect, int col)
    {
        Name = name;
        Guidval = guid;
        Rect = rect;
        Color = col;
    }
    public string Name { get; set; }
    public string Guidval { get; set; }
    public Rectangle Rect { get; set; }
    public int Color { get; set; }
};
