# PCA_RegistryCommands

Easily modify Windows Registry x86 or x64 keys.

Contains many custom and useful functions.

<b>Settings</b>

- Variables:
  - SubKey = Registry key of your choice. (string)

<b>Functions</b>

    // Initialize
    RegistryCommands(RegistryView registryView, RegistryKey baseKey, string subKey, bool throwErrors = false, string remotePC = null)
    RegistryCommands(RegistryKey baseKey, string subKey, bool throwErrors = false, string remotePC = null)
    RegistryCommands(string subKey, RegistryView registryView)
    RegistryCommands(string subKey)

- RegistryCommands
  - registryView = 32 or 64 registry view – ex: RegistryView.Registry64 (RegistryView)
  - baseKey = Base registry location – ex: Registry.CurrentUser (RegistryKey)
  - subKey = Registry subkey location (string)
  - throwErrors = Throw on error (bool)
  - remotePC = Name of remote PC (string)

    DeleteKey(string KeyName)
    DeleteSubKeyTree(string KeyName, bool throwOnMissing = false)
    DeleteSubKeyTree(bool throwOnMissing = false)
    GetAllSubKeys()
    Read(string KeyName)
    ReadSubFolderKey(string subKeyFolder, string KeyName)
    SubKeyCount()
    ValueCount()
    Write(string KeyName, object Value)

- DeleteKey (returns bool)
  - KeyName = Key name to delete (string)

- DeleteSubKeyTree (returns bool)
  - KeyName = Tree Key name to delete (string)
  - throwOnMissing = Throw error if not found (bool)

- GetAllSubKeys (returns string[])

- Read (returns string)
  - KeyName = Key name to read (string)

- ReadSubFolderKey (returns string)
  - subKeyFolder = SubTree Key name look in (string)
  - KeyName= Key name to read (bool)

- SubKeyCount (returns int)

- ValueCount (returns int)

- Write (returns bool)
  - KeyName = Key name to write (string)
  - Value= Value to write (bool)
<hr>
Simple Example:

    RegistryCommands reg = new RegistryCommands(Registry.CurrentUser, @"Software\Google\Chrome\BLBeacon");
    string regKey = reg.Read("version");

Another Example:

    string Computer = Environment.MachineName;
    const string profileListKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList\";
    RegistryCommands regProfileList = new RegistryCommands(RegistryView.Registry64, Registry.LocalMachine, profileListKey, false, Computer);
    string[] userRegistries = regProfileList.GetAllSubKeys();
    foreach (string userKey in userRegistries)
    {
        RegistryCommands regProfile = new RegistryCommands(RegistryView.Registry64, Registry.LocalMachine, userKey, false,     -Computer);
        string profilePath = regProfile.Read("ProfileImagePath");
        if (profilePath.Contains("user key to delete"))
        {
            bool test = regProfile.DeleteSubKeyTree(false);
            Console.WriteLine($"{userKey} - {(test ? "Removed" : "Unable to remove.")}");
        }
    }
