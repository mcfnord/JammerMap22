using System.Drawing;
using System.Net.Sockets;
using System.Text.Json;

internal class Program
{
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

            // 1. find 3 places where this could fit.
            List<Rectangle> possible = new List<Rectangle>();
            while(possible.Count < 3)
            {
                // pick a random center spot
                // anything in integer range, minus 1000px in
                int LOWEST = int.MinValue + 1000;
                int HIGHEST = int.MaxValue - 1000;
                
                Random rnd = new Random();
                AGAIN_X:
                int x = rnd.Next();
                if (x < LOWEST || x > HIGHEST)
                    goto AGAIN_X;
                AGAIN_Y:
                int y = rnd.Next();
                if (y < LOWEST || y > HIGHEST)
                    goto AGAIN_Y;

                Rectangle proposed = new Rectangle(
                        x - size.Width / 2,
                        y - size.Height / 2,
                        size.Width,
                        size.Height);

                // if it's unoccupied, then it's possible.
                if(NobodyThere(placedNames, proposed))
                {
                    var newGuy = new PlacedName(name, humanGuid, proposed, rnd.Next() % 360);
                    placedNames.Add(newGuy);
                }
            }

            // 2. choose the place that pushes the fewest seconds far
            // weight = seconds * pixels ? a floating point? an int64!
            Rectangle? topPick = null;
            foreach(var option in possible)
            {
                if (topPick == null)
                    topPick = option;
                // for this option, what's the spot weight?
                Int64 spotWeight = 0;
                foreach(var placed in placedNames)
                {
                    // add to spotWeight the seconds * pixels between tehse two
                    xxx

                }




            }
            // 3. with additional gravitation toward 0.0?
            // 4. shuffle forward and back in the list a little.
        }
    }
}

class PlacedName
{
    public PlacedName(string name, string guid, Rectangle rect, int col)
    {
        Name = name;
        Guid = guid;
        Rect = rect;
        Color = col;
    }
    public string Name { get; set; }
    public Rectangle Rect { get; set; }
    public int Color { get; set; }
};
