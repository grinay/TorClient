namespace TorClient.Sample {
    using System;
    using System.Threading.Tasks;
    using global::TorClient.Options;

    public class Program {
        public static async Task Main(string[] args) {
            var geoLocationServiceUrl = "http://ip-api.com/json/{0}?fields=country&lang=en";
            using (var client = new TorClient(new TorOptions { Password = "<Your Password Here>" })) {
                for (;;){
                    await client.TorControl.RenewIpAddress();
                    Console.WriteLine(await client.Http.GetStringAsync(string.Format(
                        geoLocationServiceUrl,
                        client.IpAddress))
                    );
                }
            }
        }
    }
}