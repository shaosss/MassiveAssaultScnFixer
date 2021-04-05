# MassiveAssaultScnFixer
Fixer for scenarios in Massive Assault game

For some reason the order in ids of units in scenario file is matter and sometimes causing some of them to not appear on map. This could later cause the message "Game appear to be broken! Please restore it!"
Though it is not yet verified for all scenarios/campaigns this is fixed the issue with this message and missing Naval transports in Batle for Paradise, Step 4.

# How to use
Just run the fixer with game folder as a parameter. It will fix all unit ids for all scenratios and create a backup folder for scenarious called (scenarios_backup) in the game folder, so if you have any issues with the scenarios you can restore them.
For some files you can see the message like "Unit with id 4103 is not mapped!". This is normal situation. For some reason the files seems to be human written, so there are some units commented or removed from scn file, but their ids are still in msc file.
