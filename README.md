# temporary_local_admin
C# Program to grant the logged on user local administrator rights

![alt tag](https://raw.githubusercontent.com/omgdanieltam/temporary_local_admin/master/temp-admin.png)

#INTRODUCTION
With the removal of local administrator rights for domain users, we had to manually install a lot of programs for end users. This causes huge delays in our work days to spend time installing program one by one per user, for whatever program they needed. Since each person doesn't have the same programs everytime, there was no way to automate this process. Running this program will place the logged on user into the local administrator group (not domain administrator) so that their credentials will now install allow them to install programs on their own computer.

#HOW IT WORKS
You will need a local administrator credential to run (requireAdministor in the manifest). Once running, it will query for the owner of "explorer.exe" which is the main interface for the computer; this will typically be the logged on user. Once we have that user, we simply run a command line command to place that user into the local administrator group. As insurance, we also remove 'domain users'. There is a timer as well that will run for 30 minutes and will close the program automatically afterwards. Once the program is closed (automatically or manually) we listen for the close event and remove the user from the local administrator group.

#SOURCES
http://stackoverflow.com/questions/5218778/how-do-i-get-the-currently-logged-username-from-a-windows-service-in-net - xanblax; solution to grabbing the logged on user using 'explorer.exe'
