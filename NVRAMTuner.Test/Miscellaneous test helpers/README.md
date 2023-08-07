## Test helpers

This folder contains any miscellaneous helpers that will prove useful when testing NVRAMTuner:

* `NVRAMTuner Windows Firewall Testing.reg`
	* *This file contains 2 Windows Firewall rules that apply directly to the NVRAMTuner binaries (one rule for release, another for debug). Import this file into your Registry to add the two rules to your firewall (all profiles). By blocking or allowing the connections specified by these rules, you can simulate network drops etc... by completely removing NVRAMTuner's ability to communicate over port 22 at will.*
	* **Notes**
		1. The rules assume a build directory of `%SystemDrive%\\Dev\\NVRAMTuner\\NVRAMTuner.Client\\bin\\<configuration>\\NVRAMTuner.exe`. This will need to be changed to accomodate where you're working on NVRAMTuner.
		2. The rules assume your router is using port `22` to expose its SSH Server. If this is not the case, the rules will need to be edited to reflect this. This can be done in the `.reg` file prior to importing it, or directly via the Windows Firewall after importing it.
