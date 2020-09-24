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
