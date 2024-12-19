using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Google.Apis.PeopleService.v1;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using HoteldosNobresBlazor.Client.FuncoesClient;

namespace HoteldosNobresBlazor.FuncoesClient
{
    public class FunctionGoogle
    {
        static string[] Scopes = { PeopleServiceService.Scope.Contacts };

        public FunctionGoogle() { }

        public static UserCredential? UserCredential()
        {
            try
            {
                ClientSecrets secrets = new ClientSecrets()
                {
                    ClientId = KEYs.ClientId_GOOGLE,
                    ClientSecret = KEYs.ClientSecret_GOOGLE
                };
                var token = new TokenResponse { RefreshToken = KEYs.RefreshToken_GOOGLE };

                UserCredential credential = new UserCredential(new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = secrets
                    }),
                    "user",
                    token);

                return credential;
            }
            catch (Exception e)
            {
                throw new Exception("Credencial: " + e.Message);
            }

        }

        public static async Task<string> AddPeopleAsync(string nome, string local, string telefone, string email)
        {
            UserCredential credential = UserCredential();
            var service = new PeopleServiceService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "APP_NAME",
            });
             
            string givenome = nome;
            if (!string.IsNullOrEmpty(local))
                givenome = nome + " - " + local;

            Person contactToCreate = new Person();
            List<Name> names = new List<Name>();
            names.Add(new Name()
            {
                GivenName = givenome,
            });
            contactToCreate.Names = names;

            List<PhoneNumber> phones = new List<PhoneNumber>();
            phones.Add(new PhoneNumber()
            {
                Value = "55" + telefone.Replace("+55", "").Replace(" ", "").Replace("-", ""),
            });
            contactToCreate.PhoneNumbers = phones;
             
            List<EmailAddress> emails = new List<EmailAddress>();
            emails.Add(new EmailAddress()
            {
                Value = email,
            });
            contactToCreate.EmailAddresses = emails;
             
            var request = service.People.CreateContact(contactToCreate);

            var createdContact = await request.ExecuteAsync();

            return "OK";
        }
    }
}
