# CoWinALert
Queries CoWin and notifies user if slot opens up

Swagger Endpoint:
```
[Swagger Endpoint]{https://cowin-alert.azurewebsites.net/api/swagger/ui?code=iA8UBiyrJn5CAXaKuOQ8KehuJVpz6p/GU/gRfaEgEYEa75LUlcUkEA==}
```
Input Format for User Registration:

```
{
  "name": "John Doe",
  "emailID": "john.doe@apple.com",
  "yearofBirth": 1993,
  "periodDate": {
    "startDate": "2021-05-07",
    "endDate": "2021-05-10"
  },
  "pinCode": "201607,110292",
  "districtCode": "110,297,324",
  "phone": "1234567890"
}
```