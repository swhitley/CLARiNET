# CLARiNET
<img align="right" src="https://user-images.githubusercontent.com/413552/129500187-ea5a1947-16d2-46eb-ab48-2adc6553b6d8.png" width="50" />

A Workday command-line interface (CLI) for working with Drive files and deploying Workday Studio integrations without Workday Studio.

Windows or Mac

## Workday Drive Features
- Upload files in bulk to Workday Drive. Load files for different Workday user accounts.
- Send files to the Workday Drive Trash.

## CLAR Backup and Versioning
- Quickly download and backup a unique version of a CLAR file.
- Add the backup feature to the Workday Studio menu.

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

**Notes:** 
- When uploading CLAR files, CLARiNET calls an unpublished Workday API endpoint. Functionality is not guaranteed.  
- The Drive API is published and fully-supported by Workday.
- On a Mac, you will be blocked from executing CLARiNET by an "Unidentified Developer" warning.  Use Ctrl-Click or right-click to open CLARiNET.
    - https://www.macworld.co.uk/how-to/mac-app-unidentified-developer-3669596/ 

## Uploading files to Workday Drive

- Domains: Ensure you have enabled `Modify` access on `Report/Task` and `Integration` permissions for the following domains in Workday:
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

## Download a snapshot of a CLAR file from Workday

- Enter `clarinet` on a line by itself.  The application will prompt for all necessary information.
- The `clarinet` command is **CLAR_DOWNLOAD**.
- Run `clarinet --help` to view the available options.
- Run `clarinet -w` to view the list of Workday environments and the associated numbers.
- The downloaded CLAR file name will include the Cloud Collection name and a timestamp.
- In addition to the CLAR file, the integration system configuration will be downloaded to an XML file.

## Loading a CLAR file to Workday

- Enter `clarinet` on a line by itself.  The application will prompt for all necessary information.
- The `clarinet` command is **CLAR_UPLOAD**.
- If a single `.clar` file is found in the same directory, CLARiNET will automatically select that file for processing.
- Run `clarinet --help` to view the available options.
- Run `clarinet -w` to view the list of Workday environments and the associated numbers.

## Run CLARiNET from the command line using positional parameters:

* %1 CLARiNET Command:  `CLAR_UPLOAD`, `CLAR_DOWNLOAD`, `DRIVE_UPLOAD`, `DRIVE_TRASH`<br/>
* %2 Path or Path and File Name<br/>
* %3 Parameters for the command.  Enter the **Cloud Collection** name when performing a **CLAR_UPLOAD** or **CLAR_DOWNLOAD**.  For other commands, defaults will be used<br/>
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

## Downloading CLAR files from Workday Studio
CLARiNET can be run from within Workday Studio. The External Tools feature of Workday Studio (Eclipse) allows you to add custom executables to your Workday Studio menu After adding CLARiNET to Workday Studio, you can click on a project in Project Explorer and then click on the CLARiNET menu item to backup the CLAR file.

![image](https://user-images.githubusercontent.com/413552/152689476-49886183-06d6-4cfd-a147-190bdea8be19.png)

### External Tools Configuration
1. In Workday Studio, click *Run* on the menu.
2. Navigate to External Tools -> External Tools Configurations...
3. Select *Program*. Click *New launch configuration* (icon in the farthest left position of the External Tools dialog box).
4. Give the configuration a name. Example: "CLARiNET Production Backup"
5. For my setup, I placed the CLARiNET executable in my Workday workspace directory.
6. Select an executable in the Location field. 
        
        Example 1: ${workspace_loc}\CLARiNET.exe 
        ${workspace_loc} is an external tools variable that will convert to the workspace directory when run.
        
        Example 2: C:\CLARiNET\CLARiNET.exe 
7. Set the Working Directory. 
        
        Example 1: ${workspace_loc}
        
        Example 2: C:\CLARiNET
8. Set the Arguments. 
        
        Example: CLAR_DOWNLOAD "" "${project_loc}" {environment number} {tenant name} {username} {encrypted password} 
        ${project_loc} is an external tools variable that will convert to the project directory. 
        
        CLARiNET will use the last directory name of the project path as the `Cloud Collection`.

- Run CLARiNET with the "-w" parameter to get the environment number.
- Run CLARiNET with the "-e" parameter to generate an encrypted password.

## Credits

Command Line Parser - https://github.com/commandlineparser/commandline ([MIT](https://github.com/commandlineparser/commandline/blob/master/License.md))

CLARiNET is compatible with Workday®
It is not sponsored, affiliated with, or endorsed by Workday.
