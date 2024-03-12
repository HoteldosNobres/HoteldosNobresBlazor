using System.Net;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using HoteldosNobresBlazor.Classes;

namespace HoteldosNobresBlazor.Funcoes
{
    public class FuncoesFNRH
    {
        static string ChaveAcesso = "$TOKEN_FNRH";


        public static string Inserir(Reserva reserva)
        {
            try
            {
                string retorno = SendSoapRequest(CriarXML(reserva));
                if (!retorno.Equals("Erro"))
                {
                    var retornoObj = GetContentFromTag(retorno, "return");
                    return retornoObj;
                }else
                {
                    return "Erro";
                }
                    
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return "Erro";
            }

        }

        public static string Atualizar(Reserva reserva)
        {
            try
            {
                string retorno = SendSoapRequest(CriarXMLfnrhAtualizar(reserva));
                var retornoObj = GetContentFromTag(retorno, "return");
                return retornoObj;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return "Erro";
            }

        }

        public static string CheckIn(Reserva reserva)
        {
            try
            {
                string retorno = SendSoapRequest(CriarXMLfnrhCheckin(reserva));
                var retornoObj = GetContentFromTag(retorno, "return");
                return retornoObj;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return "Erro";
            }

        }

        public static string CheckOut(Reserva reserva)
        {
            try
            {
                string retorno = SendSoapRequest(CriarXMLfnrhCheckout(reserva));
                var retornoObj = GetContentFromTag(retorno, "return");
                return retornoObj;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return "Erro";
            }

        }


        private static string SendSoapRequest(string xmlString)
        {
            try
            {
                string url = "http://fnrhws.hospedagem.turismo.gov.br/FnrhWs/FnrhWs?wsdl";
                string action = "soapenv";


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                byte[] bytes;
                bytes = Encoding.UTF8.GetBytes(xmlString);
                request.ContentType = "application/soap+xml;";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                request.Headers.Add("SOAPAction", action);
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        string soapResult = rd.ReadToEnd();
                        //Console.WriteLine(soapResult);
                        return soapResult;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Erro";
            }


        }

        public static T DeserializeObject<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static string GetContentFromTag(string xml, string tagName)
        {
            try
            {
                var doc = XDocument.Parse(xml);
            var content = doc.Descendants(tagName).FirstOrDefault()?.Value;
            return content;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }
        }

