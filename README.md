# CLARiNET
<img align="right" src="https://user-images.githubusercontent.com/413552/129500187-ea5a1947-16d2-46eb-ab48-2adc6553b6d8.png" width="50" />
Deploy Workday Studio Integrations without Workday Studio.

## WHY?
You might be asking yourself, "Why load a CLAR file from outside of Workday Studio?"

Here are a few ideas. (Care to add to this list in the discussions?)

- Distribute an integration to someone who is not a developer.
- Enable an operations role to move an integration to production without requiring development tools.
- Migrate an integration from a laptop that hasn't been configured for Studio.
- Save time when reloading integrations to Sandbox following the weekly refresh.

## Installation

1. Download the [latest release](https://github.com/swhitley/CLARiNET/releases/latest) for your operating system (Mac or Windows).
3. Unzip the executable files into a new directory.
4. Run `clarinet --help` to view the available options.
5. You may need to install the [dotnet core runtime](https://dotnet.microsoft.com/download/dotnet/5.0/runtime).
6. Optional: Download [Test.clar](https://github.com/swhitley/CLARiNET/blob/main/Test.clar) to try it out.

**Note:** CLARiNET calls an unpublished Workday API endpoint. Functionality is not guaranteed.

## Loading a CLAR file to Workday

- Enter `clarinet` on a line by itself.  The application will prompt for all necessary information.
- If a single `.clar` file is found in the same directory, CLARiNET will automatically select that file for processing.
- Run `clarinet --help` to view the available options.
- Run `clarinet -w` to view the list of Workday environments and the associated numbers.

### Run CLARiNET from the command line using positional parameters:

* %1 CLAR File<br/>
* %2 Cloud Collection Name<br/>
* %3 Workday Environment Number (run `clarinet -w` to see the list of numbers)<br/>
* %4 Tenant<br/>
* %5 Username<br/>
* %6 Encrypted Password (run `clarinet -e` to encrypt a password) <br/>

Example: `clarinet "C:\example_folder\Test.clar" Test 7 mytenant myusername AQAAANCMnd8BFdERjHoAwE/Cl+...G3Q=`

The entire list of parameters is not required. Prompts will appear for the parameters that are not included.  These are all valid examples:

Example #1: `clarinet "C:\example_folder\Test.clar"`

Example #2: `clarinet "C:\example_folder\Test.clar" Test 7 mytenant myusername`

Example #3: `clarinet "C:\example_folder\Test.clar" Test`

### Sample `clarinet` Run
![image](https://user-images.githubusercontent.com/413552/129465336-0168f0e3-7e75-4309-83e1-8aebe9b9ae6e.png)

## Credits

Command Line Parser - https://github.com/commandlineparser/commandline ([MIT](https://github.com/commandlineparser/commandline/blob/master/License.md))

CLARiNET is compatible with Workday®
It is not sponsored, affiliated with, or endorsed by Workday.
