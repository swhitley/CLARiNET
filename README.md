# CLARiNET
<img align="right" src="https://user-images.githubusercontent.com/413552/129500187-ea5a1947-16d2-46eb-ab48-2adc6553b6d8.png" width="50" />

A Workday command-line interface (CLI) for working with Drive files and deploying Workday Studio integrations without Workday Studio.

Windows or Mac

## Workday Drive Features
- Upload files in bulk to Workday Drive. Load files for different Workday user accounts.
- Send files to the Workday Drive Trash.

## CLAR Deployment: Why deploy CLAR files with CLARiNET?
You might be asking yourself, "Why load a CLAR file from outside of Workday Studio?"

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

**Note:** 
- When uploading CLAR files, CLARiNET calls an unpublished Workday API endpoint. Functionality is not guaranteed.  
- The Drive API is published and fully-supported by Workday.

## Uploading files to Workday Drive

- Domains: Ensure you have enabled `Modify` access on Report/Task and Integration permissions for the following domains in Workday:
    - Drive Administrator
    - Drive Web Services
- Ensure that a directory named `inbound` has been created alongside the `clarinet` program file.  The directory will be created automatically when `clarinet` is executed.
- Place each file in the inbound directory with the following file name format:   {Workday User Account}\~{File Name}<br/>
  Example:  swhitley\~MyExampleFile.txt
- Enter `clarinet` on a line by itself.  The application will prompt for all necessary information.
- The `clarinet` command is **DRIVE_UPLOAD**.
- Each file in the `inbound` directory will be uploaded to the drive for the appropriate Workday user account.  The file name in Workday Drive will only show the text following the tilde (\~).
- Once uploaded successfully, each file will be moved to the `processed` directory.

## Sending Drive files to the Trash
- Create a comma-separated value (CSV) file with the following layout:<br/>
    {Workday User Account (Owned By)},{File Name},{Drive Document Workday ID (WID)}<br/>
    Example:  swhitley,MyExampleFile.txt,8a8a350990e401003bb7a37564c10000
- In the file, use the Workday account of the original file owner, or it will be changed during this operation.
- Place the file in the same folder as the `clarinet` application.
- When naming the file, it will be convenient to include the word **trash**.
- Enter `clarinet` on a line by itself.  The application will prompt for all necessary information.
- The `clarinet` command is **DRIVE_TRASH**.

Please note that a file is not deleted when sent to the trash.  Files can be individually restored from the trash if needed.

## Loading a CLAR file to Workday

- Enter `clarinet` on a line by itself.  The application will prompt for all necessary information.
- The `clarinet` command is **CLAR_UPLOAD**.
- If a single `.clar` file is found in the same directory, CLARiNET will automatically select that file for processing.
- Run `clarinet --help` to view the available options.
- Run `clarinet -w` to view the list of Workday environments and the associated numbers.

## Run CLARiNET from the command line using positional parameters:

* %1 CLARiNET Command:  `CLAR_UPLOAD`, `DRIVE_UPLOAD`, `DRIVE_TRASH`<br/>
* %2 Path or Path and File Name<br/>
* %3 Parameters for the command.  Enter the **Cloud Collection** name when performing a **CLAR_UPLOAD**.  For other commands, defaults will be used<br/>
* %4 Workday Environment Number (run `clarinet -w` to see the list of numbers)<br/>
* %5 Tenant<br/>
* %6 Username<br/>
* %7 Encrypted Password (run `clarinet -e` to encrypt a password) <br/>

Example: `clarinet CLAR_UPLOAD "C:\example_folder\Test.clar" Test 7 mytenant myusername AQAAANCMnd8BFdERjHoAwE/Cl+...G3Q=`

The entire list of parameters is not required. Prompts will appear for the parameters that are not included.  These are all valid examples:

Example #1: `clarinet CLAR_UPLOAD "C:\example_folder\Test.clar"`

Example #2: `clarinet CLAR_UPLOAD "C:\example_folder\Test.clar" Test 7 mytenant myusername`

Example #3: `clarinet CLAR_UPLOAD "C:\example_folder\Test.clar" Test`

### Sample `clarinet` Run
![image](https://user-images.githubusercontent.com/413552/129465336-0168f0e3-7e75-4309-83e1-8aebe9b9ae6e.png)

## Credits

Command Line Parser - https://github.com/commandlineparser/commandline ([MIT](https://github.com/commandlineparser/commandline/blob/master/License.md))

CLARiNET is compatible with Workday®
It is not sponsored, affiliated with, or endorsed by Workday.
