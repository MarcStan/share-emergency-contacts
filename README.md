# Share Emergeny Contacts

Share Emergeny Contacts is a cross-platform application that allows two or more people to instantly exchange their emergency contact information from phone to phone without transmitting the data over the internet.

It is useful for any person taking part in group activities that may involve low to high risk of injury or even death (motorcycling, climbing, paragliding, speedflying, base jumping, ...).

Even if you are good friends with everyone in the group you usually don't know the insurance details, family contact information, allergies, blood type, etc. of everyone.

In case of an emergency such information could be life-saving.

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

All information is fully optional! Only fill out what you want to share.

## 2. Share your information

Once the data is entered, the user can use the "share my details" feature to transfer his data to another phone.

Additionally the user gets to select how long the information should be stored on the other persons phone (e.g. 1 day, 1 week, 1 month, forever).

A barcode is then displayed and the other person can scan the barcode with their app.

## 2.b Receiving someone's information

THIS REQUIRES A WORKING CAMERA.

By scanning the barcode from another person the data is instantly transfered without being uploaded to the internet. The information is only stored locally within the application.

The barcode also contains the maximum duration (if) set by the other person and the app will automatically delete the contact details once they expired.

Once received the other person can add an optional category for the newly added person as well as a nickname.

E.g. Category: "Freeclimbing Trip Italy 2017", Nickname: "Alex from USA"

## 3. In case of emergency

Should anything happen to someone in the group that renderes them unconscious or barely able to talk (falling from height, high speed impact, etc.) anyone else can pull up the emergency contact information in their app and immediately share this information with paramedics.


# Optional additional features

TODO: May or may not add these feature.

## Emergency service numbers

Additionally the application can provide the emergency phone number (either via gps detection or manual country selection) for the country you are in.

It also allows you to store optional emergency numbers.

E.g. if you are freeclimbing in a well known spot, it might be better to have the local number ready instead of the countries generic emergency number (who would then have to redirect you to the local number, costing vital time).

## Export contact details

Allows exporting the received contacts to an android contact provider (excluding insurance details and emergency contacts).

Export of emergency numbers is also possible.

## Deadman switch

Configure the phone as a dead man's switch and have it automatically send sms/email or call an emergency contact with predefined text if you do not reset the switch in time.

E.g. setting the switch to 24 hours when you are mountaineering alone in the swiss alps. Unless you open the app once every 24 hours and press "reset" the automatic sms/email/phone call is made.

When in "dead man's switch" mode, the application will prompt you multiple times before the time is about to expire (via notifications).

Unless you manually reset it, the application will execute your command (sms/email/phone call).

Problems:

* Phone call/sms cannot be sent automatically from a windowsphone or iOS at all.
* No phone supports calling with a prerecorded voice
* People using dead man's switch might bei in mountain regions (no/bad cellular service) and might run out of battery

Possible solution:

Send command to server and execute from there unless dead man switch is reset.

Problems:

* Data needs to be unencrypted to be sent out at a later date
* Server needs really high availability and reliability (not sending someone's dead man switch is really bad)
* User must reset the server thus requiring an active cellular connection (either sms/internet). Don't want to accidently send something just because user was out of cellular reach