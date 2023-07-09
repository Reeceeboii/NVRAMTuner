namespace NVRAMTuner.Client.Models.Nvram.Concrete
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Model representing a special case variable: "nc_setting_conf".
    /// This variable is very hard to decipher as plaintext so further formatting is required before it can be displayed nicely.
    ///
    /// This variable is a series of key-value pairs enclosed in angle brackets.
    /// Each KV pair consists of three components: the key, and then two subsequent values.
    ///
    /// This is represented in this variable as a <see cref="List{T}"/> of triple tuples: <see cref="Tuple{T1, T2, T3}"/>
    /// </summary>
    public class NcSettingConf : VariableBase<List<Tuple<string, string, string>>>
    {
    }
}
