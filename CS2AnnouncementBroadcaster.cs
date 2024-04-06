using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json;

namespace CS2AnnouncementBroadcaster;
public class CS2AnnouncementBroadcaster : BasePlugin
{
    public override string ModuleName => "CS2 Announcement Broadcaster";

    public override string ModuleVersion => "0.0.2";

    public override string ModuleAuthor => "Lengran";

    public override string ModuleDescription => "A plugin that helps server admins to broadcast announcements to users. https://github.com/lengran/CS2AnnouncementBroadcaster";

    private readonly List<OnPlayerConnectMsg> _onPlayerConnectMsgs = new List<OnPlayerConnectMsg>();

    private readonly List<OnRoundStartMsg> _onRoundStartMsgs = new List<OnRoundStartMsg>();

    public override void Load(bool hotReload)
    {
        ReadConfig();
    }

    private void ReadConfig()
    {
        try
        {
            // Read message configs
            string fileName = ModuleDirectory + "/cfg/messages.json";
            string jsonString = File.ReadAllText(fileName);
            MessageConfig messages = JsonSerializer.Deserialize<MessageConfig>(jsonString)!;

            // Load OnPlayerConnect messages
            if (messages.OnPlayerConnectMsgs != null)
            {
                foreach (var msg in messages.OnPlayerConnectMsgs!)
                {
                    _onPlayerConnectMsgs.Add(new OnPlayerConnectMsg(ParseMsg(msg.msg)));
                }
            }
            
            // Load OnRoundStart messages
            if (messages.OnRoundStartMsgs != null)
            {
                foreach (var msg in messages.OnRoundStartMsgs!)
                {
                    _onRoundStartMsgs.Add(new OnRoundStartMsg(ParseMsg(msg.msg)));
                }
            }

            // Register commands and corresponding messages

            // Success
            Console.WriteLine($"[CS2 Announcement Broadcaster] {_onPlayerConnectMsgs.Count} messages have loaded.");
        }
        catch (System.Exception)
        {
            Console.WriteLine("[CS2 Announcement Broadcaster] Failed to parse the configuration files.");
            throw;
        }
    }

    [ConsoleCommand("css_abreload", "Reload configuration files for the announcement broadcaster plugin.")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/root")]
    public void OnABReloadCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        ReadConfig();
        commandInfo.ReplyToCommand($" {ChatColors.Red}[CS2 Announcement Broadcaster] {ChatColors.White} Configuration of Announcement Broadcaster has been reloaded.");
    }

    [GameEventHandler]
    public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        var player = @event.Userid;
        
        if (!player.IsValid || player.IsBot || player.IsHLTV)
        {
            return HookResult.Continue;
        }

        foreach (var msg in _onPlayerConnectMsgs)
        {
            player.PrintToChat(msg.msg);
        }

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        var players = Utilities.GetPlayers();

        foreach (var tmpPlayer in players)
        {
            if (!tmpPlayer.IsValid || tmpPlayer.IsBot || tmpPlayer.IsHLTV)
            {
                continue;
            }

            foreach (var msg in _onRoundStartMsgs)
            {
                tmpPlayer.PrintToChat(msg.msg);
            }
        }

        return HookResult.Continue;
    }

    private string ParseMsg(string coloredMsg)
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

}