        private static string CriarXML(Reserva reserva)
        {
            string soapEnvelope = "";
            try
            { 
                soapEnvelope = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                        xmlns:web=""http://webservice.ws.snrhos.id2.inf.br/"">
                        <soapenv:Header/>
                        <soapenv:Body>
                        <web:fnrhInserir>
                        <chaveAcesso>{KEYs.TOKEN_FNRH}</chaveAcesso>
                        <fnrh>
                        <snnumcpf>{reserva.ProxyCPF}</snnumcpf>
                        <sntipdoc></sntipdoc>
                        <snnumdoc></snnumdoc>
                        <snorgexp></snorgexp>
                        <snnomecompleto>{reserva.NomeHospede}</snnomecompleto> 
                        <snemail>{reserva.Email}</snemail>
                        <snnacionalidade>{reserva.ProxyPais}</snnacionalidade>
                        <sndtnascimento>{reserva.DataNascimento.ToString("yyyy-MM-dd")}</sndtnascimento>
                        <snsexo>{reserva.Genero.ToUpper()}</snsexo> 
                        <sncelularddd>{reserva.CelularDDD}</sncelularddd>
                         <sncelularddi>{reserva.CelularDDI}</sncelularddi>
                         <sncelularnum>{reserva.Celular}</sncelularnum>
                         <snresidencia>{reserva.Address}</snresidencia>
                         <snpaisres>{reserva.ProxyPais.Trim()}</snpaisres>
                         <snestadores>{reserva.ProxyEstado.Trim()}</snestadores>
                         <sncidaderes>{reserva.ProxyCidade.Trim()}</sncidaderes>
                         <bgstdscpais>BRASIL</bgstdscpais>
                         <bgstdscpaisdest>BRASIL</bgstdscpaisdest>
                         <bgstdscestado>MINAS GERAIS</bgstdscestado>
                         <bgstdscestadodest>MINAS GERAIS</bgstdscestadodest>
                         <bgstdsccidade>POÇOS DE CALDAS</bgstdsccidade>
                         <bgstdsccidadedest>POÇOS DE CALDAS</bgstdsccidadedest>
                         <snmotvia>09</snmotvia>
                         <sntiptran>07</sntiptran>  
                         <snprevent>{reserva.DataCheckIn.ToString("yyyy-MM-ddT14:00:00")}</snprevent>
                         <snprevsai>{reserva.DataCheckOut.ToString("yyyy-MM-ddT11:59:00")}</snprevsai>
                         <snobs>{reserva.Obs.Trim()}</snobs>
                         <snnumhosp>{reserva.Hospedes.GetValueOrDefault(1)}</snnumhosp>
                         <snuhnum>{reserva.Snuhnum}</snuhnum> 
                         <snidcidadeibgeres>{reserva.ProxyCodigoIBGE.Trim()}</snidcidadeibgeres>
                         <snidcidadeibge>3151800</snidcidadeibge>
                         <snidcidadeibgedest>3151800</snidcidadeibgedest>
                         </fnrh>
                         </web:fnrhInserir>
                         </soapenv:Body>
                        </soapenv:Envelope> ";

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return soapEnvelope;
        }

        private static string CriarXMLfnrhAtualizar(Reserva reserva)
        {
            string soapEnvelope = "";
            try
            {
                soapEnvelope = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                        xmlns:web=""http://webservice.ws.snrhos.id2.inf.br/"">
                        <soapenv:Header/>
                        <soapenv:Body>
                        <web:fnrhAtualizar>
                        <chaveAcesso>{KEYs.TOKEN_FNRH}</chaveAcesso>
                        <snNum>{reserva.SnNum}</snNum>
                        <fnrh>
                        <snnumcpf>{reserva.ProxyCPF}</snnumcpf>
                        <sntipdoc></sntipdoc>
                        <snnumdoc></snnumdoc>
                        <snorgexp></snorgexp>
                        <snnomecompleto>{reserva.NomeHospede}</snnomecompleto> 
                        <snemail>{reserva.Email}</snemail>
                        <snnacionalidade>{reserva.ProxyPais}</snnacionalidade>
                        <sndtnascimento>{reserva.DataNascimento.ToString("yyyy-MM-dd")}</sndtnascimento>
                        <snsexo>{reserva.Genero.ToUpper()}</snsexo> 
                        <sncelularddd>{reserva.CelularDDD}</sncelularddd>
                         <sncelularddi>{reserva.CelularDDI}</sncelularddi>
                         <sncelularnum>{reserva.Celular}</sncelularnum>
                         <snresidencia>{reserva.Address}</snresidencia>
                         <snpaisres>{reserva.ProxyPais.Trim()}</snpaisres>
                         <snestadores>{reserva.ProxyEstado.Trim()}</snestadores>
                         <sncidaderes>{reserva.ProxyCidade.Trim()}</sncidaderes>
                         <bgstdscpais>{reserva.ProxyPais.Trim()}</bgstdscpais>
                         <bgstdscpaisdest>BRASIL</bgstdscpaisdest>
                         <bgstdscestado>{reserva.ProxyEstado.Trim()}</bgstdscestado>
                         <bgstdscestadodest>MINAS GERAIS</bgstdscestadodest>
                         <bgstdsccidade>{reserva.ProxyCidade.Trim()}</bgstdsccidade>
                         <bgstdsccidadedest>POÇOS DE CALDAS</bgstdsccidadedest>
                         <snmotvia>09</snmotvia>
                         <sntiptran>07</sntiptran>
                         <snprevent>{reserva.DataCheckIn.ToString("yyyy-MM-ddT14:00:00")}</snprevent>
                         <snprevsai>{reserva.DataCheckOut.ToString("yyyy-MM-ddT11:59:00")}</snprevsai>
                         <snobs>{reserva.Obs.Trim()}</snobs>
                         <snnumhosp>{reserva.Hospedes.GetValueOrDefault(1)}</snnumhosp>
                         <snuhnum>{reserva.Snuhnum}</snuhnum> 
                         <snexcluirficha>{reserva.ProxySnexcluirficha }</snexcluirficha>
                         <snidcidadeibgeres>{reserva.ProxyCodigoIBGE.Trim()}</snidcidadeibgeres>
                         <snidcidadeibge>3151800</snidcidadeibge>
                         <snidcidadeibgedest>3151800</snidcidadeibgedest>
                         </fnrh>
                         </web:fnrhAtualizar>
                         </soapenv:Body>
                        </soapenv:Envelope> ";

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return soapEnvelope;
        }

        private static string CriarXMLfnrhCheckin(Reserva reserva)
        {
            string soapEnvelope = "";
            try
            {
                soapEnvelope = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                        xmlns:web=""http://webservice.ws.snrhos.id2.inf.br/"">
                        <soapenv:Header/>
                        <soapenv:Body>
                        <web:fnrhCheckin>
                        <chaveAcesso>{KEYs.TOKEN_FNRH}</chaveAcesso>
                        <snNum>{reserva.SnNum}</snNum>
                        <dataCheckin>{reserva.DataCheckInRealizado.ToString("yyyy-MM-ddTHH:mm:ss")}</dataCheckin>
                        </web:fnrhCheckin>
                        </soapenv:Body>
                    </soapenv:Envelope>";

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return soapEnvelope;
        }

        private static string CriarXMLfnrhCheckout(Reserva reserva)
        {
            string soapEnvelope = "";
            try
            {
                soapEnvelope = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                        xmlns:web=""http://webservice.ws.snrhos.id2.inf.br/"">
                        <soapenv:Header/>
                        <soapenv:Body>
                        <web:fnrhCheckout>
                        <chaveAcesso>{KEYs.TOKEN_FNRH}</chaveAcesso>
                        <snNum>{reserva.SnNum}</snNum>
                        <dataCheckout>{reserva.DataCheckOutRealizado.ToString("yyyy-MM-ddTHH:mm:ss")}</dataCheckout>
                        </web:fnrhCheckout>
                        </soapenv:Body>
                    </soapenv:Envelope>";

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return soapEnvelope;
        }

    }
}
