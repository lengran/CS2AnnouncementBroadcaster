namespace CS2AnnouncementBroadcaster;

public class MessageConfig
{
    public OnPlayerConnectMsg[] OnPlayerConnectMsgs { get; set; }
}

public class OnPlayerConnectMsg
{
    public string msg { get; set; }

    public OnPlayerConnectMsg(string msg)
    {
        this.msg = msg;
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