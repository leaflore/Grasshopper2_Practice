using System;
using System.Reflection;
using Grasshopper2.UI;
using Grasshopper2.UI.Icon;

namespace Grasshopper2_PlugIn
{
    public sealed class Grasshopper2_PlugInPluginInfo : Grasshopper2.Framework.Plugin
    {
        static T GetAttribute<T>() where T : Attribute => typeof(Grasshopper2_PlugInPluginInfo).Assembly.GetCustomAttribute<T>();

        public Grasshopper2_PlugInPluginInfo()
          : base(new Guid("1a2da724-16bd-4d4b-bec9-beb075cf546c"),
                 new Nomen(
                    GetAttribute<AssemblyTitleAttribute>()?.Title,
                    GetAttribute<AssemblyDescriptionAttribute>()?.Description),
                 typeof(Grasshopper2_PlugInPluginInfo).Assembly.GetName().Version)
        {
            Icon = AbstractIcon.FromResource("Grasshopper2_PlugInPlugin", typeof(Grasshopper2_PlugInPluginInfo));
        }

        public override string Author => GetAttribute<AssemblyCompanyAttribute>()?.Company;

        public override sealed IIcon Icon { get; }

        public override sealed string Copyright => GetAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? base.Copyright;

        // public override sealed string Website => "https://mywebsite.example.com";

        // public override sealed string Contact => "myemail@example.com";

        // public override sealed string LicenceAgreement => "license or URL";

    }
}