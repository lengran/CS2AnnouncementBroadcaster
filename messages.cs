using System.Text.Json;
using CounterStrikeSharp.API.Modules.Utils;

namespace CS2AnnouncementBroadcaster;

public class MessageConfig
{
    public List<OnPlayerConnectMsg> ?OnPlayerConnectMsgs { get; set; }

    public List<OnPlayerConnectMsg> ?OnAdminConnectMsgs { get; set; }       // deprecated. will be deleted in future updates. use the admin flag instead.

    public List<OnRoundStartMsg> ?OnRoundStartMsgs { get; set; }

    public List<OnCommandMsg> ?OnCommandMsgs { get; set; }

    public List<TimerMsg> ?TimerMsgs { get; set; }
}

public class TriggerCondition
{
    public string flag { get; set; } = "convar";
    
    /*
     * operation:
     *   0: disabled (the condition is always true)
     *   1: equal
     *   2: smaller than
     *   3: greater than
     */
    public int op {get; set; } = 0;

    public int value {get; set; } = 0;
}

public class BaseMsg
{
    public string msg { get; set; } = "msg";

    public TriggerCondition ?cond { get; set; }

    public bool ?admin { get; set; } = false;
}

public class OnPlayerConnectMsg : BaseMsg
{
    public float delay { get; set; } = -1;
}

public class OnRoundStartMsg : BaseMsg
{
}

public class OnCommandMsg: BaseMsg
{   
    public required string cmd { set; get; } = "cmd";
}

public class TimerMsg : BaseMsg
{
    public required float timer {set; get;} = 60;              // Time interval between broadcasts (seconds).

}

public class OnAdminConnectMsg : BaseMsg
{
    public float delay { get; set; } = -1;
}

public class MessageManager
{
    public MessageConfig? MsgCfg;

    private string _moduleDirectory;

    public MessageManager(string moduleDirectory)
    {
        _moduleDirectory = moduleDirectory;
        ReadConfig();
    }

    private static string ParseColorInfo(string coloredMsg)
    {
        return coloredMsg
            .Replace("[GREEN]", " " + ChatColors.Green.ToString())
            .Replace("[RED]", " " + ChatColors.Red.ToString())
            .Replace("[YELLOW]", " " + ChatColors.Yellow.ToString())
            .Replace("[BLUE]", " " + ChatColors.Blue.ToString())
            .Replace("[PURPLE]", " " + ChatColors.Purple.ToString())
            .Replace("[ORANGE]", " " + ChatColors.Orange.ToString())
            .Replace("[WHITE]", " " + ChatColors.White.ToString())
            .Replace("[NORMAL]", " " + ChatColors.White.ToString())
            .Replace("[GREY]", " " + ChatColors.Grey.ToString())
            .Replace("[LIGHT_RED]", " " + ChatColors.LightRed.ToString())
            .Replace("[LIGHT_BLUE]", " " + ChatColors.LightBlue.ToString())
            .Replace("[LIGHT_PURPLE]", " " + ChatColors.LightPurple.ToString())
            .Replace("[LIGHT_YELLOW]", ChatColors.LightYellow.ToString())
            .Replace("[DARK_RED]", " " + ChatColors.DarkRed.ToString())
            .Replace("[DARK_BLUE]", " " + ChatColors.DarkBlue.ToString())
            .Replace("[BLUE_GREY]", " " + ChatColors.BlueGrey.ToString())
            .Replace("[OLIVE]", " " + ChatColors.Olive.ToString())
            .Replace("[LIME]", " " + ChatColors.Lime.ToString())
            .Replace("[GOLD]", " " + ChatColors.Gold.ToString())
            .Replace("[SILVER]", " " + ChatColors.Silver.ToString())
            .Replace("[MAGENTA]", " " + ChatColors.Magenta.ToString());
    }

    public void ReadConfig()
    {
        try
        {
            // Read message configs
            string fileName = _moduleDirectory + "/cfg/messages.json";
            string jsonString = File.ReadAllText(fileName);
            MessageConfig messages = JsonSerializer.Deserialize<MessageConfig>(jsonString)!;

            int msgCount = 0;
            if (messages.OnPlayerConnectMsgs != null)
            {
                msgCount += messages.OnPlayerConnectMsgs.Count;

                foreach (var msg in messages.OnPlayerConnectMsgs)
                {
                    msg.msg = ParseColorInfo(msg.msg);
                }
            }

            if (messages.OnAdminConnectMsgs != null)
            {
                msgCount += messages.OnAdminConnectMsgs.Count;

                foreach (var msg in messages.OnAdminConnectMsgs)
                {
                    msg.msg = ParseColorInfo(msg.msg);
                }
            }

            if (messages.OnRoundStartMsgs != null)
            {
                msgCount += messages.OnRoundStartMsgs.Count;

                foreach (var msg in messages.OnRoundStartMsgs)
                {
                    msg.msg = ParseColorInfo(msg.msg);
                }
            }

            if (messages.OnCommandMsgs != null)
            {
                msgCount += messages.OnCommandMsgs.Count;

                foreach (var msg in messages.OnCommandMsgs)
                {
                    msg.msg = ParseColorInfo(msg.msg);
                }
            }

            if (messages.TimerMsgs != null)
            {
                msgCount += messages.TimerMsgs.Count;

                foreach (var msg in messages.TimerMsgs)
                {
                    msg.msg = ParseColorInfo(msg.msg);
                }
            }

            // Success
            Console.WriteLine($"[CS2 Announcement Broadcaster] {msgCount} messages have been loaded.");

            MsgCfg = messages;
        }
        catch (System.Exception)
        {
            Console.WriteLine("[CS2 Announcement Broadcaster] Failed to parse the configuration files.");
            throw;
        }
    }
}