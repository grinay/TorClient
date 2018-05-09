# TorClient
[![Build status](https://ci.appveyor.com/api/projects/status/5e1umwa6jpqi9skr/branch/master?svg=true)](https://ci.appveyor.com/project/paulalves/torclient/branch/master)

Just a lightweight Tor client written in C# with .NET Core and lot of :coffee: as usual. Make HTTP calls through a proxy as Privoxy that will fowards the requests by the Tor Network.

## Sample 

```csharp 
var serviceUrl = "https://get.geojs.io/v1/ip/country/full/{0}";

using (var client = new TorClient(new TorOptions { Password = "<Your Password Here>" })) {
    await client.TorControl.RenewIpAddressAsync();

    var location = await client.Http.GetStringAsync(string.Format(
        serviceUrl,
        client.IpAddress));

    Console.WriteLine(location);
}
```
## Issues

Follow development progress, report bugs and suggest features using [github issues](https://github.com/paulalves/TorClient/issues).

## Contribute

If you have any idea or want to contribute for any of issues, feel free to fork it and submit your changes back to me. 

Thanks!

## License

The MIT License (MIT)

Copyright © 2018 Paul Alves 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
