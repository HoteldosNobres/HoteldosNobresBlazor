using System;
using System.ComponentModel;

namespace HoteldosNobresBlazor.Classes;

public enum EStatus : byte
{
    [Description("")]
    None = 0,  
     
    [Description("Em processo")]
    in_progress = 1,
     
    [Description("Confirmado")]
    confirmed = 2,
     
    [Description("Não Confirmado")]
    not_confirmed = 3,
     
    [Description("Cancelado")]
    canceled = 4,
     
    [Description("Checked-in")]
    checked_in = 5,
     
    [Description("Checked-Out")]
    checked_out = 6,
             
    [Description("No-Show")]
    no_show = 7,
     
   
       
}

