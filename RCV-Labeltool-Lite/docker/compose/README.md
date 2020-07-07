RCV Labeltool LITE
------------------

RCV Labeltool LITE Docker application for labeling images. The application
is hosted within Docker.

RCV Labeltool LITE exposes port 80 to the localhost, where the application 
is started. After startup, browse to http://localhost to start labeling
images.


How to start
Open powershell and open folder where docker-compose.yml is located. 
Start RCV Labeltool LITE by typing the command:
	docker-compose.exe up
After the command, RCV Labeltool LITE artifacts will be installed in the
local Docker repository. After the successfully installation RCV Labeltool
LITE will be started on http://localhost.


Volumne-configuration of RCV Labeltool LITE
RCV Labeltool LITE mounts data from the filesystem. The default location
where data is located is 'C:\rcv-labeltool-lite'. This folder can be 
changed by modification of section 'rcvlabeltoollitebackend - volumes'. 
The volumes-mount is described by syntax 'hostpath:containerpath'. If
RCV Labeltool LITE should mount data from C:\labeling, configuration
has to be modified like:

  rcvlabeltoollitebackend:
    volumes:
      - "/c//labeling:/training-data"


Port-configuration of RCV Labeltool LITE
RCV Labeltool LITE can be configured through docker-compose.yml. The default 
exposed port for RCV Labeltool LITE is 80. This setting can be changed by
modification of section 'rcvlabeltoollite - ports'. The port-forwarding is 
described by syntax 'hostport:interalport'. If RCV Labeltool LITE should
run on port 8080 of localhost, configuration has to be modified like:

  rcvlabeltoollite:    
    ports:
      - "8080:80"
	  
	  

Known problems
If RCV Labeltool LITE is running at a windows operating system, it is 
neccessary to allow docker access to the local file system. This can be done
through Docker settings (Shared drives). The authorization has to be renewed
everytime the password of the user changes.