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

### How to write a configuration file? 

You can start from the example file provided in the *cfg/messages.json.example*. Detailed usage will be provided in [next section of this README file](https://github.com/lengran/CS2AnnouncementBroadcaster?tab=readme-ov-file#type-of-messages).

The overall structure of a configuration file will look like this.

```json
{
    "Type of messages": [
        {
            "msg": "The body of a message.",
            "properties": "Value"
        },
        {
            "msg": "The body of a message.",
            "properties": "Value"
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

Coloring is supported. You can put a simple square bracketed color in the message. For example,

```json
{
    "msg": "To enter [GREEN]prefire practice[NORMAL] mode, please type [RED]!prefire[NORMAL]."
}
```

Available colors: 

    [GREEN], [RED], [YELLOW], [BLUE], [PURPLE], [ORANGE], [WHITE], [NORMAL], [GREY], [LIGHT_RED], [LIGHT_BLUE], [LIGHT_PURPLE], [LIGHT_YELLOW], [DARK_RED], [DARK_BLUE], [BLUE_GREY], [OLIVE], [LIME], [GOLD], [SILVER], [MAGENTA].


### Type of messages

- OnPlayerConnectMessages
- OnRoundStartMessages

    ```json
    {
        "msg": "Messages sent to players when they connect to the server."
    }
    ```

## Development

### TODO

- Pass in parameters to messages. For example, "{PlayerName} has just connected. Welcome!"
- Command triggered message.
- Timer triggered messages.
- Broadcast group. Add the ability to label players and send them message according to their labels.