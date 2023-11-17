# NVRAMTuner
### NVRAMTuner is a GUI tool for managing NVRAM parameters on ASUS routers

## Important notice 🚨
NVRAMTuner is still under development and is not yet complete.
While targeted specifically at ASUS routers running the [asuswrt-merlin](https://www.asuswrt-merlin.net/) firmware, it may also work on other routers with similar NVRAM configs, SSH setups and NVRAM related binaries. Your mileage will vary.

## Current features

### Adding routers
You can enter details about your router via the setup page, NVRAMTuner will securely encrypt its details and store them for later use.

![Setup page](https://github.com/Reeceeboii/NVRAMTuner/assets/42159320/8a9ca64e-c445-4434-a225-d6af3845f8ca)

### Multiple authentication methods & auto SSH key discovery
NVRAMTuner allows authentication via both passwords and SSH keys. When using SSH keys, NVRAMTuner will automatically discover and load keys from your filesystem.

![SSH keys search](https://github.com/Reeceeboii/NVRAMTuner/assets/42159320/77f4bcec-efa1-4eee-930c-4b4fe127dc3e)

### Config and auth verification
Once your router details are entered, NVRAMTuner will verify them.

![Verified Router](https://github.com/Reeceeboii/NVRAMTuner/assets/42159320/9381b39b-2434-4f0a-916c-9842214bb8b7)

### The main guts
NVARMTuner operates on (sort of) a Git basis. Make changes, stage them, and then commit them to the router. It summarises byte changes to your router's NVRAM allocation as you stage more changes, keeping you informed on what impact your changes will have.

(Yes, it is a noted bug that the Net change calculation is incorrect).

![Main window](https://github.com/Reeceeboii/NVRAMTuner/assets/42159320/a1e4d8ec-0936-4944-93db-3c6993cd3ac8)

## Variable diffs
Fully integrated diff viewer for your changes, allows visual splits on common variable delimiters to make for easier viewing.

![Diff](https://github.com/Reeceeboii/NVRAMTuner/assets/42159320/0c38e28e-9dc2-4a65-a443-ef79dbe1e17a)

## Configurable
There are already several options to configure your experience, including 6 different UI themes, custom SSH keep-alive intervals and safeguards/warnings. Any settings changes are automatically applied.

![Settings flyout](https://github.com/Reeceeboii/NVRAMTuner/assets/42159320/a631697b-a1aa-41b3-a087-ab77aa7424ee)


## Others
- Is implemented in C#/WPF using .NET Framework. As such is currently Windows only.
- Uses the Windows DPAPI (Data Protection API) for encryption of router details.
- Allows the export of all staged changes as Bash scripts if you wish to manually tinker further and/or see what the shell will end up being passed over the network when you press commit.
- Allows refreshing of NVRAM contents from the router
- Some "special" variables that are hard to decipher while viewed in plaintext have bespoke formatted tabular views, see above.
- I don't often have the energy to code in my spare time outside of work. I may polish this at some point as there are some gaping holes and unfinished features.

### Disclaimer
As NVRAMTuner directly manipulates your router's memory, it has access to system-wide settings that should not be played with unless you know what you are doing. You are at full risk of bricking your router if you set options without understanding what you are changing. Be warned :)
