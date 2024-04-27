using System;
using System.ComponentModel;

namespace HoteldosNobresBlazor.Classes
{
    public enum EUF : byte
    {
        [Description("")]
        None = 0,  
         
        [Description("Acre")] 
        AC = 12,
         
        [Description("Alagoas")] 
        AL = 27,
         
        [Description("Amapa")]         
        AP = 16,
         
        [Description("Amazonas")]       
        AM = 13,
         
        [Description("Bahia")] 
        BA = 29,
         
        [Description("Ceara")] 
        CE = 23,
                 
        [Description("Distrito Federal")] 
        DF = 53,
         
        [Description("Espirito Santo")] 
        ES = 32,
         
        [Description("Exterior")] 
        EX = 8,
         
        [Description("Goais")] 
        GO = 52,
         
        [Description("Maranhao")] 
        MA = 21,
         
        [Description("Mato Grosso")] 
        MT = 51,
         
        [Description("Mato Grosso do Sul")] 
        MS = 50,
         
        [Description("Minas Gerais")] 
        MG = 31,
 
        [Description("Para")] 
        PA = 15,
         
        [Description("Paraiba")] 
        PB = 25,
         
        [Description("Parana")] 
        PR = 41,
         
        [Description("Pernanbuco")] 
        PE = 26,
         
        [Description("Piaui")] 
        PI = 22,
          
        [Description("Rio de Janeiro")] 
        RJ = 33,
         
        [Description("Rio Grande do Norte")] 
        RN = 24,
         
        [Description("Rio Grande do Sul")] 
        RS = 43,
         
        [Description("Rondonia")] 
        RO = 11,
         
        [Description("Roraima")] 
        RR = 14,
         
        [Description("Santa Catarina")] 
        SC = 42,
         
        [Description("Sao Paulo")] 
        SP = 35,
         
        [Description("Sergipe")] 
        SE = 28,
         
        [Description("Tocantins")] 
        TO = 17,
           
    }

    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attributes != null && attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }

}