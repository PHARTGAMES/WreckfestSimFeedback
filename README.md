# WreckfestSimFeedback


ABOUT
=====
Wreckfest Plugin for motion telemetry through SimFeedback.

https://opensfx.com/

https://github.com/SimFeedback/SimFeedback-AC-Servo


RELEASE NOTES
=============
v1.0 - First release.

v1.1 - Increased sample rate, improves linear acceleration response.

v1.2 - Added player selection combo box to select player in multiplayer.

INSTALLATION INSTRUCTIONS 
=========================

1. Ensure you have the .NET Framework v4.8 runtime installed.

Download: https://dotnet.microsoft.com/download/dotnet-framework/net48

2. Download and extract the latest release zip of WreckfestSimFeedback.

Download: https://github.com/PHARTGAMES/WreckfestSimFeedback/tree/master/Releases

3. Copy the contents of the SimFeedbackPlugin folder within the WreckfestSimFeedback .zip into your SimFeedback root folder.


USAGE INSTRUCTIONS 
==================

1. Launch Wreckfest (32bit). This plugin ONLY works with the 32bit version. The 64bit version may be supported in a future release.

2. In SimFeedback, activate a Wreckfest profile and press Start.

3. A new dialog should show titled "Wreckfest Telemetry".

4. Inside Wreckfest, configure a new event and start it to get to the the event screen which shows your selected car as well as event, standing, difficulty and tune options.

5. While on the event screen, press the "Initialize" button on the "Wreckfest Telemetry" dialog.

6. Wait for the progress bar to finish; if the process works you should see "Success!" in the centre of the dialog.

7. From this point you should have telemetry until you quit the event. When you quit the event telemetry will be lost. Repeat steps 4 through 7 for each new event.

8. For multiplayer, look at the list of players in the lobby, first player is 00, last player is 23, 9th player will be 08. Find your name and count from the top. When the event starts, change the Choose Player combo box to match the number of your player and click Initialize.


AUTHOR
======

PEZZALUCIFER


SUPPORT
=======

Support available through SimFeedback owner's discord

https://opensfx.com/simfeedback-setup-and-tuning/#modes
