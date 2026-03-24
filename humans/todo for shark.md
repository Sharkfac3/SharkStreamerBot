









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