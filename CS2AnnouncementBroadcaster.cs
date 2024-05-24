using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;


namespace CS2AnnouncementBroadcaster;
public class CS2AnnouncementBroadcaster : BasePlugin
{
    public override string ModuleName => "CS2 Announcement Broadcaster";

    public override string ModuleVersion => "0.3.0";

    public override string ModuleAuthor => "Lengran";

    public override string ModuleDescription => "A plugin that helps server admins to broadcast announcements to users. https://github.com/lengran/CS2AnnouncementBroadcaster";

    // private readonly List<OnPlayerConnectMsg> _onPlayerConnectMsgs = new();

    // private readonly List<OnRoundStartMsg> _onRoundStartMsgs = new();

    // private readonly List<OnCommandMsg> _onCommandMsgs = new();
    
    private readonly List<CommandDefinition> _registeredCmds = new();

    // private readonly List<TimerMsg> _timerMsgs = new();

    private readonly List<CounterStrikeSharp.API.Modules.Timers.Timer> _registeredTimers = new();

    private MessageManager? _msgManager;

    private CommandDefinition? reloadCmd; 

    public override void Load(bool hotReload)
    {
        // Load configuration from json
        _msgManager = new MessageManager(ModuleDirectory);
        ParseMessages();

        // Register the reload command
        if (reloadCmd != null)
        {
            CommandManager.RemoveCommand(reloadCmd);
            reloadCmd = null;
        }

        reloadCmd = new CommandDefinition("css_abreload", "", (player, commandInfo) => {
            if (player == null || !AdminManager.PlayerHasPermissions(player, "@css/admin"))
            {
                return;
            }

            _msgManager = new MessageManager(ModuleDirectory);
            ParseMessages();

            commandInfo.ReplyToCommand($" {ChatColors.Red}[CS2 Announcement Broadcaster] {ChatColors.White} Configuration of Announcement Broadcaster has been reloaded.");
        });
        CommandManager.RegisterCommand(reloadCmd);

        base.Load(hotReload);
    }

    public override void Unload(bool hotReload)
    {
        // Unregister reload command
        if (reloadCmd != null)
        {
            CommandManager.RemoveCommand(reloadCmd);
            reloadCmd = null;
        }

        base.Unload(hotReload);
    }

    [GameEventHandler]
    public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        var player = @event.Userid;
        
        if (!player.IsValid || player.IsBot || player.IsHLTV || _msgManager!.MsgCfg!.OnPlayerConnectMsgs == null)
        {
            return HookResult.Continue;
        }

        foreach (var msg in _msgManager.MsgCfg.OnPlayerConnectMsgs)
        {
            player.PrintToChat(msg.msg);
        }

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if (_msgManager!.MsgCfg!.OnRoundStartMsgs == null)
        {
            return HookResult.Continue;
        }

        foreach (var msg in _msgManager.MsgCfg.OnRoundStartMsgs)
        {
            Server.PrintToChatAll(msg.msg);
        }

        return HookResult.Continue;
    }

    private void RegisterCommand(OnCommandMsg msg)
    {
        var command = new CommandDefinition("css_" + msg.cmd, "A command registered automatically by Announcement Broadcaster.", (player, commandInfo) => 
        {
            commandInfo.ReplyToCommand(msg.msg);
        });

        _registeredCmds.Add(command);
        CommandManager.RegisterCommand(command);
    }

    private void UnregisterCommand()
    {
        foreach (var cmd in _registeredCmds)
        {
            CommandManager.RemoveCommand(cmd);
        }
        
        _registeredCmds.Clear();
    }

    private void RegisterTimer(TimerMsg msg)
    {
        var timer = AddTimer(msg.timer, () => {
            // var players = Utilities.GetPlayers();

            // foreach (var tmpPlayer in players)
            // {
            //     if (!tmpPlayer.IsValid || tmpPlayer.IsBot || tmpPlayer.IsHLTV)
            //     {
            //         continue;
            //     }

            //     tmpPlayer.PrintToChat(msg.msg);
            // }
            Server.PrintToChatAll(msg.msg);
        }, CounterStrikeSharp.API.Modules.Timers.TimerFlags.REPEAT);

        _registeredTimers.Add(timer);
    }

    private void UnregisterTimer()
    {
        foreach (var timer in _registeredTimers)
        {
            timer.Kill();
        }

    }

    private void ParseMessages()
    {
        if (_msgManager!.MsgCfg == null)
        {
            Console.WriteLine("[CS2 Announcement Broadcaster] Failed to parse the configuration files.");
            return;
        }

        try
        {
            // Register command triggered messages
            UnregisterCommand();
            // _onCommandMsgs.Clear();
            if (_msgManager.MsgCfg.OnCommandMsgs != null)
            {
                foreach (var msg in _msgManager.MsgCfg.OnCommandMsgs!)
                {
                    RegisterCommand(msg);
                }
            }

            // Load timer triggered messages
            UnregisterTimer();
            // _timerMsgs.Clear();
            if (_msgManager.MsgCfg.TimerMsgs != null)
            {
                foreach(var msg in _msgManager.MsgCfg.TimerMsgs!)
                {
                    // TimerMsg timerMsg = new TimerMsg(ParseMsg(msg.msg), msg.timer);
                    RegisterTimer(msg);
                    // _timerMsgs.Add(timerMsg);
                }
            }

            // Success
            Console.WriteLine($"[CS2 Announcement Broadcaster] Loaded configuration have been successfully parsed.");
        }
        catch (System.Exception)
        {
            Console.WriteLine("[CS2 Announcement Broadcaster] Failed to parse the configuration files.");
            throw;
        }
    }
}