using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Google.Apis.PeopleService.v1;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses; 

namespace HoteldosNobresBlazor.Funcoes
{
    public class FunctionGoogle
    {
        static string[] Scopes = { PeopleServiceService.Scope.Contacts  };
        
         
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
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }
          
        public static string AddPeople(string nome, string local, string telefone, string email)
        {
            try
            {
                UserCredential credential = UserCredential();
                string givenome = nome;
                if (!string.IsNullOrEmpty(local))
                    givenome = nome + " - " + local;

                 var service = new PeopleServiceService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "APP_NAME",
                });

                Person contactToCreate = new Person();
                List<Name> names = new List<Name>();
                names.Add(new Name() {
                    GivenName = givenome,
                });               
                contactToCreate.Names = names;

                List<PhoneNumber> phones = new List<PhoneNumber>();
                phones.Add(new PhoneNumber()
                { 
                    Value = "55"+telefone.Replace("+55",""),
                });
                contactToCreate.PhoneNumbers = phones;

                List<EmailAddress> emails = new List<EmailAddress>();
                emails.Add(new EmailAddress()
                {
                    Value =  email,
                });
                contactToCreate.EmailAddresses = emails;

                var request = service.People.CreateContact(contactToCreate);

                Person createdContact = request.Execute();

                return " Criou google";
                 
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }
        }
    }
}
