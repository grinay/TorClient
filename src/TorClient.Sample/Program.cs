namespace TorClient.Sample {
    using System;
    using System.Threading.Tasks;
    using global::TorClient.Options;

    public class Program {
        public static async Task Main(string[] args) {
            var serviceUrl = "https://get.geojs.io/v1/ip/country/full/{0}";

            using (var client = new TorClient(new TorOptions { Password = "<Your Password Here>" })) {
                await client.TorControl.RenewIpAddress();

                var location = await client.Http.GetStringAsync(string.Format(
                    serviceUrl,
                    client.IpAddress));

                Console.WriteLine(location);
            }
        }
    }
}