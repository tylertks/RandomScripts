#Restart Script
#This script depends on tmux, and that the minecraft server be running in a tmux session named "mineserver"
#This script also relies on having a shell script named ./launch.sh to launch the server
#To start the tmux session, just open a terminal and type 'tmux new-session -s mineserver
#It is adviced to CD into the server directory and use ./launch.sh to start the server initially
#Set this script up as a cron job with crontab -e
#Entries added in the following format:
# * * * * *  command to execute
# | | | | |
# | | | | |
# | | | | |------ day of week (0 - 6) (0 to 6 are Sunday to Saturday, 7 is Sunday, the same as 0)
# | | | |------------- month (1 - 12)
# | | |--------------------- day of month (1 - 31)
# | |------------------------------ hour (0 - 23)
# |------------------------------------- min (0 - 59)
#
# e.g.
# 0 5 * * * /directory/to/autorestart.sh
# Would run the restart script every day at 5am

go='tmux send-keys -t mineserver'
today=$(date +"%Y-%m-%d %T")
#10 second countdown timer
timeleft=10
#Directory of the Minecraft Server. Use the ABSOLUTE path, ex /home/username/directory/for/server not ~/directory/for/server
serverDir=""
#Name of the world and backkup folders. These do not need to be changed.
worldLoc="world"
backupLoc="world-backups"
serverStart="./launch.sh"

#Server Restart Countdown
$go "say SERVER IS RESTARTING IN 5 MINUTES" C-m
sleep 180s
$go "say SERVER IS RESTARTING IN 2 MINUTES" C-m
sleep 60s
$go "say SERVER IS RESTARTING IN 1 MINUTE" C-m
sleep 30s
$go "say SERVER IS RESTARTING IN 30 SECONDS" C-m
sleep 10s
$go "say SERVER IS RESTARTING IN 20 SECONDS" C-m
sleep 10s
while (( $timeleft >= 1 ))
do
	$go "say $timeleft SECONDS" C-m
	timeleft=$(($timeleft - 1))
	sleep 1s
done
$go "say RESTARTING..." C-m
sleep 1s

#Stop the server
$go "stop" C-m

#wait to ensure the server is completely shutdown
sleep 60s

#Copy the world into the backup directory
cd $serverDir
cp -R $worldLoc $backupLoc

#CD into the backup directory, and change the name of the backup to include the date
cd $backupLoc && mv world "world $today"

#CD into the server directory and relaunch the server
$go "cd $serverDir && $serverStart" C-m
