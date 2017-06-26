# Requirements

Users must be able to share information from phone to phone without transfering data via internet.

QR code supports a flexible amount of characters and can be read by any phone camera.

## Barcode size

QR Code can easily support content of 4096 or more (when using digits + letters + special chars) characters but will get unreasonably large and might be hard to read with older phone cameras.

Testing showed that:

* Full details of own person
* List of multiple insurances
* Two emergency contact persons (name, address, phone)

ends up with less than 800 characters and procudes a barcode of reasonable size.

**TODO:** Optionally data can be split into multiple barcodes (of bigger squares) making it easier to read with phones that have bad cameras.

# Data types

User should be able to input data using preexisting fileds but may also add additional data in freeform.

All data that can be entered must be optional.

## Own person

Data about oneself should be more detailed than emergency contacts:

* Full name
* Phone number
* Email
* Date of birth
* Address
* Bloodtype
* Allergies
* Weight
* Height
* Additional information (freetext)

## Insurance details

Multiple insurance details can be attached each supporting:

* Insurance name
* Insurance coverage
* Insurance number
* Email
* Phone number
* Address
* Additional information (freetext)

## Emergency contact (ICE)

Lastly multiple ICE contacts can be added each supporting:

* Full name
* Phone number
* Email
* Address
* Additional information (freetext)

# Profile support

The user should be able to create multiple profiles about himself (different insurances for different sport types).

Profiles should be cloneable (e.g. clone "freeclimbing" and rename to cayaking, then just replace the insurance details as all the other data is identical).

# Sharing

On each profile the user can select "share my info" and a dialog will ask him for a duration to be set.

The duration is transmitted with the barcode and allows the data to be removed after a certain time (1 day, 1 week, 1 month, never, etc.).

This feature allows multiple people who don't know each other very well to share information on a short trip (e.g. motorcycle tour with random people from fb) and have their data auto. removed from each others phone after the time period.

(Sidenote: Of course the other person can always screenshot the information or write it down otherwise, but given that the people already interact in a high risk sport the assumtion is that no one has "evil intention" with the data).

# Category support

Received profiles can be added to categories and given a nickname.