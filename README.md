# LabRats

Educational VR application built with Unity and the XR Interaction Toolkit.

## Table of Contents

- [Controls](#controls)
- [Application Limitations](#application-limitations--known-issues)
- [Puzzle Answers](#puzzle--experiment-answers)
- [References & Credits](#references--credits)
- [Group Member Credits](#group-member-credits)

---

## Controls

### VR Controls (XR Interaction Toolkit)

#### Movement
- Use the joystick/thumbstick to move around the environment
- Turn using snap turn

#### Interaction
- Use controller ray or direct interaction to select UI elements
- **Trigger button**: Select/Activate UI buttons
- **Poke**: Press rat, settings and hint buttons

#### Rat Audio Button
- Press the Rat Button to hear the instructions for the currently active panel
- Press the Rat Button again to stop the audio
- Press it again after stopping to replay the audio from the beginning
- On specific instructional panels, audio will automatically play upon entering the panel

#### Gravity Lab
- Move sliders to change gravity and height

---

## Application Limitations / Known Issues

- **Panel Detection Logic**: The system plays audio based on the currently active panel. If multiple panels are accidentally active at the same time, the first detected panel may play instead of the intended one.

- **Audio Overlap Prevention**: Only one narration clip can play at a time. Playing a new clip stops the previous one.

- **VR UI Interaction Sensitivity**: Sliders may require precise ray positioning depending on controller tracking.

---

## Puzzle / Experiment Answers

### Experiment 1: Gravity Lab
Change each slider at least 5 times and the completion panel will pop up.

---

## References & Credits

### External Assets
- **Audio assets** sourced from:
  - ElevenLabs AI voice over
- **XR Framework**:
  - Unity XR Interaction Toolkit

### Software Used
- Unity
- Maya
- Adobe Audition for editing audio files
- Adobe Photoshop for texture atlas
- Substance Painter for texture atlas

---

## Group Member Credits

### Liu GuangXuan
- Main game flow coding (fixing some bugs, refining flow)
- Main ITD coder
- Gravity lab mechanics and game
- Scene set up for gravity lab
- Some vfx
- Website authentication
- Website UI design

### Keagan (Ng Kiang Hwee)
- Main game flow coding (from classroom scene to experiment)
- Secondary ITD coder
- Database saving
- Authentication
- Prop modelling (Lightbulb)

### Arai Mio Ashley
- Scene set up for corridor and labs
- Main asset modeller for modular assets (corridor, labs)
- Terrain set up and texturing
- Set up of shader graphs and vfx

### Gracie Arianne Peh
- Scene set up for classroom
- Modular asset and prop modelling (Classroom, battery+holder, rat in petri dish and buttons)
- Audio button mechanic
- In game UI set up
- Website database reading
