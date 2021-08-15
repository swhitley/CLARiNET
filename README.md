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

Run CLARiNET from the command line using the positional parameters:

Example: `clarinet "C:\example_folder\Test.clar" Test 7 mytenant myusername mypassword`

You can also run CLARiNET by entering `clarinet` by itself.  The application will prompt for the necessary information.

![image](https://user-images.githubusercontent.com/413552/129465336-0168f0e3-7e75-4309-83e1-8aebe9b9ae6e.png)


