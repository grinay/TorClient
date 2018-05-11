namespace TorClient.Sample {
    using System;
    using System.Threading.Tasks;
    using global::TorClient.Options;

    public class Program {
        public static async Task Main(string[] args) {
            var serviceUrl = "https://get.geojs.io/v1/ip/country/full/{0}";

            using (var client = new TorClient(new TorControl(new TorOptions { Password = "<Your Password Here>" }))) {

                var location = await client.Http.GetStringAsync(string.Format(serviceUrl,
                    client.IpAddress)).ConfigureAwait(false);

                Console.WriteLine(location);
            }
        }
    }
}