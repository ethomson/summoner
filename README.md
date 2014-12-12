Summon̈er
========

A simple Windows application to notify you of mentions in chat rooms
like Campfire or TFS Team Rooms, so that you will never miss an
important conversation just because you had your browser closed.
It can also make you look omniscient, eerily spooking your coworkers.

Summon̈er scans chat rooms ("clients") for keywords and then fires
notifications when it sees them.

Clients
-------

  - [Campfire](https://campfirenow.com/)
  - [TFS Team Rooms](http://tfs.visualstudio.com/en-us/learn/collaborate-in-a-team-room.aspx)

Notifications
-------------

  - Console (Standard Output)
  - [Growl for Windows](http://www.growlforwindows.com/gfw/)
  - SMS (via [Bandwidth Developer Platform](https://catapult.inetwork.com/pages/login.jsf))

Usage
-----

Simply running `summoner.exe` will start the program.

Command line options:

- `/config:<filename>`: specifies the configuration file (default: `summoner.config` in the same location as the `.exe`

Configuration
-------------

Configuration is in the form of a JSON file.

### Configuration Options

- `poll_interval`: the number of seconds to pause before polling clients
- `clients`: an array of client configuration definitions
- `notifications`: an array of notification definitions

### Client Configuration

Client configuration is specified as key/value pairs.  The `type` 
configuration is mandatory.  Other configuration values will be
passed to the client.

#### Campfire

- `type`: must be `campfire`
- `uri`: URI to the Campfire account
- `api-token`: API token for authentication
- `room`: name of the room to monitor

#### TFS Team Room

- `type`: must be `tfs`
- `uri`: URI to the TFS Project Collection
- `username`: username to authenticate with, configured with Alternate Credentials for Team Foundation Service
- `password`: password to authenticate with
- `room`: name of the room to monitor

### Notification Configuration

Notification configuration is specified as key/value pairs.  The
`type` configuration is mandatory.  Other configuration values will be
passed to the client.

Any notification may have the following configuration specified,
in addition to the notification-specific configuration:

- `contains`: limits the notification to message that contain the given text

#### Console Notification

- `type`: must be `console`

#### Growl Notification

- `type`: must be `growl`

#### SMS Notification

- `type`: must be `sms`
- `userid`: user information for the Catapult API
- `api-token`: API token for authentication
- `api-secret`: API secret for authentication
- `from`: sender phone number (in international format)
- `to`: recipient phone number (in international format)

#### Example

    {
        "poll_interval": 5,
        
        "clients": [
            {
                "type": "campfire",
                "uri": "https://mycampfire.campfirenow.com/",
                "api-token": "secret",
                "room": "Discussion Room"
            },
            
            {
                "type": "tfs",
                "uri": "https://mytfs.visualstudio.com/DefaultCollection",
                "username": "alternate_credential_username",
                "password": "alternate_credential_password",
                "room": "My Team Room"
            },
        ],
        
        "notifications": [
            { "type": "console" },
            { "type": "growl" },
            {
                "type": "sms",
                "contains": "urgent",
                "userid": "userid",
                "api-token": "token",
                "api-secret": "secret",
                "from": "+19195551212",
                "to": "+13125558989",
            }
        ]
    }

Notes
-----

This is super beta and may not work for you.  There would be much
work to make this production-ready, which is not a high priority.

Summoner *monitors* rooms, it does not *join* them.  It (you) will
not be visible in the room list.  This lends to your aura of mystery.

License
----

MIT

