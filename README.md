# CLARiNET
Load CLAR files without Workday Studio

## WHY?
You might be asking yourself, "Why load a CLAR file from outside of Workday Studio?"

Here are a few ideas. (Care to add to this list in the discussions?)

- Distribute an integration to someone who is not a developer.
- Enable an operations role to move an integration to production without requiring development tools.
- Migrate an integration from a laptop that hasn't been configured for Studio.
- Save time when reloading integrations to Sandbox following the weekly refresh.

## Installation

1. Download the latest release for your operating system.
2. Unzip the executable files into a new directory.
3. Run `clarinet --help` to view the available options.

## Loading a CLAR file to Workday

- Enter `clarinet` on a line by itself.  The application will prompt for all necessary information.
- If a single `.clar` file is found in the same directory, CLARiNET will automatically select that file for processing.
- Run `clarinet --help` to view the available options.
- Run `clarinet -e` to view the list of Workday environments and the associated numbers.

Run CLARiNET from the command line using the positional parameters:

%1 Clar File
%2 Cloud Collection Name
%3 Workday Environment Number (run `clarinet -e` to see the list of numbers)
%4 Tenant
%5 Username
%6 Password

Example: `clarinet "C:\example_folder\Test.clar" Test 7 mytenant myusername mypassword`

A prompt will appear if parameters are not included. For example, you may choose to always run the application without supplying the password so that it will be requested.

Example: `clarinet "C:\example_folder\Test.clar" Test 7 mytenant myusername`

### Sample `clarinet` Run
![image](https://user-images.githubusercontent.com/413552/129465336-0168f0e3-7e75-4309-83e1-8aebe9b9ae6e.png)

## Credits

CLARiNET is compatible with Workday®
It is not sponsored, affiliated with, or endorsed by Workday.
