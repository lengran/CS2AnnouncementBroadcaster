using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;

namespace CS2AnnouncementBroadcaster;

public class CS2AnnouncementBroadcaster : BasePlugin
{
    public override string ModuleName => "CS2 Announcement Broadcaster";

    public override string ModuleVersion => "0.5.0";

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

    // Fake Convars
    public FakeConVar<int> IntFlag1 = new FakeConVar<int>("CS2AB_flag_1", "A fake convar registered by CS2 Announcement Broadcaster.", 0);
    public FakeConVar<int> IntFlag2 = new FakeConVar<int>("CS2AB_flag_2", "A fake convar registered by CS2 Announcement Broadcaster.", 0);
    public FakeConVar<int> IntFlag3 = new FakeConVar<int>("CS2AB_flag_3", "A fake convar registered by CS2 Announcement Broadcaster.", 0);
    public FakeConVar<int> IntFlag4 = new FakeConVar<int>("CS2AB_flag_4", "A fake convar registered by CS2 Announcement Broadcaster.", 0);
    public FakeConVar<int> IntFlag5 = new FakeConVar<int>("CS2AB_flag_5", "A fake convar registered by CS2 Announcement Broadcaster.", 0);

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
            if (player != null && !AdminManager.PlayerHasPermissions(player, "@css/admin"))
            {
                return;
            }

            _msgManager.ReadConfig();
            ParseMessages();

            if (player != null)
            {
                commandInfo.ReplyToCommand($" {ChatColors.Red}[CS2 Announcement Broadcaster] {ChatColors.White} Configuration of Announcement Broadcaster has been reloaded.");
            }
            Console.WriteLine($"[CS2 Announcement Broadcaster] Configuration of Announcement Broadcaster has been reloaded.");
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
        
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)
        {
            return HookResult.Continue;
        }

        if (_msgManager!.MsgCfg!.OnPlayerConnectMsgs != null)
        {
            foreach (var msg in _msgManager.MsgCfg.OnPlayerConnectMsgs)
            {
                if (CheckCond(msg) && CheckAdmin(msg, player))
                {
                    if (msg.delay > 0)
                    {
                        AddTimer(msg.delay, () => player.PrintToChat(msg.msg));
                    }
                    else
                    {
                        player.PrintToChat(msg.msg);
                    }
                }
            }
        }
        
        // deprecated. use the admin flag instead.
        if (AdminManager.PlayerHasPermissions(player, "@css/admin") && _msgManager!.MsgCfg!.OnAdminConnectMsgs != null)
        {
            foreach (var msg in _msgManager.MsgCfg.OnAdminConnectMsgs)
            {
                if (CheckCond(msg))
                {
                    if (msg.delay > 0)
                    {
                        AddTimer(msg.delay, () => player.PrintToChat(msg.msg));
                    }
                    else
                    {
                        player.PrintToChat(msg.msg);
                    }
                }
            }
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
            if (CheckCond(msg))
            {
                // Line 210 is copied from here. If any changes are made, sync to line 210 please.
                // for none-admin messages
                if (msg.admin == false)
                {
                    Server.PrintToChatAll(msg.msg);
                }
                else
                {
                    // Iterate through all human players and send the message only to admins.
                    var players = Utilities.GetPlayers();
                    foreach (var player in players)
                    {
                        if (player == null || player.IsBot || player.IsHLTV)
                        {
                            continue;
                        }

                        if (CheckAdmin(msg, player))
                        {
                            player.PrintToChat(msg.msg);
                        }
                    }
                }
            }
        }

        return HookResult.Continue;
    }

    private void RegisterCommand(OnCommandMsg msg)
    {
        var command = new CommandDefinition("css_" + msg.cmd, "A command registered automatically by Announcement Broadcaster.", (player, commandInfo) => 
        {
            if (CheckCond(msg) && CheckAdmin(msg, player))
            {
                commandInfo.ReplyToCommand(msg.msg);
            }
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
            if (CheckCond(msg))
            {
                // Copied from line 152
                // for none-admin messages
                if (msg.admin == false)
                {
                    Server.PrintToChatAll(msg.msg);
                }
                else
                {
                    // Iterate through all human players and send the message only to admins.
                    var players = Utilities.GetPlayers();
                    foreach (var player in players)
                    {
                        if (player == null || player.IsBot || player.IsHLTV)
                        {
                            continue;
                        }

                        if (CheckAdmin(msg, player))
                        {
                            player.PrintToChat(msg.msg);
                        }
                    }
                }
            }
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
        // List<string> fakeConvars = new List<string>();

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
                    RegisterTimer(msg);
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

    private bool CheckCond(BaseMsg msg)
    {
        // Disabled ones always return true
        if (msg.cond == null || msg.cond.op == 0)
        {
            return true;
        }

        // Find the convar or fake convar
        int tmpValue;
        switch (msg.cond.flag)
        {
            case "CS2AB_flag_1":
                tmpValue = IntFlag1.Value;
                break;
            case "CS2AB_flag_2":
                tmpValue = IntFlag2.Value;
                break;
            case "CS2AB_flag_3":
                tmpValue = IntFlag4.Value;
                break;
            case "CS2AB_flag_4":
                tmpValue = IntFlag4.Value;
                break;
            case "CS2AB_flag_5":
                tmpValue = IntFlag5.Value;
                break;
            default:
                Console.WriteLine($"[CS2 Announcement Broadcaster] Flag {msg.cond.flag} is illegle. Will return false.");
                return false;
        }

        // Evaluate the value
        // Console.WriteLine($"[CS2 Announcement Broadcaster] DEBUG: {msg.cond.flag} = {tmpValue}.");
        switch (msg.cond.op)
        {
            case 1:
                return tmpValue == msg.cond.value;
            case 2:
                return tmpValue < msg.cond.value;
            case 3:
                return tmpValue > msg.cond.value;
            default:
                Console.WriteLine($"[CS2 Announcement Broadcaster] Operation assigned to (fake) convar {msg.cond.flag} is illegle. Will return false.");
                return false;
        }
    }

    private bool CheckAdmin(BaseMsg msg, CCSPlayerController ?player)
    {
        if (msg.admin == false)
        {
            return true;
        }

        if (player == null || AdminManager.PlayerHasPermissions(player, "@css/admin"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}