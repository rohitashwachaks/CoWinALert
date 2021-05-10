# CoWinALert

Queries CoWin and notifies user if slot opens up

*Link:* <https://cowin-alert.azurewebsites.net/api/swagger/ui?code=iA8UBiyrJn5CAXaKuOQ8KehuJVpz6p/GU/gRfaEgEYEa75LUlcUkEA==>

## Step-1

Go to webite and click on the _/user/registration_ tab

## Step-2

On the right hand side of the drop-down window, press the _Try it out_ button

## Step-3

Select your Vaccine and Payment Preference.

## Step-4

In the text box, **DELETE ALL TEXT AND REPLACE** with the following text

```{
  "name": "John Doe",
  "emailID": "john.doe@apple.com",
  "yearofBirth": 1973,
  "periodDate": {
    "startDate": "2021-05-10",
    "endDate": "2021-05-20"
  },
  "pinCode": "[201607,110292]"
  "phone": "1234567890"
}```

Please note:

- Email should be all small
- Start Date and End Date should be in YYYY-MM-DD format
- Pincode is a list of comma separated values with **"[** , **]"** on either sides
- Phone, Name and EmailId has double - quotes( **"**) on both sides
- Please do not delete the extra brackets, quotes, colons or commas
You will a registration confirmation email and mails if a slot opens up from captain.nemo.github@gmail.com .
Check the spam if you haven't recieved it.
