using HoteldosNobresBlazor.Classes;
using pix_payload_generator.net.Models.CobrancaModels;
using pix_payload_generator.net.Models.PayloadModels;
using System.Globalization;

namespace HoteldosNobresBlazor.Funcoes;
public class FunctionSicoob
{
    #region Pedido
    public static async Task<string> QrCode(Reserva reserva, string Valor)
    {
        if (Valor is null)
            Valor = "0";

        var valordecimal = decimal.Parse(Valor, new CultureInfo("pt-BR"));
        Valor = valordecimal.ToString("N2", new CultureInfo("pt-BR"));

        Cobranca cobranca = new Cobranca(_chave: "35337342000186")
        {
            SolicitacaoPagador = "Pagamento do Reserva " + reserva.IDReserva,
            Valor = new Valor
            {
                Original = valordecimal.ToString("N2", CultureInfo.InvariantCulture) // "0.01"
            }

        };

        if (reserva.IDReserva is null)
            reserva.IDReserva = "Sua reserva";

        var payload = cobranca.ToPayload(reserva.IDReserva, new Merchant("HOTEL DOS NOBRES LTDA", "Pocos de Caldas"));

        return payload.GenerateStringToQrCode();

    }
     

    #endregion Pedido
     

}