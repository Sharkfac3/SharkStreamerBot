









-------------- notes from focus loop after 1 prompt
 ### Manual setup steps

 1. Create a new Streamer.bot action group for the loop, e.g. Rest Focus Loop.
 2. Create these four Streamer.bot timers with exact names:
     - Rest Focus - Pre Rest
     - Rest Focus - Rest
     - Rest Focus - Pre Focus
     - Rest Focus - Focus
 3. Wire timer-end triggers:
     - Rest Focus - Pre Rest → rest-focus-pre-rest-end.cs
     - Rest Focus - Rest → rest-focus-rest-end.cs
     - Rest Focus - Pre Focus → rest-focus-pre-focus-end.cs
     - Rest Focus - Focus → rest-focus-focus-end.cs
 4. Wire your voice command to rest-focus-loop-start.cs.
 5. Create/wire chat command actions:
     - !castrest → Actions/Commanders/Water Wizard/water-wizard-castrest.cs
     - !generalfocus → Actions/Commanders/Captain Stretch/captain-stretch-generalfocus.cs
 6. Replace placeholder Mix It Up IDs in code:
     - REPLACE_WITH_WIZARDS_REST_COMMAND_ID
           - Actions/Rest Focus Loop/rest-focus-pre-rest-end.cs
           - Actions/Commanders/Water Wizard/water-wizard-castrest.cs
     - REPLACE_WITH_CAPTAINS_FOCUS_WARNING_COMMAND_ID
           - Actions/Rest Focus Loop/rest-focus-rest-end.cs
     - REPLACE_WITH_CAPTAINS_FOCUS_COMMAND_ID
           - Actions/Rest Focus Loop/rest-focus-pre-focus-end.cs
           - Actions/Commanders/Captain Stretch/captain-stretch-generalfocus.cs
     - REPLACE_WITH_FOCUS_TIMER_END_COMMAND_ID
           - Actions/Rest Focus Loop/rest-focus-focus-end.cs
 7. Important: these scripts use CPH.SetTimerInterval(string, int) for dynamic durations. That method is still marked unconfirmed in repo docs. Test it on
 your Streamer.bot build before relying on it live.
 8. Because new globals were added, stream-start.cs and Actions/SHARED-CONSTANTS.md were already updated for you.





  Here’s the current command → script mapping I found under Actions/.

 Chat commands

 ### Commanders

 ┌────────────────────────────────────────────────────────────┬────────────────────────────────────────────────────────────────────┐
 │ Command                                                    │ Script                                                             │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !commanderhelp (example name; operator chooses exact name) │ Actions/Commanders/commander-help.cs                               │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !thank                                                     │ Actions/Commanders/Captain Stretch/captain-stretch-thank.cs        │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !stretch                                                   │ Actions/Commanders/Captain Stretch/captain-stretch-stretch.cs      │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !generalfocus                                              │ Actions/Commanders/Captain Stretch/captain-stretch-generalfocus.cs │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !shrimp                                                    │ Actions/Commanders/Captain Stretch/captain-stretch-shrimp.cs       │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !award                                                     │ Actions/Commanders/The Director/the-director-award.cs              │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !checkchat                                                 │ Actions/Commanders/The Director/the-director-checkchat.cs          │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !primary                                                   │ Actions/Commanders/The Director/the-director-primary.cs            │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !secondary                                                 │ Actions/Commanders/The Director/the-director-secondary.cs          │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !toad                                                      │ Actions/Commanders/The Director/the-director-toad.cs               │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !hail                                                      │ Actions/Commanders/Water Wizard/water-wizard-hail.cs               │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !hydrate                                                   │ Actions/Commanders/Water Wizard/wizard-hydrate.cs                  │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !castrest                                                  │ Actions/Commanders/Water Wizard/water-wizard-castrest.cs           │
 ├────────────────────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────┤
 │ !orb                                                       │ Actions/Commanders/Water Wizard/water-wizard-orb.cs                │
 └────────────────────────────────────────────────────────────┴────────────────────────────────────────────────────────────────────┘

 ### Squad

 ┌───────────┬───────────────────────────────────────┐
 │ Command   │ Script                                │
 ├───────────┼───────────────────────────────────────┤
 │ !offering │ Actions/Squad/offering.cs             │
 ├───────────┼───────────────────────────────────────┤
 │ !pedro    │ Actions/Squad/Pedro/pedro-main.cs     │
 ├───────────┼───────────────────────────────────────┤
 │ !rebel    │ Actions/Squad/Clone/clone-position.cs │
 └───────────┴───────────────────────────────────────┘

 Notes:
 - Pedro also uses Actions/Squad/Pedro/pedro-call.cs for normal chat messages containing pedro during the active window.
 - Duck does not use a !duck command currently; it listens to normal chat for quack via Actions/Squad/Duck/duck-call.cs.

 ### LotAT

 ┌─────────┬──────────────────────────────────┐
 │ Command │ Script                           │
 ├─────────┼──────────────────────────────────┤
 │ !join   │ Actions/LotAT/lotat-join.cs      │
 ├─────────┼──────────────────────────────────┤
 │ !roll   │ Actions/LotAT/lotat-dice-roll.cs │
 └─────────┴──────────────────────────────────┘

 Shared LotAT command routers:

 #### Commander-window commands → one shared script

 All of these route to:
 Actions/LotAT/lotat-commander-input.cs

 - !stretch
 - !shrimp
 - !hydrate
 - !orb
 - !checkchat
 - !toad

 #### Decision-vote commands → one shared script

 All of these route to:
 Actions/LotAT/lotat-decision-input.cs

 - !scan
 - !target
 - !analyze
 - !reroute
 - !deploy
 - !contain
 - !inspect
 - !drink
 - !simulate

 ### Overlay / utility

 ┌──────────────┬─────────────────────────────────┐
 │ Command      │ Script                          │
 ├──────────────┼─────────────────────────────────┤
 │ !testoverlay │ Actions/Overlay/test-overlay.cs │
 └──────────────┴─────────────────────────────────┘

 ### Destroyer overlay game

 ┌──────────────────────────────┬──────────────────────────────────────┐
 │ Command                      │ Script                               │
 ├──────────────────────────────┼──────────────────────────────────────┤
 │ !destroyer                   │ Actions/Destroyer/destroyer-spawn.cs │
 ├──────────────────────────────┼──────────────────────────────────────┤
 │ !up / !down / !left / !right │ Actions/Destroyer/destroyer-move.cs  │
 └──────────────────────────────┴──────────────────────────────────────┘

 ────────────────────────────────────────────────────────────────────────────────

 Voice-command-driven actions

 These are mapped to Streamer.bot voice-command actions, but the spoken phrase is not specified in the repo. Only the action/script mapping exists.

 ┌──────────────────────────────────────────────┬──────────────────────────────────────────────┐
 │ Action script                                │ What it does                                 │
 ├──────────────────────────────────────────────┼──────────────────────────────────────────────┤
 │ Actions/Voice Commands/mode-garage.cs        │ sets stream_mode = garage                    │
 ├──────────────────────────────────────────────┼──────────────────────────────────────────────┤
 │ Actions/Voice Commands/mode-workspace.cs     │ sets stream_mode = workspace                 │
 ├──────────────────────────────────────────────┼──────────────────────────────────────────────┤
 │ Actions/Voice Commands/mode-gamer.cs         │ sets stream_mode = gamer                     │
 ├──────────────────────────────────────────────┼──────────────────────────────────────────────┤
 │ Actions/Voice Commands/scene-chat.cs         │ switches to mode-specific Chat scene         │
 ├──────────────────────────────────────────────┼──────────────────────────────────────────────┤
 │ Actions/Voice Commands/scene-main.cs         │ switches to mode-specific Main scene         │
 ├──────────────────────────────────────────────┼──────────────────────────────────────────────┤
 │ Actions/Voice Commands/scene-housekeeping.cs │ switches to mode-specific Housekeeping scene │
 ├──────────────────────────────────────────────┼──────────────────────────────────────────────┤
 │ Actions/Voice Commands/scene-dance.cs        │ switches to mode-specific Disco Party scene  │
 └──────────────────────────────────────────────┴──────────────────────────────────────────────┘

 So for these, the repo tells me which script belongs to which voice action, but not the literal spoken command phrase you configured in Streamer.bot UI.

 ────────────────────────────────────────────────────────────────────────────────

 Not command-mapped

 A lot of scripts under Actions/ are event, timer, channel point, or subscription handlers rather than chat commands. Examples:
 - Actions/Twitch Core Integrations/*.cs
 - Actions/Twitch Bits Integrations/*.cs
 - Actions/Twitch Channel Points/*.cs
 - Actions/Twitch Hype Train/*.cs
 - timer handlers like duck-resolve.cs, clone-volley.cs, lotat-join-timeout.cs

 ────────────────────────────────────────────────────────────────────────────────

 If you want, I can do a second pass and turn this into a clean operator cheat sheet by feature folder, or a “command → Streamer.bot action group → script” wiring
 table.