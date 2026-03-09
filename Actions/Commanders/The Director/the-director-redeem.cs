using System; // Gives us access to built-in .NET types like DateTime.

// Streamer.bot expects a class named CPHInline for inline C# actions.
public class CPHInline
{
    // Execute() is the entry point Streamer.bot calls when this action runs.
    // Returning true tells Streamer.bot the action completed successfully.
    public bool Execute()
    {
        // Start with a safe default empty string in case the trigger has no user.
        string user = string.Empty;

        // Try to read the trigger argument named "user" into the variable above.
        // If the argument is missing, user stays as empty string.
        CPH.TryGetArg("user", out user);

        // Defensive check: if user is null/blank/whitespace, use a friendly fallback.
        if (string.IsNullOrWhiteSpace(user)) return true; // No user to credit, but action still succeeds.

        // Save who redeemed the commander so future messages can credit them.
        CPH.SetGlobalVar("current_the_director", user, false);

        // Explicit success return value for Streamer.bot action flow.
        return true;
    }
}
