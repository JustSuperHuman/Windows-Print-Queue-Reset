## Made in like 6 minutes...
It's not AMAZING but it fixes a need for a customer of mine. 

I hope the code explains what is happening well enough for someone to use this to make something that fits their needs.

### PHP Side
Host IIS on the webserver, and install PHP. 
I like to use the PHPManager for IIS project here https://github.com/phpmanager/phpmanager/releases/

NGL, there's some permissions stuff I had to set for commands to run properly but can't remember for the life of me what it was. 

- Under bindings, I have "connect as" set to an administrator account.
- Using "DefaultAppPool"
- Under advanced settings under "Physical Path Credentials" I have it set to "Specific User" and an administrator account.

### Client side
All this code does is make sure the tool is run as admin, resets the local print queue, then sends the reset print queue commands to the server by calling the PHP script.

Can be published as a single file executable and you can tell users to run it from their desktop 🤷‍♀️ I didn't say it was AMAZING! 