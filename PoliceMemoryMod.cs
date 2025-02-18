using System;
using GTA;
using GTA.Native;
using GTA.Math;
using System.Windows.Forms; // For key detection.

public class PoliceMemoryMod : Script
{
    private Vehicle lastKnownVehicle; // Store the player's last known vehicle.
    private Vector3 lastKnownPosition; // Store the player's last known position.
    private bool vehicleFlagged = false; // Whether the vehicle is flagged by the police.
    private bool modEnabled = true; // Mod toggle state.
    private int cooldownTime = 60000; // 1-minute cooldown to reset the vehicle flag.

    public PoliceMemoryMod()
    {
        // Hook into game events.
        Tick += OnTick;
        KeyDown += OnKeyDown;
        Aborted += OnAborted;
    }

    private void OnAborted(object sender, EventArgs e)
    {
        // Cleanup on script termination.
        UI.Notify("Police Memory Mod has been unloaded.");
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        // Toggle the mod on or off with the F9 key.
        if (e.KeyCode == Keys.F9)
        {
            modEnabled = !modEnabled;
            UI.Notify(modEnabled ? "~g~Mod Enabled!" : "~r~Mod Disabled!");
        }
    }

    private void OnTick(object sender, EventArgs e)
    {
        if (!modEnabled)
        {
            // If the mod is disabled, do nothing.
            return;
        }

        // Get the player character and their vehicle.
        Ped player = Game.Player.Character;
        Vehicle currentVehicle = player.CurrentVehicle;

        // Track player evasion status.
        if (Game.Player.WantedLevel == 0 && lastKnownVehicle != null && vehicleFlagged)
        {
            // If the player escapes and there was a flagged vehicle, set a cooldown.
            StartCooldown();
        }

        // If the player is in a flagged vehicle, reapply the wanted level.
        if (vehicleFlagged && currentVehicle != null && currentVehicle == lastKnownVehicle)
        {
            Game.Player.WantedLevel = 2; // Apply a wanted level if the player re-enters the remembered vehicle.
            UI.ShowSubtitle("~r~Police recognize your vehicle!");
        }

        // If the player commits a crime, remember their vehicle and position.
        if (Game.Player.WantedLevel > 0)
        {
            lastKnownPosition = player.Position;
            lastKnownVehicle = currentVehicle;
            vehicleFlagged = true;

            if (lastKnownVehicle != null)
            {
                UI.ShowSubtitle($"~o~Police flagged your vehicle: {lastKnownVehicle.DisplayName}");
            }
        }
    }

    private async void StartCooldown()
    {
        // Wait for the cooldown period to expire before resetting the flagged vehicle.
        await BaseScript.Delay(cooldownTime);
        vehicleFlagged = false;
        lastKnownVehicle = null;
        UI.ShowSubtitle("~g~Police have forgotten about your vehicle.");
    }
}
