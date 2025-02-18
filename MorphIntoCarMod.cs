using System;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;

public class MorphIntoCar : Script
{
    private bool isMorphed = false; // Tracks morph state
    private Vehicle playerVehicle; // Holds the morphed vehicle instance
    private VehicleHash[] carOptions = new VehicleHash[] { VehicleHash.Adder, VehicleHash.Zentorno, VehicleHash.Cheetah }; // Cars to morph into
    private int currentCarIndex = 0; // Current car selection index

    public MorphIntoCar()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.M) // Press 'M' to morph/unmorph
        {
            if (!isMorphed)
                MorphPlayerIntoCar();
            else
                UnmorphPlayer();
        }

        if (e.KeyCode == Keys.N) // Press 'N' to switch car types
        {
            CycleCarType();
        }
    }

    private void OnTick(object sender, EventArgs e)
    {
        if (isMorphed && playerVehicle != null)
        {
            // Keep the vehicle functional if the player is morphed
            if (!playerVehicle.Exists())
            {
                isMorphed = false;
                Game.Player.Character.IsVisible = true;
            }
        }
    }

    private void MorphPlayerIntoCar()
    {
        Ped player = Game.Player.Character;

        // Ensure player is not already in a vehicle
        if (player.IsInVehicle())
        {
            UI.Notify("Exit your vehicle before morphing.");
            return;
        }

        // Spawn the vehicle
        Vector3 spawnPosition = player.Position + player.ForwardVector * 3f; // Spawn in front of player
        playerVehicle = World.CreateVehicle(carOptions[currentCarIndex], spawnPosition);

        if (playerVehicle != null)
        {
            // Attach player to the vehicle
            player.SetIntoVehicle(playerVehicle, VehicleSeat.Driver);
            player.IsVisible = false; // Hide the player model
            isMorphed = true;
            UI.Notify("Morphed into " + carOptions[currentCarIndex].ToString() + "!");
        }
        else
        {
            UI.Notify("Failed to morph. Vehicle creation error.");
        }
    }

    private void UnmorphPlayer()
    {
        if (playerVehicle != null)
        {
            Ped player = Game.Player.Character;

            // Get out of the vehicle
            Vector3 exitPosition = playerVehicle.Position + playerVehicle.RightVector * 2f;
            player.Position = exitPosition;
            player.IsVisible = true; // Show the player model

            // Delete the vehicle
            playerVehicle.Delete();
            playerVehicle = null;
            isMorphed = false;
            UI.Notify("Returned to normal form.");
        }
        else
        {
            UI.Notify("No vehicle to unmorph from.");
        }
    }

    private void CycleCarType()
    {
        currentCarIndex = (currentCarIndex + 1) % carOptions.Length; // Cycle through car options
        UI.Notify("Selected car: " + carOptions[currentCarIndex].ToString());
    }
}