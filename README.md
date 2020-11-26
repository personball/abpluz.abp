# abpluz.abp
This project is an extension for abp.io



# modules samples

### nupkg:Abpluz.Abp.Http.Client.SwitchableIdentityClients

`PluzAbpHttpClientIdentityModelModule`

```
  using (PluzIdentityClientSwitcher.Use("Internal"))
  {
      await _sampleAppService.GetAsync();
  }
```
appsettings.json
```
"IdentityClients":{
 "Default":{...},
 "Internal":{...}
}
```

[samples/SwithableIdentityClients](samples/SwithableIdentityClients/Pluz.Sample/)
```
    public class DemoAppService : SampleAppService, IDemoAppService
    {
        public async Task AccessWithClientAuthAsync()
        {
            if (CurrentUser.Id.HasValue)
            {
                throw new Exception("client auth should not has userId!");
            }
        }

        public async Task AccessWithDefaultPasswordAuthAsync()
        {
            if (!CurrentUser.Id.HasValue)
            {
                throw new Exception("password auth should has userId!");
            }
        }
    }
    
    // test/Pluz.Sample.HttpApi.Client.ConsoleTestApp/ClientDemoService.cs
        public async Task RunAsync()
        {
            var output = await _profileAppService.GetAsync();
            Console.WriteLine($"UserName : {output.UserName}");
            Console.WriteLine($"Email    : {output.Email}");
            Console.WriteLine($"Name     : {output.Name}");
            Console.WriteLine($"Surname  : {output.Surname}");

            await _demoAppService.AccessWithDefaultPasswordAuthAsync();

            // await _demoAppService.AccessWithClientAuthAsync();// will throw

            using (PluzIdentityClientSwitcher.Use("client"))
            {
                await _demoAppService.AccessWithClientAuthAsync();// will not throw
            }
        }
    // appsettings.json
  {
  "RemoteServices": {
    "Default": {
      "BaseUrl": "https://localhost:44376",
      "IdentityClient": "Default"
    }
  },
  "IdentityClients": {
    "Default": {
      "GrantType": "password",
      "ClientId": "Sample_App",
      "ClientSecret": "1q2w3e*",
      "UserName": "admin",
      "UserPassword": "1q2w3E*",
      "Authority": "https://localhost:44376",
      "Scope": "Sample"
    },
    "client": {
      "GrantType": "client_credentials",
      "ClientId": "Sample_App",
      "ClientSecret": "1q2w3e*",
      "Authority": "https://localhost:44376",
      "Scope": "Sample"
    }
  }
}
```

### LocalizableContents
Extends abp efcore Auto DataFilter multi culture entries with aspnetcore cookie/querystring/header. 
[samples/LocalizableContents](samples/LocalizableContents/Pluz.Sample)
