# MultiTarget Builder Project Configuration Tool
[![openupm](https://img.shields.io/npm/v/com.linuxsenpai.multitargetbuilder?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.linuxsenpai.multitargetbuilder/)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

A Unity Provider setting tool that supports adding multiple build targets with different configurations. Where all the targets can be built with a single button. Allowing less time spent on the build configuration. Where it can be export and imported onto other projects easily.

## Features
- Multiple build targets.
- Overridable Scene List.
- Run targets from the editor.
- Import/Export configuration settings.


## Installation

### Simple Download
[Latest Unity Packages](../../releases/latest)

### Unity Package Manager (UPM)

> You will need to have git installed and set in your system PATH.

Find `Packages/manifest.json` in your project and add the following:
```json
{
  "dependencies": {
    "com.linuxsenpai.multitargetbuilder": "https://github.com/voldien/UMultiTargetBuilder.git#0.1.3",
    "...": "..."
  }
}
```

### OpenUPM

Install the package with [OpenUPM](https://openupm.com/) through the commandline

```sh
# Install openupm-cli
$ npm install -g openupm-cli

# Enter your unity project folder
$ cd YOUR_UNITY_PROJECT_FOLDER

# Add package to your project
$ openupm add com.linuxsenpai.multitargetbuilder
```

## Resources ##

Quick Video of the main functionalities: [Video](https://www.youtube.com/watch?v=F8CBExsLApk)

A Comprehensive guide: Still work in progress.



## Server/ Command Line Build Feature ##
The target configurations can be build from using the command line. Where the build script [build.sh](build.sh) can be used. However, note that this script is a UNIX shell script and thus only supports Unix based system Linux.


## Note ##
It is designed to handle the common build target such as Windows, Linux, Android, iPhone and Mac OS-X. However, it has only been currently been manually tested for Windows, Linux, and Android. But, this should be work just fine since all the internal logic is handle by the Unity engine. 

## Contributing ##

If you have any idea, feel inclined to fork it and submit your changes back to me.


## Donations ##
The project was originally intended to be put on the Unity asset store. However, it was decided to make it free instead. If you find this tool useful consider making a donation.

[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=R35V8EE62CBDL&source=url)

## License ##
This project is licensed under the GPL+3 License - see the [LICENSE](LICENSE) file for details
