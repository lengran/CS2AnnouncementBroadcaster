# CS2 Announcement Broadcaster

A plugin that helps server admins to broadcast announcements to users.

## Get started

### Requirement

- CounterStrikeSharp

### Installation

Download [the latest release files](https://github.com/lengran/CS2AnnouncementBroadcaster/releases) and extract all files into "**game/csgo/addons/counterstrikesharp/plugins/CS2AnnouncementBroadcaster/**".

To install the latest version of CounterStrikeSharp, please refer to this [guide](https://docs.cssharp.dev/docs/guides/getting-started.html).

## How to use

### How does the plugin work?

The plugin reads the configuration files in the **cfg** directory to load the messages and broadcast them accordingly.

The plugin will automatically load the configuration files when server starts. Admins (@css/root or @css/admin) could also use the command "**css_abreload**" or type "**!abreload**" to mannually reload configuration.

### How to write a configuration file? 

You can start from the example file provided in the *cfg/messages.json.example*. Detailed usage will be provided in [next section of this README file](https://github.com/lengran/CS2AnnouncementBroadcaster?tab=readme-ov-file#type-of-messages).

The overall structure of a configuration file will look like this. 

```json
{
    "Type of messages": [
        {
            "msg": "The body of a message.",
            "properties": "Value",
            "cond": {
                "flag": "CS2AB_flag_1",
                "op": 1,
                "value": 10                
            }
        },
        {
            "msg": "The body of a message.",
            "properties": "Value",
            "cond": {
                "flag": "CS2AB_flag_5",
                "op": 2,
                "value": 10                
            }
        }
    ],
    "Type of messages": [
        {
            "msg": "The body of a message.",
            "properties": "Value"
        },
        {
            "msg": "The body of a message.",
            "properties": "Value"
        }
    ]
}
```

The **cond** section is optional. It allows admins to set a pre-defined condition that can enable or disable a message from being broadcasted on-the-fly. There are 5 available flags, namely *CS2AB_flag_1, CS2AB_flag_2, CS2AB_flag_3, CS2AB_flag_4, CS2AB_flag_5*. Each of them can be set in the same way of setting an integer convar. The following operations are supported in the definition of a **cond**. Default values of these flags are 0.

- Operations:
  - 0: disabled (the condition is always true)
  - 1: equal to
  - 2: smaller than
  - 3: greater than


Coloring is supported. You can put a simple square bracketed color in the message. For example,

```json
{
    "msg": "To enter [GREEN]prefire practice[NORMAL] mode, please type [RED]!prefire[NORMAL]."
}
```

Available colors: 

    [GREEN], [RED], [YELLOW], [BLUE], [PURPLE], [ORANGE], [WHITE], [NORMAL], [GREY], [LIGHT_RED], [LIGHT_BLUE], [LIGHT_PURPLE], [LIGHT_YELLOW], [DARK_RED], [DARK_BLUE], [BLUE_GREY], [OLIVE], [LIME], [GOLD], [SILVER], [MAGENTA].


### Type of messages

- OnPlayerConnectMsgs

    ```json
    {
        "msg": "The message to sent."
    }
    ```

- OnRoundStartMsgs

    ```json
    {
        "msg": "The message to sent."
    }
    ```
- OnCommandMsgs
    
    cmd is the command used to trigger the message. You can call it by either "!*command*" in chatbox or "css_*command*" in the in-game console.

    ```json
    {
        "msg": "The message to sent.",
        "cmd": "command"
    }
    ```

- TimerMsgs

    Timer triggered messages. You can specify a time interval (float, seconds), and the plugin will broadcast the message to all players according to that.

    ```json
    {
        "msg": "A timer triggered message.",
        "timer": 60.0
    }
    ```

## Development

### TODO

- Pass in parameters to messages. For example, "{PlayerName} has just connected. Welcome!"
- Broadcast group. Add the ability to label players and send them message according to their labels.