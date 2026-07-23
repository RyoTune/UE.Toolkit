using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using UE.Toolkit.DumperMod.Template.Configuration;

namespace UE.Toolkit.DumperMod;

public class Config : Configurable<Config>
{
    [DisplayName("Log Level")]
    [DefaultValue(LogLevel.Information)]
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    [DisplayName("Dump Mode")]
    [Description("Defines how the dump will be saved onto the file system")]
    [DefaultValue(DumpFileMode.SingleFile)]
    public DumpFileMode Mode { get; set; } = DumpFileMode.SingleFile;
    
    [DisplayName("Dump Schema")]
    [Description("Defines the format for the type dump")]
    [DefaultValue(DumpSchema.Dynamic)]
    public DumpSchema Schema { get; set; } = DumpSchema.Dynamic;

    [DisplayName("File Namespace")]
    [DefaultValue("")]
    public string FileNamespace { get; set; } = string.Empty;
    
    [DisplayName("File Usings (Separate with ; )")]
    [DefaultValue("")]
    public string FileUsings { get; set; } = string.Empty;

    [DisplayName("Single-File File Name")]
    [DefaultValue("")]
    public string SingleFileOutputName { get; set; } = string.Empty;
    
    [DisplayName("Dump Functions")]
    [DefaultValue(true)]
    public bool DumpFunctions { get; set; } = true;
}

public enum DumpFileMode
{
    [Display(Name = "Single-File")]
    SingleFile,
    
    [Display(Name = "File Per Type")]
    FilePerType,
    
    [Display(Name = "File Per Module")]
    FilePerModule,
}

public enum DumpSchema
{
    [Display(Name = "Structs Only")]
    Static,
    [Display(Name = "Structs and Classes")]
    Dynamic,
}

/// <summary>
/// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
/// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
/// </summary>
public class ConfiguratorMixin : ConfiguratorMixinBase
{
}