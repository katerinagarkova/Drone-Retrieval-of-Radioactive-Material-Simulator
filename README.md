# Drone-Retrieval-of-Radioactive-Material-Simulator

UAV Radiation Search & Retrieval Simulation

This project is a high-fidelity simulation developed in Unity to test autonomous drone behaviors for locating and retrieving hazardous radioactive materials. By leveraging Cesium for Unity, the simulation utilizes real-world geospatial data to provide a safe, risk-free environment for testing pathfinding algorithms, battery consumption models, and radiation sensor logic.

## Project Overview

Locating "orphaned" radioactive sources is a dangerous task for human operators. UAVs provide a safer alternative, but they face significant constraints regarding battery life and sensor weight. This simulation allows researchers to:

    Simulate Radiation Physics: Uses the Inverse Square Law to emulate sensor readings as the drone approaches a source.

    Analyze Battery Lifecycle: Tracks energy depletion based on flight time and distance.

    Test Pathfinding: Validates shortest-path algorithms and autonomous navigation over complex terrain.

## Features
1. Pre-Mission Configuration

Users can set mission parameters before launching the simulation:

    Geospatial Mapping: Select start and target locations directly on the 3D terrain.

    Resource Management: Configure starting battery percentages via a slider interface.

    Visual Markers: Interactive pins indicate the selected drone deployment and source locations.

2. In-Mission Controls & Telemetry

Once the mission begins, the drone autonomously navigates to the source.

    Dynamic Follow Camera: A custom camera system follows the drone with scroll-to-zoom and right-click orbit functionality.

    Time Manipulation: Features to Pause, Fast-Forward (2x), and Rewind (step back 5 seconds) to analyze specific flight behaviors.

    Real-time HUD: Displays live battery percentage, distance to target, and calculated radiation levels (mSv).

3. Automated Outcome Logic

The simulation ends automatically based on performance:

    Mission Successful: Triggered if the drone reaches the source (within 5m) with remaining battery.

    Mission Unsuccessful: Triggered if the battery hits 0% before reaching the target.

    Post-Mission Analysis: A dedicated results panel offers options to Restart (same parameters) or start a New Mission (reset setup).

## Getting Started
Prerequisites

    Unity 2021.3 LTS or newer.

    Cesium for Unity package (installed via Package Manager).

    TextMeshPro for UI components.

Installation

    Clone this repository to your local machine.

    Open the project in the Unity Editor.

    Ensure the Cesium World Terrain is correctly set up with your Ion API Key.

    Open the MainSimulation scene.

Running the Simulation

    Press Play in the Unity Editor.

    Use the Pre-Mission UI to set your battery and click the map to place your drone and source.

    Click Start Mission to watch the autonomous search.

    Use the scroll wheel to zoom in on the drone during flight.

## System Architecture

    DroneController.cs: Manages physics-based flight, waypoint navigation, and altitude maintenance via raycasting.

    MissionManager.cs: The central state machine handling mission logic, battery drain, success/failure conditions, and time manipulation.

    PreMissionController.cs: Manages the setup UI and the instantiation of simulation prefabs.

    CameraFollow.cs: A custom, non-package dependent camera system for smooth drone tracking and zoom.
