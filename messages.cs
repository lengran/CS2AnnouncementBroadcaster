namespace CS2AnnouncementBroadcaster;

public class MessageConfig
{
    public OnPlayerConnectMsg[] ?OnPlayerConnectMsgs { get; set; }

    public OnRoundStartMsg[] ?OnRoundStartMsgs { get; set; }

    public OnCommandMsg[] ?OnCommandMsgs { get; set; }
}

public class BaseMsg
{
    public string msg { get; set; }

    public BaseMsg(string msg)
    {
        this.msg = msg;
    }
}

public class OnPlayerConnectMsg : BaseMsg
{
    public OnPlayerConnectMsg(string msg) : base(msg)
    {}
}

public class OnRoundStartMsg : BaseMsg
{
    public OnRoundStartMsg(string msg) : base(msg)
    {}
}

public class OnCommandMsg: BaseMsg
{   
    public string cmd { set; get; }

    public OnCommandMsg(string msg, string cmd) : base(msg)
    {
        this.msg = msg;
        this.cmd = cmd;
    }
}

public class TimerMsg
{
    public string msg { set; get; }

    public float timer {set; get;} = 300;              // Time interval between broadcasts (seconds).

    public TimerMsg(string msg, float timer)
    {
        this.msg = msg;
        this.timer = timer;
    }
}