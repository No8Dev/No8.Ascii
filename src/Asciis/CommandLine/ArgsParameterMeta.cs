using System.Reflection;

namespace No8.Ascii.CommandLine;

class ArgsParameterMeta
{
    public ArgsParameterAttribute Attr      { get; }
    public PropertyInfo           Info      { get; }
    public bool                   IsDefault { get; set; }

    public ArgsParameterMeta(
        ArgsParameterAttribute attr,
        PropertyInfo           info)
    {
        Attr = attr;
        Info = info;
    }

    public string       Name => Attr.Name ?? Info.Name;
    public List<string> Names => Attr.Names;
}