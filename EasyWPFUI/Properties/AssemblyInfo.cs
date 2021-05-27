using System.Windows;
using System.Windows.Markup;
using System.Resources;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]

[assembly: NeutralResourcesLanguage("en", UltimateResourceFallbackLocation.MainAssembly)]
[assembly: XmlnsDefinition("https://easywpfui.com/schemas", "EasyWPFUI")]
[assembly: XmlnsDefinition("https://easywpfui.com/schemas", "EasyWPFUI.Controls")]
[assembly: XmlnsDefinition("https://easywpfui.com/schemas", "EasyWPFUI.Controls.Helpers")]
[assembly: XmlnsDefinition("https://easywpfui.com/schemas", "EasyWPFUI.Controls.Primitives")]
[assembly: XmlnsDefinition("https://easywpfui.com/schemas", "EasyWPFUI.Media")]
