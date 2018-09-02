See [First time setup](First%20time%20Setup.md) for requirements to compile locally.

Then just compile from Visual Studio as you would any other Xamarin app.

Supports Android, UWP and iOS (iOS last tested with v1.0.0).

# Share Emergeny Contacts

Share Emergeny Contacts is a cross-platform mobile application that allows two or more people to instantly exchange their emergency contact information from phone to phone without transmitting the data over the internet.

It is useful for any person taking part in group activities that may involve low to high risk of injury or even death (motorcycling, climbing, paragliding, speedflying, base jumping, ...).

Even if you are good friends with everyone in the group you usually don't know the insurance details, family contact information, allergies, blood type, etc. of everyone.

While many modern phones already support ICE (In Case of Emergency) contact details they are usually locked behind the phones lockscreen. Rarely do manufacturers get this feature right and display this information right on the lockscreen or hide it in the "emergency call" feature.

In case of an emergency such information could be life-saving.

With high risk activities it is however entirely possible that someone looses or breaks their phone (motorcycle accident, ..) thus this app provides a way to share this emergency information with everyone.

## Screenshots

![Profile details](sampledata/screenshots/sec_droid_1.png?raw=true) ![QR code](sampledata/screenshots/sec_droid_2.png?raw=true) ![Received contacts](sampledata/screenshots/sec_uwp_1.png?raw=true)

**The app works fully offline and never uploads any user data.**

Contact information is transfers via QR code from one phone to another (making the only requirement a camera on each phone that needs to receive contact information).

# Usage

Three simple steps are required to use the application:

## 1. Setup

Each person needs to fill out their own information in the application first (this has to be done only once).

Optionally multiple profiles can be created (e.g. different insurances for different sport types, different emergency contacts, ..).

Information that can be stored includes:

* Full contact details of own person: name, age, phone number, address, blood type, weight, allergies, etc.
* insurance details (name, insurance number, contact information)
* Full contact details of emergency contact person (TODO: allow more than 1 emergency contact?)

All information is **optional**! Only fill out what you want to share.

## 2. Share your information

Once the data is entered, the user can use the "share my details" feature to transfer his data to another phone.

Additionally the user gets to select how long the information should be stored on the other persons phone (e.g. 1 day, 1 week, 1 month, forever).

A barcode is then displayed and the other person can scan the barcode with their app.

## 2.b Receiving someone's information

**THIS REQUIRES A WORKING CAMERA.**

By scanning the barcode from another person the data is instantly transfered without being uploaded to the internet. The information is only stored locally within the application.

The barcode also contains the maximum duration (if) set by the other person and the app will automatically delete the contact details once they expired.

Once received the other person can add an optional category for the newly added person as well as a nickname.

E.g. Category: "Freeclimbing Trip Italy 2017", Nickname: "Alex from USA"

## 3. In case of emergency

Should anything happen to someone in the group that renderes them unconscious or barely able to talk (falling from height, high speed impact, etc.) anyone else can pull up the emergency contact information in their app and immediately share this information with paramedics.

# Issues

While developing the app I used my own [IssueTracker](https://github.com/MarcStan/IssueTracker) to track the issues (see "issues" directory).

# Known issues

## Compiler warnings

modernhttpclient is not netstandard2. Since I'm not directly using it (dependency of Xamarin.Forms.Pages) I have to wait [for them to fix it](https://github.com/xamarin/Xamarin.Forms/issues/1886).

ResourceDictionary.MergedWith is obsolete: 'Use Source': I currently [don't know how to fix it](https://github.com/xamarin/Xamarin.Forms/pull/1229), since I need classes from an external dll as the "source".
