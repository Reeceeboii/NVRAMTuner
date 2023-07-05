﻿namespace NVRAMTuner.Client.Views.Templates
{
    using Models.Nvram;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// A <see cref="DataTemplateSelector"/> used to return a <see cref="DataTemplate"/> based on the type
    /// of a provided bound item
    /// </summary>
    public class VariableTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the data template for the <see cref="NvramVariable"/> variable type
        /// </summary>
        public DataTemplate NvramVariableTemplate { get; set; }

        /// <summary>
        /// Gets or sets the data template for the <see cref="NcSettingConf"/> variable type
        /// </summary>
        public DataTemplate NcSettingConfTemplate { get; set; }

        /// <summary>
        /// Returns a different <see cref="DataTemplate"/> based on the type of <param name="item"></param>
        /// </summary>
        /// <param name="item">The item used to determine which data template is to be returned</param>
        /// <param name="container">Instance of <see cref="DependencyObject"/></param>
        /// <returns>A <see cref="DataTemplate"/> instance</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                null => base.SelectTemplate(null, container),
                NvramVariable _ => this.NvramVariableTemplate,
                NcSettingConf _ => this.NcSettingConfTemplate,
                _ => base.SelectTemplate(item, container)
            };
        }
    }
}