using System.ComponentModel.DataAnnotations;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Reloaded.Common.GameConfigs;

public enum GameConfigVersion
{
    Auto,
    
    [Display(Name = "UE 5.4.4 (Clair Obscur)")]
    UE5_4_4_ClairObscur,
    
    [Display(Name = "UE 4.27.2 (Persona 3 Reload)")]
    UE4_27_2_P3R,
    
    [Display(Name = "UE 5.0")]
    UE_5_0,
    
    [Display(Name = "UE 5.1")]
    UE_5_1,
    
    [Display(Name = "UE 5.2")]
    UE_5_2,
    
    [Display(Name = "UE 5.3")]
    UE_5_3,
    
    [Display(Name = "UE 5.5")]
    UE_5_5,
    
    [Display(Name = "UE 5.6")]
    UE_5_6,
    
    [Display(Name = "UE 5.7")]
    UE_5_7,
}